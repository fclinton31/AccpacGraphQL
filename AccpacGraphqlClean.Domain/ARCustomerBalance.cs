namespace AccpacGraphqlClean.Domain;

public sealed class ARCustomerBalance
{
    [SageField("IDCUST")]
    public string? CustomerNumber000 { get; set; }

    [SageField("NAMECUST")]
    public string? CustomerName001 { get; set; }

    [SageField("CURNCUST")]
    public string? CustomerCurrency002 { get; set; }

    [SageField("SWCHKCUST")]
    public string? CheckCustomerCreditLimit003 { get; set; }

    [SageField("AMTLIMITC")]
    public string? CustomerCreditLimit004 { get; set; }

    [SageField("AMTBALCUST")]
    public string? CustomerBalance005 { get; set; }

    [SageField("SWCHKOVERC")]
    public string? CalcCustomerOverdue006 { get; set; }

    [SageField("OVERDAYSC")]
    public string? CustomerDaysOverdue007 { get; set; }

    [SageField("OVERAMTC")]
    public string? CustomerAmountOverdue008 { get; set; }

    [SageField("OVERBALC")]
    public string? CustomerBalanceOverdue009 { get; set; }

    [SageField("IDNATACCT")]
    public string? NationalAccountNumber010 { get; set; }

    [SageField("NAMEACCT")]
    public string? NationalAccountName011 { get; set; }
}
