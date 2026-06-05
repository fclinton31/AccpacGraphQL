namespace AccpacGraphqlClean.Domain;

public sealed class APPayment
{
    public string? Compid { get; set; }
    public DateTime? BatchDate { get; set; }
    public string? BatchDescription { get; set; }

    public string? BatchNumber001 { get; set; }
    public string? EntryNumber002 { get; set; }
    public string? CheckNumber003 { get; set; }
    public string? VendorNumber004 { get; set; }
    public DateTime? PaymentDateAdjustmentDate005 { get; set; }
    public string? EntryDescription006 { get; set; }
    public string? VendorPayeeName007 { get; set; }
    public string? VendorExchangeRate010 { get; set; }
    public string? VendorRateOverridden011 { get; set; }
    public string? TotalPrepayVendorCurr013 { get; set; }
    public string? PaymentCode015 { get; set; }
    public string? BankRateType017 { get; set; }
    public string? BankExchangeRate018 { get; set; }
    public string? BankRateOverridden019 { get; set; }
    public string? PaymentTransType020 { get; set; }
    public string? DocumentType021 { get; set; }
    public DateTime? VendorRateDate025 { get; set; }
    public string? VendorRateType026 { get; set; }
    public DateTime? BankRateDate028 { get; set; }
    public string? DocumentNumber032 { get; set; }
    public string? PaymentEdited033 { get; set; }
    public string? CheckPrintRequired034 { get; set; }
    public string? VendorRemitToLocation035 { get; set; }
    public string? EntryReference036 { get; set; }
    public string? CheckPrintedStatus040 { get; set; }
    public string? AddressLine1041 { get; set; }
    public string? AddressLine2042 { get; set; }
    public string? AddressLine3043 { get; set; }
    public string? AddressLine4044 { get; set; }
    public string? City045 { get; set; }
    public string? State046 { get; set; }
    public string? ZipPostalCode047 { get; set; }
    public string? Country048 { get; set; }
    public string? PaymentLanguage049 { get; set; }
    public DateTime? PrepayActivationDate054 { get; set; }
    public string? JobRelated055 { get; set; }
    public string? JobApplyMethod056 { get; set; }
    public string? MatchingDocumentNumber059 { get; set; }
    public string? SourceApplication062 { get; set; }
    public string? BankCode063 { get; set; }
    public string? BankCurrencyCode064 { get; set; }
    public string? CashAccount066 { get; set; }
    public string? S1099CPRSCode070 { get; set; }
    public string? S1099CPRSAmount071 { get; set; }
    public string? CalculateTaxAmountControl072 { get; set; }
    public string? CalculateTaxBaseControl073 { get; set; }
    public string? TaxGroup074 { get; set; }
    public string? TaxClass1081 { get; set; }
    public string? TaxClass2082 { get; set; }
    public string? TaxClass3083 { get; set; }
    public string? TaxClass4084 { get; set; }
    public string? TaxClass5085 { get; set; }
    public string? TaxIncluded1086 { get; set; }
    public string? TaxIncluded2087 { get; set; }
    public string? TaxIncluded3088 { get; set; }
    public string? TaxIncluded4089 { get; set; }
    public string? TaxIncluded5090 { get; set; }
    public string? TaxBase1091 { get; set; }
    public string? TaxBase2092 { get; set; }
    public string? TaxBase3093 { get; set; }
    public string? TaxBase4094 { get; set; }
    public string? TaxBase5095 { get; set; }
    public string? TaxAmount1096 { get; set; }
    public string? TaxAmount2097 { get; set; }
    public string? TaxAmount3098 { get; set; }
    public string? TaxAmount4099 { get; set; }
    public string? TaxAmount5100 { get; set; }
    public string? TaxReportingCurrencyCode106 { get; set; }
    public string? TaxReportingCalculateMethod107 { get; set; }
    public string? TaxReportingExchangeRate108 { get; set; }
    public string? TaxReportingRateType109 { get; set; }
    public DateTime? TaxReportingRateDate110 { get; set; }
    public string? TaxReportingAmount1113 { get; set; }
    public string? TaxReportingAmount2114 { get; set; }
    public string? TaxReportingAmount3115 { get; set; }
    public string? TaxReportingAmount4116 { get; set; }
    public string? TaxReportingAmount5117 { get; set; }
    public string? EnteredBy144 { get; set; }
    public DateTime? PostingDate145 { get; set; }
    public string? AccountSet146 { get; set; }

