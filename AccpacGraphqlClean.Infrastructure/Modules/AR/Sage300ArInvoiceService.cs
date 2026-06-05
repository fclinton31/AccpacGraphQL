using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArInvoiceService : IArInvoiceService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArInvoiceService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARInvoice Invoice)> CreateOrUpdateAsync(
        ARInvoice invoice,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0031,AR0032,AR0033,AR0034,AR0402,AR0160,AR0401", compose: true);
            dynamic vBatch = views.ViewById("AR0031");
            dynamic vHeader = views.ViewById("AR0032");
            dynamic vDetail = views.ViewById("AR0033");

            EnsureOpenBatch(vBatch, invoice);

            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);

            if (!string.IsNullOrWhiteSpace(invoice.BatchNumber015))
            {
                batchNumber = invoice.BatchNumber015;
            }

            var isUpdate = !string.IsNullOrWhiteSpace(invoice.BatchNumber015)
                && !string.IsNullOrWhiteSpace(invoice.EntryNumber016);

            if (isUpdate)
            {
                vHeader.Fields.FieldByName("CNTBTCH").Value = invoice.BatchNumber015;
                vHeader.Fields.FieldByName("CNTITEM").Value = invoice.EntryNumber016;
                if ((bool)vHeader.Exists)
                {
                    vHeader.Read();
                }
                else
                {
                    vHeader.RecordGenerate(false);
                }
            }
            else
            {
                vHeader.RecordGenerate(false);
            }

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CNTBTCH", batchNumber);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDCUST", invoice.CustomerNumber017);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDINVC", invoice.DocumentNumber018);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDSHPT", invoice.ShipToLocationCode019);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTTRX", invoice.DocumentType021);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "ORDRNBR", invoice.OrderNumber023);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CUSTPO", invoice.PONumber024);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "INVCDESC", invoice.InvoiceDescription025);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "INVCAPPLTO", invoice.ApplytoDocument027);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTSET", invoice.AccountSet028);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEINVC", invoice.DocumentDate029);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEASOF", invoice.AsofDate030);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECURN", invoice.CurrencyCode033);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPE", invoice.RateType034);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWMANRTE", invoice.RateOverridden035);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "EXCHRATEHC", invoice.ExchangeRate036);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "ORIGRATEHC", invoice.ApplytoExchangeRate037);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TERMCODE", invoice.Terms038);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTERMOVRD", invoice.TermsCodeOverridden039);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEDUE", invoice.DueDate040);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEDISC", invoice.DiscountDate041);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "PCTDISC", invoice.DiscountPercentage042);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "AMTDISCAVL", invoice.DiscountAmountAvailable043);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "ENTEREDBY", invoice.EnteredBy191);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEBUS", invoice.PostingDate192);

            if (invoice.ARInvoiceLines is { Count: > 0 })
            {
                foreach (var line in invoice.ARInvoiceLines)
                {
                    vDetail.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDITEM", line.ItemNumber004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDDIST", line.DistributionCode005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "TEXTDESC", line.Description006);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "UNITMEAS", line.UnitofMeasure008);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "QTYINVC", line.Quantity009);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTCOST", line.Cost010);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTPRIC", line.Price011);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTEXTN", line.ExtendedAmountwTIP012);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "COMMENT", line.Comments046);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CONTRACT", line.Contract049);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "PROJECT", line.Project050);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CATEGORY", line.Category051);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "RESOURCE", line.ProjectCategoryResource052);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vDetail, "BILLDATE", line.BillingDate055);
                    vDetail.Insert();
                }
            }

            if (isUpdate && (bool)vHeader.Exists)
            {
                vHeader.Update();
            }
            else
            {
                vHeader.Insert();
            }

            vBatch.Update();
            session.CommitTransaction(tran);

            var documentNumber = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value);
            var entryNumber = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value);
            invoice.BatchNumber015 = Convert.ToString(vHeader.Fields.FieldByName("CNTBTCH").Value);
            invoice.EntryNumber016 = entryNumber;
            invoice.DocumentNumber018 = documentNumber;

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Invoice Number : {documentNumber}, Batch Number : {invoice.BatchNumber015}",
                DocumentNumber: documentNumber,
                BatchNumber: invoice.BatchNumber015,
                ErrorCode: "0000");

            return (response, invoice);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), invoice);
        }
    }

    public async Task<(ProcessOut Response, ARInvoiceBatch Batch)> CreateInvoiceBatchAsync(
        ARInvoiceBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0031,AR0032,AR0033,AR0034,AR0402,AR0160,AR0401", compose: true);
            dynamic vBatch = views.ViewById("AR0031");
            dynamic vHeader = views.ViewById("AR0032");
            dynamic vDetail = views.ViewById("AR0033");

            vBatch.RecordGenerate(false);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", batch.BatchDate);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BTCHDESC", batch.BatchDescription);
            vBatch.Fields.FieldByName("SRCEAPPL").Value = "YH";
            vBatch.Fields.FieldByName("BTCHSTTS").Value = 1;
            vBatch.Insert();

            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);

            if (batch.BatchEntries is { Count: > 0 })
            {
                foreach (var invoice in batch.BatchEntries)
                {
                    vHeader.RecordGenerate(false);
                    vHeader.Fields.FieldByName("CNTBTCH").Value = batchNumber;
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDCUST", invoice.CustomerNumber017);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDINVC", invoice.DocumentNumber018);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEINVC", invoice.DocumentDate029);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "INVCDESC", invoice.InvoiceDescription025);

                    if (invoice.ARInvoiceLines is { Count: > 0 })
                    {
                        foreach (var line in invoice.ARInvoiceLines)
                        {
                            vDetail.RecordGenerate(false);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDITEM", line.ItemNumber004);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDDIST", line.DistributionCode005);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "TEXTDESC", line.Description006);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "QTYINVC", line.Quantity009);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTPRIC", line.Price011);
                            vDetail.Insert();
                        }
                    }

                    vHeader.Insert();
                }
            }

            vBatch.Update();
            session.CommitTransaction(tran);

            batch.BatchNumber = batchNumber;
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Invoice Batch : {batchNumber}",
                DocumentNumber: batchNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, batch);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), batch);
        }
    }

    public async Task<(ProcessOut Response, ARInvoice Invoice)> ReadInvoiceAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var invoice = new ARInvoice { BatchNumber015 = batchNumber, EntryNumber016 = entryNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AR0031,AR0032,AR0033,AR0034,AR0402,AR0160,AR0401", compose: true);
            dynamic vHeader = views.ViewById("AR0032");
            dynamic vDetail = views.ViewById("AR0033");

            vHeader.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            vHeader.Fields.FieldByName("CNTITEM").Value = entryNumber;
            if (!(bool)vHeader.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "Invoice not found!"), invoice);
            }

            vHeader.Read();
            invoice.CustomerNumber017 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value);
            invoice.DocumentNumber018 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value);
            invoice.DocumentDate029 = vHeader.Fields.FieldByName("DATEINVC").Value as DateTime?;
            invoice.InvoiceDescription025 = Convert.ToString(vHeader.Fields.FieldByName("INVCDESC").Value);

            var lines = new List<ARInvoiceLine>();
            while (vDetail.Fetch())
            {
                lines.Add(new ARInvoiceLine
                {
                    EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                    LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                    ItemNumber004 = Convert.ToString(vDetail.Fields.FieldByName("IDITEM").Value),
                    DistributionCode005 = Convert.ToString(vDetail.Fields.FieldByName("IDDIST").Value),
                    Description006 = Convert.ToString(vDetail.Fields.FieldByName("TEXTDESC").Value),
                    UnitofMeasure008 = Convert.ToString(vDetail.Fields.FieldByName("UNITMEAS").Value),
                    Quantity009 = Convert.ToString(vDetail.Fields.FieldByName("QTYINVC").Value),
                    Price011 = Convert.ToString(vDetail.Fields.FieldByName("AMTPRIC").Value),
                    ExtendedAmountwTIP012 = Convert.ToString(vDetail.Fields.FieldByName("AMTEXTN").Value),
                    Comments046 = Convert.ToString(vDetail.Fields.FieldByName("COMMENT").Value)
                });
            }

            invoice.ARInvoiceLines = lines;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Invoice Number : {invoice.DocumentNumber018}",
                DocumentNumber: invoice.DocumentNumber018,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, invoice);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), invoice);
        }
    }

    public async Task<(ProcessOut Response, ARInvoiceBatch Batch)> ReadInvoiceBatchStatusAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);

        var batch = new ARInvoiceBatch { BatchNumber = batchNumber };
        try
        {
            var views = new Sage300ViewSet(session, "AR0031,AR0032,AR0033,AR0034,AR0402,AR0160,AR0401", compose: true);
            dynamic vBatch = views.ViewById("AR0031");

            vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)vBatch.Exists)
            {
                vBatch.Read();
            }

            batch.BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BTCHDESC").Value);
            batch.BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?;
            batch.BatchStatus = Convert.ToString(vBatch.Fields.FieldByName("BTCHSTTS").Value);
            batch.SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value);

            batch.BatchStatusDescription = batch.BatchStatus switch
            {
                "1" => "Open",
                "3" => "Posted",
                "4" => "Deleted",
                "5" => "Post In Progress",
                "7" => "Ready to Post",
                _ => ""
            };

            var response = new ProcessOut(
                "0000",
                $"Sage 300 ARInvoice Batch Status : {batch.BatchStatusDescription}",
                DocumentNumber: batchNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, batch);
        }
        catch (Exception ex)
        {
            return (ProcessOut.Fail("9999", ex.Message), batch);
        }
    }

    public async Task<(ProcessOut Response, ARInvoiceBatch Batch)> ReadInvoiceBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var batch = new ARInvoiceBatch { BatchNumber = batchNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AR0031,AR0032,AR0033,AR0034,AR0402,AR0160,AR0401", compose: true);
            dynamic vBatch = views.ViewById("AR0031");
            dynamic vHeader = views.ViewById("AR0032");
            dynamic vDetail = views.ViewById("AR0033");

            vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)vBatch.Exists)
            {
                vBatch.Read();
                batch.BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BTCHDESC").Value);
                batch.BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?;
                batch.SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value);
                batch.BatchStatus = Convert.ToString(vBatch.Fields.FieldByName("BTCHSTTS").Value);
            }

            var entries = new List<ARInvoice>();
            while (vHeader.Fetch())
            {
                var inv = new ARInvoice
                {
                    BatchNumber015 = batchNumber,
                    EntryNumber016 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                    CustomerNumber017 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value),
                    DocumentNumber018 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value),
                    InvoiceDescription025 = Convert.ToString(vHeader.Fields.FieldByName("INVCDESC").Value),
                    DocumentDate029 = vHeader.Fields.FieldByName("DATEINVC").Value as DateTime?
                };

                var lines = new List<ARInvoiceLine>();
                while (vDetail.Fetch())
                {
                    lines.Add(new ARInvoiceLine
                    {
                        EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                        LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                        ItemNumber004 = Convert.ToString(vDetail.Fields.FieldByName("IDITEM").Value),
                        Quantity009 = Convert.ToString(vDetail.Fields.FieldByName("QTYINVC").Value),
                        Price011 = Convert.ToString(vDetail.Fields.FieldByName("AMTPRIC").Value)
                    });
                }

                inv.ARInvoiceLines = lines;
                entries.Add(inv);
            }

            batch.BatchEntries = entries;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 ARInvoice Batch : {batchNumber}",
                DocumentNumber: batchNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, batch);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), batch);
        }
    }

    public async Task<(ProcessOut Response, SyncARInvoices Sync)> SyncInvoicesAsync(
        SyncARInvoices request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var module = "AR";
        var txnType = "IN";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var recordLimit = request.RecordLimit > 0 ? request.RecordLimit : 100;

        try
        {
            var views = new Sage300ViewSet(session, "AR0031,AR0032,AR0033,AR0034,AR0402,AR0160,AR0401,YH0301,CS0120", compose: true);
            dynamic vBatch = views.ViewById("AR0031");
            dynamic vHeader = views.ViewById("AR0032");
            dynamic vDetail = views.ViewById("AR0033");
            dynamic yh = views.ViewById("YH0301");

            BrowseForSync(yh, module, txnType, request, timestamp);

            var batches = new List<ARInvoiceBatch>();
            while (yh.Fetch() && batches.Count < recordLimit)
            {
                var batchNumber = Convert.ToString(yh.Fields.FieldByName("CNTBTCH").Value);
                if (string.IsNullOrWhiteSpace(batchNumber))
                {
                    continue;
                }

                vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
                vBatch.Read();
                if (Sage300ApPaymentService.SageViewPut.ParseInt(Convert.ToString(vBatch.Fields.FieldByName("BTCHSTTS").Value)) != 3)
                {
                    continue;
                }

                var batch = new ARInvoiceBatch
                {
                    BatchNumber = batchNumber,
                    BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?,
                    BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BTCHDESC").Value),
                    SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value),
                    BatchStatus = Convert.ToString(vBatch.Fields.FieldByName("BTCHSTTS").Value)
                };

                var entries = new List<ARInvoice>();
                while (vHeader.Fetch())
                {
                    var inv = new ARInvoice
                    {
                        BatchNumber015 = batchNumber,
                        EntryNumber016 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                        CustomerNumber017 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value),
                        DocumentNumber018 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value),
                        InvoiceDescription025 = Convert.ToString(vHeader.Fields.FieldByName("INVCDESC").Value),
                        DocumentDate029 = vHeader.Fields.FieldByName("DATEINVC").Value as DateTime?
                    };

                    var lines = new List<ARInvoiceLine>();
                    while (vDetail.Fetch())
                    {
                        lines.Add(new ARInvoiceLine
                        {
                            EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                            LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                            ItemNumber004 = Convert.ToString(vDetail.Fields.FieldByName("IDITEM").Value),
                            Quantity009 = Convert.ToString(vDetail.Fields.FieldByName("QTYINVC").Value),
                            Price011 = Convert.ToString(vDetail.Fields.FieldByName("AMTPRIC").Value)
                        });
                    }

                    inv.ARInvoiceLines = lines;
                    entries.Add(inv);
                }

                batch.BatchEntries = entries;
                batches.Add(batch);
            }

            request.ARInvoiceBatches = batches;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                "Sync AR Invoices completed.",
                DocumentNumber: request.Timestamp,
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, request);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), request);
        }
    }

    private static void EnsureOpenBatch(dynamic vBatch, ARInvoice invoice)
    {
        vBatch.Fields.FieldByName("SRCEAPPL").Value = "YH";
        vBatch.Fields.FieldByName("BTCHSTTS").Value = 1;

        var filter = "BTCHSTTS = 1 AND SRCEAPPL = \"YH\"";
        if (!string.IsNullOrWhiteSpace(invoice.Description002))
        {
            filter += $" AND BTCHDESC = \"{invoice.Description002}\"";
        }

        if (invoice.BatchDate001 is { } dt && dt != default)
        {
            filter += $" AND DATEBTCH = {dt:yyyyMMdd}";
        }

        vBatch.Browse(filter, true);
        if ((bool)vBatch.Fetch())
        {
            return;
        }

        vBatch.RecordGenerate(false);
        Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", invoice.BatchDate001);
        Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BTCHDESC", invoice.Description002);
        vBatch.Fields.FieldByName("SRCEAPPL").Value = "YH";
        vBatch.Fields.FieldByName("BTCHSTTS").Value = 1;
        vBatch.Fields.FieldByName("BTCHTYPE").Value = "IN";
        vBatch.Insert();
    }

    private static void BrowseForSync(dynamic yh, string module, string txnType, SyncARInvoices request, string timestamp)
    {
        if (string.Equals(request.CallMethod, "SYNC", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(request.Timestamp))
            {
                var old = $"MODULE = \"{module}\" AND TXNTYPE = \"{txnType}\" AND TIMESTAMP = \"{request.Timestamp}\"";
                yh.Browse(old, true);
                while (yh.Fetch())
                {
                    yh.Fields.FieldByName("YHSTATUS").Value = 1;
                    yh.Update();
                }
            }

            request.Timestamp = timestamp;
            yh.Fields.FieldByName("MODULE").Value = module;
            yh.Fields.FieldByName("TXNTYPE").Value = txnType;
            yh.Fields.FieldByName("CNTBTCH").Value = 0;
            yh.Browse($"MODULE = \"{module}\" AND TXNTYPE = \"{txnType}\" AND YHSTATUS = 0", true);
            return;
        }

        if (string.Equals(request.CallMethod, "UPDATE", StringComparison.OrdinalIgnoreCase))
        {
            yh.Fields.FieldByName("MODULE").Value = module;
            yh.Fields.FieldByName("TXNTYPE").Value = txnType;
            yh.Fields.FieldByName("CNTBTCH").Value = 0;
            yh.Browse($"TIMESTAMP = \"{request.Timestamp}\"", true);
            return;
        }

        throw new InvalidOperationException("Incorrect call method!");
    }
}
