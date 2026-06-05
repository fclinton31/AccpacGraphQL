namespace AccpacGraphqlClean.Domain;

public sealed class APVendorGroup
{
    public string? Compid { get; set; }

    [SageField("GROUPID")]
    public string? GroupCode000 { get; set; }

    [SageField("DESCRIPTN")]
    public string? Description001 { get; set; }

    [SageField("ACTIVESW")]
    public string? Status002 { get; set; }

    [SageField("INACTIVEDT")]
    public DateTime? InactiveDate003 { get; set; }

    [SageField("LSTMNTDATE")]
    public DateTime? DateLastMaintained004 { get; set; }

    [SageField("ACCTSETID")]
    public string? AccountSet005 { get; set; }

    [SageField("RATETYPEID")]
    public string? RateType007 { get; set; }

    [SageField("BANKID")]
    public string? BankCode008 { get; set; }

    [SageField("PRTSEPCHKS")]
    public string? PrintSeparateChecks009 { get; set; }

    [SageField("DISTSETID")]
    public string? DistributionSet010 { get; set; }

    [SageField("DISTCODE")]
    public string? DistributionCode011 { get; set; }

    [SageField("GLACCTID")]
    public string? GeneralLedgerAccountNo012 { get; set; }

    [SageField("TERMCODE")]
    public string? Terms013 { get; set; }

    [SageField("DUPLAMT")]
    public string? DuplicateAmountCode014 { get; set; }

    [SageField("DUPLDATE")]
    public string? DuplicateDateCode015 { get; set; }

    [SageField("TAXGRP")]
    public string? TaxGroup016 { get; set; }

    [SageField("TAXCLASS1")]
    public string? TaxClass1017 { get; set; }

    [SageField("TAXCLASS2")]
    public string? TaxClass2018 { get; set; }

    [SageField("TAXCLASS3")]
    public string? TaxClass3019 { get; set; }

    [SageField("TAXCLASS4")]
    public string? TaxClass4020 { get; set; }

    [SageField("TAXCLASS5")]
    public string? TaxClass5021 { get; set; }

    [SageField("TAXRPTSW")]
    public string? TaxReportingType022 { get; set; }

    [SageField("CLASSID")]
    public string? S1099CPRSCode023 { get; set; }

    [SageField("PAYMCODE")]
    public string? PaymentCode024 { get; set; }

    [SageField("SWDISTBY")]
    public string? DistributionType025 { get; set; }

    [SageField("SWTXINC1")]
    public string? TaxIncluded1026 { get; set; }

    [SageField("SWTXINC2")]
    public string? TaxIncluded2027 { get; set; }

    [SageField("SWTXINC3")]
    public string? TaxIncluded3028 { get; set; }

    [SageField("SWTXINC4")]
    public string? TaxIncluded4029 { get; set; }

    [SageField("SWTXINC5")]
    public string? TaxIncluded5030 { get; set; }
}

