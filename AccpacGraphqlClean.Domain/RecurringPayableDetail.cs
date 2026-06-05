namespace AccpacGraphqlClean.Domain;

public sealed class RecurringPayableDetail
{
    [SageField("IDRECURR")]
    public string? RecurringPayableCode001 { get; set; }

    [SageField("CNTLINE")]
    public string? LineNumber002 { get; set; }

    [SageField("IDDISTCODE")]
    public string? DistributionCode003 { get; set; }

    [SageField("DESC")]
    public string? DistributionDescription004 { get; set; }

    [SageField("IDGLACCT")]
    public string? GLAccount005 { get; set; }

    [SageField("TAXCLASS1")]
    public string? TaxClass1006 { get; set; }

    [SageField("TAXCLASS2")]
    public string? TaxClass2007 { get; set; }

    [SageField("TAXCLASS3")]
    public string? TaxClass3008 { get; set; }

    [SageField("TAXCLASS4")]
    public string? TaxClass4009 { get; set; }

    [SageField("TAXCLASS5")]
    public string? TaxClass5010 { get; set; }

    [SageField("SWTAXINCL1")]
    public string? TaxInclusive1011 { get; set; }

    [SageField("SWTAXINCL2")]
    public string? TaxInclusive2012 { get; set; }

    [SageField("SWTAXINCL3")]
    public string? TaxInclusive3013 { get; set; }

    [SageField("SWTAXINCL4")]
    public string? TaxInclusive4014 { get; set; }

    [SageField("SWTAXINCL5")]
    public string? TaxInclusive5015 { get; set; }

    [SageField("AMTTAX1")]
    public string? TaxAmount1016 { get; set; }

    [SageField("AMTTAX2")]
    public string? TaxAmount2017 { get; set; }

    [SageField("AMTTAX3")]
    public string? TaxAmount3018 { get; set; }

    [SageField("AMTTAX4")]
    public string? TaxAmount4019 { get; set; }

    [SageField("AMTTAX5")]
    public string? TaxAmount5020 { get; set; }

    [SageField("AMTDIST")]
    public string? DistributedAmount021 { get; set; }

    [SageField("AMTTAXINCL")]
    public string? DistTaxincludedinPrice023 { get; set; }

    [SageField("AMTTAXEXCL")]
    public string? DistTaxexcludedfromPrice024 { get; set; }

    [SageField("BASETAX1")]
    public string? TaxBase1026 { get; set; }

    [SageField("BASETAX2")]
    public string? TaxBase2027 { get; set; }

    [SageField("BASETAX3")]
    public string? TaxBase3028 { get; set; }

    [SageField("BASETAX4")]
    public string? TaxBase4029 { get; set; }

    [SageField("BASETAX5")]
    public string? TaxBase5030 { get; set; }

    [SageField("SWDISCABL")]
    public string? Discountable031 { get; set; }

    [SageField("COMMENT")]
    public string? Comment034 { get; set; }

    [SageField("CONTRACT")]
    public string? Contract035 { get; set; }

    [SageField("PROJECT")]
    public string? Project036 { get; set; }

    [SageField("CATEGORY")]
    public string? Category037 { get; set; }

    [SageField("RESOURCE")]
    public string? ProjectCategoryResource038 { get; set; }
}

