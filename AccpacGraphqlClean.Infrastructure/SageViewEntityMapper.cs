using System.Reflection;
using AccpacGraphqlClean.Domain;

namespace AccpacGraphqlClean.Infrastructure;

public static class SageViewEntityMapper
{
    public static void WriteEntityToView<TEntity>(TEntity entity, dynamic view)
        where TEntity : class
    {
        foreach (var map in SageFieldMap<TEntity>.Fields)
        {
            var value = map.Property.GetValue(entity);
            if (value is null)
            {
                continue;
            }

            if (value is DateTime dt && dt == default)
            {
                continue;
            }

            SetFieldValue(view, map.FieldName, value);
        }
    }

    public static TEntity ReadEntityFromView<TEntity>(dynamic view)
        where TEntity : class, new()
    {
        var entity = new TEntity();
        foreach (var map in SageFieldMap<TEntity>.Fields)
        {
            var raw = GetFieldValue(view, map.FieldName);
            if (raw is null)
            {
                continue;
            }

            var converted = ConvertField(raw, map.Property.PropertyType);
            map.Property.SetValue(entity, converted);
        }

        return entity;
    }

    private static void SetFieldValue(dynamic view, string fieldName, object value)
    {
        view.Fields.FieldByName(fieldName).Value = value;
    }

    private static object? GetFieldValue(dynamic view, string fieldName)
    {
        return view.Fields.FieldByName(fieldName).Value;
    }

    private static object? ConvertField(object raw, Type targetType)
    {
        var nullableUnderlying = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlying is not null)
        {
            targetType = nullableUnderlying;
        }

        if (targetType.IsInstanceOfType(raw))
        {
            return raw;
        }

        if (targetType == typeof(string))
        {
            return Convert.ToString(raw);
        }

        if (targetType == typeof(bool))
        {
            if (raw is bool b)
            {
                return b;
            }

            if (raw is string s)
            {
                if (bool.TryParse(s, out var parsed))
                {
                    return parsed;
                }

                if (s == "1")
                {
                    return true;
                }

                if (s == "0")
                {
                    return false;
                }
            }

            return Convert.ToBoolean(raw);
        }

        if (targetType == typeof(DateTime))
        {
            if (raw is DateTime dt)
            {
                return dt;
            }

            if (raw is string s && DateTime.TryParse(s, out var parsed))
            {
                return parsed;
            }
        }

        return Convert.ChangeType(raw, targetType);
    }
}

internal static class SageFieldMap<TEntity>
{
    internal static IReadOnlyList<(PropertyInfo Property, string FieldName)> Fields { get; } = Build();

    private static IReadOnlyList<(PropertyInfo Property, string FieldName)> Build()
    {
        var props = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var list = new List<(PropertyInfo, string)>();
        foreach (var p in props)
        {
            var attr = p.GetCustomAttribute<SageFieldAttribute>();
            if (attr is null)
            {
                continue;
            }

            if (!p.CanRead || !p.CanWrite)
            {
                continue;
            }

            list.Add((p, attr.FieldName));
        }

        return list;
    }
}

