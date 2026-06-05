using System.Security.Claims;
using System.Text.Json;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class AccpacOperationExecutor : IAccpacOperationExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IApVendorService _apVendorService;
    private readonly IApVendorGroupService _apVendorGroupService;
    private readonly IApPaymentCodeService _apPaymentCodeService;
    private readonly IApPaymentTermsService _apPaymentTermsService;
    private readonly IApRemitToLocationsService _apRemitToLocationsService;
    private readonly IApRecurringPayablesService _apRecurringPayablesService;
    private readonly IApInvoiceService _apInvoiceService;
    private readonly IApPaymentService _apPaymentService;
    private readonly IApAdjustmentService _apAdjustmentService;
    private readonly IArInvoiceService _arInvoiceService;
    private readonly IArAdjustmentService _arAdjustmentService;
    private readonly IArReceiptService _arReceiptService;
    private readonly IArRefundService _arRefundService;
    private readonly IArBillingCyclesService _arBillingCyclesService;
    private readonly IArSalesPersonsService _arSalesPersonsService;
    private readonly IArCustomerService _arCustomerService;
    private readonly IArTermsCodesService _arTermsCodesService;
    private readonly IArShipToLocationService _arShipToLocationService;
    private readonly IArCustomerGroupService _arCustomerGroupService;
    private readonly IArItemService _arItemService;

    public AccpacOperationExecutor(
        IApVendorService apVendorService,
        IApVendorGroupService apVendorGroupService,
        IApPaymentCodeService apPaymentCodeService,
        IApPaymentTermsService apPaymentTermsService,
        IApRemitToLocationsService apRemitToLocationsService,
        IApRecurringPayablesService apRecurringPayablesService,
        IApInvoiceService apInvoiceService,
        IApPaymentService apPaymentService,
        IApAdjustmentService apAdjustmentService,
        IArInvoiceService arInvoiceService,
        IArAdjustmentService arAdjustmentService,
        IArReceiptService arReceiptService,
        IArRefundService arRefundService,
        IArBillingCyclesService arBillingCyclesService,
        IArSalesPersonsService arSalesPersonsService,
        IArCustomerService arCustomerService,
        IArTermsCodesService arTermsCodesService,
        IArShipToLocationService arShipToLocationService,
        IArCustomerGroupService arCustomerGroupService,
        IArItemService arItemService)
    {
        _apVendorService = apVendorService;
        _apVendorGroupService = apVendorGroupService;
        _apPaymentCodeService = apPaymentCodeService;
        _apPaymentTermsService = apPaymentTermsService;
        _apRemitToLocationsService = apRemitToLocationsService;
        _apRecurringPayablesService = apRecurringPayablesService;
        _apInvoiceService = apInvoiceService;
        _apPaymentService = apPaymentService;
        _apAdjustmentService = apAdjustmentService;
        _arInvoiceService = arInvoiceService;
        _arAdjustmentService = arAdjustmentService;
        _arReceiptService = arReceiptService;
        _arRefundService = arRefundService;
        _arBillingCyclesService = arBillingCyclesService;
        _arSalesPersonsService = arSalesPersonsService;
        _arCustomerService = arCustomerService;
        _arTermsCodesService = arTermsCodesService;
        _arShipToLocationService = arShipToLocationService;
        _arCustomerGroupService = arCustomerGroupService;
        _arItemService = arItemService;
    }

    public async Task<AccpacOperationResult> ExecuteAsync(
        string restRoute,
        object? input,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (restRoute)
            {
                case "api/APVendor/CreateAPVendor":
                {
                    var vendor = DeserializeOrThrow<APVendor>(input);
                    var (response, saved) = await _apVendorService.CreateOrUpdateAsync(vendor, user, cancellationToken);
                    return new AccpacOperationResult(response, new { vendor = saved });
                }
                case "api/APVendor/ReadAPVendor":
                {
                    var vendorNumber = ExtractKey(input, "VendorNumber000");
                    var (response, vendor) = await _apVendorService.ReadAsync(vendorNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { vendor });
                }
                case "api/APVendorGroup/CreateAPVendorGroups":
                case "api/APVendorGroup/UpdateAPVendorGroups":
                {
                    var vendorGroup = DeserializeOrThrow<APVendorGroup>(input);
                    var (response, saved) = await _apVendorGroupService.CreateOrUpdateAsync(vendorGroup, user, cancellationToken);
                    return new AccpacOperationResult(response, new { vendorGroup = saved });
                }
                case "api/APPaymentCode/CreateAPPaymentCodes":
                case "api/APPaymentCode/UpdateAPPaymentCodes":
                {
                    var paymentCode = DeserializeOrThrow<APPaymentCodes>(input);
                    var (response, saved) = await _apPaymentCodeService.CreateOrUpdateAsync(paymentCode, user, cancellationToken);
                    return new AccpacOperationResult(response, new { paymentCode = saved });
                }
                case "api/APPaymentTerms/CreateAPPaymentTerms":
                case "api/APPaymentTerms/UpdateAPPaymentTerms":
                {
                    var paymentTerms = DeserializeOrThrow<APPaymentTerms>(input);
                    var (response, saved) = await _apPaymentTermsService.CreateOrUpdateAsync(paymentTerms, user, cancellationToken);
                    return new AccpacOperationResult(response, new { paymentTerms = saved });
                }
                case "api/APRemitToLocation/CreateAPRemitToLocations":
                case "api/APRemitToLocation/UpdateAPRemitToLocations":
                {
                    var remit = DeserializeOrThrow<APRemitToLocations>(input);
                    var (response, saved) = await _apRemitToLocationsService.CreateOrUpdateAsync(remit, user, cancellationToken);
                    return new AccpacOperationResult(response, new { remitToLocations = saved });
                }
                case "api/APRecurringPayable/CreateAPRecurringPayables":
                case "api/APRecurringPayable/UpdateAPRecurringPayables":
                {
                    var recurring = DeserializeOrThrow<APRecurringPayables>(input);
                    var (response, saved) = await _apRecurringPayablesService.CreateOrUpdateAsync(recurring, user, cancellationToken);
                    return new AccpacOperationResult(response, new { recurringPayables = saved });
                }
                case "api/APInvoice/CreateInvoice":
                {
                    var invoice = DeserializeOrThrow<APInvoices>(input);
                    var (response, saved) = await _apInvoiceService.CreateInvoiceAsync(invoice, user, cancellationToken);
                    return new AccpacOperationResult(response, new { invoices = saved });
                }
                case "api/APInvoice/CreateInvoiceBatch":
                {
                    var batch = DeserializeOrThrow<APInvoiceBatch>(input);
                    var (response, saved) = await _apInvoiceService.CreateInvoiceBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { invoices = saved });
                }
                case "api/APInvoice/ReadInvoice":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), new { restRoute });
                    }

                    var (response, invoice) = await _apInvoiceService.ReadInvoiceAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { invoices = invoice });
                }
                case "api/APInvoice/ReadInvoiceBatchStatus":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _apInvoiceService.ReadInvoiceBatchStatusAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { InvoiceBatch = batch });
                }
                case "api/APInvoice/ReadInvoiceBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _apInvoiceService.ReadInvoiceBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { InvoiceBatch = batch });
                }
                case "api/APInvoice/SyncInvoices":
                {
                    var req = DeserializeOrThrow<SyncAPInvoices>(input);
                    var (response, sync) = await _apInvoiceService.SyncInvoicesAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/APPayment/CreatePayment":
                {
                    var payment = DeserializeOrThrow<APPayment>(input);
                    var (response, saved) = await _apPaymentService.CreatePaymentAsync(payment, user, cancellationToken);
                    return new AccpacOperationResult(response, new { payment = saved });
                }
                case "api/APPayment/CreatePaymentBatch":
                {
                    var paymentBatch = DeserializeOrThrow<APPaymentBatch>(input);
                    var (response, saved) = await _apPaymentService.CreatePaymentBatchAsync(paymentBatch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { paymentBatch = saved });
                }
                case "api/APPayment/ReadPayment":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), new { restRoute });
                    }

                    var (response, payment) = await _apPaymentService.ReadPaymentAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { payment });
                }
                case "api/APPayment/ReadPaymentBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, paymentBatch) = await _apPaymentService.ReadPaymentBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { paymentBatch });
                }
                case "api/APPayment/SyncAPPayments":
                {
                    var req = DeserializeOrThrow<SyncAPPayments>(input);
                    var (response, sync) = await _apPaymentService.SyncPaymentsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/APAdjustment/CreateAdjustment":
                {
                    var adjustment = DeserializeOrThrow<APAdjustments>(input);
                    var (response, saved) = await _apAdjustmentService.CreateAdjustmentAsync(adjustment, user, cancellationToken);
                    return new AccpacOperationResult(response, new { adjustment = saved });
                }
                case "api/APAdjustment/CreateAdjustmentBatch":
                {
                    var adjustmentBatch = DeserializeOrThrow<APAdjustmentBatch>(input);
                    var (response, saved) = await _apAdjustmentService.CreateAdjustmentBatchAsync(adjustmentBatch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { adjustmentBatch = saved });
                }
                case "api/APAdjustment/ReadAdjustment":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), new { restRoute });
                    }

                    var (response, adjustment) = await _apAdjustmentService.ReadAdjustmentAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { adjustment });
                }
                case "api/APAdjustment/ReadAdjustmentBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, adjustmentBatch) = await _apAdjustmentService.ReadAdjustmentBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { adjustmentBatch });
                }
                case "api/APAdjustment/SyncAdjustment":
                {
                    var req = DeserializeOrThrow<SyncAPAdjustments>(input);
                    var (response, sync) = await _apAdjustmentService.SyncAdjustmentsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/ARInvoice/CreateARInvoice":
                case "api/ARInvoice/UpdateARInvoice":
                {
                    var invoice = DeserializeOrThrow<ARInvoice>(input);
                    var (response, saved) = await _arInvoiceService.CreateOrUpdateAsync(invoice, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arInvoice = saved });
                }
                case "api/ARInvoice/CreateARInvoiceBatch":
                {
                    var batch = DeserializeOrThrow<ARInvoiceBatch>(input);
                    var (response, saved) = await _arInvoiceService.CreateInvoiceBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arInvoiceBatch = saved });
                }
                case "api/ARInvoice/ReadARInvoice":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), new { restRoute });
                    }

                    var (response, invoice) = await _arInvoiceService.ReadInvoiceAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arInvoice = invoice });
                }
                case "api/ARInvoice/ReadARInvoiceBatchStatus":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arInvoiceService.ReadInvoiceBatchStatusAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arInvoiceBatch = batch });
                }
                case "api/ARInvoice/ReadARInvoiceBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arInvoiceService.ReadInvoiceBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arInvoiceBatch = batch });
                }
                case "api/ARInvoice/SyncARInvoices":
                {
                    var req = DeserializeOrThrow<SyncARInvoices>(input);
                    var (response, sync) = await _arInvoiceService.SyncInvoicesAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/ARAdjustment/CreateARAdjustment":
                case "api/ARAdjustment/UpdateAdjustment":
                {
                    var adjustment = DeserializeOrThrow<ARAdjustment>(input);
                    var (response, saved) = await _arAdjustmentService.CreateOrUpdateAsync(adjustment, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arAdjustment = saved });
                }
                case "api/ARAdjustment/CreateARAdjustmentBatch":
                {
                    var batch = DeserializeOrThrow<ARAdjustmentBatch>(input);
                    var (response, saved) = await _arAdjustmentService.CreateAdjustmentBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arAdjustmentBatch = saved });
                }
                case "api/ARAdjustment/ReadARAdjustment":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), new { restRoute });
                    }

                    var (response, adj) = await _arAdjustmentService.ReadAdjustmentAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arAdjustment = adj });
                }
                case "api/ARAdjustment/ReadARAdjustmentBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arAdjustmentService.ReadAdjustmentBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arAdjustmentBatch = batch });
                }
                case "api/ARAdjustment/SyncARAdjustments":
                {
                    var req = DeserializeOrThrow<SyncARAdjustments>(input);
                    var (response, sync) = await _arAdjustmentService.SyncAdjustmentsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/ARReceipt/CreateARReceipt":
                case "api/ARReceipt/CreateARReceiptAppendPrepayment":
                case "api/ARReceipt/UpdateReceipt":
                {
                    var receipt = DeserializeOrThrow<ARReceipt>(input);
                    var (response, saved) = await _arReceiptService.CreateOrUpdateAsync(receipt, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arReceipt = saved });
                }
                case "api/ARReceipt/CreateARReceiptBatch":
                {
                    var batch = DeserializeOrThrow<ARReceiptBatch>(input);
                    var (response, saved) = await _arReceiptService.CreateReceiptBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arReceiptBatch = saved });
                }
                case "api/ARReceipt/ReadARReceipt":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), new { restRoute });
                    }

                    var (response, receipt) = await _arReceiptService.ReadReceiptAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arReceipt = receipt });
                }
                case "api/ARReceipt/ReadARReceiptBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arReceiptService.ReadReceiptBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arReceiptBatch = batch });
                }
                case "api/ARReceipt/SyncARReceipts":
                {
                    var req = DeserializeOrThrow<SyncARReceipts>(input);
                    var (response, sync) = await _arReceiptService.SyncReceiptsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/ARRefund/CreateARRefund":
                case "api/ARRefund/UpdateARRefund":
                {
                    var refund = DeserializeOrThrow<ARRefund>(input);
                    var (response, saved) = await _arRefundService.CreateOrUpdateAsync(refund, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arRefund = saved });
                }
                case "api/ARRefund/CreateARRefundBatch":
                {
                    var batch = DeserializeOrThrow<ARRefundBatch>(input);
                    var (response, saved) = await _arRefundService.CreateRefundBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arRefundBatch = saved });
                }
                case "api/ARRefund/ReadARRefund":
                {
                    var documentNumber = ExtractKey(input, "DocumentNumber023");
                    var (response, refund) = await _arRefundService.ReadRefundAsync(documentNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arRefund = refund });
                }
                case "api/ARRefund/ReadARRefundBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arRefundService.ReadRefundBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arRefundBatch = batch });
                }
                case "api/ARRefund/SyncARRefunds":
                {
                    var req = DeserializeOrThrow<SyncARRefunds>(input);
                    var (response, sync) = await _arRefundService.SyncRefundsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/ARBillingCycle/CreateARBillingCycles":
                case "api/ARBillingCycle/UpdateARBillingCycles":
                {
                    var billingCycles = DeserializeOrThrow<ARBillingCycles>(input);
                    var (response, saved) = await _arBillingCyclesService.CreateOrUpdateAsync(billingCycles, user, cancellationToken);
                    return new AccpacOperationResult(response, new { billingCycles = saved });
                }
                case "api/ARBillingCycle/ReadARBillingCycles":
                {
                    var billingCycle = ExtractKey(input, "BillingCycle000");
                    var (response, billingCycles) = await _arBillingCyclesService.ReadAsync(billingCycle, user, cancellationToken);
                    return new AccpacOperationResult(response, new { billingCycles });
                }
                case "api/ARSalesperson/CreateARSalesPersons":
                case "api/ARSalesperson/UpdateARSalesPersons":
                {
                    var salesPerson = DeserializeOrThrow<ARSalesPersons>(input);
                    var (response, saved) = await _arSalesPersonsService.CreateOrUpdateAsync(salesPerson, user, cancellationToken);
                    return new AccpacOperationResult(response, new { salesPerson = saved });
                }
                case "api/ARCustomer/CreateARCustomer":
                case "api/ARCustomer/UpdateARCustomer":
                {
                    var customer = DeserializeOrThrow<ARCustomers>(input);
                    var (response, saved) = await _arCustomerService.CreateOrUpdateAsync(customer, user, cancellationToken);
                    return new AccpacOperationResult(response, new { customer = saved });
                }
                case "api/ARCustomer/ReadARCustomer":
                {
                    var customerNumber = ExtractKey(input, "CustomerNumber000");
                    var (response, customer) = await _arCustomerService.ReadAsync(customerNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { customer });
                }
                case "api/ARCustomer/ReadARCustomerBalance":
                {
                    var customerNumber = ExtractKey(input, "CustomerNumber000");
                    var (response, customerBalance) = await _arCustomerService.ReadBalanceAsync(customerNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, new { customerBalance });
                }
                case "api/ARTermsCode/CreateARTermsCodes":
                case "api/ARTermsCode/UpdateARTermsCodes":
                {
                    var termsCodes = DeserializeOrThrow<ARTermsCodes>(input);
                    var (response, saved) = await _arTermsCodesService.CreateOrUpdateAsync(termsCodes, user, cancellationToken);
                    return new AccpacOperationResult(response, new { termsCodes = saved });
                }
                case "api/ARShipToLocation/CreateARShipToLocation":
                case "api/ARShipToLocation/UpdateARShipToLocation":
                {
                    var shipTo = DeserializeOrThrow<ARShipToLocations>(input);
                    var (response, saved) = await _arShipToLocationService.CreateOrUpdateAsync(shipTo, user, cancellationToken);
                    return new AccpacOperationResult(response, new { shipToLocation = saved });
                }
                case "api/ARShipToLocation/ReadARShipToLocation":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var customerNumber = doc.RootElement.TryGetProperty("CustomerNumber", out var cn) ? cn.GetString() : null;
                    var shipToLocation = doc.RootElement.TryGetProperty("ShipToLocation", out var st) ? st.GetString() : null;
                    if (string.IsNullOrWhiteSpace(customerNumber) || string.IsNullOrWhiteSpace(shipToLocation))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "CustomerNumber and ShipToLocation are required."), new { restRoute });
                    }

                    var (response, shipTo) = await _arShipToLocationService.ReadAsync(customerNumber, shipToLocation, user, cancellationToken);
                    return new AccpacOperationResult(response, new { shipToLocation = shipTo });
                }
                case "api/ARShipToLocation/ReadCustomerShipToLocations":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), new { restRoute });
                    }

                    using var doc = JsonDocument.Parse(json);
                    var customerNumber = doc.RootElement.TryGetProperty("CustomerNumber", out var cn) ? cn.GetString() : null;
                    var shipToId = doc.RootElement.TryGetProperty("ShipToID", out var st) ? st.GetString() : null;
                    if (string.IsNullOrWhiteSpace(customerNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "CustomerNumber is required."), new { restRoute });
                    }

                    var (response, shipTos) = await _arShipToLocationService.ReadCustomerShipToLocationsAsync(customerNumber, shipToId, user, cancellationToken);
                    return new AccpacOperationResult(response, new { shipToLocations = shipTos });
                }
                case "api/ARShipToLocation/SyncARShipToLocation":
                {
                    var req = DeserializeOrThrow<SyncARShipToLocations>(input);
                    var (response, sync) = await _arShipToLocationService.SyncAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, new { sync });
                }
                case "api/ARCustomerGroup/CreateARCustomerGroupss":
                case "api/ARCustomerGroup/UpdateARCustomerGroups":
                {
                    var customerGroup = DeserializeOrThrow<ARCustomerGroups>(input);
                    var (response, saved) = await _arCustomerGroupService.CreateOrUpdateAsync(customerGroup, user, cancellationToken);
                    return new AccpacOperationResult(response, new { customerGroup = saved });
                }
                case "api/ARItem/CreateARItems":
                case "api/ARItem/UpdateARItems":
                {
                    var item = DeserializeOrThrow<ARItems>(input);
                    var (response, saved) = await _arItemService.CreateOrUpdateAsync(item, user, cancellationToken);
                    return new AccpacOperationResult(response, new { arItems = saved });
                }
                default:
                    return new AccpacOperationResult(
                        ProcessOut.Fail("9998", $"Operation not implemented yet: {restRoute}"),
                        new { restRoute, input });
            }
        }
        catch (Exception ex)
        {
            return new AccpacOperationResult(
                ProcessOut.Fail("9999", ex.Message),
                new { restRoute });
        }
    }

    private static T DeserializeOrThrow<T>(object? input)
    {
        var json = input as string;
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("inputJson is required.");
        }

        var obj = JsonSerializer.Deserialize<T>(json, JsonOptions);
        if (obj is null)
        {
            throw new InvalidOperationException("Unable to deserialize inputJson.");
        }

        return obj;
    }

    private static string ExtractKey(object? input, string keyPropertyName)
    {
        var json = input as string;
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("inputJson is required.");
        }

        if (json.TrimStart().StartsWith("{", StringComparison.Ordinal))
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(keyPropertyName, out var prop))
            {
                var value = prop.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return json.Trim().Trim('"');
    }
}
