namespace AccpacGraphqlClean.Domain;

public sealed class ARInvoice
{
    public string? Compid { get; set; }

    public string? BatchNumber000 { get; set; }
    public DateTime? BatchDate001 { get; set; }
    public string? Description002 { get; set; }

    public string? BatchNumber015 { get; set; }
    public string? EntryNumber016 { get; set; }
    public string? CustomerNumber017 { get; set; }
    public string? DocumentNumber018 { get; set; }
    public string? ShipToLocationCode019 { get; set; }
    public string? DocumentType021 { get; set; }
    public string? OrderNumber023 { get; set; }
    public string? PONumber024 { get; set; }
    public string? InvoiceDescription025 { get; set; }
    public string? ApplytoDocument027 { get; set; }
    public string? AccountSet028 { get; set; }
    public DateTime? DocumentDate029 { get; set; }
    public DateTime? AsofDate030 { get; set; }
    public string? CurrencyCode033 { get; set; }
    public string? RateType034 { get; set; }
    public string? RateOverridden035 { get; set; }
    public string? ExchangeRate036 { get; set; }
    public string? ApplytoExchangeRate037 { get; set; }
    public string? Terms038 { get; set; }
    public string? TermsCodeOverridden039 { get; set; }
    public DateTime? DueDate040 { get; set; }
    public DateTime? DiscountDate041 { get; set; }
    public string? DiscountPercentage042 { get; set; }
    public string? DiscountAmountAvailable043 { get; set; }
    public string? EnteredBy191 { get; set; }
    public DateTime? PostingDate192 { get; set; }

    public IReadOnlyList<ARInvoiceLine> ARInvoiceLines { get; set; } = Array.Empty<ARInvoiceLine>();
}

public sealed class ARInvoiceLine
{
    public string? EntryNumber001 { get; set; }
    public string? LineNumber002 { get; set; }
    public string? ItemNumber004 { get; set; }
    public string? DistributionCode005 { get; set; }
    public string? Description006 { get; set; }
    public string? UnitofMeasure008 { get; set; }
    public string? Quantity009 { get; set; }
    public string? Cost010 { get; set; }
    public string? Price011 { get; set; }
    public string? ExtendedAmountwTIP012 { get; set; }
    public string? Comments046 { get; set; }
    public string? Contract049 { get; set; }
    public string? Project050 { get; set; }
    public string? Category051 { get; set; }
    public string? ProjectCategoryResource052 { get; set; }
    public DateTime? BillingDate055 { get; set; }
}

public sealed class ARInvoiceBatch
{
    public string? BatchNumber { get; set; }
    public string? BatchDescription { get; set; }
    public DateTime? BatchDate { get; set; }
    public string? SourceApplication { get; set; }
    public IReadOnlyList<ARInvoice> BatchEntries { get; set; } = Array.Empty<ARInvoice>();
    public string? BatchStatus { get; set; }
    public string? BatchStatusDescription { get; set; }
}

public sealed class SyncARInvoices
{
    public string CallMethod { get; set; } = "SYNC";
    public string Timestamp { get; set; } = "";
    public int RecordLimit { get; set; } = 100;
    public IReadOnlyList<ARInvoiceBatch> ARInvoiceBatches { get; set; } = Array.Empty<ARInvoiceBatch>();
}
