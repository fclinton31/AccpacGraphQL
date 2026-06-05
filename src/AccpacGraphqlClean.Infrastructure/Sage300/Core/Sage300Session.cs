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
        var (progId, type) = ResolveComType(candidates);
        if (type is null || string.IsNullOrWhiteSpace(progId))
        {
            throw new InvalidOperationException($"Sage 300 COM ProgID not found. Tried: {string.Join(", ", candidates)}");
        }

        dynamic session = Activator.CreateInstance(type) ?? throw new InvalidOperationException("Unable to create Accpac session.");

        var appId = configuration["Sage300:AppId"] ?? "XX";
        var appVersion = configuration["Sage300:AppVersion"] ?? "69A";

        session.Init("", appId, appId + "1000", appVersion);
        session.Open(details.UserName, details.Password, details.CompanyId, DateTime.Today, 0, "");

        var dbLinkType = int.TryParse(configuration["Sage300:DbLinkTypeCompany"], out var t) ? t : 1;
        var dbLinkFlags = int.TryParse(configuration["Sage300:DbLinkFlagsReadWrite"], out var f) ? f : 2;

        object dbLink = session.OpenDBLink(dbLinkType, dbLinkFlags);
        return new Sage300Session(session, dbLink);
    }

    private static IReadOnlyList<string> BuildProgIdCandidates(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured
                .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        return new[]
        {
            "AccpacCOMAPI.AccpacSession",
            "Accpac.Session",
            "ACCPAC.xapiSession",
            "ACCPAC.ASPSession"
        };
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
        var view = InvokeOpenView(_dbLink, viewId);
        return view;
    }

    public int BeginTransaction()
    {
        var args = new object?[] { 0 };
        Invoke(_dbLink, "TransactionBegin", args);
        return args[0] is int i ? i : 0;
    }

    public void CommitTransaction(int transactionId)
    {
        Invoke(_dbLink, "TransactionCommit", new object?[] { transactionId });
    }

    public void RollbackTransaction(int transactionId)
    {
        Invoke(_dbLink, "TransactionRollback", new object?[] { transactionId });
    }

    private static object InvokeOpenView(object dbLink, string viewId)
    {
        var args = new object?[] { viewId, null };
        Invoke(dbLink, "OpenView", args);
        return args[1] ?? throw new InvalidOperationException("OpenView returned null.");
    }

    private static object? Invoke(object target, string methodName, object?[] args)
    {
        var type = target.GetType();
        var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        if (method is null)
        {
            throw new MissingMethodException(type.FullName, methodName);
        }

        return method.Invoke(target, args);
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
