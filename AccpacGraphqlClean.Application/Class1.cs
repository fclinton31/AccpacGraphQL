using System.Security.Claims;
using AccpacGraphqlClean.Domain;

namespace AccpacGraphqlClean.Application;

public enum AccpacOperationKind
{
    Query = 0,
    Mutation = 1
}

public sealed record AccpacEndpointDefinition(
    string RestRoute,
    AccpacOperationKind Kind,
    string GraphQlFieldName
);

public interface IAccpacEndpointCatalog
{
    IReadOnlyList<AccpacEndpointDefinition> Endpoints { get; }
}

public interface IAccpacOperationExecutor
{
    Task<AccpacOperationResult> ExecuteAsync(
        string restRoute,
        object? input,
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    );
}

public sealed record LoginRequest(
    string UserName,
    string Password,
    string? CompanyKey
);

public interface ITokenService
{
    AuthToken CreateToken(
        string userName,
        string email,
        IEnumerable<string> roles,
        string? companyKey
    );
}
