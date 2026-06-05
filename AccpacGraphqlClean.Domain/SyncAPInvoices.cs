namespace AccpacGraphqlClean.Domain;

public sealed class SyncAPInvoices
{
    public string CallMethod { get; set; } = "SYNC";
    public string Timestamp { get; set; } = "";
    public int RecordLimit { get; set; } = 100;

    public IReadOnlyList<APInvoiceBatch> APInvoiceBatches { get; set; } = Array.Empty<APInvoiceBatch>();
}

