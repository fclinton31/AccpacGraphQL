using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApPaymentService : IApPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ApPaymentService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, APPayment Payment)> CreatePaymentAsync(
        APPayment payment,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406", compose: true);
            dynamic vBatch = views.ViewById("AP0030");
            dynamic vHeader = views.ViewById("AP0031");
            dynamic vMisc = views.ViewById("AP0032");
            dynamic vApplied = views.ViewById("AP0033");

            var sourceApp = !string.IsNullOrWhiteSpace(payment.SourceApplication062) ? payment.SourceApplication062 : "YH";
            vBatch.Fields.FieldByName("BATCHSTAT").Value = "1";
            vBatch.Fields.FieldByName("PAYMTYPE").Value = "PY";

            var batchFilter = $"SRCEAPPL = \"{sourceApp}\" AND PAYMTYPE = \"PY\" AND BATCHSTAT = 1";
            vBatch.Browse(batchFilter, true);

            var hasOpenBatch = (bool)vBatch.Fetch();
            if (!hasOpenBatch)
            {
                vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
                vBatch.Init();
                SageViewPut.PutIfNotNull(vBatch, "BATCHDESC", payment.BatchDescription);
                SageViewPut.PutIfDate(vBatch, "DATEBTCH", payment.BatchDate);
                SageViewPut.PutIfNotNull(vBatch, "IDBANK", payment.BankCode063);
                SageViewPut.PutIfNotNull(vBatch, "SRCEAPPL", sourceApp);
                vBatch.Update();
            }

            vHeader.RecordGenerate(false);
            SageViewPut.PutIfNotNull(vHeader, "IDVEND", payment.VendorNumber004);
            SageViewPut.PutIfDate(vHeader, "DATERMIT", payment.PaymentDateAdjustmentDate005);
            SageViewPut.PutIfNotNull(vHeader, "TEXTRMIT", payment.EntryDescription006);
            SageViewPut.PutIfNotNull(vHeader, "NAMERMIT", payment.VendorPayeeName007);
            SageViewPut.PutIfNotNull(vHeader, "RATEEXCHTC", payment.VendorExchangeRate010);
            SageViewPut.PutIfNotNull(vHeader, "SWRATETC", payment.VendorRateOverridden011);
            SageViewPut.PutIfNotNull(vHeader, "AMTPPAYTC", payment.TotalPrepayVendorCurr013);
            SageViewPut.PutIfNotNull(vHeader, "PAYMCODE", payment.PaymentCode015);
            SageViewPut.PutIfNotNull(vHeader, "RATETYPEHC", payment.BankRateType017);
            SageViewPut.PutIfNotNull(vHeader, "RATEEXCHHC", payment.BankExchangeRate018);
            SageViewPut.PutIfNotNull(vHeader, "SWRATEHC", payment.BankRateOverridden019);
            SageViewPut.PutIfNotNull(vHeader, "RMITTYPE", payment.PaymentTransType020);
            SageViewPut.PutIfNotNull(vHeader, "DOCTYPE", payment.DocumentType021);
            SageViewPut.PutIfDate(vHeader, "DATERATETC", payment.VendorRateDate025);
            SageViewPut.PutIfNotNull(vHeader, "RATETYPETC", payment.VendorRateType026);
            SageViewPut.PutIfDate(vHeader, "DATERATEHC", payment.BankRateDate028);
            SageViewPut.PutIfNotNull(vHeader, "DOCNBR", payment.DocumentNumber032);
            SageViewPut.PutIfNotNull(vHeader, "PAYMSTTS", payment.PaymentEdited033);
            SageViewPut.PutIfNotNull(vHeader, "SWPRNTRMIT", payment.CheckPrintRequired034);
            SageViewPut.PutIfNotNull(vHeader, "IDRMITTO", payment.VendorRemitToLocation035);
            SageViewPut.PutIfNotNull(vHeader, "TXTRMITREF", payment.EntryReference036);
            SageViewPut.PutIfNotNull(vHeader, "SWPRINTED", payment.CheckPrintedStatus040);
            SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE1", payment.AddressLine1041);
            SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE2", payment.AddressLine2042);
            SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE3", payment.AddressLine3043);
            SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE4", payment.AddressLine4044);
            SageViewPut.PutIfNotNull(vHeader, "NAMECITY", payment.City045);
            SageViewPut.PutIfNotNull(vHeader, "CODESTTE", payment.State046);
            SageViewPut.PutIfNotNull(vHeader, "CODEPSTL", payment.ZipPostalCode047);
            SageViewPut.PutIfNotNull(vHeader, "CODECTRY", payment.Country048);
            SageViewPut.PutIfNotNull(vHeader, "CHECKLANG", payment.PaymentLanguage049);
            SageViewPut.PutIfDate(vHeader, "DATEACTVPP", payment.PrepayActivationDate054);
            SageViewPut.PutIfNotNull(vHeader, "SWJOB", payment.JobRelated055);
            SageViewPut.PutIfNotNull(vHeader, "APPLYMETH", payment.JobApplyMethod056);
            SageViewPut.PutIfNotNull(vHeader, "IDINVCMTCH", payment.MatchingDocumentNumber059);
            SageViewPut.PutIfNotNull(vHeader, "SRCEAPPL", payment.SourceApplication062);
            SageViewPut.PutIfNotNull(vHeader, "CODECURNBC", payment.BankCurrencyCode064);
            SageViewPut.PutIfNotNull(vHeader, "CASHACCT", payment.CashAccount066);
            SageViewPut.PutIfNotNull(vHeader, "CODE1099", payment.S1099CPRSCode070);
            SageViewPut.PutIfNotNull(vHeader, "AMT1099", payment.S1099CPRSAmount071);
            SageViewPut.PutIfNotNull(vHeader, "SWTXAMTCTL", payment.CalculateTaxAmountControl072);
            SageViewPut.PutIfNotNull(vHeader, "SWTXBSECTL", payment.CalculateTaxBaseControl073);
            SageViewPut.PutIfNotNull(vHeader, "CODETAXGRP", payment.TaxGroup074);
            SageViewPut.PutIfNotNull(vHeader, "TAXCLASS1", payment.TaxClass1081);
            SageViewPut.PutIfNotNull(vHeader, "TAXCLASS2", payment.TaxClass2082);
            SageViewPut.PutIfNotNull(vHeader, "TAXCLASS3", payment.TaxClass3083);
            SageViewPut.PutIfNotNull(vHeader, "TAXCLASS4", payment.TaxClass4084);
            SageViewPut.PutIfNotNull(vHeader, "TAXCLASS5", payment.TaxClass5085);
            SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL1", payment.TaxIncluded1086);
            SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL2", payment.TaxIncluded2087);
            SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL3", payment.TaxIncluded3088);
            SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL4", payment.TaxIncluded4089);
            SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL5", payment.TaxIncluded5090);
            SageViewPut.PutIfNotNull(vHeader, "TXBSE1TC", payment.TaxBase1091);
            SageViewPut.PutIfNotNull(vHeader, "TXBSE2TC", payment.TaxBase2092);
            SageViewPut.PutIfNotNull(vHeader, "TXBSE3TC", payment.TaxBase3093);
            SageViewPut.PutIfNotNull(vHeader, "TXBSE4TC", payment.TaxBase4094);
            SageViewPut.PutIfNotNull(vHeader, "TXBSE5TC", payment.TaxBase5095);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT1TC", payment.TaxAmount1096);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT2TC", payment.TaxAmount2097);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT3TC", payment.TaxAmount3098);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT4TC", payment.TaxAmount4099);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT5TC", payment.TaxAmount5100);
            SageViewPut.PutIfNotNull(vHeader, "CODECURNRC", payment.TaxReportingCurrencyCode106);
            SageViewPut.PutIfNotNull(vHeader, "SWTXCTLRC", payment.TaxReportingCalculateMethod107);
            SageViewPut.PutIfNotNull(vHeader, "RATERC", payment.TaxReportingExchangeRate108);
            SageViewPut.PutIfNotNull(vHeader, "RATETYPERC", payment.TaxReportingRateType109);
            SageViewPut.PutIfDate(vHeader, "RATEDATERC", payment.TaxReportingRateDate110);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT1RC", payment.TaxReportingAmount1113);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT2RC", payment.TaxReportingAmount2114);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT3RC", payment.TaxReportingAmount3115);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT4RC", payment.TaxReportingAmount4116);
            SageViewPut.PutIfNotNull(vHeader, "TXAMT5RC", payment.TaxReportingAmount5117);
            SageViewPut.PutIfNotNull(vHeader, "ENTEREDBY", payment.EnteredBy144);
            SageViewPut.PutIfDate(vHeader, "DATEBUS", payment.PostingDate145);
            SageViewPut.PutIfNotNull(vHeader, "IDACCTSET", payment.AccountSet146);

            var transType = SageViewPut.ParseInt(payment.PaymentTransType020);
            if (transType != 4)
            {
                if (payment.APPaymentItems is { Count: > 0 })
                {
                    foreach (var dtl in payment.APPaymentItems)
                    {
                        vApplied.RecordGenerate(false);
                        SageViewPut.PutIfNotNull(vApplied, "IDINVC", dtl.DocumentNumber005);
                        SageViewPut.PutIfNotNull(vApplied, "CNTPAYM", dtl.PaymentNumber006);
                        SageViewPut.PutIfNotNull(vApplied, "TRXTYPE", dtl.TransactionType007);
                        SageViewPut.PutIfNotNull(vApplied, "PYMTRESL", dtl.PaymentResolution008);
                        SageViewPut.PutIfNotNull(vApplied, "AMTPAYM", dtl.PaymentAmount009);
                        SageViewPut.PutIfNotNull(vApplied, "AMTERNDISC", dtl.DiscountAmountTaken010);
                        SageViewPut.PutIfNotNull(vApplied, "TEXTADJ", dtl.Description014);
                        SageViewPut.PutIfNotNull(vApplied, "GLREF", dtl.Reference015);
                        SageViewPut.PutIfNotNull(vApplied, "IDDOCMTCH", dtl.PPMatchingDocNo017);
                        SageViewPut.PutIfNotNull(vApplied, "CDAPPLYTO", dtl.PPMatchingDocType018);
                        SageViewPut.PutIfDate(vApplied, "DATEACTVPP", dtl.ActivationDate019);
                        SageViewPut.PutIfNotNull(vApplied, "APPLYMETH", dtl.JobApplyMethod031);
                        SageViewPut.PutIfNotNull(vApplied, "RTGAMT", dtl.RetainageAmount034);
                        SageViewPut.PutIfDate(vApplied, "RTGDATEDUE", dtl.RetainageDueDate035);
                        SageViewPut.PutIfNotNull(vApplied, "RTGTERMS", dtl.RetainageTermsCode036);
                        SageViewPut.PutIfNotNull(vApplied, "SWRTGRATE", dtl.RetainageExchangeRate037);
                        SageViewPut.PutIfNotNull(vApplied, "DOCTYPE", dtl.DocumentType043);
                        vApplied.Insert();
                    }
                }

                if (!string.IsNullOrWhiteSpace(payment.TotalPrepayVendorCurr013) && string.Equals(payment.PaymentTransType020, "2", StringComparison.OrdinalIgnoreCase))
                {
                    vHeader.Fields.FieldByName("AMTPPAYTC").Value = payment.TotalPrepayVendorCurr013;
                }
            }
            else
            {
                if (payment.APMiscPaymentItems is { Count: > 0 })
                {
                    foreach (var dtl in payment.APMiscPaymentItems)
                    {
                        vMisc.RecordGenerate(false);
                        SageViewPut.PutIfNotNull(vMisc, "IDDISTCODE", dtl.DistributionCode004);
                        SageViewPut.PutIfNotNull(vMisc, "IDACCT", dtl.AccountNumber005);
                        SageViewPut.PutIfNotNull(vMisc, "GLREF", dtl.GLReference006);
                        SageViewPut.PutIfNotNull(vMisc, "GLDESC", dtl.GLDescription007);
                        SageViewPut.PutIfNotNull(vMisc, "TAXCLASS1", dtl.TaxClass1008);
                        SageViewPut.PutIfNotNull(vMisc, "TAXCLASS2", dtl.TaxClass2009);
                        SageViewPut.PutIfNotNull(vMisc, "TAXCLASS3", dtl.TaxClass3010);
                        SageViewPut.PutIfNotNull(vMisc, "TAXCLASS4", dtl.TaxClass4011);
                        SageViewPut.PutIfNotNull(vMisc, "TAXCLASS5", dtl.TaxClass5012);
                        SageViewPut.PutIfNotNull(vMisc, "SWTAXINCL1", dtl.TaxIncluded1013);
                        SageViewPut.PutIfNotNull(vMisc, "SWTAXINCL2", dtl.TaxIncluded2014);
                        SageViewPut.PutIfNotNull(vMisc, "SWTAXINCL3", dtl.TaxIncluded3015);
                        SageViewPut.PutIfNotNull(vMisc, "SWTAXINCL4", dtl.TaxIncluded4016);
                        SageViewPut.PutIfNotNull(vMisc, "SWTAXINCL5", dtl.TaxIncluded5017);
                        SageViewPut.PutIfNotNull(vMisc, "TXBSE1TC", dtl.TaxBase1018);
                        SageViewPut.PutIfNotNull(vMisc, "TXBSE2TC", dtl.TaxBase2019);
                        SageViewPut.PutIfNotNull(vMisc, "TXBSE3TC", dtl.TaxBase3020);
                        SageViewPut.PutIfNotNull(vMisc, "TXBSE4TC", dtl.TaxBase4021);
                        SageViewPut.PutIfNotNull(vMisc, "TXBSE5TC", dtl.TaxBase5022);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT1TC", dtl.TaxAmount1028);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT2TC", dtl.TaxAmount2029);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT3TC", dtl.TaxAmount3030);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT4TC", dtl.TaxAmount4031);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT5TC", dtl.TaxAmount5032);
                        SageViewPut.PutIfNotNull(vMisc, "AMTDISTTC", dtl.DistAmount034);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT1RC", dtl.TaxReportingAmount1054);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT2RC", dtl.TaxReportingAmount2055);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT3RC", dtl.TaxReportingAmount3056);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT4RC", dtl.TaxReportingAmount4057);
                        SageViewPut.PutIfNotNull(vMisc, "TXAMT5RC", dtl.TaxReportingAmount5058);
                        SageViewPut.PutIfNotNull(vMisc, "CONTRACT", dtl.Contract098);
                        SageViewPut.PutIfNotNull(vMisc, "PROJECT", dtl.Project099);
                        SageViewPut.PutIfNotNull(vMisc, "CATEGORY", dtl.Category100);
                        SageViewPut.PutIfNotNull(vMisc, "RESOURCE", dtl.ProjectCategoryResource101);
                        SageViewPut.PutIfNotNull(vMisc, "BILLTYPE", dtl.BillingType103);
                        SageViewPut.PutIfNotNull(vMisc, "IDITEM", dtl.ItemNumber104);
                        SageViewPut.PutIfNotNull(vMisc, "UNITMEAS", dtl.UnitofMeasure105);
                        SageViewPut.PutIfNotNull(vMisc, "QTYINVC", dtl.Quantity106);
                        SageViewPut.PutIfNotNull(vMisc, "AMTCOST", dtl.Cost107);
                        SageViewPut.PutIfDate(vMisc, "BILLDATE", dtl.BillingDate108);
                        SageViewPut.PutIfNotNull(vMisc, "BILLRATE", dtl.BillingRate109);
                        vMisc.Insert();
                    }
                }
            }

            SageViewPut.PutIfNotNull(vHeader, "IDRMIT", payment.CheckNumber003);
            vHeader.Insert();
            vBatch.Update();

            session.CommitTransaction(tran);

            var documentNumber = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value);
            var batchNumber = Convert.ToString(vHeader.Fields.FieldByName("CNTBTCH").Value);
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Payment Document Number : {documentNumber}, Batch Number : {batchNumber}",
                DocumentNumber: documentNumber,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, payment);
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

            return (ProcessOut.Fail("9999", ex.Message), payment);
        }
    }

    public async Task<(ProcessOut Response, APPaymentBatch Batch)> CreatePaymentBatchAsync(
        APPaymentBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406", compose: true);
            dynamic vBatch = views.ViewById("AP0030");
            dynamic vHeader = views.ViewById("AP0031");
            dynamic vMisc = views.ViewById("AP0032");
            dynamic vApplied = views.ViewById("AP0033");

            vBatch.Fields.FieldByName("PAYMTYPE").Value = "PY";
            vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
            vBatch.Init();
            SageViewPut.PutIfNotNull(vBatch, "BATCHDESC", batch.BatchDescription);
            SageViewPut.PutIfDate(vBatch, "DATEBTCH", batch.BatchDate);
            SageViewPut.PutIfNotNull(vBatch, "IDBANK", batch.BankCode);
            SageViewPut.PutIfNotNull(vBatch, "CODECURN", batch.CurrencyCode);
            vBatch.Update();

            if (batch.BatchEntries is { Count: > 0 })
            {
                foreach (var payment in batch.BatchEntries)
                {
                    vHeader.RecordGenerate(false);
                    SageViewPut.PutIfNotNull(vHeader, "IDVEND", payment.VendorNumber004);
                    SageViewPut.PutIfDate(vHeader, "DATERMIT", payment.PaymentDateAdjustmentDate005);
                    SageViewPut.PutIfNotNull(vHeader, "TEXTRMIT", payment.EntryDescription006);
                    SageViewPut.PutIfNotNull(vHeader, "NAMERMIT", payment.VendorPayeeName007);
                    SageViewPut.PutIfNotNull(vHeader, "RMITTYPE", payment.PaymentTransType020);
                    SageViewPut.PutIfNotNull(vHeader, "DOCTYPE", payment.DocumentType021);
                    SageViewPut.PutIfNotNull(vHeader, "DOCNBR", payment.DocumentNumber032);

                    var transType = SageViewPut.ParseInt(payment.PaymentTransType020);
                    if (transType != 4)
                    {
                        if (payment.APPaymentItems is { Count: > 0 })
                        {
                            foreach (var dtl in payment.APPaymentItems)
                            {
                                vApplied.RecordGenerate(false);
                                SageViewPut.PutIfNotNull(vApplied, "IDINVC", dtl.DocumentNumber005);
                                SageViewPut.PutIfNotNull(vApplied, "CNTPAYM", dtl.PaymentNumber006);
                                SageViewPut.PutIfNotNull(vApplied, "TRXTYPE", dtl.TransactionType007);
                                SageViewPut.PutIfNotNull(vApplied, "PYMTRESL", dtl.PaymentResolution008);
                                SageViewPut.PutIfNotNull(vApplied, "AMTPAYM", dtl.PaymentAmount009);
                                SageViewPut.PutIfNotNull(vApplied, "AMTERNDISC", dtl.DiscountAmountTaken010);
                                SageViewPut.PutIfNotNull(vApplied, "TEXTADJ", dtl.Description014);
                                SageViewPut.PutIfNotNull(vApplied, "GLREF", dtl.Reference015);
                                vApplied.Insert();
                            }
                        }
                    }
                    else
                    {
                        if (payment.APMiscPaymentItems is { Count: > 0 })
                        {
                            foreach (var dtl in payment.APMiscPaymentItems)
                            {
                                vMisc.RecordGenerate(false);
                                SageViewPut.PutIfNotNull(vMisc, "IDDISTCODE", dtl.DistributionCode004);
                                SageViewPut.PutIfNotNull(vMisc, "IDACCT", dtl.AccountNumber005);
                                SageViewPut.PutIfNotNull(vMisc, "AMTDISTTC", dtl.DistAmount034);
                                vMisc.Insert();
                            }
                        }
                    }

                    SageViewPut.PutIfNotNull(vHeader, "IDRMIT", payment.CheckNumber003);
                    vHeader.Insert();
                }
            }

            vBatch.Update();

            if (SageViewPut.IsTruthy(batch.PostBatch))
            {
                dynamic post = session.OpenView("AP0040");
                post.Fields.FieldByName("TYPEBTCH").Value = "PY";
                post.Fields.FieldByName("BATCHIDFR").Value = vBatch.Fields.FieldByName("CNTBTCH").Value;
                post.Fields.FieldByName("BATCHIDTO").Value = vBatch.Fields.FieldByName("CNTBTCH").Value;
                post.Process();
            }

            session.CommitTransaction(tran);

            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);
            batch.BatchNumber = batchNumber;
            var response = new ProcessOut(
                "0000",
                $"Created Payment Batch: {batchNumber}",
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

    public async Task<(ProcessOut Response, APPayment Payment)> ReadPaymentAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var payment = new APPayment { BatchNumber001 = batchNumber, EntryNumber002 = entryNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406", compose: true);
            dynamic vHeader = views.ViewById("AP0031");
            dynamic vMisc = views.ViewById("AP0032");
            dynamic vApplied = views.ViewById("AP0033");

            vHeader.Fields.FieldByName("BTCHTYPE").Value = "PY";
            vHeader.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            vHeader.Fields.FieldByName("CNTENTR").Value = entryNumber;

            if (!(bool)vHeader.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "Payment not found!"), payment);
            }

            vHeader.Read();
            payment.CheckNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value);
            payment.VendorNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value);
            payment.PaymentDateAdjustmentDate005 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?;
            payment.EntryDescription006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value);
            payment.VendorPayeeName007 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value);
            payment.PaymentTransType020 = Convert.ToString(vHeader.Fields.FieldByName("RMITTYPE").Value);
            payment.DocumentType021 = Convert.ToString(vHeader.Fields.FieldByName("DOCTYPE").Value);
            payment.DocumentNumber032 = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value);

            var transType = SageViewPut.ParseInt(payment.PaymentTransType020);
            if (transType != 4)
            {
                var items = new List<APPaymentItem>();
                while (vApplied.Fetch())
                {
                    items.Add(new APPaymentItem
                    {
                        EntryNumber002 = Convert.ToString(vApplied.Fields.FieldByName("CNTRMIT").Value),
                        LineNumber003 = Convert.ToString(vApplied.Fields.FieldByName("CNTLINE").Value),
                        DocumentNumber005 = Convert.ToString(vApplied.Fields.FieldByName("IDINVC").Value),
                        PaymentNumber006 = Convert.ToString(vApplied.Fields.FieldByName("CNTPAYM").Value),
                        TransactionType007 = Convert.ToString(vApplied.Fields.FieldByName("TRXTYPE").Value),
                        PaymentResolution008 = Convert.ToString(vApplied.Fields.FieldByName("PYMTRESL").Value),
                        PaymentAmount009 = Convert.ToString(vApplied.Fields.FieldByName("AMTPAYM").Value),
                        DiscountAmountTaken010 = Convert.ToString(vApplied.Fields.FieldByName("AMTERNDISC").Value),
                        Description014 = Convert.ToString(vApplied.Fields.FieldByName("TEXTADJ").Value),
                        Reference015 = Convert.ToString(vApplied.Fields.FieldByName("GLREF").Value),
                        PPMatchingDocNo017 = Convert.ToString(vApplied.Fields.FieldByName("IDDOCMTCH").Value),
                        PPMatchingDocType018 = Convert.ToString(vApplied.Fields.FieldByName("CDAPPLYTO").Value),
                        ActivationDate019 = vApplied.Fields.FieldByName("DATEACTVPP").Value as DateTime?,
                        JobApplyMethod031 = Convert.ToString(vApplied.Fields.FieldByName("APPLYMETH").Value),
                        RetainageAmount034 = Convert.ToString(vApplied.Fields.FieldByName("RTGAMT").Value),
                        RetainageDueDate035 = vApplied.Fields.FieldByName("RTGDATEDUE").Value as DateTime?,
                        RetainageTermsCode036 = Convert.ToString(vApplied.Fields.FieldByName("RTGTERMS").Value),
                        RetainageExchangeRate037 = Convert.ToString(vApplied.Fields.FieldByName("SWRTGRATE").Value),
                        DocumentType043 = Convert.ToString(vApplied.Fields.FieldByName("DOCTYPE").Value)
                    });
                }

                payment.APPaymentItems = items;
            }
            else
            {
                var items = new List<APMiscPaymentItem>();
                while (vMisc.Fetch())
                {
                    items.Add(new APMiscPaymentItem
                    {
                        EntryNumber002 = Convert.ToString(vMisc.Fields.FieldByName("CNTRMIT").Value),
                        LineNumber003 = Convert.ToString(vMisc.Fields.FieldByName("CNTLINE").Value),
                        DistributionCode004 = Convert.ToString(vMisc.Fields.FieldByName("IDDISTCODE").Value),
                        AccountNumber005 = Convert.ToString(vMisc.Fields.FieldByName("IDACCT").Value),
                        GLReference006 = Convert.ToString(vMisc.Fields.FieldByName("GLREF").Value),
                        GLDescription007 = Convert.ToString(vMisc.Fields.FieldByName("GLDESC").Value),
                        TaxClass1008 = Convert.ToString(vMisc.Fields.FieldByName("TAXCLASS1").Value),
                        TaxClass2009 = Convert.ToString(vMisc.Fields.FieldByName("TAXCLASS2").Value),
                        TaxClass3010 = Convert.ToString(vMisc.Fields.FieldByName("TAXCLASS3").Value),
                        TaxClass4011 = Convert.ToString(vMisc.Fields.FieldByName("TAXCLASS4").Value),
                        TaxClass5012 = Convert.ToString(vMisc.Fields.FieldByName("TAXCLASS5").Value),
                        DistAmount034 = Convert.ToString(vMisc.Fields.FieldByName("AMTDISTTC").Value)
                    });
                }

                payment.APMiscPaymentItems = items;
            }

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Payments Number : {payment.DocumentNumber032}, Batch Number : {batchNumber}",
                DocumentNumber: payment.DocumentNumber032,
                BatchNumber: batchNumber,
                ErrorCode: "0000");

            return (response, payment);
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

            return (ProcessOut.Fail("9999", ex.Message), payment);
        }
    }

    public async Task<(ProcessOut Response, APPaymentBatch Batch)> ReadPaymentBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var batch = new APPaymentBatch { BatchNumber = batchNumber };
        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406", compose: true);
            dynamic vBatch = views.ViewById("AP0030");
            dynamic vHeader = views.ViewById("AP0031");

            vBatch.Fields.FieldByName("PAYMTYPE").Value = "PY";
            vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)vBatch.Exists)
            {
                vBatch.Read();
                batch.BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BATCHDESC").Value);
                batch.BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?;
                batch.BankCode = Convert.ToString(vBatch.Fields.FieldByName("IDBANK").Value);
                batch.CurrencyCode = Convert.ToString(vBatch.Fields.FieldByName("CODECURN").Value);
                batch.SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value);
            }

            var entries = new List<APPayment>();
            while (vHeader.Fetch())
            {
                entries.Add(new APPayment
                {
                    BatchNumber001 = batchNumber,
                    EntryNumber002 = Convert.ToString(vHeader.Fields.FieldByName("CNTENTR").Value),
                    CheckNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value),
                    VendorNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value),
                    PaymentDateAdjustmentDate005 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?,
                    EntryDescription006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value),
                    VendorPayeeName007 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value),
                    PaymentTransType020 = Convert.ToString(vHeader.Fields.FieldByName("RMITTYPE").Value),
                    DocumentType021 = Convert.ToString(vHeader.Fields.FieldByName("DOCTYPE").Value),
                    DocumentNumber032 = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value)
                });
            }

            batch.BatchEntries = entries;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Payment Batch : {batchNumber}",
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

    public async Task<(ProcessOut Response, SyncAPPayments Sync)> SyncPaymentsAsync(
        SyncAPPayments request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var recordLimit = request.RecordLimit > 0 ? request.RecordLimit : 100;
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406,YH0301,CS0120", compose: true);
            dynamic vBatch = views.ViewById("AP0030");
            dynamic vHeader = views.ViewById("AP0031");
            dynamic vMisc = views.ViewById("AP0032");
            dynamic vApplied = views.ViewById("AP0033");
            dynamic yh = views.ViewById("YH0301");

            ApYhSync.BrowseForSync(yh, "AP", "PY", request, timestamp);

            var batches = new List<APPaymentBatch>();
            while (yh.Fetch() && batches.Count < recordLimit)
            {
                var batchNumber = Convert.ToString(yh.Fields.FieldByName("CNTBTCH").Value);
                if (string.IsNullOrWhiteSpace(batchNumber))
                {
                    continue;
                }

                vBatch.Fields.FieldByName("PAYMTYPE").Value = "PY";
                vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
                vBatch.Read();

                if (SageViewPut.ParseInt(Convert.ToString(vBatch.Fields.FieldByName("BATCHSTAT").Value)) != 3)
                {
                    continue;
                }

                var batch = new APPaymentBatch
                {
                    BatchNumber = batchNumber,
                    BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?,
                    BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BATCHDESC").Value),
                    BankCode = Convert.ToString(vBatch.Fields.FieldByName("IDBANK").Value),
                    CurrencyCode = Convert.ToString(vBatch.Fields.FieldByName("CODECURN").Value),
                    SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value)
                };

                var entries = new List<APPayment>();
                while (vHeader.Fetch())
                {
                    var entry = new APPayment
                    {
                        BatchNumber001 = batchNumber,
                        EntryNumber002 = Convert.ToString(vHeader.Fields.FieldByName("CNTENTR").Value),
                        CheckNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value),
                        VendorNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value),
                        PaymentDateAdjustmentDate005 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?,
                        EntryDescription006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value),
                        VendorPayeeName007 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value),
                        PaymentTransType020 = Convert.ToString(vHeader.Fields.FieldByName("RMITTYPE").Value),
                        DocumentType021 = Convert.ToString(vHeader.Fields.FieldByName("DOCTYPE").Value),
                        DocumentNumber032 = Convert.ToString(vHeader.Fields.FieldByName("DOCNBR").Value)
                    };

                    var transType = SageViewPut.ParseInt(entry.PaymentTransType020);
                    if (transType != 4)
                    {
                        var items = new List<APPaymentItem>();
                        while (vApplied.Fetch())
                        {
                            items.Add(new APPaymentItem
                            {
                                EntryNumber002 = Convert.ToString(vApplied.Fields.FieldByName("CNTRMIT").Value),
                                LineNumber003 = Convert.ToString(vApplied.Fields.FieldByName("CNTLINE").Value),
                                DocumentNumber005 = Convert.ToString(vApplied.Fields.FieldByName("IDINVC").Value),
                                PaymentNumber006 = Convert.ToString(vApplied.Fields.FieldByName("CNTPAYM").Value),
                                TransactionType007 = Convert.ToString(vApplied.Fields.FieldByName("TRXTYPE").Value),
                                PaymentAmount009 = Convert.ToString(vApplied.Fields.FieldByName("AMTPAYM").Value),
                                DiscountAmountTaken010 = Convert.ToString(vApplied.Fields.FieldByName("AMTERNDISC").Value),
                                Description014 = Convert.ToString(vApplied.Fields.FieldByName("TEXTADJ").Value),
                                Reference015 = Convert.ToString(vApplied.Fields.FieldByName("GLREF").Value)
                            });
                        }

                        entry.APPaymentItems = items;
                    }
                    else
                    {
                        var items = new List<APMiscPaymentItem>();
                        while (vMisc.Fetch())
                        {
                            items.Add(new APMiscPaymentItem
                            {
                                EntryNumber002 = Convert.ToString(vMisc.Fields.FieldByName("CNTRMIT").Value),
                                LineNumber003 = Convert.ToString(vMisc.Fields.FieldByName("CNTLINE").Value),
                                DistributionCode004 = Convert.ToString(vMisc.Fields.FieldByName("IDDISTCODE").Value),
                                AccountNumber005 = Convert.ToString(vMisc.Fields.FieldByName("IDACCT").Value),
                                DistAmount034 = Convert.ToString(vMisc.Fields.FieldByName("AMTDISTTC").Value)
                            });
                        }

                        entry.APMiscPaymentItems = items;
                    }

                    entries.Add(entry);
                }

                batch.BatchEntries = entries;
                batches.Add(batch);
            }

            request.APPaymentBatches = batches;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                "Sync AP Payments completed.",
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

    internal static class SageViewPut
    {
        public static void PutIfNotNull(dynamic view, string fieldName, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                view.Fields.FieldByName(fieldName).Value = value;
            }
        }

        public static void PutIfDate(dynamic view, string fieldName, DateTime? value)
        {
            if (value is { } dt && dt != default)
            {
                view.Fields.FieldByName(fieldName).Value = dt;
            }
        }

        public static int ParseInt(string? value) => int.TryParse(value, out var i) ? i : 0;

        public static bool IsTruthy(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("y", StringComparison.OrdinalIgnoreCase)
                || value.Equals("1", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static class ApYhSync
    {
        public static void BrowseForSync(dynamic yh, string module, string txnType, SyncAPPayments request, string timestamp)
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
}
