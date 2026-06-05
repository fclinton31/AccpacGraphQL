using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArSalesPersonsService : IArSalesPersonsService
{
    private readonly Sage300SingleViewCrud<ARSalesPersons> _crud;

    public Sage300ArSalesPersonsService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _crud = new Sage300SingleViewCrud<ARSalesPersons>(
            configuration,
            companyDetails,
            viewId: "AR0018",
            keyField: "CODESLSP",
            getKey: e => e.Salesperson000,
            setKey: (e, k) => e.Salesperson000 = k);
    }

    public Task<(ProcessOut Response, ARSalesPersons SalesPerson)> CreateOrUpdateAsync(
        ARSalesPersons salesPerson,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        => _crud.CreateOrUpdateAsync(salesPerson, user, "AR Sales Person", cancellationToken);
}
