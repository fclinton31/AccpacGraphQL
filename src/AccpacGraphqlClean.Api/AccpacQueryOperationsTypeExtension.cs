using HotChocolate.Types;

namespace AccpacGraphqlClean.Api;

public sealed class AccpacQueryOperationsTypeExtension : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        AccpacOperationFieldRegistrar.Register(
            descriptor,
            "Query",
            AccpacGraphqlClean.Application.AccpacOperationKind.Query,
            reservedFieldNames: new HashSet<string>(StringComparer.Ordinal)
            {
                "health",
                "accpacEndpoints",
                "accpac"
            });
    }
}
