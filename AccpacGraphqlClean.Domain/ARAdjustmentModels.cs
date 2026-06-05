namespace AccpacGraphqlClean.Domain;

public sealed class ARAdjustment
{
    public string? Compid { get; set; }

    public string? BatchNumber001 { get; set; }
    public DateTime? BatchDate002 { get; set; }
    public string? Description003 { get; set; }
    public string? BatchType000 { get; set; }
    public string? BatchType006 { get; set; }
    public string? BatchStatus007 { get; set; }
    public string? BankCode008 { get; set; }
    public string? DefaultBankCurrency010 { get; set; }
    public DateTime? BankRateDate011 { get; set; }
    public string? BankRateType013 { get; set; }
    public string? BankExchangeRate014 { get; set; }
    public string? DepositNumber015 { get; set; }
    public string? BankRateOverridden026 { get; set; }
    public string? SourceApplication027 { get; set; }
    public string? BatchType032 { get; set; }
    public string? BatchNumber033 { get; set; }

    public string? EntryNumber034 { get; set; }
    public string? CheckReceiptNo035 { get; set; }
    public string? CustomerNumber036 { get; set; }
    public DateTime? ReceiptDateAdjustmentDate037 { get; set; }
    public string? EntryDescription038 { get; set; }
    public string? EntryReference039 { get; set; }
    public string? BankReceiptAmount040 { get; set; }
    public string? CustExchangeRate042 { get; set; }
    public string? CustRateOverridden043 { get; set; }
    public string? PaymentCode047 { get; set; }
    public string? BankRateType049 { get; set; }
    public string? BankExchangeRate050 { get; set; }
    public string? BankRateOverridden051 { get; set; }
    public string? ReceiptTransType052 { get; set; }
    public string? DocumentType053 { get; set; }
    public string? MatchingDocumentNumber054 { get; set; }
    public string? Payer058 { get; set; }
    public DateTime? CustRateDate059 { get; set; }
    public string? CustRateType060 { get; set; }
    public DateTime? BankRateDate062 { get; set; }
    public string? DocumentNumber067 { get; set; }
    public string? SourceApplication082 { get; set; }
    public string? BankCode083 { get; set; }
    public string? BankCurrencyCode084 { get; set; }
    public string? EnteredBy147 { get; set; }
    public DateTime? PostingDate148 { get; set; }
    public string? AccountSet149 { get; set; }

    public IReadOnlyList<ARAdjustmentLine> ARAdjustmentLines { get; set; } = Array.Empty<ARAdjustmentLine>();
}

public sealed class ARAdjustmentLine
{
    public string? BatchNumber001 { get; set; }
    public string? EntryNumber002 { get; set; }
    public string? LineNumber003 { get; set; }
    public string? CustomerNumber004 { get; set; }
    public string? DocumentNumber005 { get; set; }
    public string? PaymentNumber006 { get; set; }
    public string? TransactionType007 { get; set; }
    public string? PaymentResolution008 { get; set; }
    public string? CustReceiptAmount009 { get; set; }
    public string? CustDiscountAmountTaken010 { get; set; }
    public string? Description014 { get; set; }
    public string? Reference015 { get; set; }
    public string? PPMatchingDocNo017 { get; set; }
    public string? PPMatchingDocType018 { get; set; }
    public string? DocumentType027 { get; set; }
    public string? JobApplyMethod031 { get; set; }
    public string? RetainageAmount034 { get; set; }
    public DateTime? RetainageDueDate035 { get; set; }
    public string? RetainageTermsCode036 { get; set; }
    public string? RetainageExchangeRate037 { get; set; }
}

public sealed class ARAdjustmentBatch
{
    public string? Compid { get; set; }
    public string? BatchNumber { get; set; }
    public string? BatchDescription { get; set; }
    public DateTime? BatchDate { get; set; }
    public string? SourceApplication { get; set; }
    public IReadOnlyList<ARAdjustment> BatchEntries { get; set; } = Array.Empty<ARAdjustment>();
}

public sealed class SyncARAdjustments
{
    public string CallMethod { get; set; } = "SYNC";
    public string Timestamp { get; set; } = "";
    public int RecordLimit { get; set; } = 100;
    public IReadOnlyList<ARAdjustmentBatch> ARAdjustmentBatches { get; set; } = Array.Empty<ARAdjustmentBatch>();
}
