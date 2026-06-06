using AccpacGraphqlClean.Application;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace AccpacGraphqlClean.Api;

internal static class AccpacOperationFieldRegistrar
{
    internal static void Register(
        IObjectTypeDescriptor descriptor,
        string typeName,
        AccpacOperationKind kind,
        HashSet<string> reservedFieldNames)
    {
        descriptor.Name(typeName);

        var endpoints = new AccpacGraphqlClean.Infrastructure.AccpacRestEndpointCatalog().Endpoints;

        foreach (var endpoint in endpoints.Where(e => e.Kind == kind))
        {
            if (reservedFieldNames.Contains(endpoint.GraphQlFieldName))
            {
                continue;
            }

            var restRoute = endpoint.RestRoute;
            descriptor.Field(endpoint.GraphQlFieldName)
                .Authorize()
                .Argument("inputJson", a => a.Type<StringType>())
                .Type<NonNullType<AccpacOperationResultType>>()
                .Resolve(async context =>
                {
                    var inputJson = context.ArgumentValue<string?>("inputJson");
                    var input = (object?)inputJson;
                    var executor = context.Service<IAccpacOperationExecutor>();
                    var httpContextAccessor = context.Service<IHttpContextAccessor>();
                    var user = httpContextAccessor.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                    return await executor.ExecuteAsync(restRoute, input, user, context.RequestAborted);
                });
        }
    }
}
