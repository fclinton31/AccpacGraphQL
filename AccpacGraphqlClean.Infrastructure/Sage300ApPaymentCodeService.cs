using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApPaymentCodeService : IApPaymentCodeService
{
    private readonly Sage300SingleViewCrud<APPaymentCodes> _crud;

    public Sage300ApPaymentCodeService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _crud = new Sage300SingleViewCrud<APPaymentCodes>(
            configuration,
            companyDetails,
            viewId: "AP0010",
            keyField: "PAYMCODE",
            getKey: x => x.PaymentCode000,
            setKey: (x, key) => x.PaymentCode000 = key);
    }

    public Task<(ProcessOut Response, APPaymentCodes PaymentCode)> CreateOrUpdateAsync(
        APPaymentCodes paymentCode,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        return _crud.CreateOrUpdateAsync(paymentCode, user, operationName: "APPaymentCodes", cancellationToken);
    }

    public Task<(ProcessOut Response, APPaymentCodes PaymentCode)> ReadAsync(
        string paymentCode,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        return _crud.ReadAsync(paymentCode, user, operationName: "APPaymentCodes", cancellationToken);
    }
}

