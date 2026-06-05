namespace AccpacGraphqlClean.Domain;

public sealed class APInvoices
{
    public string? Compid { get; set; }
    public string? AppendByReference { get; set; }
    public string? PostBatch { get; set; }

    public string? BatchNumber000 { get; set; }
    public DateTime? BatchDate { get; set; }
    public string? BatchDescription { get; set; }

    public string? EntryNumber001 { get; set; }
    public string? Originator002 { get; set; }
    public string? VendorNumber003 { get; set; }
    public string? DocumentNumber004 { get; set; }
    public string? RemitToLocation005 { get; set; }
    public string? DocumentType006 { get; set; }
    public string? OrderNumber009 { get; set; }
    public string? PONumber010 { get; set; }
    public string? InvoiceDescription011 { get; set; }
    public string? ApplytoDocument013 { get; set; }
    public string? AccountSet014 { get; set; }
    public DateTime? DocumentDate015 { get; set; }
    public DateTime? AsofDate016 { get; set; }
    public string? CurrencyCode019 { get; set; }
    public string? RateType020 { get; set; }
    public string? RateOverridden021 { get; set; }
    public string? ExchangeRate022 { get; set; }
    public string? ApplytoExchangeRate023 { get; set; }
    public string? Terms024 { get; set; }
    public string? TermsOverridden025 { get; set; }
    public DateTime? DueDate026 { get; set; }
    public DateTime? DiscountDate027 { get; set; }
    public string? DiscountPercentage028 { get; set; }
    public string? DiscountAmountAvailable029 { get; set; }
    public string? TaxAmountControl032 { get; set; }
    public string? TaxGroup033 { get; set; }
    public string? TaxClass1039 { get; set; }
    public string? TaxClass2040 { get; set; }
    public string? TaxClass3041 { get; set; }
    public string? TaxClass4042 { get; set; }
    public string? TaxClass5043 { get; set; }
    public string? TaxBase1044 { get; set; }
    public string? TaxBase2045 { get; set; }
    public string? TaxBase3046 { get; set; }
    public string? TaxBase4047 { get; set; }
    public string? TaxBase5048 { get; set; }
    public string? TaxAmount1049 { get; set; }
    public string? TaxAmount2050 { get; set; }
    public string? TaxAmount3051 { get; set; }
    public string? TaxAmount4052 { get; set; }
    public string? TaxAmount5053 { get; set; }
    public string? S1099CPRSAmount054 { get; set; }
    public string? DistributionSetAmount055 { get; set; }
    public string? LocationName063 { get; set; }
    public string? AddressLine1064 { get; set; }
    public string? AddressLine2065 { get; set; }
    public string? AddressLine3066 { get; set; }
    public string? AddressLine4067 { get; set; }
    public string? City068 { get; set; }
    public string? StateProv069 { get; set; }
    public string? ZipPostalCode070 { get; set; }
    public string? Country071 { get; set; }
    public string? ContactName072 { get; set; }
    public string? PhoneNumber073 { get; set; }
    public string? FaxNumber074 { get; set; }
    public DateTime? RateDate075 { get; set; }
    public string? DistributionSet080 { get; set; }
    public string? S1099CPRSCode081 { get; set; }
    public string? DocumentTotalIncludingTax085 { get; set; }
    public string? TaxInclusive1088 { get; set; }
    public string? TaxInclusive2089 { get; set; }
    public string? TaxInclusive3090 { get; set; }
    public string? TaxInclusive4091 { get; set; }
    public string? TaxInclusive5092 { get; set; }
    public string? JobRelated114 { get; set; }
    public string? Email119 { get; set; }
    public string? ContactsPhone120 { get; set; }
    public string? ContactsFax121 { get; set; }
    public string? ContactsEmail122 { get; set; }
    public string? DiscountBase128 { get; set; }

    public string? VendorName220 { get; set; }
    public string? EnteredBy228 { get; set; }
    public DateTime? PostingDate229 { get; set; }

    public IReadOnlyList<OptionalField>? APInvoiceOpt { get; set; }
    public IReadOnlyList<APInvoiceItem>? APInvoiceItems { get; set; }
}

