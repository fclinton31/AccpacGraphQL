namespace AccpacGraphqlClean.Domain;

public sealed class ARCustomerGroups
{
    public string? Compid { get; set; }

    [SageField("IDGRP")]
    public string? GroupCode000 { get; set; }

    [SageField("TEXTDESC")]
    public string? Description001 { get; set; }

    [SageField("SWACTV")]
    public string? Status002 { get; set; }

    [SageField("IDACCTSET")]
    public string? AccountSet005 { get; set; }

    [SageField("IDAUTOCASH")]
    public string? AutocashProfile006 { get; set; }

    [SageField("IDBILLCYCL")]
    public string? BillingCycle007 { get; set; }

    [SageField("IDSVCCHG")]
    public string? InterestProfile008 { get; set; }

    [SageField("SWBALFWD")]
    public string? AccountType009 { get; set; }

    [SageField("CODETERM")]
    public string? Terms010 { get; set; }

    [SageField("RATETYPE")]
    public string? RateType011 { get; set; }

    [SageField("SWCROVRD")]
    public string? AllowEditofCreditLimit012 { get; set; }

    [SageField("CDCRLMCUR1")]
    public string? CreditLimit1Currency013 { get; set; }

    [SageField("AMCRLMCUR1")]
    public string? CreditLimit1Amount014 { get; set; }

    [SageField("CDCRLMCUR2")]
    public string? CreditLimit2Currency015 { get; set; }

    [SageField("AMCRLMCUR2")]
    public string? CreditLimit2Amount016 { get; set; }

    [SageField("CDCRLMCUR3")]
    public string? CreditLimit3Currency017 { get; set; }

    [SageField("AMCRLMCUR3")]
    public string? CreditLimit3Amount018 { get; set; }

    [SageField("CDCRLMCUR4")]
    public string? CreditLimit4Currency019 { get; set; }

    [SageField("AMCRLMCUR4")]
    public string? CreditLimit4Amount020 { get; set; }

    [SageField("CDCRLMCUR5")]
    public string? CreditLimit5Currency021 { get; set; }

    [SageField("AMCRLMCUR5")]
    public string? CreditLimit5Amount022 { get; set; }

    [SageField("CODETAXGRP")]
    public string? TaxGroup025 { get; set; }

    [SageField("TAXSTTS1")]
    public string? TaxClassCode1026 { get; set; }

    [SageField("TAXSTTS2")]
    public string? TaxClassCode2027 { get; set; }

    [SageField("TAXSTTS3")]
    public string? TaxClassCode3028 { get; set; }

    [SageField("TAXSTTS4")]
    public string? TaxClassCode4029 { get; set; }

    [SageField("TAXSTTS5")]
    public string? TaxClassCode5030 { get; set; }

    [SageField("CODESLSP1")]
    public string? Salesperson1031 { get; set; }

    [SageField("CODESLSP2")]
    public string? Salesperson2032 { get; set; }

    [SageField("CODESLSP3")]
    public string? Salesperson3033 { get; set; }

    [SageField("CODESLSP4")]
    public string? Salesperson4034 { get; set; }

    [SageField("CODESLSP5")]
    public string? Salesperson5035 { get; set; }

    [SageField("PCTSASPLT1")]
    public string? SalesSplitPercentage1036 { get; set; }

    [SageField("PCTSASPLT2")]
    public string? SalesSplitPercentage2037 { get; set; }

    [SageField("PCTSASPLT3")]
    public string? SalesSplitPercentage3038 { get; set; }

    [SageField("PCTSASPLT4")]
    public string? SalesSplitPercentage4039 { get; set; }

    [SageField("PCTSASPLT5")]
    public string? SalesSplitPercentage5040 { get; set; }

    [SageField("SWPRTSTMT")]
    public string? PrintStatements041 { get; set; }

    [SageField("SWCHKLIMIT")]
    public string? CheckCreditLimit042 { get; set; }

    [SageField("SWCHKOVER")]
    public string? CheckOverdueAmounts043 { get; set; }

    [SageField("OVERDAYS")]
    public string? DaysOverdue044 { get; set; }

    [SageField("OVERAMT1")]
    public string? AmountOverdue1045 { get; set; }

    [SageField("OVERAMT2")]
    public string? AmountOverdue2046 { get; set; }

    [SageField("OVERAMT3")]
    public string? AmountOverdue3047 { get; set; }

    [SageField("OVERAMT4")]
    public string? AmountOverdue4048 { get; set; }

    [SageField("OVERAMT5")]
    public string? AmountOverdue5049 { get; set; }
}