    public IReadOnlyList<APPaymentItem> APPaymentItems { get; set; } = Array.Empty<APPaymentItem>();
    public IReadOnlyList<APMiscPaymentItem> APMiscPaymentItems { get; set; } = Array.Empty<APMiscPaymentItem>();
}

public sealed class APPaymentItem
{
    public string? BatchNumber001 { get; set; }
    public string? EntryNumber002 { get; set; }
    public string? LineNumber003 { get; set; }
    public string? DocumentNumber005 { get; set; }
    public string? PaymentNumber006 { get; set; }
    public string? TransactionType007 { get; set; }
    public string? PaymentResolution008 { get; set; }
    public string? PaymentAmount009 { get; set; }
    public string? DiscountAmountTaken010 { get; set; }
    public string? Description014 { get; set; }
    public string? Reference015 { get; set; }
    public string? PPMatchingDocNo017 { get; set; }
    public string? PPMatchingDocType018 { get; set; }
    public DateTime? ActivationDate019 { get; set; }
    public string? JobApplyMethod031 { get; set; }
    public string? RetainageAmount034 { get; set; }
    public DateTime? RetainageDueDate035 { get; set; }
    public string? RetainageTermsCode036 { get; set; }
    public string? RetainageExchangeRate037 { get; set; }
    public string? DocumentType043 { get; set; }
}

public sealed class APMiscPaymentItem
{
    public string? BatchNumber001 { get; set; }
    public string? EntryNumber002 { get; set; }
    public string? LineNumber003 { get; set; }
    public string? DistributionCode004 { get; set; }
    public string? AccountNumber005 { get; set; }
    public string? GLReference006 { get; set; }
    public string? GLDescription007 { get; set; }
    public string? TaxClass1008 { get; set; }
    public string? TaxClass2009 { get; set; }
    public string? TaxClass3010 { get; set; }
    public string? TaxClass4011 { get; set; }
    public string? TaxClass5012 { get; set; }
    public string? TaxIncluded1013 { get; set; }
    public string? TaxIncluded2014 { get; set; }
    public string? TaxIncluded3015 { get; set; }
    public string? TaxIncluded4016 { get; set; }
    public string? TaxIncluded5017 { get; set; }
    public string? TaxBase1018 { get; set; }
    public string? TaxBase2019 { get; set; }
    public string? TaxBase3020 { get; set; }
    public string? TaxBase4021 { get; set; }
    public string? TaxBase5022 { get; set; }
    public string? TaxAmount1028 { get; set; }
    public string? TaxAmount2029 { get; set; }
    public string? TaxAmount3030 { get; set; }
    public string? TaxAmount4031 { get; set; }
    public string? TaxAmount5032 { get; set; }
    public string? DistAmount034 { get; set; }
    public string? TaxReportingAmount1054 { get; set; }
    public string? TaxReportingAmount2055 { get; set; }
    public string? TaxReportingAmount3056 { get; set; }
    public string? TaxReportingAmount4057 { get; set; }
    public string? TaxReportingAmount5058 { get; set; }
    public string? Contract098 { get; set; }
    public string? Project099 { get; set; }
    public string? Category100 { get; set; }
    public string? ProjectCategoryResource101 { get; set; }
    public string? BillingType103 { get; set; }
    public string? ItemNumber104 { get; set; }
    public string? UnitofMeasure105 { get; set; }
    public string? Quantity106 { get; set; }
    public string? Cost107 { get; set; }
    public DateTime? BillingDate108 { get; set; }
    public string? BillingRate109 { get; set; }
}

public sealed class APPaymentBatch
{
    public string? Compid { get; set; }
    public string? BatchNumber { get; set; }
    public string? BatchDescription { get; set; }
    public DateTime? BatchDate { get; set; }
    public string? BankCode { get; set; }
    public string? CurrencyCode { get; set; }
    public string? SourceApplication { get; set; }
    public string? PostBatch { get; set; }
    public IReadOnlyList<APPayment> BatchEntries { get; set; } = Array.Empty<APPayment>();
}

public sealed class SyncAPPayments
{
    public string CallMethod { get; set; } = "SYNC";
    public string Timestamp { get; set; } = "";
    public int RecordLimit { get; set; } = 100;
    public IReadOnlyList<APPaymentBatch> APPaymentBatches { get; set; } = Array.Empty<APPaymentBatch>();
}
