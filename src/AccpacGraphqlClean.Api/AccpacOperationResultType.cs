using AccpacGraphqlClean.Domain;
using HotChocolate.Types;
using System.Text.Json;

namespace AccpacGraphqlClean.Api;

public sealed class AccpacOperationResultType : ObjectType<AccpacOperationResult>
{
    protected override void Configure(IObjectTypeDescriptor<AccpacOperationResult> descriptor)
    {
        descriptor.Name("AccpacOperationResult");
        descriptor.Field(f => f.Response).Type<NonNullType<ProcessOutType>>();
        descriptor.Field(f => f.Data)
            .Type<NonNullType<AccpacDataType>>()
            .Resolve(ctx => AccpacDataMapper.Map(ctx.Parent<AccpacOperationResult>().Data));
    }
}

public sealed class AccpacDataType : ObjectType<AccpacData>
{
    protected override void Configure(IObjectTypeDescriptor<AccpacData> descriptor)
    {
        descriptor.Name("AccpacData");
        descriptor.Field(f => f.Raw).Type<AnyType>();

        // AP
        descriptor.Field(f => f.Vendor).Type<ObjectType<APVendor>>();
        descriptor.Field(f => f.VendorGroup).Type<ObjectType<APVendorGroup>>();
        descriptor.Field(f => f.PaymentCode).Type<ObjectType<APPaymentCodes>>();
        descriptor.Field(f => f.PaymentTerms).Type<ObjectType<APPaymentTerms>>();
        descriptor.Field(f => f.RemitToLocations).Type<ObjectType<APRemitToLocations>>();
        descriptor.Field(f => f.RecurringPayables).Type<ObjectType<APRecurringPayables>>();
        descriptor.Field(f => f.Invoices).Type<ObjectType<APInvoices>>();
        descriptor.Field(f => f.InvoiceBatch).Type<ObjectType<APInvoiceBatch>>();
        descriptor.Field(f => f.Payment).Type<ObjectType<APPayment>>();
        descriptor.Field(f => f.PaymentBatch).Type<ObjectType<APPaymentBatch>>();
        descriptor.Field(f => f.Adjustment).Type<ObjectType<APAdjustments>>();
        descriptor.Field(f => f.AdjustmentBatch).Type<ObjectType<APAdjustmentBatch>>();

        // AR
        descriptor.Field(f => f.Customer).Type<ObjectType<ARCustomers>>();
        descriptor.Field(f => f.CustomerBalance).Type<ObjectType<ARCustomerBalance>>();
        descriptor.Field(f => f.CustomerGroup).Type<ObjectType<ARCustomerGroups>>();
        descriptor.Field(f => f.TermsCodes).Type<ObjectType<ARTermsCodes>>();
        descriptor.Field(f => f.ShipToLocation).Type<ObjectType<ARShipToLocations>>();
        descriptor.Field(f => f.ShipToLocations).Type<ListType<ObjectType<ARShipToLocations>>>();
        descriptor.Field(f => f.BillingCycles).Type<ObjectType<ARBillingCycles>>();
        descriptor.Field(f => f.SalesPerson).Type<ObjectType<ARSalesPersons>>();
        descriptor.Field(f => f.ArItems).Type<ObjectType<ARItems>>();
        descriptor.Field(f => f.ArInvoice).Type<ObjectType<ARInvoice>>();
        descriptor.Field(f => f.ArInvoiceBatch).Type<ObjectType<ARInvoiceBatch>>();
        descriptor.Field(f => f.ArAdjustment).Type<ObjectType<ARAdjustment>>();
        descriptor.Field(f => f.ArAdjustmentBatch).Type<ObjectType<ARAdjustmentBatch>>();
        descriptor.Field(f => f.ArReceipt).Type<ObjectType<ARReceipt>>();
        descriptor.Field(f => f.ArReceiptBatch).Type<ObjectType<ARReceiptBatch>>();
        descriptor.Field(f => f.ArRefund).Type<ObjectType<ARRefund>>();
        descriptor.Field(f => f.ArRefundBatch).Type<ObjectType<ARRefundBatch>>();
        descriptor.Field(f => f.AgedAnalysis).Type<ObjectType<ARAgedAnalysis>>();
        descriptor.Field(f => f.StatementRun).Type<ObjectType<ARStatementRun>>();

        // AR Open Documents
        descriptor.Field(f => f.OpenInvoices).Type<ObjectType<AROpenInvoices>>();

        // SageRecords
        descriptor.Field(f => f.Account).Type<SageRecordType>();
        descriptor.Field(f => f.JournalEntry).Type<SageRecordType>();
        descriptor.Field(f => f.RecurringEntry).Type<SageRecordType>();
        descriptor.Field(f => f.Category).Type<SageRecordType>();
        descriptor.Field(f => f.Item).Type<SageRecordType>();
        descriptor.Field(f => f.Pricing).Type<SageRecordType>();
        descriptor.Field(f => f.Location).Type<SageRecordType>();
        descriptor.Field(f => f.LocationDetails).Type<SageRecordType>();
        descriptor.Field(f => f.Receipt).Type<SageRecordType>();
        descriptor.Field(f => f.Shipment).Type<SageRecordType>();
        descriptor.Field(f => f.Transfer).Type<SageRecordType>();
        descriptor.Field(f => f.Assembly).Type<SageRecordType>();
        descriptor.Field(f => f.InternalUsage).Type<SageRecordType>();
        descriptor.Field(f => f.PurchaseOrder).Type<SageRecordType>();
        descriptor.Field(f => f.Requisition).Type<SageRecordType>();
        descriptor.Field(f => f.Return).Type<SageRecordType>();
        descriptor.Field(f => f.SalesOrder).Type<SageRecordType>();
        descriptor.Field(f => f.Invoice).Type<SageRecordType>();
        descriptor.Field(f => f.CreditDebitNote).Type<SageRecordType>();
        descriptor.Field(f => f.DebitCreditNote).Type<SageRecordType>();
        descriptor.Field(f => f.Status).Type<SageRecordType>();
        descriptor.Field(f => f.Sync).Type<SageRecordType>();
        descriptor.Field(f => f.Records).Type<SageRecordType>();
        descriptor.Field(f => f.Timestamp).Type<SageRecordType>();
        descriptor.Field(f => f.Route).Type<SageRecordType>();
        descriptor.Field(f => f.RestRoute).Type<SageRecordType>();
    }
}

