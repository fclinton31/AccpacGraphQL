using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArBillingCyclesService : IArBillingCyclesService
{
    private readonly Sage300SingleViewCrud<ARBillingCycles> _crud;

    public Sage300ArBillingCyclesService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _crud = new Sage300SingleViewCrud<ARBillingCycles>(
            configuration,
            companyDetails,
            viewId: "AR0014",
            keyField: "IDCYCL",
            getKey: e => e.BillingCycle000,
            setKey: (e, k) => e.BillingCycle000 = k);
    }

    public Task<(ProcessOut Response, ARBillingCycles BillingCycles)> CreateOrUpdateAsync(
        ARBillingCycles billingCycles,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        => _crud.CreateOrUpdateAsync(billingCycles, user, "AR Billing Cycle", cancellationToken);

    public Task<(ProcessOut Response, ARBillingCycles BillingCycles)> ReadAsync(
        string billingCycle,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        => _crud.ReadAsync(billingCycle, user, "AR Billing Cycle", cancellationToken);
}
