using System.Globalization;
using AccpacGraphqlClean.Application;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class AccpacRestEndpointCatalog : IAccpacEndpointCatalog
{
    public IReadOnlyList<AccpacEndpointDefinition> Endpoints { get; } = Build();

    private static IReadOnlyList<AccpacEndpointDefinition> Build()
    {
        var routes = new[]
        {
            "api/AtoilCustomerCard/GetCardInfo",
            "api/APAdjustment/CreateAdjustment",
            "api/APAdjustment/CreateAdjustmentBatch",
            "api/APAdjustment/ReadAdjustment",
            "api/APAdjustment/ReadAdjustmentBatch",
            "api/APAdjustment/SyncAdjustment",
            "api/APInvoice/CreateInvoice",
            "api/APInvoice/CreateInvoiceBatch",
            "api/APInvoice/ReadInvoice",
            "api/APInvoice/ReadInvoiceBatch",
            "api/APInvoice/ReadInvoiceBatchStatus",
            "api/APInvoice/SyncInvoices",
            "api/APPayment/CreatePayment",
            "api/APPayment/CreatePaymentBatch",
            "api/APPayment/ReadPayment",
            "api/APPayment/ReadPaymentBatch",
            "api/APPayment/SyncAPPayments",
            "api/APPaymentCode/CreateAPPaymentCodes",
            "api/APPaymentCode/UpdateAPPaymentCodes",
            "api/APPaymentTerms/CreateAPPaymentTerms",
            "api/APPaymentTerms/UpdateAPPaymentTerms",
            "api/APRecurringPayable/CreateAPRecurringPayables",
            "api/APRecurringPayable/UpdateAPRecurringPayables",
            "api/APRemitToLocation/CreateAPRemitToLocations",
            "api/APRemitToLocation/UpdateAPRemitToLocations",
            "api/APVendor/ConfirmVendorSync",
            "api/APVendor/CreateAPVendor",
            "api/APVendor/ReadAPVendor",
            "api/APVendor/SyncAPVendor",
            "api/APVendorGroup/CreateAPVendorGroups",
            "api/APVendorGroup/UpdateAPVendorGroups",
            "api/ARAdjustment/CreateARAdjustment",
            "api/ARAdjustment/CreateARAdjustmentBatch",
            "api/ARAdjustment/ReadARAdjustment",
            "api/ARAdjustment/ReadARAdjustmentBatch",
            "api/ARAdjustment/SyncARAdjustments",
            "api/ARAdjustment/UpdateAdjustment",
            "api/ARBillingCycle/CreateARBillingCycles",
            "api/ARBillingCycle/ReadARBillingCycles",
            "api/ARBillingCycle/UpdateARBillingCycles",
            "api/ARCustomer/CreateARCustomer",
            "api/ARCustomer/ReadARCustomer",
            "api/ARCustomer/ReadARCustomerBalance",
            "api/ARCustomer/ReadARStatementRun",
            "api/ARCustomer/Read_ARStatementRun",
            "api/ARCustomer/SyncARCustomer",
            "api/ARCustomer/UpdateARCustomer",
            "api/ARCustomerGroup/CreateARCustomerGroupss",
            "api/ARCustomerGroup/UpdateARCustomerGroups",
            "api/ARDocuments/GetAgedBalances",
            "api/ARDocuments/GetDocuments",
            "api/ARInvoice/CreateARInvoice",
            "api/ARInvoice/CreateARInvoiceBatch",
            "api/ARInvoice/ReadARInvoice",
            "api/ARInvoice/ReadARInvoiceBatch",
            "api/ARInvoice/ReadARInvoiceBatchStatus",
            "api/ARInvoice/SyncARInvoices",
            "api/ARInvoice/UpdateARInvoice",
            "api/ARItem/CreateARItems",
            "api/ARItem/UpdateARItems",
            "api/ARReceipt/CreateARReceipt",
            "api/ARReceipt/CreateARReceiptAppendPrepayment",
            "api/ARReceipt/CreateARReceiptBatch",
            "api/ARReceipt/ReadARReceipt",
            "api/ARReceipt/ReadARReceiptBatch",
            "api/ARReceipt/SyncARReceipts",
            "api/ARReceipt/UpdateReceipt",
            "api/ARRefund/CreateARRefund",
            "api/ARRefund/CreateARRefundBatch",
            "api/ARRefund/ReadARRefund",
            "api/ARRefund/ReadARRefundBatch",
            "api/ARRefund/SyncARRefunds",
            "api/ARRefund/UpdateARRefund",
            "api/ARSalesperson/CreateARSalesPersons",
            "api/ARSalesperson/UpdateARSalesPersons",
            "api/ARShipToLocation/CreateARShipToLocation",
            "api/ARShipToLocation/ReadARShipToLocation",
            "api/ARShipToLocation/ReadCustomerShipToLocations",
            "api/ARShipToLocation/SyncARShipToLocation",
            "api/ARShipToLocation/UpdateARShipToLocation",
            "api/ARStatementRun/ReadARStatementRun",
            "api/ARStatementRun/Read_ARStatementRun",
            "api/ARTermsCode/CreateARTermsCodes",
            "api/ARTermsCode/UpdateARTermsCodes",
            "api/Custom/LoadAtoilFunds",
            "api/Custom/LoadFunds",
            "api/Custom/PurchaseTransaction",
            "api/GLAccount/CreateGLAccount",
            "api/GLAccount/SyncGLAccounts",
            "api/GLAccount/SyncGLFiscalSets",
            "api/GLAccount/UpdateGLAccount",
            "api/GLJournalEntry/CreateGLJournalEntry",
            "api/GLRecurringEntries/CreateGLRecurringEntries",
            "api/GLRecurringEntries/UpdateGLRecurringEntries",
            "api/ICAdjustment/CreateICAdjustments",
            "api/ICAdjustment/SyncICAdjustments",
            "api/ICAdjustment/UpdateICAdjustments",
            "api/ICAssembly/CreateICAssembly",
            "api/ICAssembly/UpdateICAssembly",
            "api/ICCategories/CreateICCategory",
            "api/ICCategories/ReadICCategory",
            "api/ICCategories/SyncICCategories",
            "api/ICCategories/UpdateICCategory",
            "api/ICInternalUsage/CreateICInternalUsage",
            "api/ICInternalUsage/UpdateICInternalUsage",
            "api/ICItem/CreateICItem",
            "api/ICItem/ReadICItem",
            "api/ICItem/SyncICItems",
            "api/ICItem/UpdateICItem",
            "api/ICItemPricing/CreateItemPricing",
            "api/ICItemPricing/ReadItemPricing",
            "api/ICItemPricing/SyncItemPricings",
            "api/ICItemPricing/UpdateItemPricing",
            "api/ICLocationDetails/ReadICItemsLocationDetails",
            "api/ICLocationDetails/ReadICLocationDetails",
            "api/ICLocationDetails/SyncICItemsLocationDetails",
            "api/ICLocations/CreateICLocation",
            "api/ICLocations/ReadICLocation",
            "api/ICLocations/SyncICItems",
            "api/ICLocations/UpdateICLocation",
            "api/ICReceipt/CreateICReceipt",
            "api/ICReceipt/SyncICReceipts",
            "api/ICReceipt/UpdateICReceipt",
            "api/ICShipment/CreateICShipment",
            "api/ICShipment/UpdateICShipment",
            "api/ICTransfer/ConfirmSynedICTransfers",
            "api/ICTransfer/CreateICTransfer",
            "api/ICTransfer/SyncIcTransfers",
            "api/ICTransfer/UpdateICTransfer",
            "api/OECreditNote/ConfirmSyncedOECRDRNote",
            "api/OECreditNote/CreateARCreateOECreditNoteCustomer",
            "api/OECreditNote/SyncOEDebitCreditNotes",
            "api/OECreditNote/SyncYJCustomCreditNotes",
            "api/OEDebitNote/CreateOEDebitNote",
            "api/OEInvoice/CreateOEInvoice",
            "api/OEInvoice/ReadOEInvoice",
            "api/OEInvoice/SyncOEInvoices",
            "api/OEInvoice/UpdateOEInvoice",
            "api/OEInvoiceMultiShipment/CreateOEInvoiceByShipmentReference",
            "api/OEInvoiceMultiShipment/CreateOEMultiShipmentInvoice",
            "api/OESalesOrder/CompleteOESalesOrder",
            "api/OESalesOrder/ConfirmSynedOESalesOrder",
            "api/OESalesOrder/CreateOESalesOrder",
            "api/OESalesOrder/ReadOESalesOrder",
            "api/OESalesOrder/SyncOESalesOrders",
            "api/OESalesOrder/UpdateOESalesOrder",
            "api/OEShipment/CreateOEShipment",
            "api/OEShipment/SyncOEShipments",
            "api/OEShipment/UpdateOEShipment",
            "api/PODebitCreditNote/CreatePODebitCreditNote",
            "api/PODebitCreditNote/SyncPODebitCreditNotes",
            "api/PODRCR/CreatePODebitCreditNote",
            "api/POInvoice/CreateInvoice",
            "api/POInvoice/SyncPOInvoices",
            "api/POPurchaseOrder/ConfirmSynedPOList",
            "api/POPurchaseOrder/CreatePurchaseOrder",
            "api/POPurchaseOrder/SyncPurchaseOrders",
            "api/POPurchaseOrder/UpdatePurchaseOrder",
            "api/POReceipt/CreatePOReceipt",
            "api/POReceipt/SyncPOReceipts",
            "api/POReceipt/UpdatePOReceipt",
            "api/PORequisition/CreateRequisition",
            "api/PORequisition/UpdateRequisition",
            "api/POReturn/CreateReturn",
            "api/POReturn/SyncPOReturns",
            "api/test/resource1",
            "api/test/resource2",
            "api/test/resource3"
        };

        var uniqueRoutes = routes.Distinct(StringComparer.OrdinalIgnoreCase);
        var endpoints = new List<AccpacEndpointDefinition>();

        foreach (var route in uniqueRoutes)
        {
            var kind = InferKind(route);
            var fieldName = ToGraphQlFieldName(route);
            endpoints.Add(new AccpacEndpointDefinition(route, kind, fieldName));
        }

        return endpoints.OrderBy(e => e.RestRoute, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static AccpacOperationKind InferKind(string restRoute)
    {
        var lastSegment = restRoute.Split('/').LastOrDefault() ?? restRoute;
        if (lastSegment.StartsWith("Read", StringComparison.OrdinalIgnoreCase))
        {
            return AccpacOperationKind.Query;
        }

        if (lastSegment.StartsWith("Get", StringComparison.OrdinalIgnoreCase))
        {
            return AccpacOperationKind.Query;
        }

        if (restRoute.Contains("/test/resource", StringComparison.OrdinalIgnoreCase))
        {
            return AccpacOperationKind.Query;
        }

        return AccpacOperationKind.Mutation;
    }

    private static string ToGraphQlFieldName(string restRoute)
    {
        var route = restRoute.StartsWith("api/", StringComparison.OrdinalIgnoreCase)
            ? restRoute["api/".Length..]
            : restRoute;

        var segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return "unknown";
        }

        var first = ToCamelCaseSafe(segments[0]);
        var rest = string.Concat(segments.Skip(1).Select(ToPascalCaseSafe));
        var result = first + rest;

        if (string.IsNullOrWhiteSpace(result))
        {
            return "unknown";
        }

        if (char.IsDigit(result[0]))
        {
            return "_" + result;
        }

        return result;
    }

    private static string ToCamelCaseSafe(string value)
    {
        var pascal = ToPascalCaseSafe(value);
        if (pascal.Length == 0)
        {
            return pascal;
        }

        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }

    private static string ToPascalCaseSafe(string value)
    {
        var parts = new string(value.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray())
            .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        return string.Concat(parts.Select(p =>
        {
            if (p.Length == 0)
            {
                return string.Empty;
            }

            var lower = p.ToLowerInvariant();
            return textInfo.ToTitleCase(lower);
        }));
    }
}