public sealed class SageRecordType : ObjectType<SageRecord>
{
    protected override void Configure(IObjectTypeDescriptor<SageRecord> descriptor)
    {
        descriptor.Name("SageRecord");
        descriptor.Field(f => f.Raw).Type<AnyType>();

        descriptor.Field("get")
            .Argument("name", a => a.Type<NonNullType<StringType>>())
            .Type<AnyType>()
            .Resolve(ctx =>
            {
                var record = ctx.Parent<SageRecord>();
                var name = ctx.ArgumentValue<string>("name");
                if (record.Raw.ValueKind != JsonValueKind.Object)
                    return null;
                if (TryGetPropertyIgnoreCase(record.Raw, name, out var value))
                    return value;
                return null;
            });

        descriptor.Field("pick")
            .Argument("names", a => a.Type<NonNullType<ListType<NonNullType<StringType>>>>())
            .Type<AnyType>()
            .Resolve(ctx =>
            {
                var record = ctx.Parent<SageRecord>();
                var names = ctx.ArgumentValue<IReadOnlyList<string>>("names");
                if (record.Raw.ValueKind != JsonValueKind.Object)
                    return null;
                using var doc = JsonDocument.Parse(record.Raw.GetRawText());
                var root = doc.RootElement;
                var result = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
                foreach (var n in names)
                {
                    if (TryGetPropertyIgnoreCase(root, n, out var v))
                        result[n] = v;
                }
                return result;
            });
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement obj, string name, out JsonElement value)
    {
        foreach (var p in obj.EnumerateObject())
        {
            if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = p.Value;
                return true;
            }
        }
        value = default;
        return false;
    }
}

