using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class StubAccpacOperationExecutor : IAccpacOperationExecutor
{
    public Task<AccpacOperationResult> ExecuteAsync(
        string restRoute,
        object? input,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var response = ProcessOut.Ok("Stub executor: implement Sage 300 integration in Infrastructure.");
        var data = new Dictionary<string, object?>
        {
            ["restRoute"] = restRoute,
            ["input"] = input,
            ["user"] = new Dictionary<string, object?>
            {
                ["name"] = user.Identity?.Name,
                ["claims"] = user.Claims.Select(c => new Dictionary<string, object?>
                {
                    ["type"] = c.Type,
                    ["value"] = c.Value
                }).ToArray()
            }
        };

        return Task.FromResult(new AccpacOperationResult(response, data));
    }
}

