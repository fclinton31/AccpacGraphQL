using System.Security.Cryptography;
using System.Text;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AccpacGraphqlClean.Infrastructure;

public interface IUserCredentialValidator
{
    Task<(bool IsValid, string Email, IReadOnlyList<string> Roles)> ValidateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken
    );
}

public sealed class EfUserCredentialValidator : IUserCredentialValidator
{
    private readonly SettingsDbContext _db;

    public EfUserCredentialValidator(SettingsDbContext db)
    {
        _db = db;
    }

    public async Task<(bool IsValid, string Email, IReadOnlyList<string> Roles)> ValidateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        if (user is null)
        {
            return (false, string.Empty, Array.Empty<string>());
        }

        if (!PasswordHasher.Verify(password, user.PasswordHash))
        {
            return (false, string.Empty, Array.Empty<string>());
        }

        return (true, user.Email, new[] { user.Role });
    }
}

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 120_000;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string encodedHash)
    {
        var parts = encodedHash.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = Convert.FromBase64String(parts[2]);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}

public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthToken CreateToken(string userName, string email, IEnumerable<string> roles, string? companyKey)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "AccpacGraphqlClean";
        var audience = _configuration["Jwt:Audience"] ?? "AccpacGraphqlClean";
        var signingKey = _configuration["Jwt:SigningKey"] ?? "DEV_ONLY_CHANGE_ME_DEV_ONLY_CHANGE_ME";

        if (signingKey.Length < 32)
        {
            signingKey = signingKey.PadRight(32, '0');
        }

        var expiresInSeconds = int.TryParse(_configuration["Jwt:ExpiresInSeconds"], out var s) ? s : 86_400;
        var now = DateTimeOffset.UtcNow;
        var expires = now.AddSeconds(expiresInSeconds);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, email)
        };

        if (!string.IsNullOrWhiteSpace(companyKey))
        {
            claims.Add(new Claim("CmpKey", companyKey));
        }

        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: creds);

        var handler = new JwtSecurityTokenHandler();
        return new AuthToken(handler.WriteToken(token), "Bearer", expiresInSeconds);
    }
}

