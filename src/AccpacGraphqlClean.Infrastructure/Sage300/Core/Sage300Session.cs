using System.Reflection;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300Session : IDisposable
{
    private readonly dynamic _session;
    private readonly object _dbLink;

    private Sage300Session(dynamic session, object dbLink)
    {
        _session = session;
        _dbLink = dbLink;
    }

    public static Sage300Session Open(IConfiguration configuration, CompanyConnectionDetails details)
    {
        var configured = configuration["Sage300:ComSessionProgId"];
        var candidates = BuildProgIdCandidates(configured);
        var (progId, session) = CreateAndInitSession(candidates, configuration);

        var appId = configuration["Sage300:AppId"] ?? "XX";
        var appVersion = configuration["Sage300:AppVersion"] ?? "69A";

        var flags = int.TryParse(configuration["Sage300:DbLinkFlagsReadWrite"], out var f) ? f : 2;
        if (!TryInvokeCom(session, "Open", new object?[] { details.UserName, details.Password, details.CompanyId, DateTime.Today, flags })
            && !TryInvokeCom(session, "Open", new object?[] { details.UserName, details.Password, details.CompanyId, DateTime.Today, 0, "" }))
        {
            throw new InvalidOperationException($"Unable to open Sage 300 session (Open method not found for known signatures). ProgID={progId}");
        }

        var dbLinkType = int.TryParse(configuration["Sage300:DbLinkTypeCompany"], out var t) ? t : 1;
        var dbLinkFlags = flags;

        object? dbLink = null;
        if (TryInvokeCom(session, "GetSessionIntDBLink", new object?[] { dbLinkType, dbLinkFlags }, out object? getLink) && getLink is not null)
        {
            dbLink = getLink;
        }
        else if (TryInvokeCom(session, "OpenDBLink", new object?[] { dbLinkType, dbLinkFlags }, out object? openLink) && openLink is not null)
        {
            dbLink = openLink;
        }

        if (dbLink is null)
        {
            throw new InvalidOperationException("Unable to open Sage 300 DBLink (GetSessionIntDBLink/OpenDBLink not available).");
        }
        return new Sage300Session(session, dbLink);
    }

    private static (string ProgId, dynamic Session) CreateAndInitSession(IReadOnlyList<string> candidates, IConfiguration configuration)
    {
        var appId = configuration["Sage300:AppId"] ?? "XX";
        var appVersion = configuration["Sage300:AppVersion"] ?? "69A";

        foreach (var progId in candidates)
        {
            var type = Type.GetTypeFromProgID(progId, throwOnError: false);
            if (type is null)
            {
                continue;
            }

            object? instance = null;
            try
            {
                instance = Activator.CreateInstance(type);
                if (instance is null)
                {
                    continue;
                }

                var session = (dynamic)instance;

                if (TryInvokeCom(session, "Init", new object?[] { "", appId, appId + "1000", appVersion })
                    || TryInvokeCom(session, "Init", new object?[] { "", appId, appId + "1000", appVersion, "" })
                    || TryInvokeCom(session, "Init2", new object?[] { "", appId, appId + "1000", appVersion })
                    || TryInvokeCom(session, "InitSession", new object?[] { "", appId, appId + "1000", appVersion }))
                {
                    return (progId, session);
                }
            }
            catch
            {
            }
        }

        throw new InvalidOperationException(
            $"Sage 300 COM session init method not found on any configured ProgID. Tried: {string.Join(", ", candidates)}");
    }

    private static IReadOnlyList<string> BuildProgIdCandidates(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            var list = configured
                .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return OrderByKnownPriority(list);
        }

        return OrderByKnownPriority(new[]
        {
            "AccpacCOMAPI.AccpacSession",
            "Accpac.Session",
            "ACCPAC.xapiSession",
            "ACCPAC.ASPSession"
        });
    }

    private static IReadOnlyList<string> OrderByKnownPriority(IReadOnlyList<string> candidates)
    {
        var priority = new[]
        {
            "AccpacCOMAPI.AccpacSession",
            "ACCPAC.xapiSession",
            "Accpac.Session",
            "ACCPAC.ASPSession"
        };

        var priorityIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < priority.Length; i++)
        {
            priorityIndex[priority[i]] = i;
        }

        return candidates
            .OrderBy(c => priorityIndex.TryGetValue(c, out var idx) ? idx : int.MaxValue)
            .ThenBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static (string? ProgId, Type? Type) ResolveComType(IReadOnlyList<string> candidates)
    {
        foreach (var progId in candidates)
        {
            var type = Type.GetTypeFromProgID(progId, throwOnError: false);
            if (type is not null)
            {
                return (progId, type);
            }
        }

        return (null, null);
    }

    public dynamic OpenView(string viewId)
    {
        return InvokeOpenView(_dbLink, _session, viewId);
    }

    public int BeginTransaction()
    {
        var args = new object?[] { 0 };
        if (TryInvokeCom(_dbLink, "TransactionBegin", args))
        {
            return args[0] is int i ? i : 0;
        }

        if (TryInvokeCom(_session, "TransactionBegin", args))
        {
            return args[0] is int i ? i : 0;
        }

        if (TryInvokeCom(_dbLink, "BeginTransaction", Array.Empty<object?>(), out var result)
            || TryInvokeCom(_session, "BeginTransaction", Array.Empty<object?>(), out result))
        {
            return result is int i ? i : 0;
        }

        throw new MissingMethodException("Unable to begin transaction. Tried TransactionBegin/BeginTransaction on DBLink and Session.");
    }

    public void CommitTransaction(int transactionId)
    {
        if (TryInvokeCom(_dbLink, "TransactionCommit", new object?[] { transactionId })
            || TryInvokeCom(_session, "TransactionCommit", new object?[] { transactionId })
            || TryInvokeCom(_dbLink, "CommitTransaction", new object?[] { transactionId })
            || TryInvokeCom(_session, "CommitTransaction", new object?[] { transactionId })
            || TryInvokeCom(_dbLink, "CommitTransaction", Array.Empty<object?>())
            || TryInvokeCom(_session, "CommitTransaction", Array.Empty<object?>()))
        {
            return;
        }

        throw new MissingMethodException("Unable to commit transaction. Tried TransactionCommit/CommitTransaction on DBLink and Session.");
    }

    public void RollbackTransaction(int transactionId)
    {
        if (TryInvokeCom(_dbLink, "TransactionRollback", new object?[] { transactionId })
            || TryInvokeCom(_session, "TransactionRollback", new object?[] { transactionId })
            || TryInvokeCom(_dbLink, "RollbackTransaction", new object?[] { transactionId })
            || TryInvokeCom(_session, "RollbackTransaction", new object?[] { transactionId })
            || TryInvokeCom(_dbLink, "RollbackTransaction", Array.Empty<object?>())
            || TryInvokeCom(_session, "RollbackTransaction", Array.Empty<object?>()))
        {
            return;
        }

        throw new MissingMethodException("Unable to rollback transaction. Tried TransactionRollback/RollbackTransaction on DBLink and Session.");
    }

    private static object InvokeOpenView(object dbLink, object session, string viewId)
    {
        object? direct;
        if (TryInvokeCom(dbLink, "OpenView", new object?[] { viewId }, out direct) && direct is not null)
        {
            return direct;
        }

        if (TryInvokeCom(session, "OpenView", new object?[] { viewId }, out direct) && direct is not null)
        {
            return direct;
        }

        var argSets = new[]
        {
            new object?[] { viewId, null },
            new object?[] { viewId, new object() },
            new object?[] { viewId, DBNull.Value }
        };

        foreach (var args in argSets)
        {
            if (TryInvokeCom(dbLink, "OpenView", args))
            {
                return args.Length > 1 && args[1] is not null ? args[1]! : throw new InvalidOperationException("OpenView returned null.");
            }
        }

        foreach (var args in argSets)
        {
            if (TryInvokeCom(session, "OpenView", args))
            {
                return args.Length > 1 && args[1] is not null ? args[1]! : throw new InvalidOperationException("OpenView returned null.");
            }
        }

        if (TryInvokeCom(dbLink, "OpenViewEx", argSets[0]))
        {
            return argSets[0][1] ?? throw new InvalidOperationException("OpenViewEx returned null.");
        }

        if (TryInvokeCom(session, "OpenViewEx", argSets[0]))
        {
            return argSets[0][1] ?? throw new InvalidOperationException("OpenViewEx returned null.");
        }

        throw new MissingMethodException("Unable to open view. Tried OpenView/OpenViewEx on DBLink and Session.");
    }

    private static bool TryInvokeCom(object target, string methodName, object?[] args)
    {
        try
        {
            _ = target.GetType().InvokeMember(
                methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                target: target,
                args: args);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryInvokeCom(object target, string methodName, object?[] args, out object? result)
    {
        try
        {
            result = target.GetType().InvokeMember(
                methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                target: target,
                args: args);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public void Dispose()
    {
        try
        {
            if (_session is not null)
            {
                try
                {
                    if ((bool)_session.IsOpened)
                    {
                        _session.Close();
                    }
                }
                catch
                {
                }
            }
        }
        finally
        {
        }
    }
}
