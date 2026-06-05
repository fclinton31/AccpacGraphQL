using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApVendorGroupService : IApVendorGroupService
{
    private readonly Sage300SingleViewCrud<APVendorGroup> _crud;

    public Sage300ApVendorGroupService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _crud = new Sage300SingleViewCrud<APVendorGroup>(
            configuration,
            companyDetails,
            viewId: "AP0016",
            keyField: "GROUPID",
            getKey: g => g.GroupCode000,
            setKey: (g, key) => g.GroupCode000 = key);
    }

    public Task<(ProcessOut Response, APVendorGroup VendorGroup)> CreateOrUpdateAsync(
        APVendorGroup vendorGroup,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        return _crud.CreateOrUpdateAsync(vendorGroup, user, operationName: "APVendorGroups", cancellationToken);
    }
}

