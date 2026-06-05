using System.Security.Claims;
using System.Text.Json;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class AccpacOperationExecutor : IAccpacOperationExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private static JsonElement Data(params (string Key, object? Value)[] items)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in items)
        {
            dict[key] = value;
        }

        return JsonSerializer.SerializeToElement(dict, JsonOptions);
    }

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
    private readonly IArDocumentsService _arDocumentsService;
    private readonly IArStatementRunService _arStatementRunService;
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

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
        IArItemService arItemService,
        IArDocumentsService arDocumentsService,
        IArStatementRunService arStatementRunService,
        IConfiguration configuration,
        ICompanyConnectionDetailsProvider companyDetails)
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
        _arDocumentsService = arDocumentsService;
        _arStatementRunService = arStatementRunService;
        _configuration = configuration;
        _companyDetails = companyDetails;
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
                    return new AccpacOperationResult(response, Data(("vendor", saved)));
                }
                case "api/APVendor/ReadAPVendor":
                {
                    var vendorNumber = ExtractKey(input, "VendorNumber000");
                    var (response, vendor) = await _apVendorService.ReadAsync(vendorNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("vendor", vendor)));
                }
                case "api/APVendorGroup/CreateAPVendorGroups":
                case "api/APVendorGroup/UpdateAPVendorGroups":
                {
                    var vendorGroup = DeserializeOrThrow<APVendorGroup>(input);
                    var (response, saved) = await _apVendorGroupService.CreateOrUpdateAsync(vendorGroup, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("vendorGroup", saved)));
                }
                case "api/APPaymentCode/CreateAPPaymentCodes":
                case "api/APPaymentCode/UpdateAPPaymentCodes":
                {
                    var paymentCode = DeserializeOrThrow<APPaymentCodes>(input);
                    var (response, saved) = await _apPaymentCodeService.CreateOrUpdateAsync(paymentCode, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("paymentCode", saved)));
                }
                case "api/APPaymentTerms/CreateAPPaymentTerms":
                case "api/APPaymentTerms/UpdateAPPaymentTerms":
                {
                    var paymentTerms = DeserializeOrThrow<APPaymentTerms>(input);
                    var (response, saved) = await _apPaymentTermsService.CreateOrUpdateAsync(paymentTerms, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("paymentTerms", saved)));
                }
                case "api/APRemitToLocation/CreateAPRemitToLocations":
                case "api/APRemitToLocation/UpdateAPRemitToLocations":
                {
                    var remit = DeserializeOrThrow<APRemitToLocations>(input);
                    var (response, saved) = await _apRemitToLocationsService.CreateOrUpdateAsync(remit, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("remitToLocations", saved)));
                }
                case "api/APRecurringPayable/CreateAPRecurringPayables":
                case "api/APRecurringPayable/UpdateAPRecurringPayables":
                {
                    var recurring = DeserializeOrThrow<APRecurringPayables>(input);
                    var (response, saved) = await _apRecurringPayablesService.CreateOrUpdateAsync(recurring, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("recurringPayables", saved)));
                }
                case "api/APInvoice/CreateInvoice":
                {
                    var invoice = DeserializeOrThrow<APInvoices>(input);
                    var (response, saved) = await _apInvoiceService.CreateInvoiceAsync(invoice, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("invoices", saved)));
                }
                case "api/APInvoice/CreateInvoiceBatch":
                {
                    var batch = DeserializeOrThrow<APInvoiceBatch>(input);
                    var (response, saved) = await _apInvoiceService.CreateInvoiceBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("invoices", saved)));
                }
                case "api/APInvoice/ReadInvoice":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, invoice) = await _apInvoiceService.ReadInvoiceAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("invoices", invoice)));
                }
                case "api/APInvoice/ReadInvoiceBatchStatus":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _apInvoiceService.ReadInvoiceBatchStatusAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("InvoiceBatch", batch)));
                }
                case "api/APInvoice/ReadInvoiceBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _apInvoiceService.ReadInvoiceBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("InvoiceBatch", batch)));
                }
                case "api/APInvoice/SyncInvoices":
                {
                    var req = DeserializeOrThrow<SyncAPInvoices>(input);
                    var (response, sync) = await _apInvoiceService.SyncInvoicesAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/APPayment/CreatePayment":
                {
                    var payment = DeserializeOrThrow<APPayment>(input);
                    var (response, saved) = await _apPaymentService.CreatePaymentAsync(payment, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("payment", saved)));
                }
                case "api/APPayment/CreatePaymentBatch":
                {
                    var paymentBatch = DeserializeOrThrow<APPaymentBatch>(input);
                    var (response, saved) = await _apPaymentService.CreatePaymentBatchAsync(paymentBatch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("paymentBatch", saved)));
                }
                case "api/APPayment/ReadPayment":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, payment) = await _apPaymentService.ReadPaymentAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("payment", payment)));
                }
                case "api/APPayment/ReadPaymentBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, paymentBatch) = await _apPaymentService.ReadPaymentBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("paymentBatch", paymentBatch)));
                }
                case "api/APPayment/SyncAPPayments":
                {
                    var req = DeserializeOrThrow<SyncAPPayments>(input);
                    var (response, sync) = await _apPaymentService.SyncPaymentsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/APAdjustment/CreateAdjustment":
                {
                    var adjustment = DeserializeOrThrow<APAdjustments>(input);
                    var (response, saved) = await _apAdjustmentService.CreateAdjustmentAsync(adjustment, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("adjustment", saved)));
                }
                case "api/APAdjustment/CreateAdjustmentBatch":
                {
                    var adjustmentBatch = DeserializeOrThrow<APAdjustmentBatch>(input);
                    var (response, saved) = await _apAdjustmentService.CreateAdjustmentBatchAsync(adjustmentBatch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("adjustmentBatch", saved)));
                }
                case "api/APAdjustment/ReadAdjustment":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, adjustment) = await _apAdjustmentService.ReadAdjustmentAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("adjustment", adjustment)));
                }
                case "api/APAdjustment/ReadAdjustmentBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, adjustmentBatch) = await _apAdjustmentService.ReadAdjustmentBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("adjustmentBatch", adjustmentBatch)));
                }
                case "api/APAdjustment/SyncAdjustment":
                {
                    var req = DeserializeOrThrow<SyncAPAdjustments>(input);
                    var (response, sync) = await _apAdjustmentService.SyncAdjustmentsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/ARInvoice/CreateARInvoice":
                case "api/ARInvoice/UpdateARInvoice":
                {
                    var invoice = DeserializeOrThrow<ARInvoice>(input);
                    var (response, saved) = await _arInvoiceService.CreateOrUpdateAsync(invoice, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arInvoice", saved)));
                }
                case "api/ARInvoice/CreateARInvoiceBatch":
                {
                    var batch = DeserializeOrThrow<ARInvoiceBatch>(input);
                    var (response, saved) = await _arInvoiceService.CreateInvoiceBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arInvoiceBatch", saved)));
                }
                case "api/ARInvoice/ReadARInvoice":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, invoice) = await _arInvoiceService.ReadInvoiceAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arInvoice", invoice)));
                }
                case "api/ARInvoice/ReadARInvoiceBatchStatus":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arInvoiceService.ReadInvoiceBatchStatusAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arInvoiceBatch", batch)));
                }
                case "api/ARInvoice/ReadARInvoiceBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arInvoiceService.ReadInvoiceBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arInvoiceBatch", batch)));
                }
                case "api/ARInvoice/SyncARInvoices":
                {
                    var req = DeserializeOrThrow<SyncARInvoices>(input);
                    var (response, sync) = await _arInvoiceService.SyncInvoicesAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/ARAdjustment/CreateARAdjustment":
                case "api/ARAdjustment/UpdateAdjustment":
                {
                    var adjustment = DeserializeOrThrow<ARAdjustment>(input);
                    var (response, saved) = await _arAdjustmentService.CreateOrUpdateAsync(adjustment, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arAdjustment", saved)));
                }
                case "api/ARAdjustment/CreateARAdjustmentBatch":
                {
                    var batch = DeserializeOrThrow<ARAdjustmentBatch>(input);
                    var (response, saved) = await _arAdjustmentService.CreateAdjustmentBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arAdjustmentBatch", saved)));
                }
                case "api/ARAdjustment/ReadARAdjustment":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, adj) = await _arAdjustmentService.ReadAdjustmentAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arAdjustment", adj)));
                }
                case "api/ARAdjustment/ReadARAdjustmentBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arAdjustmentService.ReadAdjustmentBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arAdjustmentBatch", batch)));
                }
                case "api/ARAdjustment/SyncARAdjustments":
                {
                    var req = DeserializeOrThrow<SyncARAdjustments>(input);
                    var (response, sync) = await _arAdjustmentService.SyncAdjustmentsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/ARReceipt/CreateARReceipt":
                case "api/ARReceipt/CreateARReceiptAppendPrepayment":
                case "api/ARReceipt/UpdateReceipt":
                {
                    var receipt = DeserializeOrThrow<ARReceipt>(input);
                    var (response, saved) = await _arReceiptService.CreateOrUpdateAsync(receipt, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arReceipt", saved)));
                }
                case "api/ARReceipt/CreateARReceiptBatch":
                {
                    var batch = DeserializeOrThrow<ARReceiptBatch>(input);
                    var (response, saved) = await _arReceiptService.CreateReceiptBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arReceiptBatch", saved)));
                }
                case "api/ARReceipt/ReadARReceipt":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var batchNumber = doc.RootElement.TryGetProperty("BatchNumber", out var bn) ? bn.GetString() : null;
                    var entryNumber = doc.RootElement.TryGetProperty("EntryNumber", out var en) ? en.GetString() : null;
                    if (string.IsNullOrWhiteSpace(batchNumber) || string.IsNullOrWhiteSpace(entryNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "BatchNumber and EntryNumber are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, receipt) = await _arReceiptService.ReadReceiptAsync(batchNumber, entryNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arReceipt", receipt)));
                }
                case "api/ARReceipt/ReadARReceiptBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arReceiptService.ReadReceiptBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arReceiptBatch", batch)));
                }
                case "api/ARReceipt/SyncARReceipts":
                {
                    var req = DeserializeOrThrow<SyncARReceipts>(input);
                    var (response, sync) = await _arReceiptService.SyncReceiptsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/ARRefund/CreateARRefund":
                case "api/ARRefund/UpdateARRefund":
                {
                    var refund = DeserializeOrThrow<ARRefund>(input);
                    var (response, saved) = await _arRefundService.CreateOrUpdateAsync(refund, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arRefund", saved)));
                }
                case "api/ARRefund/CreateARRefundBatch":
                {
                    var batch = DeserializeOrThrow<ARRefundBatch>(input);
                    var (response, saved) = await _arRefundService.CreateRefundBatchAsync(batch, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arRefundBatch", saved)));
                }
                case "api/ARRefund/ReadARRefund":
                {
                    var documentNumber = ExtractKey(input, "DocumentNumber023");
                    var (response, refund) = await _arRefundService.ReadRefundAsync(documentNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arRefund", refund)));
                }
                case "api/ARRefund/ReadARRefundBatch":
                {
                    var batchNumber = ExtractKey(input, "BatchNumber");
                    var (response, batch) = await _arRefundService.ReadRefundBatchAsync(batchNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arRefundBatch", batch)));
                }
                case "api/ARRefund/SyncARRefunds":
                {
                    var req = DeserializeOrThrow<SyncARRefunds>(input);
                    var (response, sync) = await _arRefundService.SyncRefundsAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/ARBillingCycle/CreateARBillingCycles":
                case "api/ARBillingCycle/UpdateARBillingCycles":
                {
                    var billingCycles = DeserializeOrThrow<ARBillingCycles>(input);
                    var (response, saved) = await _arBillingCyclesService.CreateOrUpdateAsync(billingCycles, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("billingCycles", saved)));
                }
                case "api/ARBillingCycle/ReadARBillingCycles":
                {
                    var billingCycle = ExtractKey(input, "BillingCycle000");
                    var (response, billingCycles) = await _arBillingCyclesService.ReadAsync(billingCycle, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("billingCycles", billingCycles)));
                }
                case "api/ARSalesperson/CreateARSalesPersons":
                case "api/ARSalesperson/UpdateARSalesPersons":
                {
                    var salesPerson = DeserializeOrThrow<ARSalesPersons>(input);
                    var (response, saved) = await _arSalesPersonsService.CreateOrUpdateAsync(salesPerson, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("salesPerson", saved)));
                }
                case "api/ARCustomer/CreateARCustomer":
                case "api/ARCustomer/UpdateARCustomer":
                {
                    var customer = DeserializeOrThrow<ARCustomers>(input);
                    var (response, saved) = await _arCustomerService.CreateOrUpdateAsync(customer, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("customer", saved)));
                }
                case "api/ARCustomer/ReadARCustomer":
                {
                    var customerNumber = ExtractKey(input, "CustomerNumber000");
                    var (response, customer) = await _arCustomerService.ReadAsync(customerNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("customer", customer)));
                }
                case "api/ARCustomer/ReadARCustomerBalance":
                {
                    var customerNumber = ExtractKey(input, "CustomerNumber000");
                    var (response, customerBalance) = await _arCustomerService.ReadBalanceAsync(customerNumber, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("customerBalance", customerBalance)));
                }
                case "api/ARTermsCode/CreateARTermsCodes":
                case "api/ARTermsCode/UpdateARTermsCodes":
                {
                    var termsCodes = DeserializeOrThrow<ARTermsCodes>(input);
                    var (response, saved) = await _arTermsCodesService.CreateOrUpdateAsync(termsCodes, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("termsCodes", saved)));
                }
                case "api/ARShipToLocation/CreateARShipToLocation":
                case "api/ARShipToLocation/UpdateARShipToLocation":
                {
                    var shipTo = DeserializeOrThrow<ARShipToLocations>(input);
                    var (response, saved) = await _arShipToLocationService.CreateOrUpdateAsync(shipTo, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("shipToLocation", saved)));
                }
                case "api/ARShipToLocation/ReadARShipToLocation":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var customerNumber = doc.RootElement.TryGetProperty("CustomerNumber", out var cn) ? cn.GetString() : null;
                    var shipToLocation = doc.RootElement.TryGetProperty("ShipToLocation", out var st) ? st.GetString() : null;
                    if (string.IsNullOrWhiteSpace(customerNumber) || string.IsNullOrWhiteSpace(shipToLocation))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "CustomerNumber and ShipToLocation are required."), Data(("restRoute", restRoute)));
                    }

                    var (response, shipTo) = await _arShipToLocationService.ReadAsync(customerNumber, shipToLocation, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("shipToLocation", shipTo)));
                }
                case "api/ARShipToLocation/ReadCustomerShipToLocations":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    using var doc = JsonDocument.Parse(json);
                    var customerNumber = doc.RootElement.TryGetProperty("CustomerNumber", out var cn) ? cn.GetString() : null;
                    var shipToId = doc.RootElement.TryGetProperty("ShipToID", out var st) ? st.GetString() : null;
                    if (string.IsNullOrWhiteSpace(customerNumber))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "CustomerNumber is required."), Data(("restRoute", restRoute)));
                    }

                    var (response, shipTos) = await _arShipToLocationService.ReadCustomerShipToLocationsAsync(customerNumber, shipToId, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("shipToLocations", shipTos)));
                }
                case "api/ARShipToLocation/SyncARShipToLocation":
                {
                    var req = DeserializeOrThrow<SyncARShipToLocations>(input);
                    var (response, sync) = await _arShipToLocationService.SyncAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/ARCustomerGroup/CreateARCustomerGroupss":
                case "api/ARCustomerGroup/UpdateARCustomerGroups":
                {
                    var customerGroup = DeserializeOrThrow<ARCustomerGroups>(input);
                    var (response, saved) = await _arCustomerGroupService.CreateOrUpdateAsync(customerGroup, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("customerGroup", saved)));
                }
                case "api/ARItem/CreateARItems":
                case "api/ARItem/UpdateARItems":
                {
                    var item = DeserializeOrThrow<ARItems>(input);
                    var (response, saved) = await _arItemService.CreateOrUpdateAsync(item, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("arItems", saved)));
                }
                case "api/ARDocuments/GetDocuments":
                {
                    var documents = DeserializeOrThrow<AROpenInvoices>(input);
                    var (response, saved) = await _arDocumentsService.GetDocumentsAsync(documents, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("invoices", saved)));
                }
                case "api/ARDocuments/GetAgedBalances":
                {
                    var json = input as string;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "inputJson is required."), Data(("restRoute", restRoute)));
                    }

                    string? customer = null;
                    if (json.TrimStart().StartsWith("{", StringComparison.Ordinal))
                    {
                        using var doc = JsonDocument.Parse(json);
                        customer =
                            doc.RootElement.TryGetProperty("customer", out var c) ? c.GetString() :
                            doc.RootElement.TryGetProperty("CustomerNumber001", out var cn) ? cn.GetString() :
                            doc.RootElement.TryGetProperty("CustomerNumber000", out var c0) ? c0.GetString() :
                            null;
                    }
                    else
                    {
                        customer = json.Trim().Trim('"');
                    }

                    if (string.IsNullOrWhiteSpace(customer))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "customer is required."), Data(("restRoute", restRoute)));
                    }

                    var (response, analysis) = await _arDocumentsService.GetAgedBalancesAsync(customer, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("agedAnalysis", analysis)));
                }
                case "api/ARCustomer/ReadARStatementRun":
                case "api/ARStatementRun/ReadARStatementRun":
                {
                    var req = DeserializeOrThrow<ARStatementRun>(input);
                    var (response, statementRun) = await _arStatementRunService.ReadAsync(req, user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("statementRun", statementRun)));
                }
                case "api/ARCustomer/Read_ARStatementRun":
                case "api/ARStatementRun/Read_ARStatementRun":
                {
                    var (response, sync) = await _arStatementRunService.ReadAllAsync(user, cancellationToken);
                    return new AccpacOperationResult(response, Data(("sync", sync)));
                }
                case "api/APVendor/SyncAPVendor":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "AP",
                        txnType: "VD",
                        targetViewIdsCsv: "AP0015",
                        targetPrimaryViewId: "AP0015",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/APVendor/ConfirmVendorSync":
                {
                    var req = ParseJsonObject(input);
                    var previousTs = GetString(req, "Timestamp");
                    if (string.IsNullOrWhiteSpace(previousTs))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "Timestamp is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var tran = session.BeginTransaction();
                    try
                    {
                        var views = new Sage300ViewSet(session, "YH0305", compose: false);
                        dynamic yh = views.ViewById("YH0305");
                        yh.Browse($"MODULE = \"AP\" AND TXNTYPE = \"VD\" AND TIMESTAMP = \"{previousTs}\"", true);
                        while (yh.Fetch())
                        {
                            yh.Fields.FieldByName("YHSTATUS").Value = 1;
                            yh.Update();
                        }

                        session.CommitTransaction(tran);
                        return new AccpacOperationResult(ProcessOut.Ok("Vendor sync confirmed.", previousTs), Data(("timestamp", previousTs)));
                    }
                    catch (Exception ex)
                    {
                        try { session.RollbackTransaction(tran); } catch { }
                        return new AccpacOperationResult(ProcessOut.Fail("9999", ex.Message), Data(("restRoute", restRoute)));
                    }
                }
                case "api/ARCustomer/SyncARCustomer":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "AR",
                        txnType: "CU",
                        targetViewIdsCsv: "AR0024,AR0400",
                        targetPrimaryViewId: "AR0024",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/GLAccount/CreateGLAccount":
                case "api/GLAccount/UpdateGLAccount":
                {
                    var req = ParseJsonObject(input);
                    var acctId = GetString(req, "ACCTID") ?? GetString(req, "UnformattedAccount000");
                    if (string.IsNullOrWhiteSpace(acctId))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "ACCTID (or UnformattedAccount000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = Sage300GenericOps.UpsertSingle(
                        session,
                        viewIdsCsv: "GL0001,GL0003,GL0004,GL0012,GL0107,GL0400,GL0401,GL0057,GL0063",
                        primaryViewId: "GL0001",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["ACCTID"] = acctId },
                        payload: req,
                        operationName: "GLAccount");

                    return new AccpacOperationResult(response, Data(("account", new { ACCTID = acctId })));
                }
                case "api/GLAccount/SyncGLAccounts":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "GL",
                        txnType: "AC",
                        targetViewIdsCsv: "GL0001,GL0003,GL0004,GL0012,GL0107,GL0400,GL0401,GL0057,GL0063",
                        targetPrimaryViewId: "GL0001",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/GLAccount/SyncGLFiscalSets":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "GL",
                        txnType: "FS",
                        targetViewIdsCsv: "GL0103",
                        targetPrimaryViewId: "GL0103",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/GLJournalEntry/CreateGLJournalEntry":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "GL0008,GL0006,GL0010,GL0402", headerViewId: "GL0006", detailViewId: "GL0010", req);
                    return new AccpacOperationResult(response, Data(("journalEntry", new { header = "GL0006" })));
                }
                case "api/GLRecurringEntries/CreateGLRecurringEntries":
                case "api/GLRecurringEntries/UpdateGLRecurringEntries":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "GL0041,GL0042,GL0403", headerViewId: "GL0041", detailViewId: "GL0042", req);
                    return new AccpacOperationResult(response, Data(("recurringEntry", new { header = "GL0041" })));
                }
                case "api/ICCategories/CreateICCategory":
                case "api/ICCategories/UpdateICCategory":
                {
                    var req = ParseJsonObject(input);
                    var category = GetString(req, "CATEGORY") ?? GetString(req, "CategoryCode000");
                    if (string.IsNullOrWhiteSpace(category))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "CATEGORY (or CategoryCode000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = Sage300GenericOps.UpsertSingle(
                        session,
                        viewIdsCsv: "IC0210,IC0220,GL0001,IC0390",
                        primaryViewId: "IC0210",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["CATEGORY"] = category },
                        payload: req,
                        operationName: "ICCategory");

                    return new AccpacOperationResult(response, Data(("category", new { CATEGORY = category })));
                }
                case "api/ICCategories/ReadICCategory":
                {
                    var json = ParseJsonObject(input);
                    var category = GetString(json, "CATEGORY") ?? GetString(json, "CategoryCode000");
                    if (string.IsNullOrWhiteSpace(category))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "CATEGORY (or CategoryCode000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, record) = Sage300GenericOps.ReadSingle(
                        session,
                        viewIdsCsv: "IC0210,IC0220,GL0001,IC0390",
                        primaryViewId: "IC0210",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["CATEGORY"] = category },
                        operationName: "ICCategory");

                    return new AccpacOperationResult(response, Data(("category", record)));
                }
                case "api/ICCategories/SyncICCategories":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "IC",
                        txnType: "CT",
                        targetViewIdsCsv: "IC0210,IC0220,GL0001,IC0390",
                        targetPrimaryViewId: "IC0210",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/ICLocations/CreateICLocation":
                case "api/ICLocations/UpdateICLocation":
                {
                    var req = ParseJsonObject(input);
                    var location = GetString(req, "LOCATION") ?? GetString(req, "LocationCode000");
                    if (string.IsNullOrWhiteSpace(location))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "LOCATION (or LocationCode000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = Sage300GenericOps.UpsertSingle(
                        session,
                        viewIdsCsv: "IC0370",
                        primaryViewId: "IC0370",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["LOCATION"] = location },
                        payload: req,
                        operationName: "ICLocation");

                    return new AccpacOperationResult(response, Data(("location", new { LOCATION = location })));
                }
                case "api/ICLocations/ReadICLocation":
                {
                    var req = ParseJsonObject(input);
                    var location = GetString(req, "LOCATION") ?? GetString(req, "LocationCode000");
                    if (string.IsNullOrWhiteSpace(location))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "LOCATION (or LocationCode000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, record) = Sage300GenericOps.ReadSingle(
                        session,
                        viewIdsCsv: "IC0370",
                        primaryViewId: "IC0370",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["LOCATION"] = location },
                        operationName: "ICLocation");

                    return new AccpacOperationResult(response, Data(("location", record)));
                }
                case "api/ICLocations/SyncICItems":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "IC",
                        txnType: "LO",
                        targetViewIdsCsv: "IC0370",
                        targetPrimaryViewId: "IC0370",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/ICItem/CreateICItem":
                case "api/ICItem/UpdateICItem":
                {
                    var req = ParseJsonObject(input);
                    var itemNo = GetString(req, "ITEMNO") ?? GetString(req, "ItemNumber000");
                    if (string.IsNullOrWhiteSpace(itemNo))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "ITEMNO (or ItemNumber000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = Sage300GenericOps.UpsertSingle(
                        session,
                        viewIdsCsv: "IC0310,IC0750,IC0313",
                        primaryViewId: "IC0310",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["ITEMNO"] = itemNo },
                        payload: req,
                        operationName: "ICItem");

                    return new AccpacOperationResult(response, Data(("item", new { ITEMNO = itemNo })));
                }
                case "api/ICItem/ReadICItem":
                {
                    var req = ParseJsonObject(input);
                    var itemNo = GetString(req, "ITEMNO") ?? GetString(req, "ItemNumber000");
                    if (string.IsNullOrWhiteSpace(itemNo))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "ITEMNO (or ItemNumber000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, record) = Sage300GenericOps.ReadSingle(
                        session,
                        viewIdsCsv: "IC0310,IC0750,IC0313",
                        primaryViewId: "IC0310",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["ITEMNO"] = itemNo },
                        operationName: "ICItem");

                    return new AccpacOperationResult(response, Data(("item", record)));
                }
                case "api/ICItem/SyncICItems":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "IC",
                        txnType: "MA",
                        targetViewIdsCsv: "IC0310,IC0750,IC0313",
                        targetPrimaryViewId: "IC0310",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/ICItemPricing/CreateItemPricing":
                case "api/ICItemPricing/UpdateItemPricing":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);

                    var views = new Sage300ViewSet(session, "IC0480,IC0481,IC0482,IC0490,IC0310,IC0390,IC0395,IC0750,IC0758,IC0392,IC0290", compose: true);
                    dynamic pricing = views.ViewById("IC0480");
                    var keyNames = Sage300GenericOps.GetKeyFieldNames(pricing);
                    var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var key in keyNames)
                    {
                        var v = GetString(req, key);
                        if (!string.IsNullOrWhiteSpace(v))
                        {
                            keyValues[key] = v;
                        }
                    }

                    if (keyValues.Count == 0)
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "At least one key field is required for IC item pricing."), Data(("restRoute", restRoute)));
                    }

                    var response = Sage300GenericOps.UpsertSingle(
                        session,
                        viewIdsCsv: "IC0480,IC0481,IC0482,IC0490,IC0310,IC0390,IC0395,IC0750,IC0758,IC0392,IC0290",
                        primaryViewId: "IC0480",
                        keyFieldValues: keyValues,
                        payload: req,
                        operationName: "ICItemPricing");

                    return new AccpacOperationResult(response, Data(("pricing", keyValues)));
                }
                case "api/ICItemPricing/ReadItemPricing":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var views = new Sage300ViewSet(session, "IC0480,IC0481,IC0482,IC0490,IC0310,IC0390,IC0395,IC0750,IC0758,IC0392,IC0290", compose: true);
                    dynamic pricing = views.ViewById("IC0480");
                    var keyNames = Sage300GenericOps.GetKeyFieldNames(pricing);
                    var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var key in keyNames)
                    {
                        var v = GetString(req, key);
                        if (!string.IsNullOrWhiteSpace(v))
                        {
                            keyValues[key] = v;
                        }
                    }

                    if (keyValues.Count == 0)
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "At least one key field is required for IC item pricing."), Data(("restRoute", restRoute)));
                    }

                    var (response, record) = Sage300GenericOps.ReadSingle(
                        session,
                        viewIdsCsv: "IC0480,IC0481,IC0482,IC0490,IC0310,IC0390,IC0395,IC0750,IC0758,IC0392,IC0290",
                        primaryViewId: "IC0480",
                        keyFieldValues: keyValues,
                        operationName: "ICItemPricing");

                    return new AccpacOperationResult(response, Data(("pricing", record)));
                }
                case "api/ICItemPricing/SyncItemPricings":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "IC",
                        txnType: "IP",
                        targetViewIdsCsv: "IC0480,IC0481,IC0482,IC0490,IC0310,IC0390,IC0395,IC0750,IC0758,IC0392,IC0290",
                        targetPrimaryViewId: "IC0480",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/ICLocationDetails/ReadICLocationDetails":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var views = new Sage300ViewSet(session, "IC0290,IC0310,IC0370,IC0750", compose: true);
                    dynamic v = views.ViewById("IC0290");
                    var keyNames = Sage300GenericOps.GetKeyFieldNames(v);
                    var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var key in keyNames)
                    {
                        var val = GetString(req, key) ?? GetString(req, key.Equals("ITEMNO", StringComparison.OrdinalIgnoreCase) ? "ItemNumber000" : key);
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            keyValues[key] = val;
                        }
                    }

                    var (response, record) = Sage300GenericOps.ReadSingle(session, "IC0290,IC0310,IC0370,IC0750", "IC0290", keyValues, "ICLocationDetails");
                    return new AccpacOperationResult(response, Data(("locationDetails", record)));
                }
                case "api/ICLocationDetails/ReadICItemsLocationDetails":
                {
                    var req = ParseJsonObject(input);
                    var itemNo = GetString(req, "ITEMNO") ?? GetString(req, "ItemNumber000");
                    if (string.IsNullOrWhiteSpace(itemNo))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "ITEMNO (or ItemNumber000) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var views = new Sage300ViewSet(session, "IC0290,IC0310,IC0370,IC0750", compose: true);
                    dynamic v = views.ViewById("IC0290");
                    v.Browse($"ITEMNO = \"{itemNo}\"", true);
                    var list = new List<Dictionary<string, string?>>();
                    while (v.Fetch())
                    {
                        list.Add(Sage300GenericOps.DumpFields(v));
                        if (list.Count >= 500)
                        {
                            break;
                        }
                    }

                    return new AccpacOperationResult(ProcessOut.Ok("IC item location details read."), Data(("records", list)));
                }
                case "api/ICLocationDetails/SyncICItemsLocationDetails":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 1000);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0305",
                        module: "IC",
                        txnType: "IL",
                        targetViewIdsCsv: "IC0290,IC0310,IC0370,IC0750",
                        targetPrimaryViewId: "IC0290",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/ICAdjustment/CreateICAdjustments":
                case "api/ICAdjustment/UpdateICAdjustments":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "IC0110,IC0113,IC0115,IC0117,IC0120,IC0125", headerViewId: "IC0120", detailViewId: "IC0110", req);
                    return new AccpacOperationResult(response, Data(("adjustment", new { header = "IC0120" })));
                }
                case "api/ICAdjustment/SyncICAdjustments":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "IC0110,IC0113,IC0115,IC0117,IC0120,IC0125",
                        headerViewId: "IC0120",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/ICReceipt/CreateICReceipt":
                case "api/ICReceipt/UpdateICReceipt":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "IC0580,IC0582,IC0585,IC0587,IC0590,IC0595", headerViewId: "IC0590", detailViewId: "IC0580", req);
                    return new AccpacOperationResult(response, Data(("receipt", new { header = "IC0590" })));
                }
                case "api/ICReceipt/SyncICReceipts":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "IC0580,IC0582,IC0585,IC0587,IC0590,IC0595",
                        headerViewId: "IC0590",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/ICShipment/CreateICShipment":
                case "api/ICShipment/UpdateICShipment":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "IC0630,IC0632,IC0635,IC0636,IC0640,IC0645", headerViewId: "IC0640", detailViewId: "IC0630", req);
                    return new AccpacOperationResult(response, Data(("shipment", new { header = "IC0640" })));
                }
                case "api/ICInternalUsage/CreateICInternalUsage":
                case "api/ICInternalUsage/UpdateICInternalUsage":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "IC0282,IC0283,IC0284,IC0286,IC0287,IC0288,IC0289,IC0290,IC0310,IC0750,IC0210,IC0370", headerViewId: "IC0288", detailViewId: "IC0286", req);
                    return new AccpacOperationResult(response, Data(("internalUsage", new { header = "IC0288" })));
                }
                case "api/ICAssembly/CreateICAssembly":
                case "api/ICAssembly/UpdateICAssembly":
                {
                    var req = ParseJsonObject(input);
                    var docNum = GetString(req, "DOCNUM") ?? GetString(req, "AssemblyNumber002");
                    if (string.IsNullOrWhiteSpace(docNum))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "DOCNUM (or AssemblyNumber002) is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = Sage300GenericOps.UpsertSingle(
                        session,
                        viewIdsCsv: "IC0160,IC0165,IC0167,IC0162,IC0200,IC0370,IC0290",
                        primaryViewId: "IC0160",
                        keyFieldValues: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["DOCNUM"] = docNum },
                        payload: req,
                        operationName: "ICAssembly");

                    return new AccpacOperationResult(response, Data(("assembly", new { DOCNUM = docNum })));
                }
                case "api/ICTransfer/CreateICTransfer":
                case "api/ICTransfer/UpdateICTransfer":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "IC0740,IC0730,IC0741,IC0310,IC0370,IC0290,IC0100,IC0735,IC0738,IC0733,IC0810", headerViewId: "IC0740", detailViewId: "IC0730", req);
                    return new AccpacOperationResult(response, Data(("transfer", new { header = "IC0740" })));
                }
                case "api/ICTransfer/SyncIcTransfers":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);
                    var systemId = GetNullableInt(req, "Systemid");

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, records) = Sage300GenericOps.SyncFromYh(
                        session,
                        yhViewId: "YH0303",
                        module: "IC",
                        txnType: "TR",
                        targetViewIdsCsv: "IC0740,IC0730,IC0741,IC0310,IC0370,IC0290,IC0100,IC0735,IC0738,IC0733,IC0810",
                        targetPrimaryViewId: "IC0740",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit,
                        systemId: systemId);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, records })));
                }
                case "api/ICTransfer/ConfirmSynedICTransfers":
                {
                    var req = ParseJsonObject(input);
                    var previousTs = GetString(req, "Timestamp");
                    if (string.IsNullOrWhiteSpace(previousTs))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "Timestamp is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var tran = session.BeginTransaction();
                    try
                    {
                        var views = new Sage300ViewSet(session, "YH0303", compose: false);
                        dynamic yh = views.ViewById("YH0303");
                        yh.Browse($"MODULE = \"IC\" AND TXNTYPE = \"TR\" AND TIMESTAMP = \"{previousTs}\"", true);
                        while (yh.Fetch())
                        {
                            yh.Fields.FieldByName("YHSTATUS").Value = 1;
                            yh.Update();
                        }

                        session.CommitTransaction(tran);
                        return new AccpacOperationResult(ProcessOut.Ok("IC transfers sync confirmed.", previousTs), Data(("timestamp", previousTs)));
                    }
                    catch (Exception ex)
                    {
                        try { session.RollbackTransaction(tran); } catch { }
                        return new AccpacOperationResult(ProcessOut.Fail("9999", ex.Message), Data(("restRoute", restRoute)));
                    }
                }
                case "api/OESalesOrder/CreateOESalesOrder":
                case "api/OESalesOrder/UpdateOESalesOrder":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "OE0180,OE0500,OE0501,OE0502,OE0503,OE0508,OE0507,OE0520,OE0522,OE0526,OE0740,OE0270,OE0504,OE0506", headerViewId: "OE0520", detailViewId: "OE0500", req);
                    return new AccpacOperationResult(response, Data(("salesOrder", new { header = "OE0520" })));
                }
                case "api/OESalesOrder/ReadOESalesOrder":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);

                    var views = new Sage300ViewSet(session, "OE0180,OE0500,OE0501,OE0502,OE0503,OE0508,OE0507,OE0520,OE0522,OE0526,OE0740,OE0270,OE0504,OE0506", compose: true);
                    dynamic header = views.ViewById("OE0520");
                    var keyNames = Sage300GenericOps.GetKeyFieldNames(header);
                    var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var k in keyNames)
                    {
                        var v = GetString(req, k);
                        if (!string.IsNullOrWhiteSpace(v))
                        {
                            keyValues[k] = v;
                        }
                    }

                    var (response, record) = Sage300GenericOps.ReadSingle(session, "OE0180,OE0500,OE0501,OE0502,OE0503,OE0508,OE0507,OE0520,OE0522,OE0526,OE0740,OE0270,OE0504,OE0506", "OE0520", keyValues, "OESalesOrder");
                    return new AccpacOperationResult(response, Data(("salesOrder", record)));
                }
                case "api/OESalesOrder/SyncOESalesOrders":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "OE0180,OE0500,OE0501,OE0502,OE0503,OE0508,OE0507,OE0520,OE0522,OE0526,OE0740,OE0270,OE0504,OE0506",
                        headerViewId: "OE0520",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/OESalesOrder/ConfirmSynedOESalesOrder":
                {
                    var req = ParseJsonObject(input);
                    var previousTs = GetString(req, "Timestamp");
                    if (string.IsNullOrWhiteSpace(previousTs))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "Timestamp is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var tran = session.BeginTransaction();
                    try
                    {
                        var views = new Sage300ViewSet(session, "OE0180,OE0500,OE0501,OE0502,OE0503,OE0508,OE0507,OE0520,OE0522,OE0526,OE0740,OE0270,OE0504,OE0506", compose: true);
                        dynamic header = views.ViewById("OE0520");
                        header.Browse($"TIMESTAMP = \"{previousTs}\"", true);
                        while (header.Fetch())
                        {
                            header.Fields.FieldByName("YHSTATUS").Value = 1;
                            header.Update();
                        }

                        session.CommitTransaction(tran);
                        return new AccpacOperationResult(ProcessOut.Ok("OE sales order sync confirmed.", previousTs), Data(("timestamp", previousTs)));
                    }
                    catch (Exception ex)
                    {
                        try { session.RollbackTransaction(tran); } catch { }
                        return new AccpacOperationResult(ProcessOut.Fail("9999", ex.Message), Data(("restRoute", restRoute)));
                    }
                }
                case "api/OESalesOrder/CompleteOESalesOrder":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var tran = session.BeginTransaction();
                    try
                    {
                        var views = new Sage300ViewSet(session, "OE0180,OE0500,OE0501,OE0502,OE0503,OE0508,OE0507,OE0520,OE0522,OE0526,OE0740,OE0270,OE0504,OE0506", compose: true);
                        dynamic header = views.ViewById("OE0520");
                        var keyNames = Sage300GenericOps.GetKeyFieldNames(header);
                        header.Init();
                        foreach (var k in keyNames)
                        {
                            var v = GetString(req, k);
                            if (!string.IsNullOrWhiteSpace(v))
                            {
                                Sage300GenericOps.TryPut(header, k, v);
                            }
                        }

                        if (!(bool)header.Exists)
                        {
                            session.RollbackTransaction(tran);
                            return new AccpacOperationResult(ProcessOut.Fail("0009", "OE sales order not found."), Data(("restRoute", restRoute)));
                        }

                        header.Read();
                        Sage300GenericOps.TryPut(header, "OECOMMAND", 8);
                        header.Update();
                        session.CommitTransaction(tran);
                        return new AccpacOperationResult(ProcessOut.Ok("OE sales order completed."), Data(("status", "completed")));
                    }
                    catch (Exception ex)
                    {
                        try { session.RollbackTransaction(tran); } catch { }
                        return new AccpacOperationResult(ProcessOut.Fail("9999", ex.Message), Data(("restRoute", restRoute)));
                    }
                }
                case "api/OEShipment/CreateOEShipment":
                case "api/OEShipment/UpdateOEShipment":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "OE0691,OE0999,OE0190,OE0745,OE0694,OE0704,OE0692,OE0697,OE0702,OE0705,OE0703,OE0708,OE0709,OE0707,OE0706,OE0676,OE0675,OE0709,OE0671,OE0710", headerViewId: "OE0692", detailViewId: "OE0691", req);
                    return new AccpacOperationResult(response, Data(("shipment", new { header = "OE0692" })));
                }
                case "api/OEShipment/SyncOEShipments":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "OE0691,OE0999,OE0190,OE0745,OE0694,OE0704,OE0692,OE0697,OE0702,OE0705,OE0703,OE0708,OE0709,OE0707,OE0706,OE0676,OE0675,OE0709,OE0671,OE0710",
                        headerViewId: "OE0692",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/OEInvoice/CreateOEInvoice":
                case "api/OEInvoice/UpdateOEInvoice":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "OE0160,OE0400,OE0401,OE0402,OE0403,OE0404,OE0405,OE0406,OE0407,OE0415,OE0420,OE0422,OE0425,OE0427,OE0720", headerViewId: "OE0420", detailViewId: "OE0400", req);
                    return new AccpacOperationResult(response, Data(("invoice", new { header = "OE0420" })));
                }
                case "api/OEInvoice/ReadOEInvoice":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var views = new Sage300ViewSet(session, "OE0160,OE0400,OE0401,OE0402,OE0403,OE0404,OE0405,OE0406,OE0407,OE0415,OE0420,OE0422,OE0425,OE0427,OE0720", compose: true);
                    dynamic header = views.ViewById("OE0420");
                    var keyNames = Sage300GenericOps.GetKeyFieldNames(header);
                    var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var k in keyNames)
                    {
                        var v = GetString(req, k);
                        if (!string.IsNullOrWhiteSpace(v))
                        {
                            keyValues[k] = v;
                        }
                    }

                    var (response, record) = Sage300GenericOps.ReadSingle(session, "OE0160,OE0400,OE0401,OE0402,OE0403,OE0404,OE0405,OE0406,OE0407,OE0415,OE0420,OE0422,OE0425,OE0427,OE0720", "OE0420", keyValues, "OEInvoice");
                    return new AccpacOperationResult(response, Data(("invoice", record)));
                }
                case "api/OEInvoice/SyncOEInvoices":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "OE0160,OE0400,OE0401,OE0402,OE0403,OE0404,OE0405,OE0406,OE0407,OE0415,OE0420,OE0422,OE0425,OE0427,OE0720",
                        headerViewId: "OE0420",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/OECreditNote/CreateARCreateOECreditNoteCustomer":
                case "api/OEDebitNote/CreateOEDebitNote":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "OE0140,OE0220,OE0221,OE0222,OE0223,OE0224,OE0225,OE0226,OE0227,OE0240,OE0242,OE0250,OE0999", headerViewId: "OE0240", detailViewId: "OE0220", req);
                    return new AccpacOperationResult(response, Data(("creditDebitNote", new { header = "OE0240" })));
                }
                case "api/OECreditNote/SyncOEDebitCreditNotes":
                case "api/OECreditNote/SyncYJCustomCreditNotes":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "OE0140,OE0220,OE0221,OE0222,OE0223,OE0224,OE0225,OE0226,OE0227,OE0240,OE0242,OE0250,OE0999",
                        headerViewId: "OE0240",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/OECreditNote/ConfirmSyncedOECRDRNote":
                {
                    var req = ParseJsonObject(input);
                    var previousTs = GetString(req, "Timestamp");
                    if (string.IsNullOrWhiteSpace(previousTs))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "Timestamp is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var tran = session.BeginTransaction();
                    try
                    {
                        var views = new Sage300ViewSet(session, "OE0140,OE0220,OE0221,OE0222,OE0223,OE0224,OE0225,OE0226,OE0227,OE0240,OE0242,OE0250,OE0999", compose: true);
                        dynamic header = views.ViewById("OE0240");
                        header.Browse($"TIMESTAMP = \"{previousTs}\"", true);
                        while (header.Fetch())
                        {
                            header.Fields.FieldByName("YHSTATUS").Value = 1;
                            header.Update();
                        }

                        session.CommitTransaction(tran);
                        return new AccpacOperationResult(ProcessOut.Ok("OE credit/debit note sync confirmed.", previousTs), Data(("timestamp", previousTs)));
                    }
                    catch (Exception ex)
                    {
                        try { session.RollbackTransaction(tran); } catch { }
                        return new AccpacOperationResult(ProcessOut.Fail("9999", ex.Message), Data(("restRoute", restRoute)));
                    }
                }
                case "api/OEInvoiceMultiShipment/CreateOEInvoiceByShipmentReference":
                {
                    return new AccpacOperationResult(
                        ProcessOut.Fail("9997", "CreateOEInvoiceByShipmentReference is not supported in generic mode. Use api/OEInvoice/CreateOEInvoice with explicit header/detail fields."),
                        Data(("restRoute", restRoute)));
                }
                case "api/OEInvoiceMultiShipment/CreateOEMultiShipmentInvoice":
                {
                    return new AccpacOperationResult(
                        ProcessOut.Fail("9997", "CreateOEMultiShipmentInvoice is not supported in generic mode. Use api/OEInvoice/CreateOEInvoice with explicit header/detail fields."),
                        Data(("restRoute", restRoute)));
                }
                case "api/POPurchaseOrder/CreatePurchaseOrder":
                case "api/POPurchaseOrder/UpdatePurchaseOrder":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "PO0620,PO0630,PO0610,PO0632,PO0619,PO0623,PO0633", headerViewId: "PO0620", detailViewId: "PO0630", req);
                    return new AccpacOperationResult(response, Data(("purchaseOrder", new { header = "PO0620" })));
                }
                case "api/POPurchaseOrder/SyncPurchaseOrders":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "PO0620,PO0630,PO0610,PO0632,PO0619,PO0623,PO0633",
                        headerViewId: "PO0620",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/POPurchaseOrder/ConfirmSynedPOList":
                {
                    var req = ParseJsonObject(input);
                    var previousTs = GetString(req, "Timestamp");
                    if (string.IsNullOrWhiteSpace(previousTs))
                    {
                        return new AccpacOperationResult(ProcessOut.Fail("9999", "Timestamp is required."), Data(("restRoute", restRoute)));
                    }

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var tran = session.BeginTransaction();
                    try
                    {
                        var views = new Sage300ViewSet(session, "PO0620,PO0630,PO0610,PO0632,PO0619,PO0623,PO0633", compose: true);
                        dynamic header = views.ViewById("PO0620");
                        header.Browse($"TIMESTAMP = \"{previousTs}\"", true);
                        while (header.Fetch())
                        {
                            header.Fields.FieldByName("YHSTATUS").Value = 1;
                            header.Update();
                        }

                        session.CommitTransaction(tran);
                        return new AccpacOperationResult(ProcessOut.Ok("PO list sync confirmed.", previousTs), Data(("timestamp", previousTs)));
                    }
                    catch (Exception ex)
                    {
                        try { session.RollbackTransaction(tran); } catch { }
                        return new AccpacOperationResult(ProcessOut.Fail("9999", ex.Message), Data(("restRoute", restRoute)));
                    }
                }
                case "api/POReceipt/CreatePOReceipt":
                case "api/POReceipt/UpdatePOReceipt":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "PO0700,PO0695,PO0710,PO0718,PO0714,PO0699,PO0705,PO0703,PO0696,PO0701,PO0711,PO0717,PO0789,PO0780", headerViewId: "PO0700", detailViewId: "PO0710", req);
                    return new AccpacOperationResult(response, Data(("receipt", new { header = "PO0700" })));
                }
                case "api/POReceipt/SyncPOReceipts":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "PO0700,PO0695,PO0710,PO0718,PO0714,PO0699,PO0705,PO0703,PO0696,PO0701,PO0711,PO0717,PO0789,PO0780",
                        headerViewId: "PO0700",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/POInvoice/CreateInvoice":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "PO0420,PO0416,PO0430,PO0440,PO0436,PO0419,PO0438,PO0444,PO0423,PO0415,PO0421,PO0431,PO0433,PO0819,PO0810,PO0811,PO0818", headerViewId: "PO0420", detailViewId: "PO0430", req);
                    return new AccpacOperationResult(response, Data(("invoice", new { header = "PO0420" })));
                }
                case "api/POInvoice/SyncPOInvoices":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "PO0420,PO0416,PO0430,PO0440,PO0436,PO0419,PO0438,PO0444,PO0423,PO0415,PO0421,PO0431,PO0433,PO0819,PO0810,PO0811,PO0818",
                        headerViewId: "PO0420",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/PODebitCreditNote/CreatePODebitCreditNote":
                case "api/PODRCR/CreatePODebitCreditNote":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "PO0311,PO0309,PO0315,PO0320,PO0310,PO0325,PO0314,PO0326,PO0316,PO0318,PO0829,PO0820,PO0828,PO0821", headerViewId: "PO0311", detailViewId: "PO0315", req);
                    return new AccpacOperationResult(response, Data(("debitCreditNote", new { header = "PO0311" })));
                }
                case "api/PODebitCreditNote/SyncPODebitCreditNotes":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "PO0311,PO0309,PO0315,PO0320,PO0310,PO0325,PO0314,PO0326,PO0316,PO0318,PO0829,PO0820,PO0828,PO0821",
                        headerViewId: "PO0311",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/PORequisition/CreateRequisition":
                case "api/PORequisition/UpdateRequisition":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "PO0750,PO0760,PO0770,PO0759,PO0763,PO0773,PO0777", headerViewId: "PO0750", detailViewId: "PO0760", req);
                    return new AccpacOperationResult(response, Data(("requisition", new { header = "PO0750" })));
                }
                case "api/POReturn/CreateReturn":
                {
                    var req = ParseJsonObject(input);
                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var response = UpsertDocument(session, "PO0731,PO0729,PO0735,PO0730,PO0738,PO0732,PO0736,PO0739,PO0799,PO0790", headerViewId: "PO0731", detailViewId: "PO0735", req);
                    return new AccpacOperationResult(response, Data(("return", new { header = "PO0731" })));
                }
                case "api/POReturn/SyncPOReturns":
                {
                    var req = ParseJsonObject(input);
                    var callMethod = GetString(req, "CallMethod") ?? "SYNC";
                    var previousTs = GetString(req, "Timestamp");
                    var recordLimit = GetInt(req, "RecordLimit", 100);

                    var details = await _companyDetails.GetAsync(user, cancellationToken);
                    using var session = Sage300Session.Open(_configuration, details);
                    var (response, timestamp, headers) = Sage300GenericOps.SyncByHeaderStatus(
                        session,
                        viewIdsCsv: "PO0731,PO0729,PO0735,PO0730,PO0738,PO0732,PO0736,PO0739,PO0799,PO0790",
                        headerViewId: "PO0731",
                        callMethod: callMethod,
                        previousTimestamp: previousTs,
                        recordLimit: recordLimit);

                    return new AccpacOperationResult(response, Data(("sync", new { timestamp, headers })));
                }
                case "api/test/resource1":
                case "api/test/resource2":
                case "api/test/resource3":
                {
                    return new AccpacOperationResult(ProcessOut.Ok("OK"), Data(("route", restRoute)));
                }
                case "api/AtoilCustomerCard/GetCardInfo":
                case "api/Custom/LoadAtoilFunds":
                case "api/Custom/LoadFunds":
                case "api/Custom/PurchaseTransaction":
                {
                    return new AccpacOperationResult(
                        ProcessOut.Fail("9997", "Custom/Atoil endpoints require external integration that is not configured in this project."),
                        Data(("restRoute", restRoute)));
                }
                default:
                    return new AccpacOperationResult(
                        ProcessOut.Fail("9998", $"Operation not implemented yet: {restRoute}"),
                        Data(("restRoute", restRoute), ("input", input)));
            }
        }
        catch (Exception ex)
        {
            return new AccpacOperationResult(
                ProcessOut.Fail("9999", ex.Message),
                Data(("restRoute", restRoute)));
        }
    }

    private static JsonElement ParseJsonObject(object? input)
    {
        if (input is JsonElement je && je.ValueKind == JsonValueKind.Object)
        {
            return je;
        }

        if (input is not string json || string.IsNullOrWhiteSpace(json))
        {
            return JsonSerializer.SerializeToElement(new Dictionary<string, object?>());
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static string? GetString(JsonElement obj, string name)
    {
        if (obj.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (!obj.TryGetProperty(name, out var el))
        {
            return null;
        }

        if (el.ValueKind == JsonValueKind.String)
        {
            return el.GetString();
        }

        return el.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False
            ? el.ToString()
            : null;
    }

    private static int GetInt(JsonElement obj, string name, int defaultValue)
    {
        var s = GetString(obj, name);
        return int.TryParse(s, out var i) ? i : defaultValue;
    }

    private static int? GetNullableInt(JsonElement obj, string name)
    {
        var s = GetString(obj, name);
        return int.TryParse(s, out var i) ? i : null;
    }

    private static ProcessOut UpsertDocument(Sage300Session session, string viewIdsCsv, string headerViewId, string detailViewId, JsonElement payload)
    {
        var tran = session.BeginTransaction();
        try
        {
            var views = new Sage300ViewSet(session, viewIdsCsv, compose: true);
            dynamic header = views.ViewById(headerViewId);
            dynamic detail = views.ViewById(detailViewId);

            var headerObj = payload;
            if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("Header", out var h) && h.ValueKind == JsonValueKind.Object)
            {
                headerObj = h;
            }

            header.Init();
            var keyNames = Sage300GenericOps.GetKeyFieldNames(header);
            foreach (var key in keyNames)
            {
                var val = GetString(headerObj, key);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    Sage300GenericOps.TryPut(header, key, val);
                }
            }

            var exists = false;
            try { exists = (bool)header.Exists; } catch { }
            if (exists)
            {
                header.Read();
            }
            else
            {
                header.RecordGenerate(false);
                foreach (var key in keyNames)
                {
                    var val = GetString(headerObj, key);
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        Sage300GenericOps.TryPut(header, key, val);
                    }
                }
            }

            Sage300GenericOps.PutFromJsonObject(header, headerObj);

            if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("Details", out var details) && details.ValueKind == JsonValueKind.Array)
            {
                foreach (var row in details.EnumerateArray())
                {
                    if (row.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    detail.RecordGenerate(false);
                    Sage300GenericOps.PutFromJsonObject(detail, row);
                    detail.Insert();
                }
            }

            if (exists)
            {
                header.Update();
            }
            else
            {
                header.Insert();
            }

            session.CommitTransaction(tran);
            return ProcessOut.Ok("Document saved.");
        }
        catch (Exception ex)
        {
            try { session.RollbackTransaction(tran); } catch { }
            return ProcessOut.Fail("9999", ex.Message);
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
