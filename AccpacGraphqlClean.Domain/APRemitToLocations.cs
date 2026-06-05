namespace AccpacGraphqlClean.Domain;

public sealed class APRemitToLocations
{
    public string? Compid { get; set; }

    [SageField("IDVEND")]
    public string? VendorNumber000 { get; set; }

    [SageField("IDVENDRMIT")]
    public string? RemitToLocation001 { get; set; }

    [SageField("SWACTV")]
    public bool? Status002 { get; set; }

    [SageField("DATELASTIV")]
    public DateTime? DateofLastActivity005 { get; set; }

    [SageField("RMITNAME")]
    public string? Description006 { get; set; }

    [SageField("TEXTSTRE1")]
    public string? AddressLine1007 { get; set; }

    [SageField("TEXTSTRE2")]
    public string? AddressLine2008 { get; set; }

    [SageField("TEXTSTRE3")]
    public string? AddressLine3009 { get; set; }

    [SageField("TEXTSTRE4")]
    public string? AddressLine4010 { get; set; }

    [SageField("NAMECITY")]
    public string? City011 { get; set; }

    [SageField("CODESTTE")]
    public string? StateProv012 { get; set; }

    [SageField("CODEPSTL")]
    public string? ZipPostalCode013 { get; set; }

    [SageField("CODECTRY")]
    public string? Country014 { get; set; }

    [SageField("NAMECTAC")]
    public string? ContactName015 { get; set; }

    [SageField("TEXTPHON1")]
    public string? PhoneNumber016 { get; set; }

    [SageField("TEXTPHON2")]
    public string? FaxNumber017 { get; set; }

    [SageField("CODECHCKLG")]
    public string? CheckLanguage018 { get; set; }

    [SageField("PRIMARYSW")]
    public string? PrimaryRemittoIndicator019 { get; set; }

    [SageField("EMAIL")]
    public string? Email020 { get; set; }

    [SageField("CTACPHONE")]
    public string? ContactsPhone021 { get; set; }

    [SageField("CTACFAX")]
    public string? ContactsFax022 { get; set; }

    [SageField("CTACEMAIL")]
    public string? ContactsEmail023 { get; set; }

    [SageField("EWSUPPRESS")]
    public bool? SuppressIntegration026 { get; set; }

    [SageField("EWAPVER")]
    public string? APVersion027 { get; set; }

    [SageField("EWORGID")]
    public string? Database028 { get; set; }

    [SageField("EWMODE")]
    public string? Mode029 { get; set; }
}

