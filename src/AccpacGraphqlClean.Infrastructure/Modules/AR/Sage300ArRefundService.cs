using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArRefundService : IArRefundService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArRefundService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARRefund Refund)> CreateOrUpdateAsync(
        ARRefund refund,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0140,AR0141,AR0142,AR0143,AR0145", compose: true);
            dynamic vBatch = views.ViewById("AR0140");
            dynamic vHeader = views.ViewById("AR0141");
            dynamic vDetail = views.ViewById("AR0142");

            vBatch.RecordGenerate(false);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "CNTBTCH", refund.BatchNumber000);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "BTCHDATE", refund.BatchDate001);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BTCHDESC", refund.BatchDescription002);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BTCHTYPE", refund.BatchType003);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BTCHSTTS", refund.BatchStatus004);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "SWPRINTED", refund.BatchPrintedFlag012);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "SRCEAPPL", refund.SourceApplication013);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "CNTCHKPRNT", refund.NumberofPrintedChecks015);
            vBatch.Insert();

            if (!string.IsNullOrWhiteSpace(refund.DocumentNumber023))
            {
                vHeader.Fields.FieldByName("IDINVC").Value = refund.DocumentNumber023;
                vHeader.Order = 2;
            }

            var exists = (bool)vHeader.Exists;
            if (exists)
            {
                vHeader.Read();
            }
            else
            {
                vHeader.RecordGenerate(false);
            }

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CNTITEM", refund.EntryNumber017);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "DOCDESC", refund.DocumentDescription018);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DOCDATE", refund.DocumentDate019);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDCUST", refund.CustomerNumber022);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDINVC", refund.DocumentNumber023);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPE", refund.RateType027);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "RATEDATE", refund.RateDate028);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCH", refund.ExchangeRate029);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATE", refund.RateOverrideFlag031);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "APPLYMETH", refund.JobApplyMethod035);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SRCEAPPL", refund.SourceApplication037);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDBANKCA", refund.CashBankAccount040);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTCA", refund.CashGLAccount041);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECURNCA", refund.CashPaymentCurrency042);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPECA", refund.CashRateType043);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "RATEDATECA", refund.CashRateDate044);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCHCA", refund.CashExchangeRate045);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATECA", refund.CashRateOverrideFlag047);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDBANKCK", refund.CheckBankAccount051);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWPRINT", refund.CheckPrintingRequired052);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWPRINTED", refund.CheckHasBeenPrinted053);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CHECKNUM", refund.CheckNumber054);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECURNCK", refund.CheckPaymentCurrency056);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPECK", refund.CheckRateType057);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "RATEDATECK", refund.CheckRateDate058);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCHCK", refund.CheckExchangeRate059);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATECK", refund.CheckRateOverrideFlag061);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "NAMERMIT", refund.RemitToName065);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE1", refund.AddressLine1066);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE2", refund.AddressLine2067);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE3", refund.AddressLine3068);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE4", refund.AddressLine4069);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "NAMECITY", refund.City070);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODESTTE", refund.StateProv071);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODEPSTL", refund.ZipPostalCode072);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECTRY", refund.Country073);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CHECKLANG", refund.CheckLanguage074);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "ENTEREDBY", refund.EnteredBy077);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEBUS", refund.PostingDate078);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CCPREVID", refund.PreviousCCTransactionNumber081);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CCPREVSTTS", refund.PreviousCCProcessStatus082);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CCTRANID", refund.CurrentCCTransactionNumber083);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CCTRANSTTS", refund.CurrentCCProcessStatus084);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "PROCESSCOD", refund.ProcessingCode085);

            vHeader.Fields.FieldByName("PROCESSCMD").PutWithoutVerification("0");
            vHeader.Process();

            if (refund.ARRefundItems is { Count: > 0 })
            {
                foreach (var dtl in refund.ARRefundItems)
                {
                    vDetail.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTITEM", dtl.EntryNumber001);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTLINE", dtl.LineNumber002);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDINVC", dtl.DocumentNumber003);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CNTPAYM", dtl.PaymentNumber004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "PAYMTYPE", dtl.PaymentType005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "IDBANK", dtl.CCBankAccount006);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CODECURN", dtl.CCPaymentCurrency008);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "RATETYPE", dtl.CCRateType009);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vDetail, "RATEDATE", dtl.CCRateDate010);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "RATEEXCH", dtl.CCExchangeRate011);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "SWRATE", dtl.CCRateOverrideFlag013);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTPC", dtl.AmountPayment014);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "APPLYMETH", dtl.JobApplyMethod018);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CCRCPTNO", dtl.ReceiptDocumentNumber029);
                    vDetail.Fields.FieldByName("PROCESSCMD").PutWithoutVerification("1");
                    vDetail.Process();
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

            refund.DocumentNumber023 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value);
            refund.BatchNumber016 = Convert.ToString(vHeader.Fields.FieldByName("CNTBTCH").Value);
            refund.EntryNumber017 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Refund Number : {refund.DocumentNumber023}",
                DocumentNumber: refund.DocumentNumber023,
                BatchNumber: refund.BatchNumber016,
                ErrorCode: "0000");

            return (response, refund);
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

            return (ProcessOut.Fail("9999", ex.Message), refund);
        }
    }

    public async Task<(ProcessOut Response, ARRefundBatch Batch)> CreateRefundBatchAsync(
        ARRefundBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0140,AR0141,AR0142,AR0143,AR0145", compose: true);
            dynamic vBatch = views.ViewById("AR0140");
            dynamic vHeader = views.ViewById("AR0141");

            vBatch.Fields.FieldByName("CNTBTCH").PutWithoutVerification("0");
            vBatch.RecordGenerate(false);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", batch.BatchDate);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BTCHDESC", batch.BatchDescription);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "SRCEAPPL", batch.SourceApplication);
            vBatch.Update();

            if (batch.BatchEntries is { Count: > 0 })
            {
                foreach (var refund in batch.BatchEntries)
                {
                    vHeader.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CNTITEM", refund.EntryNumber017);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "DOCDESC", refund.DocumentDescription018);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DOCDATE", refund.DocumentDate019);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDCUST", refund.CustomerNumber022);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDINVC", refund.DocumentNumber023);
                    vHeader.Insert();
                }
            }

            vBatch.Update();
            session.CommitTransaction(tran);

            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);
            batch.BatchNumber = batchNumber;

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Refund Batch Number : {batchNumber}",
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

    public async Task<(ProcessOut Response, ARRefund Refund)> ReadRefundAsync(
        string documentNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var refund = new ARRefund { DocumentNumber023 = documentNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AR0140,AR0141,AR0142,AR0143,AR0145", compose: true);
            dynamic vHeader = views.ViewById("AR0141");
            dynamic vDetail = views.ViewById("AR0142");

            vHeader.Fields.FieldByName("IDINVC").Value = documentNumber;
            vHeader.Order = 2;

            if (!(bool)vHeader.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "Refund not found!"), refund);
            }

            vHeader.Read();
            refund.BatchNumber016 = Convert.ToString(vHeader.Fields.FieldByName("CNTBTCH").Value);
            refund.EntryNumber017 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value);
            refund.DocumentDescription018 = Convert.ToString(vHeader.Fields.FieldByName("DOCDESC").Value);
            refund.DocumentDate019 = vHeader.Fields.FieldByName("DOCDATE").Value as DateTime?;
            refund.CustomerNumber022 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value);
            refund.DocumentNumber023 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value);
            refund.RateType027 = Convert.ToString(vHeader.Fields.FieldByName("RATETYPE").Value);
            refund.RateDate028 = vHeader.Fields.FieldByName("RATEDATE").Value as DateTime?;
            refund.ExchangeRate029 = Convert.ToString(vHeader.Fields.FieldByName("RATEEXCH").Value);
            refund.RateOverrideFlag031 = Convert.ToString(vHeader.Fields.FieldByName("SWRATE").Value);
            refund.JobApplyMethod035 = Convert.ToString(vHeader.Fields.FieldByName("APPLYMETH").Value);
            refund.SourceApplication037 = Convert.ToString(vHeader.Fields.FieldByName("SRCEAPPL").Value);
            refund.CashBankAccount040 = Convert.ToString(vHeader.Fields.FieldByName("IDBANKCA").Value);
            refund.CashGLAccount041 = Convert.ToString(vHeader.Fields.FieldByName("IDACCTCA").Value);
            refund.CashPaymentCurrency042 = Convert.ToString(vHeader.Fields.FieldByName("CODECURNCA").Value);
            refund.CashRateType043 = Convert.ToString(vHeader.Fields.FieldByName("RATETYPECA").Value);
            refund.CashRateDate044 = vHeader.Fields.FieldByName("RATEDATECA").Value as DateTime?;
            refund.CashExchangeRate045 = Convert.ToString(vHeader.Fields.FieldByName("RATEEXCHCA").Value);
            refund.CashRateOverrideFlag047 = Convert.ToString(vHeader.Fields.FieldByName("SWRATECA").Value);
            refund.CheckBankAccount051 = Convert.ToString(vHeader.Fields.FieldByName("IDBANKCK").Value);
            refund.CheckPrintingRequired052 = Convert.ToString(vHeader.Fields.FieldByName("SWPRINT").Value);
            refund.CheckHasBeenPrinted053 = Convert.ToString(vHeader.Fields.FieldByName("SWPRINTED").Value);
            refund.CheckNumber054 = Convert.ToString(vHeader.Fields.FieldByName("CHECKNUM").Value);
            refund.CheckPaymentCurrency056 = Convert.ToString(vHeader.Fields.FieldByName("CODECURNCK").Value);
            refund.CheckRateType057 = Convert.ToString(vHeader.Fields.FieldByName("RATETYPECK").Value);
            refund.CheckRateDate058 = vHeader.Fields.FieldByName("RATEDATECK").Value as DateTime?;
            refund.CheckExchangeRate059 = Convert.ToString(vHeader.Fields.FieldByName("RATEEXCHCK").Value);
            refund.CheckRateOverrideFlag061 = Convert.ToString(vHeader.Fields.FieldByName("SWRATECK").Value);
            refund.RemitToName065 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value);
            refund.AddressLine1066 = Convert.ToString(vHeader.Fields.FieldByName("TEXTSTRE1").Value);
            refund.AddressLine2067 = Convert.ToString(vHeader.Fields.FieldByName("TEXTSTRE2").Value);
            refund.AddressLine3068 = Convert.ToString(vHeader.Fields.FieldByName("TEXTSTRE3").Value);
            refund.AddressLine4069 = Convert.ToString(vHeader.Fields.FieldByName("TEXTSTRE4").Value);
            refund.City070 = Convert.ToString(vHeader.Fields.FieldByName("NAMECITY").Value);
            refund.StateProv071 = Convert.ToString(vHeader.Fields.FieldByName("CODESTTE").Value);
            refund.ZipPostalCode072 = Convert.ToString(vHeader.Fields.FieldByName("CODEPSTL").Value);
            refund.Country073 = Convert.ToString(vHeader.Fields.FieldByName("CODECTRY").Value);
            refund.CheckLanguage074 = Convert.ToString(vHeader.Fields.FieldByName("CHECKLANG").Value);
            refund.EnteredBy077 = Convert.ToString(vHeader.Fields.FieldByName("ENTEREDBY").Value);
            refund.PostingDate078 = vHeader.Fields.FieldByName("DATEBUS").Value as DateTime?;
            refund.PreviousCCTransactionNumber081 = Convert.ToString(vHeader.Fields.FieldByName("CCPREVID").Value);
            refund.PreviousCCProcessStatus082 = Convert.ToString(vHeader.Fields.FieldByName("CCPREVSTTS").Value);
            refund.CurrentCCTransactionNumber083 = Convert.ToString(vHeader.Fields.FieldByName("CCTRANID").Value);
            refund.CurrentCCProcessStatus084 = Convert.ToString(vHeader.Fields.FieldByName("CCTRANSTTS").Value);
            refund.ProcessingCode085 = Convert.ToString(vHeader.Fields.FieldByName("PROCESSCOD").Value);

            var items = new List<ARRefundItem>();
            while (vDetail.Fetch())
            {
                items.Add(new ARRefundItem
                {
                    EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                    LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                    DocumentNumber003 = Convert.ToString(vDetail.Fields.FieldByName("IDINVC").Value),
                    PaymentNumber004 = Convert.ToString(vDetail.Fields.FieldByName("CNTPAYM").Value),
                    PaymentType005 = Convert.ToString(vDetail.Fields.FieldByName("PAYMTYPE").Value),
                    CCBankAccount006 = Convert.ToString(vDetail.Fields.FieldByName("IDBANK").Value),
                    CCPaymentCurrency008 = Convert.ToString(vDetail.Fields.FieldByName("CODECURN").Value),
                    CCRateType009 = Convert.ToString(vDetail.Fields.FieldByName("RATETYPE").Value),
                    CCRateDate010 = vDetail.Fields.FieldByName("RATEDATE").Value as DateTime?,
                    CCExchangeRate011 = Convert.ToString(vDetail.Fields.FieldByName("RATEEXCH").Value),
                    CCRateOverrideFlag013 = Convert.ToString(vDetail.Fields.FieldByName("SWRATE").Value),
                    AmountPayment014 = Convert.ToString(vDetail.Fields.FieldByName("AMTPC").Value),
                    JobApplyMethod018 = Convert.ToString(vDetail.Fields.FieldByName("APPLYMETH").Value),
                    ReceiptDocumentNumber029 = Convert.ToString(vDetail.Fields.FieldByName("CCRCPTNO").Value)
                });
            }

            refund.ARRefundItems = items;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Refund Number : {refund.DocumentNumber023}",
                DocumentNumber: refund.DocumentNumber023,
                BatchNumber: refund.BatchNumber016,
                ErrorCode: "0000");

            return (response, refund);
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

            return (ProcessOut.Fail("9999", ex.Message), refund);
        }
    }

    public async Task<(ProcessOut Response, ARRefundBatch Batch)> ReadRefundBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var batch = new ARRefundBatch { BatchNumber = batchNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AR0140,AR0141,AR0142,AR0143,AR0145", compose: true);
            dynamic vBatch = views.ViewById("AR0140");
            dynamic vHeader = views.ViewById("AR0141");
            dynamic vDetail = views.ViewById("AR0142");

            vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)vBatch.Exists)
            {
                vBatch.Read();
                batch.BatchDate = vBatch.Fields.FieldByName("BTCHDATE").Value as DateTime?;
                batch.BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BTCHDESC").Value);
                batch.SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value);
            }

            var entries = new List<ARRefund>();
            while (vHeader.Fetch())
            {
                var refund = new ARRefund
                {
                    BatchNumber016 = Convert.ToString(vHeader.Fields.FieldByName("CNTBTCH").Value),
                    EntryNumber017 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                    DocumentDescription018 = Convert.ToString(vHeader.Fields.FieldByName("DOCDESC").Value),
                    DocumentDate019 = vHeader.Fields.FieldByName("DOCDATE").Value as DateTime?,
                    CustomerNumber022 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value),
                    DocumentNumber023 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value)
                };

                var items = new List<ARRefundItem>();
                while (vDetail.Fetch())
                {
                    items.Add(new ARRefundItem
                    {
                        EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                        LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                        DocumentNumber003 = Convert.ToString(vDetail.Fields.FieldByName("IDINVC").Value),
                        PaymentNumber004 = Convert.ToString(vDetail.Fields.FieldByName("CNTPAYM").Value),
                        PaymentType005 = Convert.ToString(vDetail.Fields.FieldByName("PAYMTYPE").Value),
                        AmountPayment014 = Convert.ToString(vDetail.Fields.FieldByName("AMTPC").Value)
                    });
                }

                refund.ARRefundItems = items;
                entries.Add(refund);
            }

            batch.BatchEntries = entries;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AR Refund Batch Number : {batchNumber} with {entries.Count} Entries",
                DocumentNumber: entries.Count.ToString(),
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

    public async Task<(ProcessOut Response, SyncARRefunds Sync)> SyncRefundsAsync(
        SyncARRefunds request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var module = "AR";
        var txnType = "RF";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var recordLimit = request.RecordLimit > 0 ? request.RecordLimit : 100;

        try
        {
            var views = new Sage300ViewSet(session, "AR0140,AR0141,AR0142,AR0143,AR0145,YH0301,CS0120", compose: true);
            dynamic vBatch = views.ViewById("AR0140");
            dynamic vHeader = views.ViewById("AR0141");
            dynamic vDetail = views.ViewById("AR0142");
            dynamic yh = views.ViewById("YH0301");

            BrowseForSync(yh, module, txnType, request, timestamp);

            var batches = new List<ARRefundBatch>();
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

                var batch = new ARRefundBatch
                {
                    BatchNumber = batchNumber,
                    BatchDate = vBatch.Fields.FieldByName("BTCHDATE").Value as DateTime?,
                    BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BTCHDESC").Value),
                    SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value)
                };

                var entries = new List<ARRefund>();
                while (vHeader.Fetch())
                {
                    var refund = new ARRefund
                    {
                        EntryNumber017 = Convert.ToString(vHeader.Fields.FieldByName("CNTITEM").Value),
                        DocumentNumber023 = Convert.ToString(vHeader.Fields.FieldByName("IDINVC").Value),
                        DocumentDescription018 = Convert.ToString(vHeader.Fields.FieldByName("DOCDESC").Value),
                        DocumentDate019 = vHeader.Fields.FieldByName("DOCDATE").Value as DateTime?,
                        CustomerNumber022 = Convert.ToString(vHeader.Fields.FieldByName("IDCUST").Value)
                    };

                    var items = new List<ARRefundItem>();
                    while (vDetail.Fetch())
                    {
                        items.Add(new ARRefundItem
                        {
                            EntryNumber001 = Convert.ToString(vDetail.Fields.FieldByName("CNTITEM").Value),
                            LineNumber002 = Convert.ToString(vDetail.Fields.FieldByName("CNTLINE").Value),
                            DocumentNumber003 = Convert.ToString(vDetail.Fields.FieldByName("IDINVC").Value),
                            PaymentNumber004 = Convert.ToString(vDetail.Fields.FieldByName("CNTPAYM").Value),
                            PaymentType005 = Convert.ToString(vDetail.Fields.FieldByName("PAYMTYPE").Value),
                            AmountPayment014 = Convert.ToString(vDetail.Fields.FieldByName("AMTPC").Value),
                            ReceiptDocumentNumber029 = Convert.ToString(vDetail.Fields.FieldByName("CCRCPTNO").Value)
                        });
                    }

                    refund.ARRefundItems = items;
                    entries.Add(refund);
                }

                batch.BatchEntries = entries;
                batches.Add(batch);

                yh.Fields.FieldByName("TIMESTAMP").Value = request.Timestamp;
                yh.Update();
            }

            request.ARRefundBatches = batches;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                "Sync AR Refunds completed.",
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

    private static void BrowseForSync(dynamic yh, string module, string txnType, SyncARRefunds request, string timestamp)
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
