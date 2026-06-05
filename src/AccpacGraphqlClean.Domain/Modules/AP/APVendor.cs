namespace AccpacGraphqlClean.Domain;

public sealed class APVendor
{
    public string? Compid { get; set; }

    [SageField("VENDORID")]
    public string? VendorNumber000 { get; set; }

    [SageField("SHORTNAME")]
    public string? ShortName001 { get; set; }

    [SageField("IDGRP")]
    public string? GroupCode002 { get; set; }

    [SageField("SWACTV")]
    public string? Status003 { get; set; }

    [SageField("SWHOLD")]
    public string? OnHold006 { get; set; }

    [SageField("DATESTART")]
    public DateTime? StartDate007 { get; set; }

    [SageField("IDPPNT")]
    public string? ParticipantID008 { get; set; }

    [SageField("VENDNAME")]
    public string? VendorName009 { get; set; }

    [SageField("TEXTSTRE1")]
    public string? AddressLine1010 { get; set; }

    [SageField("TEXTSTRE2")]
    public string? AddressLine2011 { get; set; }

    [SageField("TEXTSTRE3")]
    public string? AddressLine3012 { get; set; }

    [SageField("TEXTSTRE4")]
    public string? AddressLine4013 { get; set; }

    [SageField("NAMECITY")]
    public string? City014 { get; set; }

    [SageField("CODESTTE")]
    public string? StateProv015 { get; set; }

    [SageField("CODEPSTL")]
    public string? ZipPostalCode016 { get; set; }

    [SageField("CODECTRY")]
    public string? Country017 { get; set; }

    [SageField("NAMECTAC")]
    public string? ContactName018 { get; set; }

    [SageField("TEXTPHON1")]
    public string? PhoneNumber019 { get; set; }

    [SageField("TEXTPHON2")]
    public string? FaxNumber020 { get; set; }

    [SageField("IDACCTSET")]
    public string? AccountSet022 { get; set; }

    [SageField("RATETYPE")]
    public string? RateType024 { get; set; }

    [SageField("BANKID")]
    public string? BankCode025 { get; set; }

    [SageField("PRTSEPCHKS")]
    public string? PrintSeparateChecks026 { get; set; }

    [SageField("DISTSETID")]
    public string? DistributionSet027 { get; set; }

    [SageField("DISTCODE")]
    public string? DistributionCode028 { get; set; }

    [SageField("GLACCNT")]
    public string? GLAccount029 { get; set; }

    [SageField("TERMSCODE")]
    public string? Terms030 { get; set; }

    [SageField("DUPAMTCODE")]
    public string? DuplicateAmountCode032 { get; set; }

    [SageField("DUPDATECD")]
    public string? DuplicateDateCode033 { get; set; }

    [SageField("CODETAXGRP")]
    public string? TaxGroup034 { get; set; }

    [SageField("TAXCLASS1")]
    public string? TaxClassCode1035 { get; set; }

    [SageField("TAXCLASS2")]
    public string? TaxClassCode2036 { get; set; }

    [SageField("TAXCLASS3")]
    public string? TaxClassCode3037 { get; set; }

    [SageField("TAXCLASS4")]
    public string? TaxClassCode4038 { get; set; }

    [SageField("TAXCLASS5")]
    public string? TaxClassCode5039 { get; set; }

    [SageField("TAXRPTSW")]
    public string? TaxReportingType040 { get; set; }

    [SageField("TAXNBR")]
    public string? CPRSTaxNumber042 { get; set; }

    [SageField("TAXIDTYPE")]
    public string? TaxType043 { get; set; }

    [SageField("CLASID")]
    public string? CPRSCode045 { get; set; }

    [SageField("AMTCRLIMT")]
    public string? CreditLimit046 { get; set; }

    [SageField("DTLASTRVAL")]
    public DateTime? DateofLastRevaluation051 { get; set; }

    [SageField("AMTBALLARV")]
    public string? LastRevaluationBalance052 { get; set; }

    [SageField("CNTINVPAID")]
    public string? NumberofPaidInvoices055 { get; set; }

    [SageField("DAYSTOPAY")]
    public string? NumberofDaystoPay056 { get; set; }

    [SageField("DATEINVCHI")]
    public DateTime? DateofLargestInvoice057 { get; set; }

    [SageField("DATEBALHI")]
    public DateTime? DateofHighestBalance058 { get; set; }

    [SageField("DATEINVHIL")]
    public DateTime? DateofLargestInvoiceLastYr059 { get; set; }

    [SageField("DATEBALHIL")]
    public DateTime? DateofHighestBalanceLastYr060 { get; set; }

    [SageField("DATELASTIV")]
    public DateTime? DateofLastInvoice062 { get; set; }

    [SageField("DATELASTCR")]
    public DateTime? DateofLastCreditNote063 { get; set; }

    [SageField("DATELASTDR")]
    public DateTime? DateofLastDebitNote064 { get; set; }

    [SageField("DATELASTPA")]
    public DateTime? DateofLastPayment065 { get; set; }

    [SageField("DATELASTDI")]
    public DateTime? DateofLastDiscount066 { get; set; }

    [SageField("DATELSTADJ")]
    public DateTime? DateofLastAdjustment067 { get; set; }

