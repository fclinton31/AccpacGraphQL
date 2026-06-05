using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApRemitToLocationsService : IApRemitToLocationsService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ApRemitToLocationsService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, APRemitToLocations RemitToLocations)> CreateOrUpdateAsync(
        APRemitToLocations remitToLocations,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0018,AP0409", compose: true);
            dynamic view = views.ViewById("AP0018");

            if (string.IsNullOrWhiteSpace(remitToLocations.VendorNumber000) ||
                string.IsNullOrWhiteSpace(remitToLocations.RemitToLocation001))
            {
                session.RollbackTransaction(tran);
                return (ProcessOut.Fail("9999", "APRemitToLocations: VendorNumber000 and RemitToLocation001 are required."), remitToLocations);
            }

            view.Init();
            view.Fields.FieldByName("IDVEND").Value = remitToLocations.VendorNumber000;
            view.Fields.FieldByName("IDVENDRMIT").Value = remitToLocations.RemitToLocation001;

            var exists = (bool)view.Exists;
            if (exists)
            {
                view.Read();
            }
            else
            {
                view.Init();
                view.Fields.FieldByName("IDVEND").Value = remitToLocations.VendorNumber000;
                view.Fields.FieldByName("IDVENDRMIT").Value = remitToLocations.RemitToLocation001;
            }

            SageViewEntityMapper.WriteEntityToView(remitToLocations, view);

            if (exists)
            {
                view.Update();
            }
            else
            {
                view.Insert();
            }

            session.CommitTransaction(tran);

            var remitId = Convert.ToString(view.Fields.FieldByName("IDVENDRMIT").Value);
            var vendorId = Convert.ToString(view.Fields.FieldByName("IDVEND").Value);

            var response = ProcessOut.Ok(
                $"Sage 300 APRemitToLocations Number : {remitId}, vendor {vendorId}",
                documentNumber: remitId);

            return (response, remitToLocations);
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

            return (ProcessOut.Fail("9999", ex.Message), remitToLocations);
        }
    }
}

