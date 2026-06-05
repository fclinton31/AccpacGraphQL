namespace AccpacGraphqlClean.Domain;

public sealed class APPaymentTerms
{
    public string? Compid { get; set; }

    [SageField("TERMSCODE")]
    public string? TermsCode000 { get; set; }

    [SageField("CODEDESC")]
    public string? Description001 { get; set; }

    [SageField("SWACTV")]
    public string? Status002 { get; set; }

    [SageField("SWMULTPAYM")]
    public string? UsePaymentSchedule005 { get; set; }

    [SageField("CODEVAT")]
    public string? CalcBaseforDiscountwithTax006 { get; set; }

    [SageField("CODEDISTYP")]
    public string? MethodofCalcforDiscountDate007 { get; set; }

    [SageField("DISDAYSTR1")]
    public string? DiscountTableStartingDay1008 { get; set; }

    [SageField("DISDAYSTR2")]
    public string? DiscountTableStartingDay2009 { get; set; }

    [SageField("DISDAYSTR3")]
    public string? DiscountTableStartingDay3010 { get; set; }

    [SageField("DISDAYSTR4")]
    public string? DiscountTableStartingDay4011 { get; set; }

    [SageField("DISDAYEND1")]
    public string? DiscountTableEndingDay1012 { get; set; }

    [SageField("DISDAYEND2")]
    public string? DiscountTableEndingDay2013 { get; set; }

    [SageField("DISDAYEND3")]
    public string? DiscountTableEndingDay3014 { get; set; }

    [SageField("DISDAYEND4")]
    public string? DiscountTableEndingDay4015 { get; set; }

    [SageField("DISMTHADD1")]
    public string? DiscountTableAddMonths1016 { get; set; }

    [SageField("DISMTHADD2")]
    public string? DiscountTableAddMonths2017 { get; set; }

    [SageField("DISMTHADD3")]
    public string? DiscountTableAddMonths3018 { get; set; }

    [SageField("DISMTHADD4")]
    public string? DiscountTableAddMonths4019 { get; set; }

    [SageField("DISDAYUSE1")]
    public string? DiscountTableDayofMonth1020 { get; set; }

    [SageField("DISDAYUSE2")]
    public string? DiscountTableDayofMonth2021 { get; set; }

    [SageField("DISDAYUSE3")]
    public string? DiscountTableDayofMonth3022 { get; set; }

    [SageField("DISDAYUSE4")]
    public string? DiscountTableDayofMonth4023 { get; set; }

    [SageField("CODEDUETYP")]
    public string? MethodofCalcforDueDate024 { get; set; }

    [SageField("DUEDAYSTR1")]
    public string? DueTableStartingDay1025 { get; set; }

    [SageField("DUEDAYSTR2")]
    public string? DueTableStartingDay2026 { get; set; }

    [SageField("DUEDAYSTR3")]
    public string? DueTableStartingDay3027 { get; set; }

    [SageField("DUEDAYSTR4")]
    public string? DueTableStartingDay4028 { get; set; }

    [SageField("DUEDAYEND1")]
    public string? DueTableEndingDay1029 { get; set; }

    [SageField("DUEDAYEND2")]
    public string? DueTableEndingDay2030 { get; set; }

    [SageField("DUEDAYEND3")]
    public string? DueTableEndingDay3031 { get; set; }

    [SageField("DUEDAYEND4")]
    public string? DueTableEndingDay4032 { get; set; }

    [SageField("DUEMTHADD1")]
    public string? DueTableAddMonths1033 { get; set; }

    [SageField("DUEMTHADD2")]
    public string? DueTableAddMonths2034 { get; set; }

    [SageField("DUEMTHADD3")]
    public string? DueTableAddMonths3035 { get; set; }

    [SageField("DUEMTHADD4")]
    public string? DueTableAddMonths4036 { get; set; }

    [SageField("DUEDAYUSE1")]
    public string? DueTableDayofMonth1037 { get; set; }

    [SageField("DUEDAYUSE2")]
    public string? DueTableDayofMonth2038 { get; set; }

    [SageField("DUEDAYUSE3")]
    public string? DueTableDayofMonth3039 { get; set; }

    [SageField("DUEDAYUSE4")]
    public string? DueTableDayofMonth4040 { get; set; }

    public IReadOnlyList<PaymentSchedule>? PaymentSchedules { get; set; }
}

