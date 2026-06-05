using AccpacGraphqlClean.Domain;
using HotChocolate.Types;

namespace AccpacGraphqlClean.Api;

public sealed class ProcessOutType : ObjectType<ProcessOut>
{
    protected override void Configure(IObjectTypeDescriptor<ProcessOut> descriptor)
    {
        descriptor.Field(f => f.ReturnCode).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.ReturnMessage).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.DocumentNumber).Type<StringType>();
        descriptor.Field(f => f.BatchNumber).Type<StringType>();
        descriptor.Field(f => f.ReferenceNumber).Type<StringType>();
        descriptor.Field(f => f.ErrorCode).Type<StringType>();
    }
}
