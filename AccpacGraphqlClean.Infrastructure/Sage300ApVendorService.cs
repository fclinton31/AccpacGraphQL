using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApVendorService : IApVendorService
{
    private readonly Sage300SingleViewCrud<APVendor> _crud;

    public Sage300ApVendorService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _crud = new Sage300SingleViewCrud<APVendor>(
            configuration,
            companyDetails,
            viewId: "AP0015",
            keyField: "VENDORID",
            getKey: v => v.VendorNumber000,
            setKey: (v, key) => v.VendorNumber000 = key);
    }

    public async Task<(ProcessOut Response, APVendor Vendor)> CreateOrUpdateAsync(
        APVendor vendor,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        return await _crud.CreateOrUpdateAsync(vendor, user, operationName: "APVendor", cancellationToken);
    }

    public async Task<(ProcessOut Response, APVendor Vendor)> ReadAsync(
        string vendorNumber,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        return await _crud.ReadAsync(vendorNumber, user, operationName: "APVendor", cancellationToken);
    }
}
