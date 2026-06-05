namespace AccpacGraphqlClean.Domain;

public sealed class ARRefund
{
    public string? Compid { get; set; }

    public string? BatchNumber000 { get; set; }
    public DateTime? BatchDate001 { get; set; }
    public string? BatchDescription002 { get; set; }
    public string? BatchType003 { get; set; }
    public string? BatchStatus004 { get; set; }
    public string? BatchPrintedFlag012 { get; set; }
    public string? SourceApplication013 { get; set; }
    public string? NumberofPrintedChecks015 { get; set; }

    public string? BatchNumber016 { get; set; }
    public string? EntryNumber017 { get; set; }
    public string? DocumentDescription018 { get; set; }
    public DateTime? DocumentDate019 { get; set; }
    public string? CustomerNumber022 { get; set; }
    public string? DocumentNumber023 { get; set; }
    public string? RateType027 { get; set; }
    public DateTime? RateDate028 { get; set; }
    public string? ExchangeRate029 { get; set; }
    public string? RateOverrideFlag031 { get; set; }
    public string? JobApplyMethod035 { get; set; }
    public string? SourceApplication037 { get; set; }
    public string? CashBankAccount040 { get; set; }
    public string? CashGLAccount041 { get; set; }
    public string? CashPaymentCurrency042 { get; set; }
    public string? CashRateType043 { get; set; }
    public DateTime? CashRateDate044 { get; set; }
    public string? CashExchangeRate045 { get; set; }
    public string? CashRateOverrideFlag047 { get; set; }
    public string? CheckBankAccount051 { get; set; }
    public string? CheckPrintingRequired052 { get; set; }
    public string? CheckHasBeenPrinted053 { get; set; }
    public string? CheckNumber054 { get; set; }
    public string? CheckPaymentCurrency056 { get; set; }
    public string? CheckRateType057 { get; set; }
    public DateTime? CheckRateDate058 { get; set; }
    public string? CheckExchangeRate059 { get; set; }
    public string? CheckRateOverrideFlag061 { get; set; }
    public string? RemitToName065 { get; set; }
    public string? AddressLine1066 { get; set; }
    public string? AddressLine2067 { get; set; }
    public string? AddressLine3068 { get; set; }
    public string? AddressLine4069 { get; set; }
    public string? City070 { get; set; }
    public string? StateProv071 { get; set; }
    public string? ZipPostalCode072 { get; set; }
    public string? Country073 { get; set; }
    public string? CheckLanguage074 { get; set; }
    public string? EnteredBy077 { get; set; }
    public DateTime? PostingDate078 { get; set; }
    public string? PreviousCCTransactionNumber081 { get; set; }
    public string? PreviousCCProcessStatus082 { get; set; }
    public string? CurrentCCTransactionNumber083 { get; set; }
    public string? CurrentCCProcessStatus084 { get; set; }
    public string? ProcessingCode085 { get; set; }

    public IReadOnlyList<ARRefundItem> ARRefundItems { get; set; } = Array.Empty<ARRefundItem>();
}

public sealed class ARRefundItem
{
    public string? EntryNumber001 { get; set; }
    public string? LineNumber002 { get; set; }
    public string? DocumentNumber003 { get; set; }
    public string? PaymentNumber004 { get; set; }
    public string? PaymentType005 { get; set; }
    public string? CCBankAccount006 { get; set; }
    public string? CCPaymentCurrency008 { get; set; }
    public string? CCRateType009 { get; set; }
    public DateTime? CCRateDate010 { get; set; }
    public string? CCExchangeRate011 { get; set; }
    public string? CCRateOverrideFlag013 { get; set; }
    public string? AmountPayment014 { get; set; }
    public string? JobApplyMethod018 { get; set; }
    public string? ReceiptDocumentNumber029 { get; set; }
}

public sealed class ARRefundBatch
{
    public string? Compid { get; set; }
    public string? BatchNumber { get; set; }
    public string? BatchDescription { get; set; }
    public DateTime? BatchDate { get; set; }
    public string? SourceApplication { get; set; }
    public IReadOnlyList<ARRefund> BatchEntries { get; set; } = Array.Empty<ARRefund>();
}

public sealed class SyncARRefunds
{
    public string CallMethod { get; set; } = "SYNC";
    public string Timestamp { get; set; } = "";
    public int RecordLimit { get; set; } = 100;
    public IReadOnlyList<ARRefundBatch> ARRefundBatches { get; set; } = Array.Empty<ARRefundBatch>();
}
