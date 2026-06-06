using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class EfCompanyConnectionDetailsProvider : ICompanyConnectionDetailsProvider
{
    private readonly IConfiguration _configuration;

    public EfCompanyConnectionDetailsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<CompanyConnectionDetails> GetAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var companyKey = user.Claims.FirstOrDefault(c => c.Type == "CmpKey")?.Value;
        if (string.IsNullOrWhiteSpace(companyKey))
        {
            throw new InvalidOperationException("Missing CmpKey claim. Login must include companyKey and requests must send Authorization: Bearer <token>.");
        }

        var normalizedKey = SettingsSqlite.NormalizeCompanyKey(companyKey);
        var company = await SettingsSqlite.TryGetCompanyByKeyAsync(_configuration, normalizedKey, cancellationToken);
        if (company is null)
        {
            var cs = SettingsSqlite.GetSettingsConnectionString(_configuration);
            throw new InvalidOperationException($"Unknown company key. SettingsDb={cs}; CmpKey={Mask(normalizedKey)}");
        }

        return new CompanyConnectionDetails(company.CompanyId, company.UserName, company.Password);
    }

    private static string Mask(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "<empty>";
        }

        if (value.Length <= 10)
        {
            return value[0] + "***" + value[^1];
        }

        return value[..6] + "..." + value[^4..];
    }
}
