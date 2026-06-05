namespace AccpacGraphqlClean.Domain;

public sealed class PaymentSchedule
{
    [SageField("PAYMNBR")]
    public string? PaymentNumber001 { get; set; }

    [SageField("PCTPAYMDUE")]
    public string? PercentageDue003 { get; set; }

    [SageField("DISCTYPE")]
    public string? Reserved004 { get; set; }

    [SageField("PCTDISC")]
    public string? DiscountPercent005 { get; set; }

    [SageField("DISNBRDAYS")]
    public string? DiscountNumberofDays006 { get; set; }

    [SageField("DISCDAY")]
    public string? DiscountDayofMonth007 { get; set; }

    [SageField("DUETYPE")]
    public string? Reserved008 { get; set; }

    [SageField("DUENBRDAYS")]
    public string? DueNumberofDays009 { get; set; }

    [SageField("DUEDAY")]
    public string? DueDayofMonth010 { get; set; }
}

