namespace AccpacGraphqlClean.Domain;

public sealed class ARSalesPersons
{
    public string? Compid { get; set; }

    [SageField("CODESLSP")]
    public string? Salesperson000 { get; set; }

    [SageField("SWACTV")]
    public string? Status001 { get; set; }

    [SageField("CODEEMPL")]
    public string? EmployeeNumber004 { get; set; }

    [SageField("NAMEEMPL")]
    public string? Name005 { get; set; }

    [SageField("SWCOMM")]
    public string? CommissionsPaid006 { get; set; }

    [SageField("AMTANLTARG")]
    public string? AnnualSalesTarget007 { get; set; }

    [SageField("SALESBASE1")]
    public string? MaximumSalesforRate1008 { get; set; }

    [SageField("SALESBASE2")]
    public string? MaximumSalesforRate2009 { get; set; }

    [SageField("SALESBASE3")]
    public string? MaximumSalesforRate3010 { get; set; }

    [SageField("SALESBASE4")]
    public string? MaximumSalesforRate4011 { get; set; }

    [SageField("SALESRATE1")]
    public string? CommissionRate1012 { get; set; }

    [SageField("SALESRATE2")]
    public string? CommissionRate2013 { get; set; }

    [SageField("SALESRATE3")]
    public string? CommissionRate3014 { get; set; }

    [SageField("SALESRATE4")]
    public string? CommissionRate4015 { get; set; }

    [SageField("SALESRATE5")]
    public string? CommissionRate5016 { get; set; }
}
