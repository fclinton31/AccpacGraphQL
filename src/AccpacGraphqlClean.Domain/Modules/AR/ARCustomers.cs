namespace AccpacGraphqlClean.Domain;

public sealed class ARCustomers
{
    public string? Compid { get; set; }

    [SageField("IDCUST")]
    public string? CustomerNumber000 { get; set; }

    [SageField("TEXTSNAM")]
    public string? ShortName001 { get; set; }

    [SageField("IDGRP")]
    public string? GroupCode002 { get; set; }

    [SageField("IDNATACCT")]
    public string? NationalAccount003 { get; set; }

    [SageField("SWACTV")]
    public string? Status004 { get; set; }

    [SageField("SWHOLD")]
    public string? OnHold007 { get; set; }

    [SageField("DATESTART")]
    public DateTime? StartDate008 { get; set; }

    [SageField("CODEDAB")]
    public string? CreditBureauNumber010 { get; set; }

    [SageField("CODEDABRTG")]
    public string? CreditBureauRating011 { get; set; }

    [SageField("DATEDAB")]
    public DateTime? CreditBureauDate012 { get; set; }

    [SageField("NAMECUST")]
    public string? CustomerName013 { get; set; }

    [SageField("TEXTSTRE1")]
    public string? AddressLine1014 { get; set; }

    [SageField("TEXTSTRE2")]
    public string? AddressLine2015 { get; set; }

    [SageField("TEXTSTRE3")]
    public string? AddressLine3016 { get; set; }

    [SageField("TEXTSTRE4")]
    public string? AddressLine4017 { get; set; }

    [SageField("NAMECITY")]
    public string? City018 { get; set; }

    [SageField("CODESTTE")]
    public string? StateProv019 { get; set; }

    [SageField("CODEPSTL")]
    public string? ZipPostalCode020 { get; set; }

    [SageField("CODECTRY")]
    public string? Country021 { get; set; }

    [SageField("NAMECTAC")]
    public string? ContactName022 { get; set; }

    [SageField("TEXTPHON1")]
    public string? PhoneNumber023 { get; set; }

    [SageField("TEXTPHON2")]
    public string? FaxNumber024 { get; set; }

    [SageField("CODETERR")]
    public string? TerritoryCode025 { get; set; }

    [SageField("IDACCTSET")]
    public string? AccountSet026 { get; set; }

    [SageField("IDAUTOCASH")]
    public string? AutocashProfile027 { get; set; }

    [SageField("IDBILLCYCL")]
    public string? BillingCycle028 { get; set; }

    [SageField("IDSVCCHRG")]
    public string? InterestProfile029 { get; set; }

    [SageField("CODECURN")]
    public string? CurrencyCode031 { get; set; }

    [SageField("SWPRTSTMT")]
    public string? PrintStatements032 { get; set; }

    [SageField("SWBALFWD")]
    public string? AccountType034 { get; set; }

    [SageField("CODETERM")]
    public string? Terms035 { get; set; }

    [SageField("IDRATETYPE")]
    public string? RateType036 { get; set; }

    [SageField("CODETAXGRP")]
    public string? TaxGroup037 { get; set; }
}
