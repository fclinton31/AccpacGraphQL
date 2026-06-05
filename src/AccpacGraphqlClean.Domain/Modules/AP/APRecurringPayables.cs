namespace AccpacGraphqlClean.Domain;

public sealed class APRecurringPayables
{
    public string? Compid { get; set; }

    [SageField("IDVEND")]
    public string? VendorNumber000 { get; set; }

    [SageField("IDRECURR")]
    public string? RecurringPayableCode001 { get; set; }

    [SageField("DESC")]
    public string? Description002 { get; set; }

    [SageField("SWACTV")]
    public string? Status003 { get; set; }

    [SageField("DATEEFF")]
    public DateTime? EffectiveDate006 { get; set; }

    [SageField("EXPIRETYPE")]
    public string? ExpirationType007 { get; set; }

    [SageField("DATEEXPIRE")]
    public DateTime? ExpirationDate008 { get; set; }

    [SageField("MAXCOUNT")]
    public string? MaximumNumberofInvoices009 { get; set; }

    [SageField("MAXAMT")]
    public string? MaximumTotalInvoiceAmount010 { get; set; }

    [SageField("ORDERNBR")]
    public string? OrderNumber015 { get; set; }

    [SageField("PONBR")]
    public string? PONumber016 { get; set; }

    [SageField("INVCDESC")]
    public string? InvoiceDescription017 { get; set; }

    [SageField("IDRMITTO")]
    public string? RemitToLocation018 { get; set; }

    [SageField("RATETYPE")]
    public string? RateType020 { get; set; }

    [SageField("TERMSCODE")]
    public string? Terms021 { get; set; }

    [SageField("IDDISTSET")]
    public string? DistributionSet022 { get; set; }

    [SageField("AMTDISTSET")]
    public string? DistributionAmount023 { get; set; }

    [SageField("TAXGRP")]
    public string? TaxGroup024 { get; set; }

    [SageField("SWCALCTAX")]
    public string? TaxAmountControl025 { get; set; }

    [SageField("TAXCLASS1")]
    public string? TaxClass1031 { get; set; }

    [SageField("TAXCLASS2")]
    public string? TaxClass2032 { get; set; }

    [SageField("TAXCLASS3")]
    public string? TaxClass3033 { get; set; }

    [SageField("TAXCLASS4")]
    public string? TaxClass4034 { get; set; }

    [SageField("TAXCLASS5")]
    public string? TaxClass5035 { get; set; }

    [SageField("SWTAXINCL1")]
    public string? TaxInclusive1036 { get; set; }

    [SageField("SWTAXINCL2")]
    public string? TaxInclusive2037 { get; set; }

    [SageField("SWTAXINCL3")]
    public string? TaxInclusive3038 { get; set; }

    [SageField("SWTAXINCL4")]
    public string? TaxInclusive4039 { get; set; }

    [SageField("SWTAXINCL5")]
    public string? TaxInclusive5040 { get; set; }

    [SageField("AMTTAX1")]
    public string? TaxAmount1041 { get; set; }

    [SageField("AMTTAX2")]
    public string? TaxAmount2042 { get; set; }

    [SageField("AMTTAX3")]
    public string? TaxAmount3043 { get; set; }

    [SageField("AMTTAX4")]
    public string? TaxAmount4044 { get; set; }

    [SageField("AMTTAX5")]
    public string? TaxAmount5045 { get; set; }

    [SageField("CODE1099")]
    public string? S1099CPRSCode048 { get; set; }

    [SageField("AMT1099")]
    public string? S1099CPRSAmount049 { get; set; }

    [SageField("SCHEDKEY")]
    public string? Schedule051 { get; set; }

    [SageField("BASETAX1")]
    public string? TaxBase1054 { get; set; }

    [SageField("BASETAX2")]
    public string? TaxBase2055 { get; set; }

    [SageField("BASETAX3")]
    public string? TaxBase3056 { get; set; }

    [SageField("BASETAX4")]
    public string? TaxBase4057 { get; set; }

    [SageField("BASETAX5")]
    public string? TaxBase5058 { get; set; }

    [SageField("SWTXBSECTL")]
    public string? TaxBaseControl060 { get; set; }

    [SageField("SWJOB")]
    public string? JobRelated066 { get; set; }

    [SageField("IDACCTSET")]
    public string? AccountSet077 { get; set; }

    public IReadOnlyList<RecurringPayableDetail>? RecurringPayableDetails { get; set; }
}

