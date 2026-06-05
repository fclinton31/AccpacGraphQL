using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApRecurringPayablesService : IApRecurringPayablesService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ApRecurringPayablesService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, APRecurringPayables RecurringPayables)> CreateOrUpdateAsync(
        APRecurringPayables recurringPayables,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0064,AP0065,AP0404,AP0405", compose: true);
            dynamic header = views.ViewById("AP0064");
            dynamic detail = views.ViewById("AP0065");

            if (string.IsNullOrWhiteSpace(recurringPayables.VendorNumber000) ||
                string.IsNullOrWhiteSpace(recurringPayables.RecurringPayableCode001))
            {
                session.RollbackTransaction(tran);
                return (ProcessOut.Fail("9999", "APRecurringPayables: VendorNumber000 and RecurringPayableCode001 are required."), recurringPayables);
            }

            header.Init();
            header.Fields.FieldByName("IDVEND").Value = recurringPayables.VendorNumber000;
            header.Fields.FieldByName("IDRECURR").Value = recurringPayables.RecurringPayableCode001;

            var exists = (bool)header.Exists;
            if (exists)
            {
                header.Read();
            }
            else
            {
                header.RecordGenerate(false);
                header.Fields.FieldByName("IDVEND").Value = recurringPayables.VendorNumber000;
                header.Fields.FieldByName("IDRECURR").Value = recurringPayables.RecurringPayableCode001;
            }

            SageViewEntityMapper.WriteEntityToView(recurringPayables, header);

            if (recurringPayables.RecurringPayableDetails is { Count: > 0 })
            {
                foreach (var line in recurringPayables.RecurringPayableDetails)
                {
                    detail.RecordGenerate(false);
                    SageViewEntityMapper.WriteEntityToView(line, detail);
                    detail.Insert();
                }
            }

            if (exists)
            {
                header.Update();
            }
            else
            {
                header.Insert();
            }

            session.CommitTransaction(tran);

            var docNumber = Convert.ToString(header.Fields.FieldByName("IDRECURR").Value);
            recurringPayables.RecurringPayableCode001 = docNumber;

            var response = ProcessOut.Ok(
                $"Sage 300 APRecurringPayables Number : {docNumber}",
                documentNumber: docNumber);

            return (response, recurringPayables);
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

            return (ProcessOut.Fail("9999", ex.Message), recurringPayables);
        }
    }
}

