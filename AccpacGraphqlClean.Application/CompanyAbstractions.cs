using System.Security.Claims;
using AccpacGraphqlClean.Domain;

namespace AccpacGraphqlClean.Application;

public interface ICompanyConnectionDetailsProvider
{
    Task<CompanyConnectionDetails> GetAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
}

public interface IApVendorService
{
    Task<(ProcessOut Response, APVendor Vendor)> CreateOrUpdateAsync(
        APVendor vendor,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APVendor Vendor)> ReadAsync(
        string vendorNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApVendorGroupService
{
    Task<(ProcessOut Response, APVendorGroup VendorGroup)> CreateOrUpdateAsync(
        APVendorGroup vendorGroup,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApPaymentCodeService
{
    Task<(ProcessOut Response, APPaymentCodes PaymentCode)> CreateOrUpdateAsync(
        APPaymentCodes paymentCode,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APPaymentCodes PaymentCode)> ReadAsync(
        string paymentCode,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApPaymentTermsService
{
    Task<(ProcessOut Response, APPaymentTerms PaymentTerms)> CreateOrUpdateAsync(
        APPaymentTerms paymentTerms,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApRemitToLocationsService
{
    Task<(ProcessOut Response, APRemitToLocations RemitToLocations)> CreateOrUpdateAsync(
        APRemitToLocations remitToLocations,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApRecurringPayablesService
{
    Task<(ProcessOut Response, APRecurringPayables RecurringPayables)> CreateOrUpdateAsync(
        APRecurringPayables recurringPayables,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApInvoiceService
{
    Task<(ProcessOut Response, APInvoices Invoice)> CreateInvoiceAsync(
        APInvoices invoice,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APInvoiceBatch Batch)> CreateInvoiceBatchAsync(
        APInvoiceBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APInvoices Invoice)> ReadInvoiceAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APInvoiceBatch Batch)> ReadInvoiceBatchStatusAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APInvoiceBatch Batch)> ReadInvoiceBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncAPInvoices Sync)> SyncInvoicesAsync(
        SyncAPInvoices request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApPaymentService
{
    Task<(ProcessOut Response, APPayment Payment)> CreatePaymentAsync(
        APPayment payment,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APPaymentBatch Batch)> CreatePaymentBatchAsync(
        APPaymentBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APPayment Payment)> ReadPaymentAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APPaymentBatch Batch)> ReadPaymentBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncAPPayments Sync)> SyncPaymentsAsync(
        SyncAPPayments request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IApAdjustmentService
{
    Task<(ProcessOut Response, APAdjustments Adjustment)> CreateAdjustmentAsync(
        APAdjustments adjustment,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APAdjustmentBatch Batch)> CreateAdjustmentBatchAsync(
        APAdjustmentBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APAdjustments Adjustment)> ReadAdjustmentAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, APAdjustmentBatch Batch)> ReadAdjustmentBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncAPAdjustments Sync)> SyncAdjustmentsAsync(
        SyncAPAdjustments request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IArInvoiceService
{
    Task<(ProcessOut Response, ARInvoice Invoice)> CreateOrUpdateAsync(
        ARInvoice invoice,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARInvoiceBatch Batch)> CreateInvoiceBatchAsync(
        ARInvoiceBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARInvoice Invoice)> ReadInvoiceAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARInvoiceBatch Batch)> ReadInvoiceBatchStatusAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARInvoiceBatch Batch)> ReadInvoiceBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncARInvoices Sync)> SyncInvoicesAsync(
        SyncARInvoices request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IArAdjustmentService
{
    Task<(ProcessOut Response, ARAdjustment Adjustment)> CreateOrUpdateAsync(
        ARAdjustment adjustment,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARAdjustmentBatch Batch)> CreateAdjustmentBatchAsync(
        ARAdjustmentBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARAdjustment Adjustment)> ReadAdjustmentAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARAdjustmentBatch Batch)> ReadAdjustmentBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncARAdjustments Sync)> SyncAdjustmentsAsync(
        SyncARAdjustments request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IArReceiptService
{
    Task<(ProcessOut Response, ARReceipt Receipt)> CreateOrUpdateAsync(
        ARReceipt receipt,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARReceiptBatch Batch)> CreateReceiptBatchAsync(
        ARReceiptBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARReceipt Receipt)> ReadReceiptAsync(
        string batchNumber,
        string entryNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARReceiptBatch Batch)> ReadReceiptBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncARReceipts Sync)> SyncReceiptsAsync(
        SyncARReceipts request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}

public interface IArRefundService
{
    Task<(ProcessOut Response, ARRefund Refund)> CreateOrUpdateAsync(
        ARRefund refund,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARRefundBatch Batch)> CreateRefundBatchAsync(
        ARRefundBatch batch,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARRefund Refund)> ReadRefundAsync(
        string documentNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, ARRefundBatch Batch)> ReadRefundBatchAsync(
        string batchNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);

    Task<(ProcessOut Response, SyncARRefunds Sync)> SyncRefundsAsync(
        SyncARRefunds request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}
