using HotChocolate.Types;

namespace AccpacGraphqlClean.Api;

public sealed class AccpacMutationOperationsTypeExtension : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        AccpacOperationFieldRegistrar.Register(
            descriptor,
            "Mutation",
            AccpacGraphqlClean.Application.AccpacOperationKind.Mutation,
            reservedFieldNames: new HashSet<string>(StringComparer.Ordinal)
            {
                "login"
            });
    }
}
