using System.Security.Claims;
using System.Text;
using AccpacGraphqlClean.Api;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using AccpacGraphqlClean.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "AccpacGraphqlClean";
        var audience = builder.Configuration["Jwt:Audience"] ?? "AccpacGraphqlClean";
        var signingKey = builder.Configuration["Jwt:SigningKey"] ?? "DEV_ONLY_CHANGE_ME_DEV_ONLY_CHANGE_ME";
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
    .AddType<AccpacOperationResultType>()
    .AddType<ProcessOutType>()
    .AddQueryType<Query>()
    .AddTypeExtension<AccpacQueryOperationsTypeExtension>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<AccpacMutationOperationsTypeExtension>();

var app = builder.Build();

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
