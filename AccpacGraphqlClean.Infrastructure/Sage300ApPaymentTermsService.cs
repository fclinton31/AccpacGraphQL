using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ApPaymentTermsService : IApPaymentTermsService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ApPaymentTermsService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, APPaymentTerms PaymentTerms)> CreateOrUpdateAsync(
        APPaymentTerms paymentTerms,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AP0012,AP0011", compose: true);
            dynamic header = views.ViewById("AP0012");
            dynamic detail = views.ViewById("AP0011");

            header.Init();

            if (string.IsNullOrWhiteSpace(paymentTerms.TermsCode000))
            {
                session.RollbackTransaction(tran);
                return (ProcessOut.Fail("9999", "APPaymentTerms: TermsCode000 is required."), paymentTerms);
            }

            header.Fields.FieldByName("TERMSCODE").Value = paymentTerms.TermsCode000;
            var exists = (bool)header.Exists;
            if (exists)
            {
                header.Read();
            }
            else
            {
                header.RecordGenerate(false);
                header.Fields.FieldByName("TERMSCODE").Value = paymentTerms.TermsCode000;
            }

            SageViewEntityMapper.WriteEntityToView(paymentTerms, header);

            if (paymentTerms.PaymentSchedules is { Count: > 0 })
            {
                foreach (var schedule in paymentTerms.PaymentSchedules)
                {
                    detail.RecordGenerate(false);
                    SageViewEntityMapper.WriteEntityToView(schedule, detail);
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
            var docNumber = Convert.ToString(header.Fields.FieldByName("TERMSCODE").Value);
            paymentTerms.TermsCode000 = docNumber;
            var response = ProcessOut.Ok($"Sage 300 AP Payment Terms Number : {docNumber}", documentNumber: docNumber);
            return (response, paymentTerms);
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

            return (ProcessOut.Fail("9999", ex.Message), paymentTerms);
        }
    }
}

