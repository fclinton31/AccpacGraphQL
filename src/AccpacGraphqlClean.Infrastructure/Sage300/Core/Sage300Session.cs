using System.Reflection;
using System.Runtime.InteropServices;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300Session : IDisposable
{
    private readonly dynamic _session;
    private readonly object _dbLink;
    private readonly string _progId;

    private Sage300Session(string progId, dynamic session, object dbLink)
    {
        _progId = progId;
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
        if (TryInvokeCom(session, "GetDBLink", new object?[] { dbLinkType, dbLinkFlags }, out object? getDbLink) && getDbLink is not null)
        {
            dbLink = getDbLink;
        }
        else if (TryInvokeCom(session, "GetSessionIntDBLink", new object?[] { dbLinkType, dbLinkFlags }, out object? getLink) && getLink is not null)
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
        return new Sage300Session(progId, session, dbLink);
    }

    private static (string ProgId, dynamic Session) CreateAndInitSession(IReadOnlyList<string> candidates, IConfiguration configuration)
    {
        var appId = configuration["Sage300:AppId"] ?? "XX";
        var appVersion = configuration["Sage300:AppVersion"] ?? "69A";

        static IEnumerable<object?[]> InitArgSets(string appId, string appVersion)
        {
            var programId = appId + "1000";
            return new[]
            {
                new object?[] { "", appId, programId, appVersion },
                new object?[] { "", appId, programId, appVersion, "" },
                new object?[] { "", appId, programId, appVersion, "ENG" },
                new object?[] { "", appId, programId, appVersion, "EN" },
                new object?[] { "", appId, programId, appVersion, null },
                new object?[] { "", appId, programId, appVersion, 0 }
            };
        }

        static IEnumerable<(string Name, object?[] Args)> InitCalls(string appId, string appVersion)
        {
            foreach (var args in InitArgSets(appId, appVersion))
            {
                yield return ("Init", args);
            }

            foreach (var args in InitArgSets(appId, appVersion))
            {
                yield return ("Init2", args);
            }

            foreach (var args in InitArgSets(appId, appVersion))
            {
                yield return ("InitSession", args);
            }
        }

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

                foreach (var (name, args) in InitCalls(appId, appVersion))
                {
                    object? ignoredResult;
                    string? ignoredError;
                    if (TryInvokeComWithResult(session, name, args, out ignoredResult, out ignoredError))
                    {
                        return (progId, session);
                    }
                }
            }
            catch
            {
            }
        }

        var arch = Environment.Is64BitProcess ? "x64" : "x86";
        var diagnostics = new List<string>();

        foreach (var progId in candidates)
        {
            var type = Type.GetTypeFromProgID(progId, throwOnError: false);
            if (type is null)
            {
                diagnostics.Add($"{progId}: ProgID not registered for this process");
                continue;
            }

            try
            {
                var instance = Activator.CreateInstance(type);
                if (instance is null)
                {
                    diagnostics.Add($"{progId}: Activator.CreateInstance returned null");
                    continue;
                }

                var session = (dynamic)instance;
                var firstErrors = new List<string>();
                foreach (var (name, args) in InitCalls(appId, appVersion))
                {
                    object? ignoredResult;
                    string? err;
                    if (TryInvokeComWithResult(session, name, args, out ignoredResult, out err))
                    {
                        diagnostics.Add($"{progId}: Init OK via {name}({args.Length} args)");
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(err))
                    {
                        firstErrors.Add($"{name}({args.Length}): {err}");
                    }

                    if (firstErrors.Count >= 3)
                    {
                        break;
                    }
                }

                if (firstErrors.Count > 0 && !diagnostics.Any(d => d.StartsWith(progId + ":", StringComparison.OrdinalIgnoreCase) && d.Contains("Init OK", StringComparison.OrdinalIgnoreCase)))
                {
                    diagnostics.Add($"{progId}: " + string.Join(" | ", firstErrors));
                }
            }
            catch
            {
            }
        }

        throw new InvalidOperationException(
            $"Sage 300 COM session init method not found on any configured ProgID. Tried: {string.Join(", ", candidates)}. ProcessArch={arch}. Details: {string.Join(" || ", diagnostics)}");
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

            return OrderByKnownPriority(ExpandProgIds(list));
        }

        return OrderByKnownPriority(ExpandProgIds(new[]
        {
            "AccpacCOMAPI.AccpacSession",
            "Accpac.Session",
            "ACCPAC.xapiSession",
            "ACCPAC.ASPSession"
        }));
    }

    private static IReadOnlyList<string> ExpandProgIds(IReadOnlyList<string> progIds)
    {
        var result = new List<string>(progIds.Count * 2);
        foreach (var p in progIds)
        {
            if (string.IsNullOrWhiteSpace(p))
            {
                continue;
            }

            result.Add(p);

            var lastDot = p.LastIndexOf('.');
            if (lastDot > 0 && lastDot < p.Length - 1 && int.TryParse(p[(lastDot + 1)..], out _))
            {
                continue;
            }

            result.Add(p + ".1");
        }

        return result.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static IReadOnlyList<string> OrderByKnownPriority(IReadOnlyList<string> candidates)
    {
        var priority = new[]
        {
            "AccpacCOMAPI.AccpacSession",
            "AccpacCOMAPI.AccpacSession.1",
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
        return InvokeOpenView(_dbLink, _session, _progId, viewId);
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

    private static object InvokeOpenView(object dbLink, object session, string progId, string viewId)
    {
        var errors = new List<string>();

        if (TryInvokeOpenViewViaIDispatch(dbLink, viewId, out var dispatchView, out var dispatchErr) && dispatchView is not null)
        {
            return dispatchView;
        }
        if (!string.IsNullOrWhiteSpace(dispatchErr))
        {
            errors.Add($"DBLink.OpenView(IDispatch): {dispatchErr}");
        }

        if (TryInvokeComWithResult(dbLink, "OpenView", new object?[] { viewId }, out var directView, out var directErr) && directView is not null)
        {
            return directView;
        }
        if (!string.IsNullOrWhiteSpace(directErr))
        {
            errors.Add($"DBLink.OpenView(viewId): {directErr}");
        }

        if (TryParseNumericViewId(viewId, out var numericDirectViewId))
        {
            if (TryInvokeComWithResult(dbLink, "OpenView", new object?[] { numericDirectViewId }, out var directNumericView, out var directNumericErr) && directNumericView is not null)
            {
                return directNumericView;
            }
            if (!string.IsNullOrWhiteSpace(directNumericErr))
            {
                errors.Add($"DBLink.OpenView(intViewId): {directNumericErr}");
            }
        }

        try
        {
            dynamic d = dbLink;
            object? view;
            d.OpenView(out view, viewId);
            if (view is not null)
            {
                return view;
            }
        }
        catch (Exception ex)
        {
            errors.Add($"DBLink.OpenView(out view, viewId): {ex.Message}");
        }

        try
        {
            dynamic d = dbLink;
            object? view = null;
            d.OpenView(ref view, viewId);
            if (view is not null)
            {
                return view;
            }
        }
        catch (Exception ex)
        {
            errors.Add($"DBLink.OpenView(ref view, viewId): {ex.Message}");
        }

        try
        {
            dynamic d = dbLink;
            object? view;
            d.OpenView(viewId, out view);
            if (view is not null)
            {
                return view;
            }
        }
        catch (Exception ex)
        {
            errors.Add($"DBLink.OpenView(viewId, out object): {ex.Message}");
        }

        try
        {
            dynamic d = dbLink;
            object? view = null;
            d.OpenView(viewId, ref view);
            if (view is not null)
            {
                return view;
            }
        }
        catch (Exception ex)
        {
            errors.Add($"DBLink.OpenView(viewId, ref object): {ex.Message}");
        }

        if (TryParseNumericViewId(viewId, out var numericViewId))
        {
            try
            {
                dynamic d = dbLink;
                object? view;
                d.OpenView(out view, numericViewId);
                if (view is not null)
                {
                    return view;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"DBLink.OpenView(out view, intViewId): {ex.Message}");
            }

            try
            {
                dynamic d = dbLink;
                object? view = null;
                d.OpenView(ref view, numericViewId);
                if (view is not null)
                {
                    return view;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"DBLink.OpenView(ref view, intViewId): {ex.Message}");
            }

            try
            {
                dynamic d = dbLink;
                object? view;
                d.OpenView(numericViewId, out view);
                if (view is not null)
                {
                    return view;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"DBLink.OpenView(intViewId, out object): {ex.Message}");
            }

            try
            {
                dynamic d = dbLink;
                object? view = null;
                d.OpenView(numericViewId, ref view);
                if (view is not null)
                {
                    return view;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"DBLink.OpenView(intViewId, ref object): {ex.Message}");
            }
        }

        if (TryInvokeComWithResult(session, "OpenView", new object?[] { viewId }, out var sessionDirectView, out var sessionDirectErr) && sessionDirectView is not null)
        {
            return sessionDirectView;
        }
        if (!string.IsNullOrWhiteSpace(sessionDirectErr))
        {
            errors.Add($"Session.OpenView(viewId): {sessionDirectErr}");
        }

        if (TryParseNumericViewId(viewId, out var numericSessionViewId))
        {
            if (TryInvokeComWithResult(session, "OpenView", new object?[] { numericSessionViewId }, out var sessionDirectNumericView, out var sessionDirectNumericErr) && sessionDirectNumericView is not null)
            {
                return sessionDirectNumericView;
            }
            if (!string.IsNullOrWhiteSpace(sessionDirectNumericErr))
            {
                errors.Add($"Session.OpenView(intViewId): {sessionDirectNumericErr}");
            }
        }

        string? err = null;
        var argSets = new[]
        {
            new object?[] { viewId, null },
            new object?[] { viewId, new object() },
            new object?[] { viewId, DBNull.Value }
        };

        foreach (var args in argSets)
        {
            if (TryInvokeComWithResult(dbLink, "OpenView", args, out _, out err))
            {
                return args.Length > 1 && args[1] is not null ? args[1]! : throw new InvalidOperationException("OpenView returned null.");
            }
            if (!string.IsNullOrWhiteSpace(err))
            {
                errors.Add($"DBLink.OpenView(viewId, out): {err}");
            }
        }

        var reversedArgSets = new[]
        {
            new object?[] { null, viewId },
            new object?[] { new object(), viewId },
            new object?[] { DBNull.Value, viewId }
        };

        foreach (var args in reversedArgSets)
        {
            if (TryInvokeComWithResult(dbLink, "OpenView", args, out _, out err))
            {
                return args[0] ?? throw new InvalidOperationException("OpenView returned null.");
            }
            if (!string.IsNullOrWhiteSpace(err))
            {
                errors.Add($"DBLink.OpenView(out, viewId): {err}");
            }
        }

        foreach (var args in argSets)
        {
            if (TryInvokeComWithResult(session, "OpenView", args, out _, out err))
            {
                return args.Length > 1 && args[1] is not null ? args[1]! : throw new InvalidOperationException("OpenView returned null.");
            }
            if (!string.IsNullOrWhiteSpace(err))
            {
                errors.Add($"Session.OpenView(viewId, out): {err}");
            }
        }

        if (TryInvokeComWithResult(dbLink, "OpenViewEx", argSets[0], out _, out err))
        {
            return argSets[0][1] ?? throw new InvalidOperationException("OpenViewEx returned null.");
        }
        if (!string.IsNullOrWhiteSpace(err))
        {
            errors.Add($"DBLink.OpenViewEx(viewId, out): {err}");
        }

        if (TryInvokeComWithResult(session, "OpenViewEx", argSets[0], out _, out err))
        {
            return argSets[0][1] ?? throw new InvalidOperationException("OpenViewEx returned null.");
        }
        if (!string.IsNullOrWhiteSpace(err))
        {
            errors.Add($"Session.OpenViewEx(viewId, out): {err}");
        }

        var suffix = errors.Count == 0 ? string.Empty : " Errors: " + string.Join(" | ", errors.Distinct());
        throw new InvalidOperationException(
            $"Unable to open view '{viewId}'. ProgID={progId}. DBLinkType={dbLink.GetType().FullName}.{suffix}");
    }

    [ComImport]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDispatch
    {
        void GetTypeInfoCount(out uint pctinfo);
        void GetTypeInfo(uint iTInfo, uint lcid, out IntPtr ppTInfo);
        void GetIDsOfNames(ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);
        void Invoke(int dispIdMember, ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Variant
    {
        [FieldOffset(0)] public ushort vt;
        [FieldOffset(2)] public ushort wReserved1;
        [FieldOffset(4)] public ushort wReserved2;
        [FieldOffset(6)] public ushort wReserved3;
        [FieldOffset(8)] public IntPtr data1;
        [FieldOffset(8)] public long data8;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DispParams
    {
        public IntPtr rgvarg;
        public IntPtr rgdispidNamedArgs;
        public int cArgs;
        public int cNamedArgs;
    }

    private const short DispatchMethod = 1;
    private const ushort VtByRef = 0x4000;

    private static bool TryInvokeOpenViewViaIDispatch(object target, string viewId, out object? view, out string? error)
    {
        view = null;
        error = null;

        try
        {
            if (target is not IDispatch dispatch)
            {
                error = "Target does not implement IDispatch";
                return false;
            }

            var dispId = GetDispId(dispatch, "OpenView");
            if (dispId is null)
            {
                error = "OpenView DISPID not found";
                return false;
            }

            var outStorage = Marshal.AllocCoTaskMem(IntPtr.Size);
            Marshal.WriteIntPtr(outStorage, IntPtr.Zero);

            var bstr = Marshal.StringToBSTR(viewId);
            try
            {
                var argCount = 2;
                var size = Marshal.SizeOf<Variant>();
                var pArgs = Marshal.AllocCoTaskMem(size * argCount);

                try
                {
                    var inVar = new Variant { vt = (ushort)VarEnum.VT_BSTR, data1 = bstr };

                    foreach (var vt in new[]
                             {
                                 (ushort)(VtByRef | (ushort)VarEnum.VT_DISPATCH),
                                 (ushort)(VtByRef | (ushort)VarEnum.VT_UNKNOWN),
                                 (ushort)(VtByRef | (ushort)(Environment.Is64BitProcess ? VarEnum.VT_I8 : VarEnum.VT_I4))
                             })
                    {
                        Marshal.WriteIntPtr(outStorage, IntPtr.Zero);
                        var outVar = new Variant { vt = vt, data1 = outStorage };

                        Marshal.StructureToPtr(outVar, pArgs, fDeleteOld: false);
                        Marshal.StructureToPtr(inVar, IntPtr.Add(pArgs, size), fDeleteOld: false);

                        var dp = new DispParams { cArgs = argCount, cNamedArgs = 0, rgvarg = pArgs, rgdispidNamedArgs = IntPtr.Zero };
                        var pDispParams = Marshal.AllocCoTaskMem(Marshal.SizeOf<DispParams>());
                        try
                        {
                            Marshal.StructureToPtr(dp, pDispParams, fDeleteOld: false);
                            var riid = Guid.Empty;
                            dispatch.Invoke(dispId.Value, ref riid, 0, DispatchMethod, pDispParams, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                            var outPtr = Marshal.ReadIntPtr(outStorage);
                            if (outPtr != IntPtr.Zero)
                            {
                                try
                                {
                                    view = Marshal.GetObjectForIUnknown(outPtr);
                                    _ = Marshal.Release(outPtr);
                                    return true;
                                }
                                catch
                                {
                                    view = outPtr;
                                    return true;
                                }
                            }
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(pDispParams);
                        }
                    }

                    error = "IDispatch.Invoke succeeded but returned null pointer";
                    return false;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pArgs);
                }
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
                Marshal.FreeCoTaskMem(outStorage);
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static int? GetDispId(IDispatch dispatch, string name)
    {
        var riid = Guid.Empty;
        var namePtr = Marshal.StringToCoTaskMemUni(name);
        var names = Marshal.AllocCoTaskMem(IntPtr.Size);
        var dispIds = Marshal.AllocCoTaskMem(sizeof(int));

        try
        {
            Marshal.WriteIntPtr(names, namePtr);
            dispatch.GetIDsOfNames(ref riid, names, 1, 0, dispIds);
            return Marshal.ReadInt32(dispIds);
        }
        catch
        {
            return null;
        }
        finally
        {
            Marshal.FreeCoTaskMem(dispIds);
            Marshal.FreeCoTaskMem(names);
            Marshal.FreeCoTaskMem(namePtr);
        }
    }

    private static bool TryParseNumericViewId(string viewId, out int numericViewId)
    {
        numericViewId = 0;
        if (string.IsNullOrWhiteSpace(viewId) || viewId.Length < 3)
        {
            return false;
        }

        var digits = viewId;
        if (char.IsLetter(viewId[0]) && char.IsLetter(viewId[1]))
        {
            digits = viewId[2..];
        }

        return int.TryParse(digits, out numericViewId);
    }

    private static bool TryInvokeCom(object target, string methodName, object?[] args)
    {
        try
        {
            _ = target.GetType().InvokeMember(
                methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
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
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
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

    private static bool TryInvokeComWithResult(object target, string methodName, object?[] args, out object? result, out string? error)
    {
        try
        {
            result = target.GetType().InvokeMember(
                methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
                binder: null,
                target: target,
                args: args);
            error = null;
            return true;
        }
        catch (TargetInvocationException tie) when (tie.InnerException is not null)
        {
            result = null;
            error = tie.InnerException.Message;
            return false;
        }
        catch (Exception ex)
        {
            result = null;
            error = ex.Message;
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