internal static class AccpacDataMapper
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static AccpacData Map(object? data)
    {
        var root = ExtractJsonElement(data);
        var result = new AccpacData { Raw = root };
        if (root.ValueKind != JsonValueKind.Object)
            return result;

        var props = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in root.EnumerateObject())
            props[p.Name] = p.Value;

        // AP
        result.Vendor = DeserializeOrNull<APVendor>(props, "vendor");
        result.VendorGroup = DeserializeOrNull<APVendorGroup>(props, "vendorGroup");
        result.PaymentCode = DeserializeOrNull<APPaymentCodes>(props, "paymentCode");
        result.PaymentTerms = DeserializeOrNull<APPaymentTerms>(props, "paymentTerms");
        result.RemitToLocations = DeserializeOrNull<APRemitToLocations>(props, "remitToLocations");
        result.RecurringPayables = DeserializeOrNull<APRecurringPayables>(props, "recurringPayables");
        result.Invoices = DeserializeOrNull<APInvoices>(props, "invoices");
        result.InvoiceBatch = DeserializeOrNull<APInvoiceBatch>(props, "InvoiceBatch");
        result.Payment = DeserializeOrNull<APPayment>(props, "payment");
        result.PaymentBatch = DeserializeOrNull<APPaymentBatch>(props, "paymentBatch");
        result.Adjustment = DeserializeOrNull<APAdjustments>(props, "adjustment");
        result.AdjustmentBatch = DeserializeOrNull<APAdjustmentBatch>(props, "adjustmentBatch");

        // AR
        result.Customer = DeserializeOrNull<ARCustomers>(props, "customer");
        result.CustomerBalance = DeserializeOrNull<ARCustomerBalance>(props, "customerBalance");
        result.CustomerGroup = DeserializeOrNull<ARCustomerGroups>(props, "customerGroup");
        result.TermsCodes = DeserializeOrNull<ARTermsCodes>(props, "termsCodes");
        result.ShipToLocation = DeserializeOrNull<ARShipToLocations>(props, "shipToLocation");
        result.ShipToLocations = DeserializeOrNull<List<ARShipToLocations>>(props, "shipToLocations");
        result.BillingCycles = DeserializeOrNull<ARBillingCycles>(props, "billingCycles");
        result.SalesPerson = DeserializeOrNull<ARSalesPersons>(props, "salesPerson");
        result.ArItems = DeserializeOrNull<ARItems>(props, "arItems");
        result.ArInvoice = DeserializeOrNull<ARInvoice>(props, "arInvoice");
        result.ArInvoiceBatch = DeserializeOrNull<ARInvoiceBatch>(props, "arInvoiceBatch");
        result.ArAdjustment = DeserializeOrNull<ARAdjustment>(props, "arAdjustment");
        result.ArAdjustmentBatch = DeserializeOrNull<ARAdjustmentBatch>(props, "arAdjustmentBatch");
        result.ArReceipt = DeserializeOrNull<ARReceipt>(props, "arReceipt");
        result.ArReceiptBatch = DeserializeOrNull<ARReceiptBatch>(props, "arReceiptBatch");
        result.ArRefund = DeserializeOrNull<ARRefund>(props, "arRefund");
        result.ArRefundBatch = DeserializeOrNull<ARRefundBatch>(props, "arRefundBatch");
        result.AgedAnalysis = DeserializeOrNull<ARAgedAnalysis>(props, "agedAnalysis");
        result.StatementRun = DeserializeOrNull<ARStatementRun>(props, "statementRun");

        // AR Open Documents - mapped from "invoices" key
        result.OpenInvoices = DeserializeOrNull<AROpenInvoices>(props, "invoices");

        // SageRecords
        result.Account = WrapRecord(props, "account");
        result.JournalEntry = WrapRecord(props, "journalEntry");
        result.RecurringEntry = WrapRecord(props, "recurringEntry");
        result.Category = WrapRecord(props, "category");
        result.Item = WrapRecord(props, "item");
        result.Pricing = WrapRecord(props, "pricing");
        result.Location = WrapRecord(props, "location");
        result.LocationDetails = WrapRecord(props, "locationDetails");
        result.Receipt = WrapRecord(props, "receipt");
        result.Shipment = WrapRecord(props, "shipment");
        result.Transfer = WrapRecord(props, "transfer");
        result.Assembly = WrapRecord(props, "assembly");
        result.InternalUsage = WrapRecord(props, "internalUsage");
        result.PurchaseOrder = WrapRecord(props, "purchaseOrder");
        result.Requisition = WrapRecord(props, "requisition");
        result.Return = WrapRecord(props, "return");
        result.SalesOrder = WrapRecord(props, "salesOrder");
        result.Invoice = WrapRecord(props, "invoice");
        result.CreditDebitNote = WrapRecord(props, "creditDebitNote");
        result.DebitCreditNote = WrapRecord(props, "debitCreditNote");
        result.Status = WrapRecord(props, "status");
        result.Sync = WrapRecord(props, "sync");
        result.Records = WrapRecord(props, "records");
        result.Timestamp = WrapRecord(props, "timestamp");
        result.Route = WrapRecord(props, "route");
        result.RestRoute = WrapRecord(props, "restRoute");

        return result;
    }

    private static SageRecord? WrapRecord(IReadOnlyDictionary<string, JsonElement> props, string key)
    {
        if (!props.TryGetValue(key, out var el))
            return null;
        return new SageRecord(el);
    }

    private static T? DeserializeOrNull<T>(IReadOnlyDictionary<string, JsonElement> props, string key)
    {
        if (!props.TryGetValue(key, out var el))
            return default;
        if (el.ValueKind == JsonValueKind.Null || el.ValueKind == JsonValueKind.Undefined)
            return default;
        return JsonSerializer.Deserialize<T>(el.GetRawText(), Json);
    }

    private static JsonElement ExtractJsonElement(object? data)
    {
        if (data is null)
        {
            using var doc = JsonDocument.Parse("{}");
            return doc.RootElement.Clone();
        }
        if (data is JsonElement je)
            return je;
        if (data is string s)
        {
            using var doc = JsonDocument.Parse(s);
            return doc.RootElement.Clone();
        }
        var json = JsonSerializer.Serialize(data, Json);
        using (var doc = JsonDocument.Parse(json))
            return doc.RootElement.Clone();
    }
}