using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300SingleViewCrud<TEntity>
    where TEntity : class, new()
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;
    private readonly string _viewId;
    private readonly string _keyField;
    private readonly Func<TEntity, string?> _getKey;
    private readonly Action<TEntity, string> _setKey;

    public Sage300SingleViewCrud(
        IConfiguration configuration,
        ICompanyConnectionDetailsProvider companyDetails,
        string viewId,
        string keyField,
        Func<TEntity, string?> getKey,
        Action<TEntity, string> setKey)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
        _viewId = viewId;
        _keyField = keyField;
        _getKey = getKey;
        _setKey = setKey;
    }

    public async Task<(ProcessOut Response, TEntity Entity)> CreateOrUpdateAsync(
        TEntity entity,
        ClaimsPrincipal user,
        string operationName,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            dynamic view = session.OpenView(_viewId);
            view.Init();

            var key = _getKey(entity);
            if (string.IsNullOrWhiteSpace(key))
            {
                session.RollbackTransaction(tran);
                return (ProcessOut.Fail("9999", $"{operationName}: key is required."), entity);
            }

            view.Fields.FieldByName(_keyField).Value = key;
            var exists = (bool)view.Exists;
            if (exists)
            {
                view.Read();
            }
            else
            {
                view.Init();
                view.Fields.FieldByName(_keyField).Value = key;
            }

            SageViewEntityMapper.WriteEntityToView(entity, view);

            if (exists)
            {
                view.Update();
            }
            else
            {
                view.Insert();
            }

            session.CommitTransaction(tran);

            var docNumber = Convert.ToString(view.Fields.FieldByName(_keyField).Value);
            if (!string.IsNullOrWhiteSpace(docNumber))
            {
                _setKey(entity, docNumber);
            }

            var response = ProcessOut.Ok($"Sage 300 {operationName} : {docNumber}", documentNumber: docNumber);
            return (response, entity);
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

            return (ProcessOut.Fail("9999", ex.Message), entity);
        }
    }

    public async Task<(ProcessOut Response, TEntity Entity)> ReadAsync(
        string key,
        ClaimsPrincipal user,
        string operationName,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            dynamic view = session.OpenView(_viewId);
            view.Fields.FieldByName(_keyField).Value = key;

            var exists = (bool)view.Exists;
            if (!exists)
            {
                session.CommitTransaction(tran);
                var notFound = new TEntity();
                _setKey(notFound, key);
                return (ProcessOut.Fail("0009", $"{operationName}: not found!"), notFound);
            }

            view.Read();
            var entity = SageViewEntityMapper.ReadEntityFromView<TEntity>(view);
            _setKey(entity, key);

            session.CommitTransaction(tran);
            var docNumber = Convert.ToString(view.Fields.FieldByName(_keyField).Value);
            var response = ProcessOut.Ok($"Sage 300 {operationName} : {docNumber}", documentNumber: docNumber);
            return (response, entity);
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

            var notFound = new TEntity();
            _setKey(notFound, key);
            return (ProcessOut.Fail("9999", ex.Message), notFound);
        }
    }
}
