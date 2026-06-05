namespace AccpacGraphqlClean.Domain;

public sealed class ARBillingCycles
{
    public string? Compid { get; set; }

    [SageField("IDCYCL")]
    public string? BillingCycle000 { get; set; }

    [SageField("TEXTDESC")]
    public string? Description001 { get; set; }

    [SageField("ACTVSW")]
    public string? Status002 { get; set; }

    [SageField("LASTSTMT")]
    public DateTime? DateStatementsLastPrinted005 { get; set; }

    [SageField("LASTINTT")]
    public DateTime? DateIntInvoicesLastPosted006 { get; set; }

    [SageField("DAYSCYCL")]
    public string? BillingCycleFrequency007 { get; set; }

    [SageField("NAME")]
    public string? RemitToName008 { get; set; }

    [SageField("STREET1")]
    public string? RemitToAddress1009 { get; set; }

    [SageField("STREET2")]
    public string? RemitToAddress2010 { get; set; }

    [SageField("STREET3")]
    public string? RemitToAddress3011 { get; set; }

    [SageField("STREET4")]
    public string? RemitToAddress4012 { get; set; }

    [SageField("CITY")]
    public string? RemitToCity013 { get; set; }

    [SageField("STATE")]
    public string? RemitToStateProv014 { get; set; }

    [SageField("POSTCODE")]
    public string? RemitToZipPostalCode015 { get; set; }

    [SageField("CNTYCODE")]
    public string? RemitToCountry016 { get; set; }
}