    [SageField("AMTINVHIT")]
    public string? LargestInvoiceVendCurr070 { get; set; }

    [SageField("AMTBALHIT")]
    public string? HighestBalanceVendCurr071 { get; set; }

    [SageField("AMTINVHILT")]
    public string? LargInvLastYrVendCurr073 { get; set; }

    [SageField("AMTBALHILT")]
    public string? HighBalLastYrVendCurr074 { get; set; }

    [SageField("AMTLASTIVT")]
    public string? LastInvoiceAmtVendCurr076 { get; set; }

    [SageField("AMTLASTCRT")]
    public string? LastCrNoteAmtVendCurr077 { get; set; }

    [SageField("AMTLASTDRT")]
    public string? LastDrNoteAmtVendCurr078 { get; set; }

    [SageField("AMTLASTPYT")]
    public string? LastPaymentVendCurr079 { get; set; }

    [SageField("AMTLASTDIT")]
    public string? LastDiscountAmtVendCurr080 { get; set; }

    [SageField("AMTLASTADT")]
    public string? LastAdjAmtVendCurr081 { get; set; }

    [SageField("AMTINVHIH")]
    public string? LargestInvoiceFuncCurr082 { get; set; }

    [SageField("AMTBALHIH")]
    public string? HighestBalanceFuncCurr083 { get; set; }

    [SageField("AMTINVHILH")]
    public string? LargInvLastYrFuncCurr085 { get; set; }

    [SageField("AMTBALHILH")]
    public string? HighBalLastYrFuncCurr086 { get; set; }

    [SageField("AMTLASTIVH")]
    public string? LastInvoiceAmtFuncCurr088 { get; set; }

    [SageField("AMTLASTCRH")]
    public string? LastCrNoteAmtFuncCurr089 { get; set; }

    [SageField("AMTLASTDRH")]
    public string? LastDrNoteAmtFuncCurr090 { get; set; }

    [SageField("AMTLASTPYH")]
    public string? LastPaymentFuncCurr091 { get; set; }

    [SageField("AMTLASTDIH")]
    public string? LastDiscountAmtFuncCurr092 { get; set; }

    [SageField("AMTLASTADH")]
    public string? LastAdjAmtFuncCurr093 { get; set; }

    [SageField("PAYMCODE")]
    public string? PaymentCode094 { get; set; }

    [SageField("IDTAXREGI1")]
    public string? TaxRegistrationCode1095 { get; set; }

    [SageField("IDTAXREGI2")]
    public string? TaxRegistrationCode2096 { get; set; }

    [SageField("IDTAXREGI3")]
    public string? TaxRegistrationCode3097 { get; set; }

    [SageField("IDTAXREGI4")]
    public string? TaxRegistrationCode4098 { get; set; }

    [SageField("IDTAXREGI5")]
    public string? TaxRegistrationCode5099 { get; set; }

    [SageField("SWDISTBY")]
    public string? DistributionType100 { get; set; }

    [SageField("CODECHECK")]
    public string? CheckLanguage101 { get; set; }

    [SageField("AMTINVPDHC")]
    public string? TotalInvoicesPaidFuncCurr104 { get; set; }

    [SageField("AMTINVPDTC")]
    public string? TotalInvoicesPaidVendCurr105 { get; set; }

    [SageField("CNTNBRCHKS")]
    public string? TotalNumberofPayments106 { get; set; }

    [SageField("SWTXINC1")]
    public string? TaxIncluded1107 { get; set; }

    [SageField("SWTXINC2")]
    public string? TaxIncluded2108 { get; set; }

    [SageField("SWTXINC3")]
    public string? TaxIncluded3109 { get; set; }

    [SageField("SWTXINC4")]
    public string? TaxIncluded4110 { get; set; }

    [SageField("SWTXINC5")]
    public string? TaxIncluded5111 { get; set; }

    [SageField("EMAIL1")]
    public string? ContactsEmail112 { get; set; }

    [SageField("EMAIL2")]
    public string? Email113 { get; set; }

    [SageField("WEBSITE")]
    public string? WebSite114 { get; set; }

    [SageField("CTACPHONE")]
    public string? ContactsPhone115 { get; set; }

    [SageField("CTACFAX")]
    public string? ContactsFax116 { get; set; }

    [SageField("DELMETHOD")]
    public string? DeliveryMethod117 { get; set; }

    [SageField("RTGPERCENT")]
    public string? PercentRetained118 { get; set; }

    [SageField("RTGDAYS")]
    public string? DaysRetained119 { get; set; }

    [SageField("RTGTERMS")]
    public string? RetainageTermsCode120 { get; set; }

    [SageField("LEGALNAME")]
    public string? LegalName126 { get; set; }

    [SageField("CHK1099AMT")]
    public string? Zero1099AmountWarning127 { get; set; }

    [SageField("IDCUST")]
    public string? CustomerNumber128 { get; set; }

    [SageField("EWSUPPRESS")]
    public bool? SuppressIntegration129 { get; set; }

    [SageField("EWAPVER")]
    public string? APVersion130 { get; set; }

    [SageField("EWORGID")]
    public string? Database131 { get; set; }

    [SageField("EWMODE")]
    public string? Mode132 { get; set; }

    public IReadOnlyList<OptionalField>? VendorOptFields { get; set; }
}

