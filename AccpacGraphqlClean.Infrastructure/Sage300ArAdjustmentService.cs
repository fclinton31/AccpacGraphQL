using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArAdjustmentService : IArAdjustmentService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArAdjustmentService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARAdjustment Adjustment)> CreateOrUpdateAsync(
        ARAdjustment adjustment,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0041,AR0042,AR0043,AR0044,AR0406,AR0170,AR0045,AR0061", compose: true);
            dynamic vBatch = views.ViewById("AR0041");
            dynamic vHeader = views.ViewById("AR0042");
            dynamic vDetail = views.ViewById("AR0044");

            vBatch.Fields.FieldByName("CODEPYMTYP").Value = string.IsNullOrWhiteSpace(adjustment.BatchType000) ? "AD" : adjustment.BatchType000;
            vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
            vBatch.Init();
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BATCHDESC", adjustment.Description003);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", adjustment.BatchDate002);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BATCHTYPE", adjustment.BatchType006);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BATCHSTAT", adjustment.BatchStatus007);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "IDBANK", adjustment.BankCode008);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "CODECURN", adjustment.DefaultBankCurrency010);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATERATE", adjustment.BankRateDate011);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "RATETYPE", adjustment.BankRateType013);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "RATEEXCHHC", adjustment.BankExchangeRate014);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "DEPSTNBR", adjustment.DepositNumber015);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "SWRATE", adjustment.BankRateOverridden026);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "SRCEAPPL", adjustment.SourceApplication027);
            vBatch.Update();

            vHeader.Fields.FieldByName("CODEPYMTYP").Value = string.IsNullOrWhiteSpace(adjustment.BatchType032) ? "AD" : adjustment.BatchType032;
            if (!string.IsNullOrWhiteSpace(adjustment.CheckReceiptNo035))
            {
                vHeader.Fields.FieldByName("IDRMIT").Value = adjustment.CheckReceiptNo035;
                vHeader.Order = 2;
            }

            if (!string.IsNullOrWhiteSpace(adjustment.DocumentNumber067))
            {
                vHeader.Fields.FieldByName("DOCNBR").Value = adjustment.DocumentNumber067;
                vHeader.Order = 3;
            }

            var exists = (bool)vHeader.Exists;
            if (exists)
            {
                vHeader.Read();
            }
            else
            {
                vHeader.RecordGenerate(false);
                Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "DOCNBR", adjustment.DocumentNumber067);
            }

            vHeader.Fields.FieldByName("RMITTYPE").Value = string.IsNullOrWhiteSpace(adjustment.ReceiptTransType052) ? "7" : adjustment.ReceiptTransType052;
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CNTBTCH", adjustment.BatchNumber033);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CNTITEM", adjustment.EntryNumber034);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDCUST", adjustment.CustomerNumber036);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERMIT", adjustment.ReceiptDateAdjustmentDate037);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTRMIT", adjustment.EntryDescription038);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXTRMITREF", adjustment.EntryReference039);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "AMTRMIT", adjustment.BankReceiptAmount040);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCHTC", adjustment.CustExchangeRate042);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATETC", adjustment.CustRateOverridden043);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODEPAYM", adjustment.PaymentCode047);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPEHC", adjustment.BankRateType049);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCHHC", adjustment.BankExchangeRate050);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATEHC", adjustment.BankRateOverridden051);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "DOCTYPE", adjustment.DocumentType053);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDINVCMTCH", adjustment.MatchingDocumentNumber054);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTPAYOR", adjustment.Payer058);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERATETC", adjustment.CustRateDate059);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPETC", adjustment.CustRateType060);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERATEHC", adjustment.BankRateDate062);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SRCEAPPL", adjustment.SourceApplication082);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDBANK", adjustment.BankCode083);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECURNBC", adjustment.BankCurrencyCode084);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "ENTEREDBY", adjustment.EnteredBy147);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEBUS", adjustment.PostingDate148);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTSET", adjustment.AccountSet149);

            if (adjustment.ARAdjustmentLines is { Count: > 0 })
            {
                foreach (var dtl in adjustment.ARAdjustmentLines)
                {
                    vDetail.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTBTCH", dtl.BatchNumber001);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTITEM", dtl.EntryNumber002);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTLINE", dtl.LineNumber003);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDCUST", dtl.CustomerNumber004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDINVC", dtl.DocumentNumber005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTPAYM", dtl.PaymentNumber006);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "TRXTYPE", dtl.TransactionType007);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "PYMTRESL", dtl.PaymentResolution008);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTPAYM", dtl.CustReceiptAmount009);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTERNDISC", dtl.CustDiscountAmountTaken010);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "TEXTADJ", dtl.Description014);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "GLREF", dtl.Reference015);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDDOCMTCH", dtl.PPMatchingDocNo017);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CDAPPLYTO", dtl.PPMatchingDocType018);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "DOCTYPE", dtl.DocumentType027);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "APPLYMETH", dtl.JobApplyMethod031);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "RTGAMT", dtl.RetainageAmount034);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vDetail, "RTGDATEDUE", dtl.RetainageDueDate035);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "RTGTERMS", dtl.RetainageTermsCode036);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "SWRTGRATE", dtl.RetainageExchangeRate037);
                    vDetail.Insert();
                }
            }

            if (exists)
            {
                vHeader.Update();
            }
            else
            {
                vHeader.Insert();
            }

            vBatch.Update();
            session.CommitTransaction(tran);

            var docNumber = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value);
            var batchNumber = Convert.ToString(vHeader.Fields.FieldByName("CNTBTCH").Value);
            adjustment.DocumentNumber067 = docNumber;
            adjustment.BatchNumber033 = batchNumber;

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Adjustment Document Number : {docNumber}, Batch Number : {batchNumber}",
                DocumentNumber: docNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, adjustment);
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

            return (ProcessOut.Fail("9999", ex.Message), adjustment);
        }
    }

    public async Task<(ProcessOut Response, ARAdjustmentBatch Batch)> CreateAdjustmentBatchAsync(
        ARAdjustmentBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0041,AR0042,AR0043,AR0044,AR0406,AR0170,AR0045,AR0061", compose: true);
            dynamic vBatch = views.ViewById("AR0041");
            dynamic vHeader = views.ViewById("AR0042");

            vBatch.Fields.FieldByName("CODEPYMTYP").Value = "AD";
            vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
            vBatch.Init();
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BATCHDESC", batch.BatchDescription);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", batch.BatchDate);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "SRCEAPPL", batch.SourceApplication);
            vBatch.Update();

            if (batch.BatchEntries is { Count: > 0 })
            {
                foreach (var adjustment in batch.BatchEntries)
                {
                    vHeader.Fields.FieldByName("CODEPYMTYP").Value = "AD";
                    vHeader.RecordGenerate(false);
                    vHeader.Fields.FieldByName("RMITTYPE").Value = string.IsNullOrWhiteSpace(adjustment.ReceiptTransType052) ? "7" : adjustment.ReceiptTransType052;
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDCUST", adjustment.CustomerNumber036);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERMIT", adjustment.ReceiptDateAdjustmentDate037);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTRMIT", adjustment.EntryDescription038);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "DOCNBR", adjustment.DocumentNumber067);
                    vHeader.Insert();
                }
            }

            vBatch.Update();
            session.CommitTransaction(tran);

            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);
            batch.BatchNumber = batchNumber;
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Adjustment Batch : {batchNumber}",
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

    public async Task<(ProcessOut Response, ARAdjustment Adjustment)> ReadAdjustmentAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var adjustment = new ARAdjustment { BatchNumber033 = batchNumber, EntryNumber034 = entryNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AR0041,AR0042,AR0043,AR0044,AR0406,AR0170,AR0045,AR0061", compose: true);
            dynamic vHeader = views.ViewById("AR0042");
            dynamic vDetail = views.ViewById("AR0044");

            vHeader.Fields.FieldByName("CODEPYMTYP").Value = "AD";
            vHeader.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            vHeader.Fields.FieldByName("CNTITEM").Value = entryNumber;

            if (!(bool)vHeader.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "Adjustment not found!"), adjustment);
            }

            vHeader.Read();
            adjustment.CheckReceiptNo035 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value);
            adjustment.CustomerNumber036 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value);
            adjustment.ReceiptDateAdjustmentDate037 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?;
            adjustment.EntryDescription038 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value);
            adjustment.EntryReference039 = Convert.ToString(vHeader.Fields.FieldByName("TXTRMITREF").Value);
            adjustment.BankReceiptAmount040 = Convert.ToString(vHeader.Fields.FieldByName("AMTRMIT").Value);
            adjustment.DocumentNumber067 = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value);

            var lines = new List<ARAdjustmentLine>();
            while (vDetail.Fetch())
            {
                lines.Add(new ARAdjustmentLine
                {
                    EntryNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                    LineNumber003 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                    CustomerNumber004 = Convert.ToString(vDetail.Fields.FieldByName("IDCUST").Value),
                    DocumentNumber005 = Convert.ToString(vDetail.Fields.FieldByName("IDINVC").Value),
                    CustReceiptAmount009 = Convert.ToString(vDetail.Fields.FieldByName("AMTPAYM").Value),
                    CustDiscountAmountTaken010 = Convert.ToString(vDetail.Fields.FieldByName("AMTERNDISC").Value),
                    Description014 = Convert.ToString(vDetail.Fields.FieldByName("TEXTADJ").Value),
                    Reference015 = Convert.ToString(vDetail.Fields.FieldByName("GLREF").Value)
                });
            }

            adjustment.ARAdjustmentLines = lines;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Adjustment Document Number : {adjustment.DocumentNumber067}, Batch Number : {batchNumber}",
                DocumentNumber: adjustment.DocumentNumber067,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, adjustment);
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

            return (ProcessOut.Fail("9999", ex.Message), adjustment);
        }
    }

    public async Task<(ProcessOut Response, ARAdjustmentBatch Batch)> ReadAdjustmentBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var batch = new ARAdjustmentBatch { BatchNumber = batchNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AR0041,AR0042,AR0043,AR0044,AR0406,AR0170,AR0045,AR0061", compose: true);
            dynamic vBatch = views.ViewById("AR0041");
            dynamic vHeader = views.ViewById("AR0042");

            vBatch.Fields.FieldByName("CODEPYMTYP").Value = "AD";
            vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)vBatch.Exists)
            {
                vBatch.Read();
                batch.BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BATCHDESC").Value);
                batch.BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?;
                batch.SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value);
            }

            var entries = new List<ARAdjustment>();
            while (vHeader.Fetch())
            {
                entries.Add(new ARAdjustment
                {
                    BatchNumber033 = batchNumber,
                    EntryNumber034 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                    CheckReceiptNo035 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value),
                    CustomerNumber036 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value),
                    ReceiptDateAdjustmentDate037 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?,
                    EntryDescription038 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value),
                    DocumentNumber067 = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value)
                });
            }

            batch.BatchEntries = entries;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Adjustment Batch : {batchNumber}",
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

    public async Task<(ProcessOut Response, SyncARAdjustments Sync)> SyncAdjustmentsAsync(
        SyncARAdjustments request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var module = "AR";
        var txnType = "AD";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var recordLimit = request.RecordLimit > 0 ? request.RecordLimit : 100;

        try
        {
            var views = new Sage300ViewSet(session, "AR0041,AR0042,AR0043,AR0044,AR0406,AR0170,AR0045,AR0061,YH0301,CS0120", compose: true);
            dynamic vBatch = views.ViewById("AR0041");
            dynamic vHeader = views.ViewById("AR0042");
            dynamic yh = views.ViewById("YH0301");

            BrowseForSync(yh, module, txnType, request, timestamp);

            var batches = new List<ARAdjustmentBatch>();
            while (yh.Fetch() && batches.Count < recordLimit)
            {
                var batchNumber = Convert.ToString(yh.Fields.FieldByName("CNTBTCH").Value);
                if (string.IsNullOrWhiteSpace(batchNumber))
                {
                    continue;
                }

                vBatch.Fields.FieldByName("CODEPYMTYP").Value = txnType;
                vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
                vBatch.Read();
                if (Sage300ApPaymentService.SageViewPut.ParseInt(Convert.ToString(vBatch.Fields.FieldByName("BATCHSTAT").Value)) != 3)
                {
                    continue;
                }

                var batch = new ARAdjustmentBatch
                {
                    BatchNumber = batchNumber,
                    BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BATCHDESC").Value),
                    BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?,
                    SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value)
                };

                var entries = new List<ARAdjustment>();
                while (vHeader.Fetch())
                {
                    entries.Add(new ARAdjustment
                    {
                        BatchNumber033 = batchNumber,
                        EntryNumber034 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                        CheckReceiptNo035 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value),
                        CustomerNumber036 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value),
                        ReceiptDateAdjustmentDate037 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?,
                        EntryDescription038 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value),
                        DocumentNumber067 = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value)
                    });
                }

                batch.BatchEntries = entries;
                batches.Add(batch);
            }

            request.ARAdjustmentBatches = batches;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                "Sync AR Adjustments completed.",
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

    private static void BrowseForSync(dynamic yh, string module, string txnType, SyncARAdjustments request, string timestamp)
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
