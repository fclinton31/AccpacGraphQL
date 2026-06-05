using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApAdjustmentService : IApAdjustmentService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ApAdjustmentService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, APAdjustments Adjustment)> CreateAdjustmentAsync(
        APAdjustments adjustment,
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
            dynamic vApply = views.ViewById("AP0033");
            dynamic vDist = views.ViewById("AP0034");

            vBatch.Fields.FieldByName("PAYMTYPE").Value = "AD";
            vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
            vBatch.Init();
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BATCHDESC", adjustment.BatchDescription);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", adjustment.BatchDate);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "IDBANK", adjustment.BankCode063);
            vBatch.Update();

            vHeader.Fields.FieldByName("BTCHTYPE").Value = "AD";
            vHeader.Fields.FieldByName("CNTENTR").Value = 0;
            vHeader.RecordGenerate(false);

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDRMIT", adjustment.CheckNumber003);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDVEND", adjustment.VendorNumber004);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERMIT", adjustment.PaymentDateAdjustmentDate005);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTRMIT", adjustment.EntryDescription006);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "NAMERMIT", adjustment.VendorPayeeName007);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCHTC", adjustment.VendorExchangeRate010);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATETC", adjustment.VendorRateOverridden011);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "AMTPPAYTC", adjustment.TotalPrepayVendorCurr013);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "PAYMCODE", adjustment.PaymentCode015);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPEHC", adjustment.BankRateType017);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATEEXCHHC", adjustment.BankExchangeRate018);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWRATEHC", adjustment.BankRateOverridden019);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RMITTYPE", adjustment.PaymentTransType020);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "DOCTYPE", adjustment.DocumentType021);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERATETC", adjustment.VendorRateDate025);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPETC", adjustment.VendorRateType026);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERATEHC", adjustment.BankRateDate028);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "PAYMSTTS", adjustment.PaymentEdited033);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWPRNTRMIT", adjustment.CheckPrintRequired034);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDRMITTO", adjustment.VendorRemitToLocation035);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXTRMITREF", adjustment.EntryReference036);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWPRINTED", adjustment.CheckPrintedStatus040);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE1", adjustment.AddressLine1041);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE2", adjustment.AddressLine2042);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE3", adjustment.AddressLine3043);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTSTRE4", adjustment.AddressLine4044);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "NAMECITY", adjustment.City045);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODESTTE", adjustment.State046);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODEPSTL", adjustment.ZipPostalCode047);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECTRY", adjustment.Country048);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CHECKLANG", adjustment.PaymentLanguage049);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEACTVPP", adjustment.PrepayActivationDate054);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWJOB", adjustment.JobRelated055);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "APPLYMETH", adjustment.JobApplyMethod056);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDINVCMTCH", adjustment.MatchingDocumentNumber059);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SRCEAPPL", adjustment.SourceApplication062);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDBANK", adjustment.BankCode063);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECURNBC", adjustment.BankCurrencyCode064);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CASHACCT", adjustment.CashAccount066);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODE1099", adjustment.S1099CPRSCode070);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "AMT1099", adjustment.S1099CPRSAmount071);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTXAMTCTL", adjustment.CalculateTaxAmountControl072);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTXBSECTL", adjustment.CalculateTaxBaseControl073);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODETAXGRP", adjustment.TaxGroup074);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TAXCLASS1", adjustment.TaxClass1081);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TAXCLASS2", adjustment.TaxClass2082);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TAXCLASS3", adjustment.TaxClass3083);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TAXCLASS4", adjustment.TaxClass4084);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TAXCLASS5", adjustment.TaxClass5085);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL1", adjustment.TaxIncluded1086);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL2", adjustment.TaxIncluded2087);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL3", adjustment.TaxIncluded3088);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL4", adjustment.TaxIncluded4089);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTAXINCL5", adjustment.TaxIncluded5090);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXBSE1TC", adjustment.TaxBase1091);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXBSE2TC", adjustment.TaxBase2092);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXBSE3TC", adjustment.TaxBase3093);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXBSE4TC", adjustment.TaxBase4094);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXBSE5TC", adjustment.TaxBase5095);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT1TC", adjustment.TaxAmount1096);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT2TC", adjustment.TaxAmount2097);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT3TC", adjustment.TaxAmount3098);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT4TC", adjustment.TaxAmount4099);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT5TC", adjustment.TaxAmount5100);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECURNRC", adjustment.TaxReportingCurrencyCode106);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWTXCTLRC", adjustment.TaxReportingCalculateMethod107);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATERC", adjustment.TaxReportingExchangeRate108);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "RATETYPERC", adjustment.TaxReportingRateType109);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "RATEDATERC", adjustment.TaxReportingRateDate110);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT1RC", adjustment.TaxReportingAmount1113);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT2RC", adjustment.TaxReportingAmount2114);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT3RC", adjustment.TaxReportingAmount3115);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT4RC", adjustment.TaxReportingAmount4116);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TXAMT5RC", adjustment.TaxReportingAmount5117);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "ENTEREDBY", adjustment.EnteredBy144);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATEBUS", adjustment.PostingDate145);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTSET", adjustment.AccountSet146);

            vApply.RecordGenerate(false);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vApply, "IDVEND", adjustment.VendorNumber004);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vApply, "IDINVC", adjustment.DocumentNumber032);

            if (adjustment.APAdjustmentItems is { Count: > 0 })
            {
                foreach (var dtl in adjustment.APAdjustmentItems)
                {
                    vDist.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "CNTSEQ", dtl.SequenceNo004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "CODTRXTYPE", dtl.TransactionType005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "AMTDIST", dtl.DistributionAmount006);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "IDDISTCODE", dtl.DistributionCode007);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "IDACCT", dtl.DistributionGLAccount008);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "CONTRACT", dtl.Contract009);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "PROJECT", dtl.Project010);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "CATEGORY", dtl.Category011);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "RESOURCE", dtl.ProjectCategoryResource012);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "COSTCLASS", dtl.CostClass014);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "BILLTYPE", dtl.BillingType015);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "AMTDISC", dtl.DiscountAmount016);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "AMTPAYM", dtl.AppliedAmount017);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "IDITEM", dtl.ItemNumber018);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "UNITMEAS", dtl.UnitofMeasure019);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "QTYINVC", dtl.Quantity020);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "AMTCOST", dtl.Cost021);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vDist, "BILLDATE", dtl.BillingDate022);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "BILLRATE", dtl.BillingRate023);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "RTGAMT", dtl.RetainageAmount025);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vDist, "RTGDATEDUE", dtl.RetainageDueDate026);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "TEXTDESC", dtl.Description032);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "TEXTREF", dtl.Reference033);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "DOCLINE", dtl.DocumentLineNumber034);
                    vDist.Insert();
                }
            }

            vApply.Insert();
            vHeader.Insert();
            vBatch.Update();

            session.CommitTransaction(tran);

            var documentNumber = Convert.ToString(vApply.Fields.FieldByName("IDINVC").Value);
            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Adjustments Number : {documentNumber}",
                DocumentNumber: documentNumber,
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

    public async Task<(ProcessOut Response, APAdjustmentBatch Batch)> CreateAdjustmentBatchAsync(
        APAdjustmentBatch batch,
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
            dynamic vApply = views.ViewById("AP0033");
            dynamic vDist = views.ViewById("AP0034");

            vBatch.Fields.FieldByName("PAYMTYPE").Value = "AD";
            vBatch.Fields.FieldByName("CNTBTCH").Value = "0";
            vBatch.Init();
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "BATCHDESC", batch.BatchDescription);
            Sage300ApPaymentService.SageViewPut.PutIfDate(vBatch, "DATEBTCH", batch.BatchDate);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vBatch, "IDBANK", batch.BankCode);
            vBatch.Update();

            if (batch.BatchEntries is { Count: > 0 })
            {
                foreach (var adjustment in batch.BatchEntries)
                {
                    vHeader.Fields.FieldByName("BTCHTYPE").Value = "AD";
                    vHeader.Fields.FieldByName("CNTENTR").Value = 0;
                    vHeader.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDVEND", adjustment.VendorNumber004);
                    Sage300ApPaymentService.SageViewPut.PutIfDate(vHeader, "DATERMIT", adjustment.PaymentDateAdjustmentDate005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTRMIT", adjustment.EntryDescription006);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "NAMERMIT", adjustment.VendorPayeeName007);

                    vApply.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vApply, "IDVEND", adjustment.VendorNumber004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vApply, "IDINVC", adjustment.DocumentNumber032);

                    if (adjustment.APAdjustmentItems is { Count: > 0 })
                    {
                        foreach (var dtl in adjustment.APAdjustmentItems)
                        {
                            vDist.RecordGenerate(false);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "CNTSEQ", dtl.SequenceNo004);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "CODTRXTYPE", dtl.TransactionType005);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "AMTDIST", dtl.DistributionAmount006);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "IDDISTCODE", dtl.DistributionCode007);
                            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDist, "IDACCT", dtl.DistributionGLAccount008);
                            vDist.Insert();
                        }
                    }

                    vApply.Insert();
                    vHeader.Insert();
                }
            }

            vBatch.Update();
            session.CommitTransaction(tran);

            var batchNumber = Convert.ToString(vBatch.Fields.FieldByName("CNTBTCH").Value);
            batch.BatchNumber = batchNumber;
            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Adjustment Batch : {batchNumber}",
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

    public async Task<(ProcessOut Response, APAdjustments Adjustment)> ReadAdjustmentAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var adjustment = new APAdjustments { BatchNumber001 = batchNumber, EntryNumber002 = entryNumber };

        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406", compose: true);
            dynamic vHeader = views.ViewById("AP0031");
            dynamic vApply = views.ViewById("AP0033");
            dynamic vDist = views.ViewById("AP0034");

            vHeader.Fields.FieldByName("BTCHTYPE").Value = "AD";
            vHeader.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            vHeader.Fields.FieldByName("CNTENTR").Value = entryNumber;

            if (!(bool)vHeader.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "Adjustment not found!"), adjustment);
            }

            vHeader.Read();
            adjustment.EntryNumber002 = Convert.ToString(vHeader.Fields.FieldByName("CNTENTR").Value);
            adjustment.CheckNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value);
            adjustment.VendorNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value);
            adjustment.PaymentDateAdjustmentDate005 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?;
            adjustment.EntryDescription006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value);
            adjustment.VendorPayeeName007 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value);
            adjustment.SourceApplication062 = Convert.ToString(vHeader.Fields.FieldByName("SRCEAPPL").Value);
            adjustment.BankCode063 = Convert.ToString(vHeader.Fields.FieldByName("IDBANK").Value);

            if (vApply.Fetch())
            {
                adjustment.DocumentNumber032 = Convert.ToString(vApply.Fields.FieldByName("IDINVC").Value);
            }

            var items = new List<APAdjustmentItem>();
            while (vDist.Fetch())
            {
                items.Add(new APAdjustmentItem
                {
                    LineNumber003 = Convert.ToString(vDist.Fields.FieldByName("CNTLINE").Value),
                    SequenceNo004 = Convert.ToString(vDist.Fields.FieldByName("CNTSEQ").Value),
                    TransactionType005 = Convert.ToString(vDist.Fields.FieldByName("CODTRXTYPE").Value),
                    DistributionAmount006 = Convert.ToString(vDist.Fields.FieldByName("AMTDIST").Value),
                    DistributionCode007 = Convert.ToString(vDist.Fields.FieldByName("IDDISTCODE").Value),
                    DistributionGLAccount008 = Convert.ToString(vDist.Fields.FieldByName("IDACCT").Value),
                    Contract009 = Convert.ToString(vDist.Fields.FieldByName("CONTRACT").Value),
                    Project010 = Convert.ToString(vDist.Fields.FieldByName("PROJECT").Value),
                    Category011 = Convert.ToString(vDist.Fields.FieldByName("CATEGORY").Value),
                    ProjectCategoryResource012 = Convert.ToString(vDist.Fields.FieldByName("RESOURCE").Value),
                    CostClass014 = Convert.ToString(vDist.Fields.FieldByName("COSTCLASS").Value),
                    BillingType015 = Convert.ToString(vDist.Fields.FieldByName("BILLTYPE").Value),
                    DiscountAmount016 = Convert.ToString(vDist.Fields.FieldByName("AMTDISC").Value),
                    AppliedAmount017 = Convert.ToString(vDist.Fields.FieldByName("AMTPAYM").Value),
                    ItemNumber018 = Convert.ToString(vDist.Fields.FieldByName("IDITEM").Value),
                    UnitofMeasure019 = Convert.ToString(vDist.Fields.FieldByName("UNITMEAS").Value),
                    Quantity020 = Convert.ToString(vDist.Fields.FieldByName("QTYINVC").Value),
                    Cost021 = Convert.ToString(vDist.Fields.FieldByName("AMTCOST").Value),
                    BillingDate022 = vDist.Fields.FieldByName("BILLDATE").Value as DateTime?,
                    BillingRate023 = Convert.ToString(vDist.Fields.FieldByName("BILLRATE").Value),
                    RetainageAmount025 = Convert.ToString(vDist.Fields.FieldByName("RTGAMT").Value),
                    RetainageDueDate026 = vDist.Fields.FieldByName("RTGDATEDUE").Value as DateTime?,
                    Description032 = Convert.ToString(vDist.Fields.FieldByName("TEXTDESC").Value),
                    Reference033 = Convert.ToString(vDist.Fields.FieldByName("TEXTREF").Value),
                    DocumentLineNumber034 = Convert.ToString(vDist.Fields.FieldByName("DOCLINE").Value)
                });
            }

            adjustment.APAdjustmentItems = items;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Adjustments Number : {adjustment.DocumentNumber032}",
                DocumentNumber: adjustment.DocumentNumber032,
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

    public async Task<(ProcessOut Response, APAdjustmentBatch Batch)> ReadAdjustmentBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var batch = new APAdjustmentBatch { BatchNumber = batchNumber };
        try
        {
            var views = new Sage300ViewSet(session, "AP0030,AP0031,AP0032,AP0033,AP0034,AP0048,AP0170,AP0406", compose: true);
            dynamic vBatch = views.ViewById("AP0030");
            dynamic vHeader = views.ViewById("AP0031");

            vBatch.Fields.FieldByName("PAYMTYPE").Value = "AD";
            vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
            if ((bool)vBatch.Exists)
            {
                vBatch.Read();
                batch.BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BATCHDESC").Value);
                batch.BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?;
                batch.BankCode = Convert.ToString(vBatch.Fields.FieldByName("IDBANK").Value);
                batch.SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value);
            }

            var entries = new List<APAdjustments>();
            while (vHeader.Fetch())
            {
                entries.Add(new APAdjustments
                {
                    BatchNumber001 = batchNumber,
                    EntryNumber002 = Convert.ToString(vHeader.Fields.FieldByName("CNTENTR").Value),
                    CheckNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value),
                    VendorNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value),
                    PaymentDateAdjustmentDate005 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?,
                    EntryDescription006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value),
                    VendorPayeeName007 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value)
                });
            }

            batch.BatchEntries = entries;

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 AP Adjustment Batch : {batchNumber}",
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

    public async Task<(ProcessOut Response, SyncAPAdjustments Sync)> SyncAdjustmentsAsync(
        SyncAPAdjustments request,
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
            dynamic vApply = views.ViewById("AP0033");
            dynamic vDist = views.ViewById("AP0034");
            dynamic yh = views.ViewById("YH0301");

            BrowseForSync(yh, "AP", "AD", request, timestamp);

            var batches = new List<APAdjustmentBatch>();
            while (yh.Fetch() && batches.Count < recordLimit)
            {
                var batchNumber = Convert.ToString(yh.Fields.FieldByName("CNTBTCH").Value);
                if (string.IsNullOrWhiteSpace(batchNumber))
                {
                    continue;
                }

                vBatch.Fields.FieldByName("PAYMTYPE").Value = "AD";
                vBatch.Fields.FieldByName("CNTBTCH").Value = batchNumber;
                vBatch.Read();
                if (Sage300ApPaymentService.SageViewPut.ParseInt(Convert.ToString(vBatch.Fields.FieldByName("BATCHSTAT").Value)) != 3)
                {
                    continue;
                }

                var batch = new APAdjustmentBatch
                {
                    BatchNumber = batchNumber,
                    BatchDate = vBatch.Fields.FieldByName("DATEBTCH").Value as DateTime?,
                    BatchDescription = Convert.ToString(vBatch.Fields.FieldByName("BATCHDESC").Value),
                    BankCode = Convert.ToString(vBatch.Fields.FieldByName("IDBANK").Value),
                    SourceApplication = Convert.ToString(vBatch.Fields.FieldByName("SRCEAPPL").Value)
                };

                var entries = new List<APAdjustments>();
                while (vHeader.Fetch())
                {
                    var entry = new APAdjustments
                    {
                        BatchNumber001 = batchNumber,
                        EntryNumber002 = Convert.ToString(vHeader.Fields.FieldByName("CNTENTR").Value),
                        CheckNumber003 = Convert.ToString(vHeader.Fields.FieldByName("IDRMIT").Value),
                        VendorNumber004 = Convert.ToString(vHeader.Fields.FieldByName("IDVEND").Value),
                        PaymentDateAdjustmentDate005 = vHeader.Fields.FieldByName("DATERMIT").Value as DateTime?,
                        EntryDescription006 = Convert.ToString(vHeader.Fields.FieldByName("TEXTRMIT").Value),
                        VendorPayeeName007 = Convert.ToString(vHeader.Fields.FieldByName("NAMERMIT").Value)
                    };

                    if (vApply.Fetch())
                    {
                        entry.DocumentNumber032 = Convert.ToString(vApply.Fields.FieldByName("IDINVC").Value);
                    }

                    var items = new List<APAdjustmentItem>();
                    while (vDist.Fetch())
                    {
                        items.Add(new APAdjustmentItem
                        {
                            LineNumber003 = Convert.ToString(vDist.Fields.FieldByName("CNTLINE").Value),
                            SequenceNo004 = Convert.ToString(vDist.Fields.FieldByName("CNTSEQ").Value),
                            TransactionType005 = Convert.ToString(vDist.Fields.FieldByName("CODTRXTYPE").Value),
                            DistributionAmount006 = Convert.ToString(vDist.Fields.FieldByName("AMTDIST").Value),
                            DistributionCode007 = Convert.ToString(vDist.Fields.FieldByName("IDDISTCODE").Value),
                            DistributionGLAccount008 = Convert.ToString(vDist.Fields.FieldByName("IDACCT").Value)
                        });
                    }

                    entry.APAdjustmentItems = items;
                    entries.Add(entry);
                }

                batch.BatchEntries = entries;
                batches.Add(batch);
            }

            request.APAdjustmentsBatches = batches;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                "Sync AP Adjustments completed.",
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

    private static void BrowseForSync(dynamic yh, string module, string txnType, SyncAPAdjustments request, string timestamp)
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
