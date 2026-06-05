using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArCustomerGroupService : IArCustomerGroupService
{
    private readonly Sage300SingleViewCrud<ARCustomerGroups> _crud;

    public Sage300ArCustomerGroupService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _crud = new Sage300SingleViewCrud<ARCustomerGroups>(
            configuration,
            companyDetails,
            viewId: "AR0025",
            keyField: "IDGRP",
            getKey: e => e.GroupCode000,
            setKey: (e, k) => e.GroupCode000 = k);
    }

    public Task<(ProcessOut Response, ARCustomerGroups CustomerGroup)> CreateOrUpdateAsync(
        ARCustomerGroups customerGroup,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        => _crud.CreateOrUpdateAsync(customerGroup, user, "AR Customer Group", cancellationToken);
}
