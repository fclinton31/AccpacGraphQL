using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApInvoiceService : IApInvoiceService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ApInvoiceService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, APInvoices Invoice)> CreateInvoiceAsync(
        APInvoices invoice,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0020,AP0021,AP0022,AP0023,AP0401,AP0402", compose: true);
            dynamic batch = views.ViewById("AP0020");
            dynamic header = views.ViewById("AP0021");
            dynamic detail = views.ViewById("AP0022");

            batch.Fields.FieldByName("SRCEAPPL").Value = "YH";
            batch.Fields.FieldByName("BTCHSTTS").Value = "1";
            batch.Fields.FieldByName("CNTBTCH").Value = "0";
            batch.Init();
            if (!string.IsNullOrWhiteSpace(invoice.BatchDescription))
            {
                batch.Fields.FieldByName("BTCHDESC").Value = invoice.BatchDescription;
            }
            if (invoice.BatchDate is { } dt && dt != default)
            {
                batch.Fields.FieldByName("DATEBTCH").Value = dt;
            }
            batch.Update();

            header.RecordGenerate(false);
            PutIfNotNull(header, "ORIGCOMP", invoice.Originator002);
            PutIfNotNull(header, "IDVEND", invoice.VendorNumber003);
            PutIfNotNull(header, "IDINVC", invoice.DocumentNumber004);
            PutIfNotNull(header, "IDRMITTO", invoice.RemitToLocation005);
            PutIfNotNull(header, "TEXTTRX", invoice.DocumentType006);
            PutIfNotNull(header, "ORDRNBR", invoice.OrderNumber009);
            PutIfNotNull(header, "PONBR", invoice.PONumber010);
            PutIfNotNull(header, "INVCDESC", invoice.InvoiceDescription011);
            PutIfNotNull(header, "INVCAPPLTO", invoice.ApplytoDocument013);
            PutIfNotNull(header, "IDACCTSET", invoice.AccountSet014);
            PutIfDate(header, "DATEINVC", invoice.DocumentDate015);
            PutIfDate(header, "DATEASOF", invoice.AsofDate016);
            PutIfNotNull(header, "CODECURN", invoice.CurrencyCode019);
            PutIfNotNull(header, "RATETYPE", invoice.RateType020);
            PutIfNotNull(header, "SWMANRTE", invoice.RateOverridden021);
            PutIfNotNull(header, "EXCHRATEHC", invoice.ExchangeRate022);
            PutIfNotNull(header, "ORIGRATEHC", invoice.ApplytoExchangeRate023);
            PutIfNotNull(header, "TERMCODE", invoice.Terms024);
            PutIfNotNull(header, "SWTERMOVRD", invoice.TermsOverridden025);
            PutIfDate(header, "DATEDUE", invoice.DueDate026);
            PutIfDate(header, "DATEDISC", invoice.DiscountDate027);
            PutIfNotNull(header, "PCTDISC", invoice.DiscountPercentage028);
            PutIfNotNull(header, "AMTDISCAVL", invoice.DiscountAmountAvailable029);
            PutIfNotNull(header, "SWCALCTX", invoice.TaxAmountControl032);
            PutIfNotNull(header, "CODETAXGRP", invoice.TaxGroup033);
            PutIfNotNull(header, "TAXCLASS1", invoice.TaxClass1039);
            PutIfNotNull(header, "TAXCLASS2", invoice.TaxClass2040);
            PutIfNotNull(header, "TAXCLASS3", invoice.TaxClass3041);
            PutIfNotNull(header, "TAXCLASS4", invoice.TaxClass4042);
            PutIfNotNull(header, "TAXCLASS5", invoice.TaxClass5043);
            PutIfNotNull(header, "BASETAX1", invoice.TaxBase1044);
            PutIfNotNull(header, "BASETAX2", invoice.TaxBase2045);
            PutIfNotNull(header, "BASETAX3", invoice.TaxBase3046);
            PutIfNotNull(header, "BASETAX4", invoice.TaxBase4047);
            PutIfNotNull(header, "BASETAX5", invoice.TaxBase5048);
            PutIfNotNull(header, "AMTTAX1", invoice.TaxAmount1049);
            PutIfNotNull(header, "AMTTAX2", invoice.TaxAmount2050);
            PutIfNotNull(header, "AMTTAX3", invoice.TaxAmount3051);
            PutIfNotNull(header, "AMTTAX4", invoice.TaxAmount4052);
            PutIfNotNull(header, "AMTTAX5", invoice.TaxAmount5053);
            PutIfNotNull(header, "AMT1099", invoice.S1099CPRSAmount054);
            PutIfNotNull(header, "AMTDISTSET", invoice.DistributionSetAmount055);
            PutIfNotNull(header, "TEXTRMIT", invoice.LocationName063);
            PutIfNotNull(header, "TEXTSTE1", invoice.AddressLine1064);
            PutIfNotNull(header, "TEXTSTE2", invoice.AddressLine2065);
            PutIfNotNull(header, "TEXTSTE3", invoice.AddressLine3066);
            PutIfNotNull(header, "TEXTSTE4", invoice.AddressLine4067);
            PutIfNotNull(header, "NAMECITY", invoice.City068);
            PutIfNotNull(header, "CODESTTE", invoice.StateProv069);
            PutIfNotNull(header, "CODEPSTL", invoice.ZipPostalCode070);
            PutIfNotNull(header, "CODECTRY", invoice.Country071);
            PutIfNotNull(header, "NAMECTAC", invoice.ContactName072);
            PutIfNotNull(header, "TEXTPHON", invoice.PhoneNumber073);
            PutIfNotNull(header, "TEXTFAX", invoice.FaxNumber074);
            PutIfDate(header, "DATERATE", invoice.RateDate075);
            PutIfNotNull(header, "IDDISTSET", invoice.DistributionSet080);
            PutIfNotNull(header, "ID1099CLAS", invoice.S1099CPRSCode081);
            PutIfNotNull(header, "AMTGROSTOT", invoice.DocumentTotalIncludingTax085);
            PutIfNotNull(header, "SWTAXINCL1", invoice.TaxInclusive1088);
            PutIfNotNull(header, "SWTAXINCL2", invoice.TaxInclusive2089);
            PutIfNotNull(header, "SWTAXINCL3", invoice.TaxInclusive3090);
            PutIfNotNull(header, "SWTAXINCL4", invoice.TaxInclusive4091);
            PutIfNotNull(header, "SWTAXINCL5", invoice.TaxInclusive5092);
            PutIfNotNull(header, "SWJOB", invoice.JobRelated114);
            PutIfNotNull(header, "EMAIL", invoice.Email119);
            PutIfNotNull(header, "CTACPHONE", invoice.ContactsPhone120);
            PutIfNotNull(header, "CTACFAX", invoice.ContactsFax121);
            PutIfNotNull(header, "CTACEMAIL", invoice.ContactsEmail122);
            PutIfNotNull(header, "AMTDSCBASE", invoice.DiscountBase128);
            PutIfNotNull(header, "TEXTVEN", invoice.VendorName220);
            PutIfNotNull(header, "ENTEREDBY", invoice.EnteredBy228);
            PutIfDate(header, "DATEBUS", invoice.PostingDate229);

            if (invoice.APInvoiceItems is { Count: > 0 })
            {
                foreach (var dtl in invoice.APInvoiceItems)
                {
                    detail.RecordGenerate(false);
                    PutIfNotNull(detail, "DESCOMP", dtl.Destination003);
                    PutIfNotNull(detail, "ROUTE", dtl.RouteNo004);
                    PutIfNotNull(detail, "IDDIST", dtl.DistributionCode005);
                    PutIfNotNull(detail, "TEXTDESC", dtl.DistributionDescription006);
                    PutIfNotNull(detail, "SWMANLDIST", dtl.Reserved007);
                    PutIfNotNull(detail, "SWMANLTX", dtl.ManualTaxEntry009);
                    PutIfNotNull(detail, "BASETAX1", dtl.BaseTax1010);
                    PutIfNotNull(detail, "BASETAX2", dtl.BaseTax2011);
                    PutIfNotNull(detail, "BASETAX3", dtl.BaseTax3012);
                    PutIfNotNull(detail, "BASETAX4", dtl.BaseTax4013);
                    PutIfNotNull(detail, "BASETAX5", dtl.BaseTax5014);
                    PutIfNotNull(detail, "TAXCLASS1", dtl.TaxClass1015);
                    PutIfNotNull(detail, "TAXCLASS2", dtl.TaxClass2016);
                    PutIfNotNull(detail, "TAXCLASS3", dtl.TaxClass3017);
                    PutIfNotNull(detail, "TAXCLASS4", dtl.TaxClass4018);
                    PutIfNotNull(detail, "TAXCLASS5", dtl.TaxClass5019);
                    PutIfNotNull(detail, "SWTAXINCL1", dtl.TaxInclusive1020);
                    PutIfNotNull(detail, "SWTAXINCL2", dtl.TaxInclusive2021);
                    PutIfNotNull(detail, "SWTAXINCL3", dtl.TaxInclusive3022);
                    PutIfNotNull(detail, "SWTAXINCL4", dtl.TaxInclusive4023);
                    PutIfNotNull(detail, "SWTAXINCL5", dtl.TaxInclusive5024);
                    PutIfNotNull(detail, "RATETAX1", dtl.TaxRate1025);
                    PutIfNotNull(detail, "RATETAX2", dtl.TaxRate2026);
                    PutIfNotNull(detail, "RATETAX3", dtl.TaxRate3027);
                    PutIfNotNull(detail, "RATETAX4", dtl.TaxRate4028);
                    PutIfNotNull(detail, "RATETAX5", dtl.TaxRate5029);
                    PutIfNotNull(detail, "AMTTAX1", dtl.TaxAmount1030);
                    PutIfNotNull(detail, "AMTTAX2", dtl.TaxAmount2031);
                    PutIfNotNull(detail, "AMTTAX3", dtl.TaxAmount3032);
                    PutIfNotNull(detail, "AMTTAX4", dtl.TaxAmount4033);
                    PutIfNotNull(detail, "AMTTAX5", dtl.TaxAmount5034);
                    PutIfNotNull(detail, "IDGLACCT", dtl.GLAccount035);
                    PutIfNotNull(detail, "AMTDIST", dtl.DistributedAmount038);
                    PutIfNotNull(detail, "COMMENT", dtl.Comment039);
                    PutIfNotNull(detail, "AMTDISTNET", dtl.DistributedAmountBeforeTaxes041);
                    PutIfNotNull(detail, "CONTRACT", dtl.Contract055);
                    PutIfNotNull(detail, "PROJECT", dtl.Project056);
                    PutIfNotNull(detail, "CATEGORY", dtl.Category057);
                    PutIfNotNull(detail, "RESOURCE", dtl.ProjectCategoryResource058);
                    PutIfNotNull(detail, "BILLTYPE", dtl.BillingType061);
                    PutIfNotNull(detail, "IDITEM", dtl.ItemNumber062);
                    PutIfNotNull(detail, "UNITMEAS", dtl.UnitofMeasure063);
                    PutIfNotNull(detail, "QTYINVC", dtl.Quantity064);
                    PutIfNotNull(detail, "AMTCOST", dtl.Cost065);
                    PutIfDate(detail, "BILLDATE", dtl.BillingDate066);
                    PutIfNotNull(detail, "BILLRATE", dtl.BillingRate067);
                    PutIfNotNull(detail, "SWDISCABL", dtl.Discountable070);
                    PutIfNotNull(detail, "OCNTLINE", dtl.OriginalLineIdentifier071);
                    detail.Insert();
                }
            }

            if (header.Fields.FieldByName("AMTUNDISTR").Value != 0)
            {
                header.Fields.FieldByName("AMTGROSTOT").Value =
                    header.Fields.FieldByName("AMTGROSTOT").Value + Decimal.Negate(header.Fields.FieldByName("AMTUNDISTR").Value);
            }

            header.Insert();
            batch.Update();
            session.CommitTransaction(tran);

            var documentNumber = Convert.ToString(header.Fields.FieldByName("IDINVC").Value);
            var batchNumber = Convert.ToString(batch.Fields.FieldByName("CNTBTCH").Value);
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Invoice Number : {documentNumber}, Batch Number : {batchNumber}",
                DocumentNumber: documentNumber,
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

    public async Task<(ProcessOut Response, APInvoiceBatch Batch)> CreateInvoiceBatchAsync(
        APInvoiceBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0020,AP0021,AP0022,AP0023,AP0401,AP0402", compose: true);
            dynamic vBatch = views.ViewById("AP0020");
            dynamic header = views.ViewById("AP0021");
            dynamic detail = views.ViewById("AP0022");

            vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
            vBatch.Init();
            PutIfNotNull(vBatch, "BTCHDESC", batch.BatchDesc);
            PutIfDate(vBatch, "DATEBTCH", batch.BatchDate);
            vBatch.Fields.FieldByName("SRCEAPPL").Value = "YH";
            vBatch.Update();

            if (batch.Invoice is { Count: > 0 })
            {
                foreach (var invoice in batch.Invoice)
                {
                    header.RecordGenerate(false);
                    PutIfNotNull(header, "ORIGCOMP", invoice.Originator002);
                    PutIfNotNull(header, "IDVEND", invoice.VendorNumber003);
                    PutIfNotNull(header, "IDINVC", invoice.DocumentNumber004);
                    PutIfNotNull(header, "IDRMITTO", invoice.RemitToLocation005);
                    PutIfNotNull(header, "TEXTTRX", invoice.DocumentType006);
                    PutIfNotNull(header, "ORDRNBR", invoice.OrderNumber009);
                    PutIfNotNull(header, "PONBR", invoice.PONumber010);
                    PutIfNotNull(header, "INVCDESC", invoice.InvoiceDescription011);
                    PutIfNotNull(header, "INVCAPPLTO", invoice.ApplytoDocument013);
                    PutIfNotNull(header, "IDACCTSET", invoice.AccountSet014);
                    PutIfDate(header, "DATEINVC", invoice.DocumentDate015);
                    PutIfDate(header, "DATEASOF", invoice.AsofDate016);
                    PutIfNotNull(header, "CODECURN", invoice.CurrencyCode019);
                    PutIfNotNull(header, "RATETYPE", invoice.RateType020);
                    PutIfNotNull(header, "SWMANRTE", invoice.RateOverridden021);
                    PutIfNotNull(header, "EXCHRATEHC", invoice.ExchangeRate022);
                    PutIfNotNull(header, "ORIGRATEHC", invoice.ApplytoExchangeRate023);
                    PutIfNotNull(header, "TERMCODE", invoice.Terms024);

                    if (invoice.APInvoiceItems is { Count: > 0 })
                    {
                        foreach (var dtl in invoice.APInvoiceItems)
                        {
                            detail.RecordGenerate(false);
                            PutIfNotNull(detail, "IDDIST", dtl.DistributionCode005);
                            PutIfNotNull(detail, "IDGLACCT", dtl.GLAccount035);
                            PutIfNotNull(detail, "AMTDIST", dtl.DistributedAmount038);
                            PutIfNotNull(detail, "COMMENT", dtl.Comment039);
                            detail.Insert();
                        }
                    }

                    header.Insert();
                }
            }

            vBatch.Update();
            session.CommitTransaction(tran);
            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);
            batch.BatchNumber = batchNumber;
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Invoice Batch : {batchNumber}",
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

    public async Task<(ProcessOut Response, APInvoices Invoice)> ReadInvoiceAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var invoice = new APInvoices { BatchNumber000 = batchNumber, EntryNumber001 = entryNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AP0020,AP0021,AP0022,AP0023,AP0401,AP0402", compose: true);
            dynamic header = views.ViewById("AP0021");
            dynamic detail = views.ViewById("AP0022");

            header.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            header.Fields.FieldByName("CNTITEM").Value = entryNumber;

            if (!(bool)header.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "Invoice not found!"), invoice);
            }

            header.Read();
            invoice.EntryNumber001 = Convert.ToString(header.Fields.FieldByName("CNTITEM").Value);
            invoice.Originator002 = Convert.ToString(header.Fields.FieldByName("ORIGCOMP").Value);
            invoice.VendorNumber003 = Convert.ToString(header.Fields.FieldByName("IDVEND").Value);
            invoice.DocumentNumber004 = Convert.ToString(header.Fields.FieldByName("IDINVC").Value);

            var items = new List<APInvoiceItem>();
            while (detail.Fetch())
            {
                var item = new APInvoiceItem
                {
                    EntryNumber001 = Convert.ToString(detail.Fields.FieldByName("CNTITEM").Value),
                    LineNumber002 = Convert.ToString(detail.Fields.FieldByName("CNTLINE").Value),
                    DistributionCode005 = Convert.ToString(detail.Fields.FieldByName("IDDIST").Value),
                    GLAccount035 = Convert.ToString(detail.Fields.FieldByName("IDGLACCT").Value),
                    DistributedAmount038 = Convert.ToString(detail.Fields.FieldByName("AMTDIST").Value),
                    Comment039 = Convert.ToString(detail.Fields.FieldByName("COMMENT").Value)
                };
                items.Add(item);
            }

            invoice.APInvoiceItems = items;

            session.CommitTransaction(tran);
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Invoice Number : {invoice.DocumentNumber004}",
                DocumentNumber: invoice.DocumentNumber004,
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

    public async Task<(ProcessOut Response, APInvoiceBatch Batch)> ReadInvoiceBatchStatusAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);

        var batchModel = new APInvoiceBatch { BatchNumber = batchNumber };
        try
        {
            var views = new Sage300ViewSet(session, "AP0020,AP0021,AP0022,AP0023,AP0401,AP0402", compose: true);
            dynamic batch = views.ViewById("AP0020");

            batch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)batch.Exists)
            {
                batch.Read();
            }

            batchModel.BatchDesc = Convert.ToString(batch.Fields.FieldByName("BTCHDESC").Value);
            batchModel.BatchDate = batch.Fields.FieldByName("DATEBTCH").Value as DateTime?;
            batchModel.BatchEntry = Convert.ToString(batch.Fields.FieldByName("CNTINVCENT").Value);
            batchModel.BatchStatus = Convert.ToString(batch.Fields.FieldByName("BTCHSTTS").Value);
            batchModel.SourceApplication = Convert.ToString(batch.Fields.FieldByName("SRCEAPPL").Value);

            batchModel.BatchStatusDescription = batchModel.BatchStatus switch
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
                $"Sage 300 APInvoice Batch Status : {batchModel.BatchStatusDescription}",
                DocumentNumber: batchNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");
            return (response, batchModel);
        }
        catch (Exception ex)
        {
            return (ProcessOut.Fail("9999", ex.Message), batchModel);
        }
    }

    public async Task<(ProcessOut Response, APInvoiceBatch Batch)> ReadInvoiceBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var batchModel = new APInvoiceBatch { BatchNumber = batchNumber };
        try
        {
            var views = new Sage300ViewSet(session, "AP0020,AP0021,AP0022,AP0023,AP0401,AP0402", compose: true);
            dynamic batch = views.ViewById("AP0020");
            dynamic header = views.ViewById("AP0021");
            dynamic detail = views.ViewById("AP0022");

            batch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)batch.Exists)
            {
                batch.Read();
            }

            var invoices = new List<APInvoices>();
            while (header.Fetch())
            {
                var inv = new APInvoices
                {
                    BatchNumber000 = batchNumber,
                    EntryNumber001 = Convert.ToString(header.Fields.FieldByName("CNTITEM").Value),
                    Originator002 = Convert.ToString(header.Fields.FieldByName("ORIGCOMP").Value),
                    VendorNumber003 = Convert.ToString(header.Fields.FieldByName("IDVEND").Value),
                    DocumentNumber004 = Convert.ToString(header.Fields.FieldByName("IDINVC").Value),
                    RemitToLocation005 = Convert.ToString(header.Fields.FieldByName("IDRMITTO").Value),
                    InvoiceDescription011 = Convert.ToString(header.Fields.FieldByName("INVCDESC").Value),
                    DocumentDate015 = header.Fields.FieldByName("DATEINVC").Value as DateTime?
                };

                var items = new List<APInvoiceItem>();
                while (detail.Fetch())
                {
                    items.Add(new APInvoiceItem
                    {
                        EntryNumber001 = Convert.ToString(detail.Fields.FieldByName("CNTITEM").Value),
                        LineNumber002 = Convert.ToString(detail.Fields.FieldByName("CNTLINE").Value),
                        DistributionCode005 = Convert.ToString(detail.Fields.FieldByName("IDDIST").Value),
                        GLAccount035 = Convert.ToString(detail.Fields.FieldByName("IDGLACCT").Value),
                        DistributedAmount038 = Convert.ToString(detail.Fields.FieldByName("AMTDIST").Value),
                        Comment039 = Convert.ToString(detail.Fields.FieldByName("COMMENT").Value)
                    });
                }

                inv.APInvoiceItems = items;
                invoices.Add(inv);
            }

            batchModel.Invoice = invoices;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 APInvoice Batch : {batchNumber}",
                DocumentNumber: batchNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, batchModel);
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

            return (ProcessOut.Fail("9999", ex.Message), batchModel);
        }
    }

    public async Task<(ProcessOut Response, SyncAPInvoices Sync)> SyncInvoicesAsync(
        SyncAPInvoices request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var module = "AP";
        var txnType = "IN";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var recordLimit = request.RecordLimit > 0 ? request.RecordLimit : 100;

        try
        {
            var views = new Sage300ViewSet(session, "AP0020,AP0021,AP0022,AP0023,AP0401,AP0402,YH0301,CS0120", compose: true);
            dynamic vBatch = views.ViewById("AP0020");
            dynamic vHeader = views.ViewById("AP0021");
            dynamic vDetail = views.ViewById("AP0022");
            dynamic yh = views.ViewById("YH0301");

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
            }
            else if (string.Equals(request.CallMethod, "UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                yh.Fields.FieldByName("MODULE").Value = module;
                yh.Fields.FieldByName("TXNTYPE").Value = txnType;
                yh.Fields.FieldByName("CNTBTCH").Value = 0;
                yh.Browse($"TIMESTAMP = \"{request.Timestamp}\"", true);
            }
            else
            {
                throw new InvalidOperationException("Incorrect call method!");
            }

            var batches = new List<APInvoiceBatch>();
            while (yh.Fetch() && batches.Count < recordLimit)
            {
                var batchNumber = Convert.ToString(yh.Fields.FieldByName("CNTBTCH").Value);
                if (string.IsNullOrWhiteSpace(batchNumber))
                {
                    continue;
                }

                vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
                vBatch.Read();
                if (Convert.ToInt32(vBatch.Fields.FieldByName("BTCHSTTS").Value) != 3)
                {
                    continue;
                }

                var batch = new APInvoiceBatch
                {
                    BatchNumber = batchNumber,
                    BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?,
                    BatchDesc = Convert.ToString(vBatch.Fields.FieldByName("BTCHDESC").Value),
                    SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value)
                };

                var invoices = new List<APInvoices>();
                while (vHeader.Fetch())
                {
                    var invoice = new APInvoices
                    {
                        BatchNumber000 = batchNumber,
                        EntryNumber001 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                        Originator002 = Convert.ToString(vHeader.Fields.FieldByName("ORIGCOMP").Value),
                        VendorNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value),
                        DocumentNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value),
                        RemitToLocation005 = Convert.ToString(vHeader.Fields.FieldByName("IDRMITTO").Value),
                        DocumentType006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTTRX").Value),
                        InvoiceDescription011 = Convert.ToString(vHeader.Fields.FieldByName("INVCDESC").Value),
                        DocumentDate015 = vHeader.Fields.FieldByName("DATEINVC").Value as DateTime?
                    };

                    var items = new List<APInvoiceItem>();
                    while (vDetail.Fetch())
                    {
                        items.Add(new APInvoiceItem
                        {
                            EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                            LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                            DistributionCode005 = Convert.ToString(vDetail.Fields.FieldByName("IDDIST").Value),
                            GLAccount035 = Convert.ToString(vDetail.Fields.FieldByName("IDGLACCT").Value),
                            DistributedAmount038 = Convert.ToString(vDetail.Fields.FieldByName("AMTDIST").Value),
                            Comment039 = Convert.ToString(vDetail.Fields.FieldByName("COMMENT").Value)
                        });
                    }

                    invoice.APInvoiceItems = items;
                    invoices.Add(invoice);
                }

                batch.Invoice = invoices;
                batches.Add(batch);
            }

            request.APInvoiceBatches = batches;

            session.CommitTransaction(tran);
            var response = new ProcessOut(
                "0000",
                "Sync AP Invoices completed.",
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

    private static void PutIfNotNull(dynamic view, string fieldName, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            view.Fields.FieldByName(fieldName).Value = value;
        }
    }

    private static void PutIfDate(dynamic view, string fieldName, DateTime? value)
    {
        if (value is { } dt && dt != default)
        {
            view.Fields.FieldByName(fieldName).Value = dt;
        }
    }
}
