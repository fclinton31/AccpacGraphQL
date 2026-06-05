using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class EfCompanyConnectionDetailsProvider : ICompanyConnectionDetailsProvider
{
    private readonly SettingsDbContext _db;

    public EfCompanyConnectionDetailsProvider(SettingsDbContext db)
    {
        _db = db;
    }

    public async Task<CompanyConnectionDetails> GetAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var companyKey = user.Claims.FirstOrDefault(c => c.Type == "CmpKey")?.Value;
        if (string.IsNullOrWhiteSpace(companyKey))
        {
            throw new InvalidOperationException("Missing CmpKey claim.");
        }

        var company = await _db.Companies.SingleOrDefaultAsync(c => c.CompanyKey == companyKey, cancellationToken);
        if (company is null)
        {
            throw new InvalidOperationException("Unknown company key.");
        }

        return new CompanyConnectionDetails(company.CompanyId, company.UserName, company.Password);
    }
}

