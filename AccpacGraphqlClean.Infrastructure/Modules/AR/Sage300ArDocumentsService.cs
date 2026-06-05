using System.Globalization;
using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArDocumentsService : IArDocumentsService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArDocumentsService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, AROpenInvoices Documents)> GetDocumentsAsync(
        AROpenInvoices documents,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(documents.CustomerCode))
        {
            return (ProcessOut.Fail("9999", "CustomerCode is required."), documents);
        }

        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        dynamic v = session.OpenView("AR0036");

        documents.Document = new List<AROpenDocument>();

        var filter = $"IDCUST = \"{documents.CustomerCode}\" AND SWPAID = 0 AND TRXTYPETXT = 1";
        v.Browse(filter, true);

        while (v.Fetch())
        {
            documents.Document.Add(new AROpenDocument
            {
                CustomerNumber000 = GetString(v, "IDCUST"),
                DocumentNumber001 = GetString(v, "IDINVC"),
                CheckReceiptNo002 = GetString(v, "IDRMIT"),
                OrderNumber003 = GetString(v, "IDORDERNBR"),
                PONumber004 = GetString(v, "IDCUSTPO"),
                DueDate005 = GetDate(v, "DATEDUE"),
                NationalAccountNumber006 = GetString(v, "IDNATACCT"),
                ShipToLocation007 = GetString(v, "IDCUSTSHPT"),
                TransactionType008 = GetString(v, "TRXTYPEID"),
                DocumentType009 = GetString(v, "TRXTYPETXT"),
                BatchDate010 = GetDate(v, "DATEBTCH"),
                BatchNumber011 = GetString(v, "CNTBTCH"),
                EntryNumber012 = GetString(v, "CNTITEM"),
                GroupCode013 = GetString(v, "IDGRP"),
                DocumentDescription014 = GetString(v, "DESCINVC"),
                DocumentDate015 = GetDate(v, "DATEINVC"),
                AsofDate016 = GetDate(v, "DATEASOF"),
                Terms017 = GetString(v, "CODETERM"),
                DiscountDate018 = GetDate(v, "DATEDISC"),
                CurrencyCode019 = GetString(v, "CODECURN"),
                RateType020 = GetString(v, "IDRATETYPE"),
                RateOverridden021 = GetString(v, "SWRATEOVRD"),
                ExchangeRate022 = GetString(v, "EXCHRATEHC"),
                FuncCurrencyInvoiceAmount023 = GetString(v, "AMTINVCHC"),
                FuncCurrencyAmountDue024 = GetString(v, "AMTDUEHC"),
                FuncCurrencyTaxableAmount025 = GetString(v, "AMTTXBLHC"),
                FuncCurrencyNonTaxableAmt026 = GetString(v, "AMTNONTXHC"),
                FuncCurrencyTaxAmount027 = GetString(v, "AMTTAXHC"),
                FuncCurrencyDiscountAmount028 = GetString(v, "AMTDISCHC"),
                CustCurrencyInvoiceAmount029 = GetString(v, "AMTINVCTC"),
                CustCurrencyAmountDue030 = GetString(v, "AMTDUETC"),
                CustCurrencyTaxableAmount031 = GetString(v, "AMTTXBLTC"),
                CustCurrencyNonTaxableAmt032 = GetString(v, "AMTNONTXTC"),
                CustCurrencyTaxAmount033 = GetString(v, "AMTTAXTC"),
                CustCurrencyDiscountAmount034 = GetString(v, "AMTDISCTC"),
                FullyPaid035 = GetString(v, "SWPAID"),
                LastActivityDate036 = GetDate(v, "DATELSTACT"),
                LastStatementDate037 = GetDate(v, "DATELSTSTM"),
                NumberofScheduledPayments040 = GetString(v, "CNTTOTPAYM"),
                ReservedLastPaymentNumberP041 = GetString(v, "CNTLSTPAID"),
                PaymentNumberonLastStatement042 = GetString(v, "CNTLSTPYST"),
                ReservedReceiptAmount043 = GetString(v, "AMTREMIT"),
                LastAppliedPaymentSeqNo044 = GetString(v, "CNTLASTSEQ"),
                DoNotCalcTax045 = GetString(v, "SWTAXINPUT"),
                TaxAuthority1046 = GetString(v, "CODETAX1"),
                TaxAuthority2047 = GetString(v, "CODETAX2"),
                TaxAuthority3048 = GetString(v, "CODETAX3"),
                TaxAuthority4049 = GetString(v, "CODETAX4"),
                TaxAuthority5050 = GetString(v, "CODETAX5"),
                FuncBaseAmount1051 = GetString(v, "AMTBASE1HC"),
                FuncBaseAmount2052 = GetString(v, "AMTBASE2HC"),
                FuncBaseAmount3053 = GetString(v, "AMTBASE3HC"),
                FuncBaseAmount4054 = GetString(v, "AMTBASE4HC"),
                FuncBaseAmount5055 = GetString(v, "AMTBASE5HC"),
                FuncTaxAmount1056 = GetString(v, "AMTTAX1HC"),
                FuncTaxAmount2057 = GetString(v, "AMTTAX2HC"),
                FuncTaxAmount3058 = GetString(v, "AMTTAX3HC"),
                FuncTaxAmount4059 = GetString(v, "AMTTAX4HC"),
                FuncTaxAmount5060 = GetString(v, "AMTTAX5HC"),
                CustBaseAmount1061 = GetString(v, "AMTBASE1TC"),
                CustBaseAmount2062 = GetString(v, "AMTBASE2TC"),
                CustBaseAmount3063 = GetString(v, "AMTBASE3TC"),
                CustBaseAmount4064 = GetString(v, "AMTBASE4TC"),
                CustBaseAmount5065 = GetString(v, "AMTBASE5TC"),
                CustTaxAmount1066 = GetString(v, "AMTTAX1TC"),
                CustTaxAmount2067 = GetString(v, "AMTTAX2TC"),
                CustTaxAmount3068 = GetString(v, "AMTTAX3TC"),
                CustTaxAmount4069 = GetString(v, "AMTTAX4TC"),
                CustTaxAmount5070 = GetString(v, "AMTTAX5TC"),
                Salesperson1071 = GetString(v, "CODESLSP1"),
                Salesperson2072 = GetString(v, "CODESLSP2"),
                Salesperson3073 = GetString(v, "CODESLSP3"),
                Salesperson4074 = GetString(v, "CODESLSP4"),
                Salesperson5075 = GetString(v, "CODESLSP5"),
                SalesSplitPercentage1076 = GetString(v, "PCTSASPLT1"),
                SalesSplitPercentage2077 = GetString(v, "PCTSASPLT2"),
                SalesSplitPercentage3078 = GetString(v, "PCTSASPLT3"),
                SalesSplitPercentage4079 = GetString(v, "PCTSASPLT4"),
                SalesSplitPercentage5080 = GetString(v, "PCTSASPLT5"),
                FiscalYear081 = GetString(v, "FISCYR"),
                FiscalPeriod082 = GetString(v, "FISCPER"),
                PrepayApplytoDocNo083 = GetString(v, "IDPREPAID"),
                PostingDate084 = GetDate(v, "DATEBUS"),
                RateDate085 = GetDate(v, "RATEDATE"),
                RateOperator086 = GetString(v, "RATEOP"),
                LastActivityYearPeriod087 = GetString(v, "YPLASTACT"),
                BankCode088 = GetString(v, "IDBANK"),
                DepositNumber089 = GetString(v, "DEPSTNBR"),
                PostingSequenceNo090 = GetString(v, "POSTSEQNCE"),
                JobRelated091 = GetString(v, "SWJOB"),
                HasRetainage092 = GetString(v, "SWRTG"),
                RetainageOutstanding093 = GetString(v, "SWRTGOUT"),
                DateRetainageDue094 = GetDate(v, "RTGDATEDUE"),
                FuncCurrOrigRtngAmt095 = GetString(v, "RTGOAMTHC"),
                FuncCurrRetainageAmount096 = GetString(v, "RTGAMTHC"),
                CustCurrOrigRtngAmt097 = GetString(v, "RTGOAMTTC"),
                CustCurrRetainageAmount098 = GetString(v, "RTGAMTTC"),
                RetainageTermsCode099 = GetString(v, "RTGTERMS"),
                RetainageExchangeRate100 = GetString(v, "SWRTGRATE"),
                OriginalDocNo101 = GetString(v, "RTGAPPLYTO"),
                OptionalFields102 = GetString(v, "VALUES"),
                SourceApplication103 = GetString(v, "SRCEAPPL"),
                ARVersionCreatedIn104 = GetString(v, "ARVERSION"),
                InvoiceType105 = GetString(v, "INVCTYPE"),
                DepositSerialNumber106 = GetString(v, "DEPSEQ"),
                DepositLineNumber107 = GetString(v, "DEPLINE"),
                BatchType108 = GetString(v, "TYPEBTCH"),
                NumberofOBLJDetails109 = GetString(v, "CNTOBLJ"),
                TaxReportingCurrencyCode110 = GetString(v, "CODECURNRC"),
                TaxReportingExchangeRate111 = GetString(v, "RATERC"),
                TaxReportingRateType112 = GetString(v, "RATETYPERC"),
                TaxReportingRateDate113 = GetDate(v, "RATEDATERC"),
                TaxReportingRateOperator114 = GetString(v, "RATEOPRC"),
                TaxReportingRateOverride115 = GetString(v, "SWRATERC"),
                ReportRetainageTax116 = GetString(v, "SWTXRTGRPT"),
                TaxGroup117 = GetString(v, "CODETAXGRP"),
                TaxStateVersion118 = GetString(v, "TAXVERSION"),
                TaxReportingCalculateMethod119 = GetString(v, "SWTXCTLRC"),
                TaxClass1120 = GetString(v, "TAXCLASS1"),
                TaxClass2121 = GetString(v, "TAXCLASS2"),
                TaxClass3122 = GetString(v, "TAXCLASS3"),
                TaxClass4123 = GetString(v, "TAXCLASS4"),
                TaxClass5124 = GetString(v, "TAXCLASS5"),
                TaxBase1125 = GetString(v, "TXBSERT1TC"),
                TaxBase2126 = GetString(v, "TXBSERT2TC"),
                TaxBase3127 = GetString(v, "TXBSERT3TC"),
                TaxBase4128 = GetString(v, "TXBSERT4TC"),
                TaxBase5129 = GetString(v, "TXBSERT5TC"),
                TaxAmount1130 = GetString(v, "TXAMTRT1TC"),
                TaxAmount2131 = GetString(v, "TXAMTRT2TC"),
                TaxAmount3132 = GetString(v, "TXAMTRT3TC"),
                TaxAmount4133 = GetString(v, "TXAMTRT4TC"),
                TaxAmount5134 = GetString(v, "TXAMTRT5TC"),
                ShipmentNumber135 = GetString(v, "IDSHIPNBR"),
                EarliestBackdatedActivityDate136 = GetDate(v, "DATEFRSTBK"),
                LastRevaluationDate137 = GetDate(v, "DATELSTRVL"),
                OrigExchangeRate138 = GetString(v, "ORATE"),
                OrigRateType139 = GetString(v, "ORATETYPE"),
                OrigRateDate140 = GetDate(v, "ORATEDATE"),
                OrigRateOperator141 = GetString(v, "ORATEOP"),
                OrigRateOverrideFlag142 = GetString(v, "OSWRATE"),
                AccountSet143 = GetString(v, "IDACCTSET"),
                DatePaid144 = GetDate(v, "DATEPAID"),
                MiscReceiptFlag145 = GetString(v, "SWNONRCVBL"),
                TerritotyCode146 = GetString(v, "CODETERR")
            });
        }

        return (
            ProcessOut.Ok($"AR Documents : {documents.Document.Count}", documents.CustomerCode),
            documents);
    }

    public async Task<(ProcessOut Response, ARAgedAnalysis Analysis)> GetAgedBalancesAsync(
        string customerNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customerNumber))
        {
            return (ProcessOut.Fail("9999", "customer is required."), new ARAgedAnalysis());
        }

        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        dynamic v = session.OpenView("AR0055");

        v.Fields.FieldByName("CMNDCODE").Value = "61";
        v.Fields.FieldByName("CUSTID").Value = customerNumber;
        v.Process();

        var analysis = new ARAgedAnalysis
        {
            CustomerNumber001 = customerNumber,
            CommandCode000 = GetString(v, "CMNDCODE"),
            PostingDate002 = GetDate(v, "BUSDATE"),
            DocumentNumber003 = GetString(v, "INVOICEID"),
            CheckReceiptNo004 = GetString(v, "RMITID"),
            DocumentType005 = GetString(v, "TRXTYPETXT"),
            TransactionType006 = GetString(v, "TRXTYPEID"),
            AmountDueSource007 = GetString(v, "OBLAMTDUET"),
            AmountDueFunctional008 = GetString(v, "OBLAMTDUEH"),
            StartingInvoiceDate009 = GetDate(v, "INVCBEGNDT"),
            DueDate010 = GetDate(v, "DUEDATE"),
            AgeasofDate011 = GetDate(v, "RUNDATE"),
            CutoffDate012 = GetDate(v, "CUTOFFDATE"),
            Ageby013 = GetString(v, "AGEINVDTSW"),
            AgeFullyPaidDocuments014 = GetString(v, "INCLPAIDSW"),
            IncludeZeroBalances015 = GetString(v, "ZEROBALSW"),
            ReportType016 = GetString(v, "PASTDUESW"),
            Current017 = GetString(v, "AGEPERIOD1"),
            FirstPeriod018 = GetString(v, "AGEPERIOD2"),
            SecondPeriod019 = GetString(v, "AGEPERIOD3"),
            ThirdPeriod020 = GetString(v, "AGEPERIOD4"),
            FourthPeriod021 = GetString(v, "AGEPERIOD5"),
            FirstForwardPeriod022 = GetString(v, "AGEPERIOD6"),
            SecondForwardPeriod023 = GetString(v, "AGEPERIOD7"),
            ThirdForwardPeriod024 = GetString(v, "AGEPERIOD8"),
            FourthForwardPeriod025 = GetString(v, "AGEPERIOD9"),
            IncludeOverCreditLimit026 = GetString(v, "SWCUSTCRLM"),
            Range1From027 = GetString(v, "IDFROM1"),
            Range1To028 = GetString(v, "IDTO1"),
            Range1Type029 = GetString(v, "INDEX1"),
            Range2From030 = GetString(v, "IDFROM2"),
            Range2To031 = GetString(v, "IDTO2"),
            Range2Type032 = GetString(v, "INDEX2"),
            Range3From033 = GetString(v, "IDFROM3"),
            Range3To034 = GetString(v, "IDTO3"),
            Range3Type035 = GetString(v, "INDEX3"),
            Range4From036 = GetString(v, "IDFROM4"),
            Range4To037 = GetString(v, "IDTO4"),
            Range4Type038 = GetString(v, "INDEX4"),
            NumberofDaysOutstandingFrom039 = GetString(v, "DAYSOUTFRM"),
            NumberofDaysOutstandingTo040 = GetString(v, "DAYSOUTTO"),
            CurrentAmountDueSource041 = GetString(v, "AMTDUE1TC"),
            CurrentAmountDueFunctional042 = GetString(v, "AMTDUE1HC"),
            CurrentDate043 = GetDate(v, "DATEDUE1"),
            Period1AmountDueSource044 = GetString(v, "AMTDUE2TC"),
            Period1AmountDueFunctional045 = GetString(v, "AMTDUE2HC"),
            Period1Date046 = GetDate(v, "DATEDUE2"),
            Period2AmountDueSource047 = GetString(v, "AMTDUE3TC"),
            Period2AmountDueFunctional048 = GetString(v, "AMTDUE3HC"),
            Period2Date049 = GetDate(v, "DATEDUE3"),
            Period3AmountDueSource050 = GetString(v, "AMTDUE4TC"),
            Period3AmountDueFunctional051 = GetString(v, "AMTDUE4HC"),
            Period3Date052 = GetDate(v, "DATEDUE4"),
            Period4AmountDueSource053 = GetString(v, "AMTDUE5TC"),
            Period4AmountDueFunctional054 = GetString(v, "AMTDUE5HC"),
            Period4Date055 = GetDate(v, "DATEDUE5"),
            Period5AmountDueSource056 = GetString(v, "AMTDUE6TC"),
            Period5AmountDueFunctional057 = GetString(v, "AMTDUE6HC"),
            Period5Date058 = GetDate(v, "DATEDUE6"),
            Period6AmountDueSource059 = GetString(v, "AMTDUE7TC"),
            Period6AmountDueFunctional060 = GetString(v, "AMTDUE7HC"),
            Period6Date061 = GetDate(v, "DATEDUE7"),
            Period7AmountDueSource062 = GetString(v, "AMTDUE8TC"),
            Period7AmountDueFunctional063 = GetString(v, "AMTDUE8HC"),
            Period7Date064 = GetDate(v, "DATEDUE8"),
            Period8AmountDueSource065 = GetString(v, "AMTDUE9TC"),
            Period8AmountDueFunctional066 = GetString(v, "AMTDUE9HC"),
            Period8Date067 = GetDate(v, "DATEDUE9"),
            PeriodOpenItemScheduleDue068 = GetString(v, "CNTPERDDUE"),
            TotalBackwardAgingSource069 = GetString(v, "TOTBKWDTC"),
            TotalBackwardAgingFunctional070 = GetString(v, "TOTBKWDHC"),
            TotalForwardAgingSource071 = GetString(v, "TOTFWDTC"),
            TotalForwardAgingFunctional072 = GetString(v, "TOTFWDHC"),
            DisplayMeter073 = GetString(v, "SWOPTMETER"),
            IncludeAppliedDetails074 = GetString(v, "SWMATCHING"),
            CustomerAccountType075 = GetString(v, "SWACCTTYPE"),
            CutoffBy076 = GetString(v, "SWCUTOFFBY"),
            CutoffYear077 = GetString(v, "CUTOFFYEAR"),
            CutoffPeriod078 = GetString(v, "CUTOFFPERD"),
            FieldName1079 = GetString(v, "FIELDNAME1"),
            FieldName2080 = GetString(v, "FIELDNAME2"),
            FieldName3081 = GetString(v, "FIELDNAME3"),
            FieldName4082 = GetString(v, "FIELDNAME4"),
            IncludePrepayment083 = GetString(v, "SWPREPAYMT"),
            SortFieldIndex1085 = GetString(v, "SORTINDEX1"),
            SortFieldIndex2086 = GetString(v, "SORTINDEX2"),
            SortFieldIndex3087 = GetString(v, "SORTINDEX3"),
            SortFieldIndex4088 = GetString(v, "SORTINDEX4"),
            SortFieldName1089 = GetString(v, "SORTNAME1"),
            SortFieldName2090 = GetString(v, "SORTNAME2"),
            SortFieldName3091 = GetString(v, "SORTNAME3"),
            SortFieldName4092 = GetString(v, "SORTNAME4"),
            IncludeInvoice093 = GetString(v, "SWINVOICE"),
            IncludeDebitNote094 = GetString(v, "SWDEBIT"),
            IncludeCreditNote095 = GetString(v, "SWCREDIT"),
            IncludeInterest096 = GetString(v, "SWINTEREST"),
            IncludeUnappliedCash097 = GetString(v, "SWUNAPCASH"),
            IncludeReceipt098 = GetString(v, "SWRECEIPT"),
            IncludeRefund099 = GetString(v, "SWREFUND"),
            IncludeAdjustment100 = GetString(v, "SWADJUST"),
            FromDate101 = GetDate(v, "FROMDATE"),
            FromYear102 = GetString(v, "FROMYEAR"),
            FromPeriod103 = GetString(v, "FROMPERD"),
            ShowAgedRetainage104 = GetString(v, "SWAGERTG")
        };

        return (
            ProcessOut.Ok($"Sage 300 AR Customer Number : {customerNumber}", customerNumber),
            analysis);
    }

    private static string? GetString(dynamic view, string fieldName)
    {
        try
        {
            return Convert.ToString(view.Fields.FieldByName(fieldName).Value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetDate(dynamic view, string fieldName)
    {
        try
        {
            var value = view.Fields.FieldByName(fieldName).Value;
            if (value is DateTime dt && dt != default)
            {
                return dt;
            }

            if (value is null)
            {
                return null;
            }

            var s = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime parsed))
            {
                return parsed;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
