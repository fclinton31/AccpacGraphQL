using System.Security.Cryptography;
using System.Text;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Data.Sqlite;

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
    private readonly IConfiguration _configuration;

    public EfUserCredentialValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<(bool IsValid, string Email, IReadOnlyList<string> Roles)> ValidateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        var record = await SettingsSqlite.TryGetUserAsync(_configuration, userName, cancellationToken);
        if (record is null)
        {
            return (false, string.Empty, Array.Empty<string>());
        }

        if (!PasswordHasher.VerifyOrPlainMatch(password, record.Password))
        {
            return (false, string.Empty, Array.Empty<string>());
        }

        var email = string.IsNullOrWhiteSpace(record.Email) ? record.UserName : record.Email;
        return (true, email, new[] { record.Role });
    }
}

internal static class SettingsSqlite
{
    internal sealed record UserRecord(string UserName, string Password, string Role, string? Email);
    internal sealed record CompanyRecord(string CompanyId, string UserName, string Password);

    public static string GetSettingsConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SettingsDb");
        return string.IsNullOrWhiteSpace(connectionString) ? "Data Source=settings.db" : connectionString;
    }

    public static async Task<UserRecord?> TryGetUserAsync(
        IConfiguration configuration,
        string userName,
        CancellationToken cancellationToken)
    {
        var connectionString = GetSettingsConnectionString(configuration);

        await using var conn = new SqliteConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        var userTable = await ResolveUserTableAsync(conn, cancellationToken);
        if (userTable is null)
        {
            return null;
        }

        var columns = await GetColumnSetAsync(conn, userTable, cancellationToken);
        var passwordCol = Pick(columns, "PasswordHash", "Password");
        var roleCol = Pick(columns, "Role", "UserRole");
        var emailCol = Pick(columns, "Email", "EmailAddress");

        if (passwordCol is null || roleCol is null)
        {
            return null;
        }

        var sql = emailCol is null
            ? $"SELECT UserName, {passwordCol}, {roleCol} FROM \"{userTable}\" WHERE lower(UserName)=lower($userName) LIMIT 1"
            : $"SELECT UserName, {passwordCol}, {roleCol}, {emailCol} FROM \"{userTable}\" WHERE lower(UserName)=lower($userName) LIMIT 1";

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("$userName", userName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var u = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
        var p = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
        var r = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
        var e = emailCol is null ? null : (reader.IsDBNull(3) ? null : reader.GetString(3));
        if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(r))
        {
            return null;
        }

        return new UserRecord(u, p, r, e);
    }

    public static async Task<CompanyRecord?> TryGetCompanyByKeyAsync(
        IConfiguration configuration,
        string companyKey,
        CancellationToken cancellationToken)
    {
        companyKey = companyKey.Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
        var connectionString = GetSettingsConnectionString(configuration);

        await using var conn = new SqliteConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        var companyTable = await ResolveCompanyTableAsync(conn, cancellationToken);
        if (companyTable is null)
        {
            return null;
        }

        var columns = await GetColumnSetAsync(conn, companyTable, cancellationToken);
        var companyIdCol = Pick(columns, "CompanyId", "CmpID");
        var companyKeyCol = Pick(columns, "CompanyKey", "Token");
        var userNameCol = Pick(columns, "UserName");
        var passwordCol = Pick(columns, "Password");

        if (companyIdCol is null || companyKeyCol is null || userNameCol is null || passwordCol is null)
        {
            return null;
        }

        var sql =
            $"SELECT {companyIdCol}, {userNameCol}, {passwordCol} FROM \"{companyTable}\" WHERE trim({companyKeyCol})=$companyKey LIMIT 1";

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("$companyKey", companyKey);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var companyId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
        var user = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
        var pass = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
        if (string.IsNullOrWhiteSpace(companyId) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            return null;
        }

        return new CompanyRecord(companyId, user, pass);
    }

    private static async Task<string?> ResolveUserTableAsync(SqliteConnection conn, CancellationToken cancellationToken)
    {
        if (await TableExistsAsync(conn, "User", cancellationToken))
        {
            return "User";
        }

        if (await TableExistsAsync(conn, "Users", cancellationToken))
        {
            return "Users";
        }

        return null;
    }

    private static async Task<string?> ResolveCompanyTableAsync(SqliteConnection conn, CancellationToken cancellationToken)
    {
        if (await TableExistsAsync(conn, "Company", cancellationToken))
        {
            return "Company";
        }

        if (await TableExistsAsync(conn, "Companies", cancellationToken))
        {
            return "Companies";
        }

        return null;
    }

    private static async Task<bool> TableExistsAsync(SqliteConnection conn, string tableName, CancellationToken cancellationToken)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name=$name LIMIT 1";
        cmd.Parameters.AddWithValue("$name", tableName);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result is not null && result is not DBNull;
    }

    private static async Task<HashSet<string>> GetColumnSetAsync(
        SqliteConnection conn,
        string tableName,
        CancellationToken cancellationToken)
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info(\"{tableName}\")";
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (!reader.IsDBNull(1))
            {
                columns.Add(reader.GetString(1));
            }
        }

        return columns;
    }

    private static string? Pick(HashSet<string> available, params string[] candidates)
    {
        foreach (var c in candidates)
        {
            if (available.Contains(c))
            {
                return c;
            }
        }

        return null;
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

    public static bool VerifyOrPlainMatch(string password, string stored)
    {
        if (stored.Contains('.', StringComparison.Ordinal) && Verify(password, stored))
        {
            return true;
        }

        return string.Equals(password, stored, StringComparison.Ordinal);
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
