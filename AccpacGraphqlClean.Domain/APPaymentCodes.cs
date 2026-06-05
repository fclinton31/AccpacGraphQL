namespace AccpacGraphqlClean.Domain;

public sealed class APPaymentCodes
{
    public string? Compid { get; set; }

    [SageField("PAYMCODE")]
    public string? PaymentCode000 { get; set; }

    [SageField("TEXTDESC")]
    public string? Description001 { get; set; }

    [SageField("ACTVSW")]
    public string? Status002 { get; set; }

    [SageField("PAYMTYPE")]
    public string? PaymentType005 { get; set; }
}

