namespace AccpacGraphqlClean.Domain;

public sealed class ARTermsCodes
{
    public string? Compid { get; set; }

    public string? TermsCode000 { get; set; }
    public string? Description001 { get; set; }
    public string? Status002 { get; set; }
    public string? UsePaymentSchedule005 { get; set; }
    public string? CalcBaseforDiscountwithTax006 { get; set; }
    public string? DiscountType007 { get; set; }
    public string? DiscountTableStartingDay1008 { get; set; }
    public string? DiscountTableStartingDay2009 { get; set; }
    public string? DiscountTableStartingDay3010 { get; set; }
    public string? DiscountTableStartingDay4011 { get; set; }
    public string? DiscountTableEndingDay1012 { get; set; }
    public string? DiscountTableEndingDay2013 { get; set; }
    public string? DiscountTableEndingDay3014 { get; set; }
    public string? DiscountTableEndingDay4015 { get; set; }
    public string? DiscountTableMonthsAdded1016 { get; set; }
    public string? DiscountTableMonthsAdded2017 { get; set; }
    public string? DiscountTableMonthsAdded3018 { get; set; }
    public string? DiscountTableMonthsAdded4019 { get; set; }
    public string? DiscountTableDayofMonth1020 { get; set; }
    public string? DiscountTableDayofMonth2021 { get; set; }
    public string? DiscountTableDayofMonth3022 { get; set; }
    public string? DiscountTableDayofMonth4023 { get; set; }
    public string? DueDateType024 { get; set; }
    public string? DueDateTableStartingDay1025 { get; set; }
    public string? DueDateTableStartingDay2026 { get; set; }
    public string? DueDateTableStartingDay3027 { get; set; }
    public string? DueDateTableStartingDay4028 { get; set; }
    public string? DueDateTableEndingDay1029 { get; set; }
    public string? DueDateTableEndingDay2030 { get; set; }
    public string? DueDateTableEndingDay3031 { get; set; }
    public string? DueDateTableEndingDay4032 { get; set; }
    public string? DueDateTableMonthsAdded1033 { get; set; }
    public string? DueDateTableMonthsAdded2034 { get; set; }
    public string? DueDateTableMonthsAdded3035 { get; set; }
    public string? DueDateTableMonthsAdded4036 { get; set; }
    public string? DueDateTableDayofMonth1037 { get; set; }
    public string? DueDateTableDayofMonth2038 { get; set; }
    public string? DueDateTableDayofMonth3039 { get; set; }
    public string? DueDateTableDayofMonth4040 { get; set; }

    public IReadOnlyList<ArTermsSchedule> TermsSchedules { get; set; } = Array.Empty<ArTermsSchedule>();
}

public sealed class ArTermsSchedule
{
    public string? PaymentNumber001 { get; set; }
    public string? PercentDue003 { get; set; }
    public string? Reserved004 { get; set; }
    public string? DiscountPercent005 { get; set; }
    public string? DiscountNumberofDays006 { get; set; }
    public string? DiscountDayofMonth007 { get; set; }
    public string? Reserved008 { get; set; }
    public string? DueNumberofDays009 { get; set; }
    public string? DueDayofMonth010 { get; set; }
}
