using AccpacGraphqlClean.Domain;
using HotChocolate.Types;

namespace AccpacGraphqlClean.Api;

public sealed class AccpacOperationResultType : ObjectType<AccpacOperationResult>
{
    protected override void Configure(IObjectTypeDescriptor<AccpacOperationResult> descriptor)
    {
        descriptor.Field(f => f.Response).Type<NonNullType<ProcessOutType>>();
        descriptor.Field(f => f.Data).Type<AnyType>();
    }
}
