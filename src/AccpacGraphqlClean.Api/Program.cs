using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AccpacGraphqlClean.Api;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using AccpacGraphqlClean.Infrastructure;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "AccpacGraphqlClean";
        var audience = builder.Configuration["Jwt:Audience"] ?? "AccpacGraphqlClean";
        var signingKey = builder.Configuration["Jwt:SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("Jwt:SigningKey must be set in appsettings.json.");
        }

        if (!builder.Environment.IsDevelopment()
            && (signingKey.Contains("DEV_ONLY_CHANGE_ME", StringComparison.OrdinalIgnoreCase)
                || signingKey.Contains("REPLACE_ME", StringComparison.OrdinalIgnoreCase)
                || signingKey.Length < 32))
        {
            throw new InvalidOperationException("Jwt:SigningKey must be set to a strong value in appsettings.json (>= 32 chars) for non-development environments.");
        }
        if (signingKey.Length < 32)
        {
            signingKey = signingKey.PadRight(32, '0');
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddType<AccpacOperationResultType>()
    .AddType<AccpacDataType>()
    .AddType<SageRecordType>()
    .AddType<ProcessOutType>()
    .AddQueryType<Query>()
    .AddTypeExtension<AccpacQueryOperationsTypeExtension>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<AccpacMutationOperationsTypeExtension>();

var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SettingsDbContext>();
    var bootstrapEnabled = bool.TryParse(builder.Configuration["Bootstrap:Enable"], out var b) && b;
    if (!bootstrapEnabled)
    {
        goto EndBootstrap;
    }

    await db.Database.EnsureCreatedAsync();

    var bootstrapUserName = builder.Configuration["Bootstrap:UserName"];
    var bootstrapPassword = builder.Configuration["Bootstrap:Password"];
    var bootstrapEmail = builder.Configuration["Bootstrap:Email"] ?? "admin@local";
    var bootstrapRole = builder.Configuration["Bootstrap:Role"] ?? "IntegrationUser";

    var bootstrapCompanyKey = builder.Configuration["Bootstrap:CompanyKey"];
    var bootstrapCompanyId = builder.Configuration["Bootstrap:CompanyId"];
    var bootstrapSageUser = builder.Configuration["Bootstrap:SageUserName"];
    var bootstrapSagePassword = builder.Configuration["Bootstrap:SagePassword"];

    if (!string.IsNullOrWhiteSpace(bootstrapUserName) && !string.IsNullOrWhiteSpace(bootstrapPassword))
    {
        var existingUser = await db.Users.SingleOrDefaultAsync(u => u.UserName == bootstrapUserName);
        if (existingUser is null)
        {
            db.Users.Add(new UserAccount
            {
                UserName = bootstrapUserName,
                Email = bootstrapEmail,
                FullName = bootstrapUserName,
                PasswordHash = PasswordHasher.Hash(bootstrapPassword),
                Role = bootstrapRole
            });
        }
    }

    if (!string.IsNullOrWhiteSpace(bootstrapCompanyKey)
        && !string.IsNullOrWhiteSpace(bootstrapCompanyId)
        && !string.IsNullOrWhiteSpace(bootstrapSageUser)
        && !string.IsNullOrWhiteSpace(bootstrapSagePassword))
    {
        var existingCompany = await db.Companies.SingleOrDefaultAsync(c => c.CompanyKey == bootstrapCompanyKey);
        if (existingCompany is null)
        {
            db.Companies.Add(new Company
            {
                CompanyKey = bootstrapCompanyKey,
                CompanyId = bootstrapCompanyId,
                UserName = bootstrapSageUser,
                Password = bootstrapSagePassword
            });
        }
    }

    await db.SaveChangesAsync();

EndBootstrap: ;
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL("/graphql");
app.MapGet("/", () => Results.Redirect("/graphql", permanent: false));

app.Run();

public sealed class Query
{
    public string health() => "ok";

    public IReadOnlyList<AccpacEndpointDefinition> accpacEndpoints([Service] IAccpacEndpointCatalog catalog) =>
        catalog.Endpoints;

    [Authorize]
    public async Task<AccpacOperationResultOf<ARCustomerReadData>> arcustomerReadarcustomer(
        string inputJson,
        ClaimsPrincipal user,
        [Service] IArCustomerService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(inputJson))
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("inputJson is required.")
                .SetCode("INPUT_REQUIRED")
                .Build());
        }

        using var doc = JsonDocument.Parse(inputJson);
        string? customerNumber = null;

        if (doc.RootElement.ValueKind == JsonValueKind.Object)
        {
            if (doc.RootElement.TryGetProperty("CustomerNumber000", out var cn) && cn.ValueKind == JsonValueKind.String)
            {
                customerNumber = cn.GetString();
            }
            else if (doc.RootElement.TryGetProperty("CustomerNumber000", out cn) && cn.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
            {
                customerNumber = cn.ToString();
            }
            else if (doc.RootElement.TryGetProperty("customerNumber", out var cn2) && cn2.ValueKind == JsonValueKind.String)
            {
                customerNumber = cn2.GetString();
            }
            else if (doc.RootElement.TryGetProperty("customerNumber", out cn2) && cn2.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
            {
                customerNumber = cn2.ToString();
            }
        }
        else if (doc.RootElement.ValueKind == JsonValueKind.String)
        {
            customerNumber = doc.RootElement.GetString();
        }
        else if (doc.RootElement.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
        {
            customerNumber = doc.RootElement.ToString();
        }

        if (string.IsNullOrWhiteSpace(customerNumber))
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("CustomerNumber000 is required. Example inputJson: {\"CustomerNumber000\":\"1200\"} or \"1200\".")
                .SetCode("INPUT_REQUIRED")
                .Build());
        }

        var (response, customer) = await service.ReadAsync(customerNumber, user, cancellationToken);
        return new AccpacOperationResultOf<ARCustomerReadData>(response, new ARCustomerReadData(customer));
    }

    public Task<AccpacOperationResult> accpac(
        string restRoute,
        string? inputJson,
        ClaimsPrincipal user,
        [Service] IAccpacOperationExecutor executor,
        CancellationToken cancellationToken)
    {
        object? input = inputJson;
        return executor.ExecuteAsync(restRoute, input, user, cancellationToken);
    }
}

public sealed class Mutation
{
    public Task<AuthToken> login(
        LoginRequest input,
        [Service] IUserCredentialValidator validator,
        [Service] ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        return LoginImpl(input, validator, tokenService, cancellationToken);
    }

    private static async Task<AuthToken> LoginImpl(
        LoginRequest input,
        IUserCredentialValidator validator,
        ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.CompanyKey))
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("companyKey is required.")
                .SetCode("AUTH_MISSING_COMPANYKEY")
                .Build());
        }

        var (isValid, email, roles) = await validator.ValidateAsync(input.UserName, input.Password, cancellationToken);
        if (!isValid)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Invalid credentials.")
                .SetCode("AUTH_INVALID_CREDENTIALS")
                .Build());
        }

        return tokenService.CreateToken(input.UserName, email, roles, input.CompanyKey);
    }
}
