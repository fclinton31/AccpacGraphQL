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
        descriptor.Field(f => f.Data).Ignore();
        descriptor.Field("data")
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
                {
                    return null;
                }

                if (TryGetPropertyIgnoreCase(record.Raw, name, out var value))
                {
                    return value;
                }

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
                {
                    return null;
                }

                using var doc = JsonDocument.Parse(record.Raw.GetRawText());
                var root = doc.RootElement;
                var result = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
                foreach (var n in names)
                {
                    if (TryGetPropertyIgnoreCase(root, n, out var v))
                    {
                        result[n] = v;
                    }
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
        {
            return result;
        }

        var props = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in root.EnumerateObject())
        {
            props[p.Name] = p.Value;
        }

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
        {
            return null;
        }

        return new SageRecord(el);
    }

    private static T? DeserializeOrNull<T>(IReadOnlyDictionary<string, JsonElement> props, string key)
    {
        if (!props.TryGetValue(key, out var el))
        {
            return default;
        }

        if (el.ValueKind == JsonValueKind.Null || el.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }

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
        {
            return je;
        }

        if (data is string s)
        {
            using var doc = JsonDocument.Parse(s);
            return doc.RootElement.Clone();
        }

        var json = JsonSerializer.Serialize(data, Json);
        using (var doc = JsonDocument.Parse(json))
        {
            return doc.RootElement.Clone();
        }
    }
}
