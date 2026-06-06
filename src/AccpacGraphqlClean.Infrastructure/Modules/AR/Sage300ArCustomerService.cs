using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArCustomerService : IArCustomerService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;
    private readonly Sage300SingleViewCrud<ARCustomerBalance> _balanceCrud;

    public Sage300ArCustomerService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
        _balanceCrud = new Sage300SingleViewCrud<ARCustomerBalance>(
            configuration,
            companyDetails,
            viewId: "AR0160",
            keyField: "IDCUST",
            getKey: e => e.CustomerNumber000,
            setKey: (e, k) => e.CustomerNumber000 = k);
    }

    public async Task<(ProcessOut Response, ARCustomers Customer)> CreateOrUpdateAsync(
        ARCustomers customer,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0024,AR0400", compose: true);
            dynamic view = views.ViewById("AR0024");
            view.Init();

            if (string.IsNullOrWhiteSpace(customer.CustomerNumber000))
            {
                session.RollbackTransaction(tran);
                return (ProcessOut.Fail("9999", "AR Customer: key is required."), customer);
            }

            view.Fields.FieldByName("IDCUST").Value = customer.CustomerNumber000;
            var exists = (bool)view.Exists;
            if (exists)
            {
                view.Read();
            }
            else
            {
                view.Init();
                view.Fields.FieldByName("IDCUST").Value = customer.CustomerNumber000;
            }

            SageViewEntityMapper.WriteEntityToView(customer, view);

            if (exists)
            {
                view.Update();
            }
            else
            {
                view.Insert();
            }

            session.CommitTransaction(tran);
            customer.Compid = details.CompanyId;
            var response = ProcessOut.Ok($"Sage 300 AR Customer Number : {customer.CustomerNumber000}", documentNumber: customer.CustomerNumber000);
            return (response, customer);
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

            return (ProcessOut.Fail("9999", ex.Message), customer);
        }
    }

    public async Task<(ProcessOut Response, ARCustomers Customer)> ReadAsync(
        string customerNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0024,AR0400", compose: true);
            dynamic view = views.ViewById("AR0024");
            view.Fields.FieldByName("IDCUST").Value = customerNumber;
            if (!(bool)view.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("0009", "AR Customer: not found!"), new ARCustomers { CustomerNumber000 = customerNumber });
            }

            view.Read();
            var customer = SageViewEntityMapper.ReadEntityFromView<ARCustomers>(view);
            customer.CustomerNumber000 = customerNumber;
            customer.Compid = details.CompanyId;
            session.CommitTransaction(tran);

            var response = ProcessOut.Ok($"Sage 300 AR Customer Number : {customerNumber}", documentNumber: customerNumber);
            return (response, customer);
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

            return (ProcessOut.Fail("9999", ex.Message), new ARCustomers { CustomerNumber000 = customerNumber });
        }
    }

    public Task<(ProcessOut Response, ARCustomerBalance Balance)> ReadBalanceAsync(
        string customerNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        => _balanceCrud.ReadAsync(customerNumber, user, "AR Customer Balance", cancellationToken);
}
