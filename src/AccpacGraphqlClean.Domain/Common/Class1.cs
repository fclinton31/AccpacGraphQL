using System.Text.Json;

namespace AccpacGraphqlClean.Domain;

public sealed record ProcessOut(
    string ReturnCode,
    string ReturnMessage,
    string? DocumentNumber = null,
    string? BatchNumber = null,
    string? ReferenceNumber = null,
    string? ErrorCode = null
)
{
    public static ProcessOut Ok(string message = "OK", string? documentNumber = null) =>
        new("0000", message, DocumentNumber: documentNumber, ErrorCode: "0000");
    public static ProcessOut Fail(string code, string message) => new(code, message, ErrorCode: code);
}

public sealed record AccpacOperationResult(ProcessOut Response, object? Data);

public sealed record AccpacOperationResultOf<TData>(ProcessOut Response, TData Data);

public sealed record SageRecord(JsonElement Raw);

public sealed class AccpacData
{
    public JsonElement Raw { get; set; }

    public APVendor? Vendor { get; set; }
    public APVendorGroup? VendorGroup { get; set; }
    public APPaymentCodes? PaymentCode { get; set; }
    public APPaymentTerms? PaymentTerms { get; set; }
    public APRemitToLocations? RemitToLocations { get; set; }
    public APRecurringPayables? RecurringPayables { get; set; }

    public APInvoices? Invoices { get; set; }
    public APInvoiceBatch? InvoiceBatch { get; set; }
    public APPayment? Payment { get; set; }
    public APPaymentBatch? PaymentBatch { get; set; }
    public APAdjustments? Adjustment { get; set; }
    public APAdjustmentBatch? AdjustmentBatch { get; set; }

    public ARCustomers? Customer { get; set; }
    public ARCustomerBalance? CustomerBalance { get; set; }
    public ARCustomerGroups? CustomerGroup { get; set; }
    public ARTermsCodes? TermsCodes { get; set; }
    public ARShipToLocations? ShipToLocation { get; set; }
    public List<ARShipToLocations>? ShipToLocations { get; set; }
    public ARBillingCycles? BillingCycles { get; set; }
    public ARSalesPersons? SalesPerson { get; set; }
    public ARItems? ArItems { get; set; }

    public ARInvoice? ArInvoice { get; set; }
    public ARInvoiceBatch? ArInvoiceBatch { get; set; }
    public ARAdjustment? ArAdjustment { get; set; }
    public ARAdjustmentBatch? ArAdjustmentBatch { get; set; }
    public ARReceipt? ArReceipt { get; set; }
    public ARReceiptBatch? ArReceiptBatch { get; set; }
    public ARRefund? ArRefund { get; set; }
    public ARRefundBatch? ArRefundBatch { get; set; }

    public ARAgedAnalysis? AgedAnalysis { get; set; }
    public ARStatementRun? StatementRun { get; set; }

    public SageRecord? Account { get; set; }
    public SageRecord? JournalEntry { get; set; }
    public SageRecord? RecurringEntry { get; set; }
    public SageRecord? Category { get; set; }
    public SageRecord? Item { get; set; }
    public SageRecord? Pricing { get; set; }
    public SageRecord? Location { get; set; }
    public SageRecord? LocationDetails { get; set; }
    public SageRecord? Receipt { get; set; }
    public SageRecord? Shipment { get; set; }
    public SageRecord? Transfer { get; set; }
    public SageRecord? Assembly { get; set; }
    public SageRecord? InternalUsage { get; set; }
    public SageRecord? PurchaseOrder { get; set; }
    public SageRecord? Requisition { get; set; }
    public SageRecord? Return { get; set; }
    public SageRecord? SalesOrder { get; set; }
    public SageRecord? Invoice { get; set; }
    public SageRecord? CreditDebitNote { get; set; }
    public SageRecord? DebitCreditNote { get; set; }
    public SageRecord? Status { get; set; }
    public SageRecord? Sync { get; set; }
    public SageRecord? Records { get; set; }
    public SageRecord? Timestamp { get; set; }
    public SageRecord? Route { get; set; }
    public SageRecord? RestRoute { get; set; }
}

public sealed record AuthToken(string AccessToken, string TokenType, int ExpiresInSeconds);

public sealed record CompanyConnectionDetails(string CompanyId, string UserName, string Password);
