namespace AccpacGraphqlClean.Domain;

public sealed class APInvoiceBatch
{
    public string? Compid { get; set; }
    public string? BatchNumber { get; set; }
    public string? BatchDesc { get; set; }
    public DateTime? BatchDate { get; set; }

    public string? BatchEntry { get; set; }
    public string? BatchStatus { get; set; }
    public string? BatchStatusDescription { get; set; }
    public string? SourceApplication { get; set; }

    public IReadOnlyList<APInvoices>? Invoice { get; set; }
}

