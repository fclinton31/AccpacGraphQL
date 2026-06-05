using System.Globalization;
using System.Text.Json;
using AccpacGraphqlClean.Domain;

namespace AccpacGraphqlClean.Infrastructure;

internal static class Sage300GenericOps
{
    public static Dictionary<string, string?> DumpFields(dynamic view)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var count = (int)view.Fields.Count;
            for (var i = 0; i < count; i++)
            {
                dynamic field = view.Fields.Item(i);
                var name = Convert.ToString(field.Name);
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                object? value = null;
                try
                {
                    value = field.Value;
                }
                catch
                {
                }

                result[name] = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
        }
        catch
        {
        }

        return result;
    }

    public static string[] GetKeyFieldNames(dynamic view)
    {
        try
        {
            dynamic key = view.Keys.Item(0);
            var count = (int)key.FieldCount;
            var names = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                var name = Convert.ToString(key.Field(i).Name);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }

            return names.ToArray();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public static bool TryPut(dynamic view, string fieldName, object? value)
    {
        if (value is null)
        {
            return false;
        }

        try
        {
            view.Fields.FieldByName(fieldName).Value = value;
            return true;
        }
        catch
        {
            try
            {
                view.Fields.FieldByName(fieldName).PutWithoutVerification(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static void PutFromJsonObject(dynamic view, JsonElement obj, IReadOnlySet<string>? skip = null)
    {
        foreach (var prop in obj.EnumerateObject())
        {
            if (skip is not null && skip.Contains(prop.Name))
            {
                continue;
            }

            if (prop.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                continue;
            }

            if (prop.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
            {
                continue;
            }

            object? value = prop.Value.ValueKind switch
            {
                JsonValueKind.String => GetStringValue(prop.Value),
                JsonValueKind.Number => GetNumberValue(prop.Value),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            };

            TryPut(view, prop.Name, value);
        }
    }

    public static ProcessOut UpsertSingle(
        Sage300Session session,
        string viewIdsCsv,
        string primaryViewId,
        IReadOnlyDictionary<string, string> keyFieldValues,
        JsonElement payload,
        string operationName)
    {
        var tran = session.BeginTransaction();
        try
        {
            var views = new Sage300ViewSet(session, viewIdsCsv, compose: true);
            dynamic v = views.ViewById(primaryViewId);

            v.Init();
            foreach (var kvp in keyFieldValues)
            {
                TryPut(v, kvp.Key, kvp.Value);
            }

            var exists = false;
            try
            {
                exists = (bool)v.Exists;
            }
            catch
            {
            }

            if (exists)
            {
                v.Read();
            }
            else
            {
                v.RecordGenerate(false);
                foreach (var kvp in keyFieldValues)
                {
                    TryPut(v, kvp.Key, kvp.Value);
                }
            }

            PutFromJsonObject(v, payload, skip: new HashSet<string>(keyFieldValues.Keys, StringComparer.OrdinalIgnoreCase));

            if (exists)
            {
                v.Update();
            }
            else
            {
                v.Insert();
            }

            session.CommitTransaction(tran);
            return ProcessOut.Ok($"{operationName} saved.");
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return ProcessOut.Fail("9999", ex.Message);
        }
    }

    public static (ProcessOut Response, Dictionary<string, string?> Record) ReadSingle(
        Sage300Session session,
        string viewIdsCsv,
        string primaryViewId,
        IReadOnlyDictionary<string, string> keyFieldValues,
        string operationName)
    {
        try
        {
            var views = new Sage300ViewSet(session, viewIdsCsv, compose: true);
            dynamic v = views.ViewById(primaryViewId);
            v.Init();
            foreach (var kvp in keyFieldValues)
            {
                TryPut(v, kvp.Key, kvp.Value);
            }

            var exists = false;
            try
            {
                exists = (bool)v.Exists;
            }
            catch
            {
            }

            if (!exists)
            {
                return (ProcessOut.Fail("0009", $"{operationName} not found."), new Dictionary<string, string?>());
            }

            v.Read();
            return (ProcessOut.Ok($"{operationName} read."), DumpFields(v));
        }
        catch (Exception ex)
        {
            return (ProcessOut.Fail("9999", ex.Message), new Dictionary<string, string?>());
        }
    }

    public static (ProcessOut Response, string Timestamp, List<Dictionary<string, string?>> Records) SyncFromYh(
        Sage300Session session,
        string yhViewId,
        string module,
        string txnType,
        string targetViewIdsCsv,
        string targetPrimaryViewId,
        string callMethod,
        string? previousTimestamp,
        int recordLimit,
        int? systemId)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var tran = session.BeginTransaction();
        try
        {
            var yhViews = new Sage300ViewSet(session, yhViewId, compose: false);
            dynamic yh = yhViews.ViewById(yhViewId);

            var timeStampField = systemId is >= 1 and <= 9 ? $"TIMESTMP{systemId}" : "TIMESTAMP";
            var statusField = systemId is >= 1 and <= 9 ? $"YHSTATUS{systemId}" : "YHSTATUS";

            if (string.Equals(callMethod, "REFRESH", StringComparison.OrdinalIgnoreCase))
            {
                yh.Browse($"MODULE = \"{module}\" AND TXNTYPE = \"{txnType}\"", true);
                while (yh.Fetch())
                {
                    TryPut(yh, statusField, 0);
                    yh.Update();
                }

                previousTimestamp = "";
            }

            if (string.Equals(callMethod, "SYNC", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(previousTimestamp))
            {
                yh.Browse($"MODULE = \"{module}\" AND TXNTYPE = \"{txnType}\" AND {timeStampField} = \"{previousTimestamp}\"", true);
                while (yh.Fetch())
                {
                    TryPut(yh, statusField, 1);
                    yh.Update();
                }
            }

            var targetViews = new Sage300ViewSet(session, targetViewIdsCsv, compose: true);
            dynamic target = targetViews.ViewById(targetPrimaryViewId);
            var keyFields = GetKeyFieldNames(target);

            var records = new List<Dictionary<string, string?>>();
            yh.Browse($"MODULE = \"{module}\" AND TXNTYPE = \"{txnType}\" AND {statusField} = 0", true);
            while (yh.Fetch())
            {
                var masterKey = Convert.ToString(yh.Fields.FieldByName("MASTERKEY").Value) ?? "";
                var parts = masterKey.Split('~');

                target.Init();
                for (var i = 0; i < keyFields.Length && i < parts.Length; i++)
                {
                    TryPut(target, keyFields[i], parts[i]);
                }

                var exists = false;
                try
                {
                    exists = (bool)target.Exists;
                }
                catch
                {
                }

                if (exists)
                {
                    target.Read();
                    records.Add(DumpFields(target));
                }

                TryPut(yh, timeStampField, timestamp);
                yh.Update();

                if (recordLimit > 0 && records.Count >= recordLimit)
                {
                    break;
                }
            }

            session.CommitTransaction(tran);
            return (ProcessOut.Ok("Sync completed.", timestamp), timestamp, records);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), timestamp, new List<Dictionary<string, string?>>());
        }
    }

    public static (ProcessOut Response, string Timestamp, List<Dictionary<string, object?>> Headers) SyncByHeaderStatus(
        Sage300Session session,
        string viewIdsCsv,
        string headerViewId,
        string callMethod,
        string? previousTimestamp,
        int recordLimit)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var tran = session.BeginTransaction();
        try
        {
            var views = new Sage300ViewSet(session, viewIdsCsv, compose: true);
            dynamic header = views.ViewById(headerViewId);

            if (string.Equals(callMethod, "SYNC", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(previousTimestamp))
                {
                    header.Browse($"TIMESTAMP = \"{previousTimestamp}\"", true);
                    while (header.Fetch())
                    {
                        TryPut(header, "YHSTATUS", 1);
                        header.Update();
                    }
                }

                header.Browse("YHSTATUS = 0", true);
            }
            else if (string.Equals(callMethod, "UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                header.Browse($"TIMESTAMP = \"{previousTimestamp}\"", true);
            }
            else
            {
                throw new InvalidOperationException("Incorrect call method!");
            }

            var list = new List<Dictionary<string, object?>>();
            while (header.Fetch())
            {
                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["header"] = DumpFields(header)
                };

                TryPut(header, "TIMESTAMP", timestamp);
                header.Update();
                list.Add(row);

                if (recordLimit > 0 && list.Count >= recordLimit)
                {
                    break;
                }
            }

            session.CommitTransaction(tran);
            return (ProcessOut.Ok("Sync completed.", timestamp), timestamp, list);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), timestamp, new List<Dictionary<string, object?>>());
        }
    }

    private static object? GetStringValue(JsonElement el)
    {
        var s = el.GetString();
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
        {
            return dt;
        }

        return s;
    }

    private static object? GetNumberValue(JsonElement el)
    {
        if (el.TryGetInt32(out var i))
        {
            return i;
        }

        if (el.TryGetInt64(out var l))
        {
            return l;
        }

        if (el.TryGetDecimal(out var d))
        {
            return d;
        }

        if (el.TryGetDouble(out var dbl))
        {
            return dbl;
        }

        return null;
    }
}

