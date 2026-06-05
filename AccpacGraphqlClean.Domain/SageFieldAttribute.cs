namespace AccpacGraphqlClean.Domain;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SageFieldAttribute : Attribute
{
    public SageFieldAttribute(string fieldName)
    {
        FieldName = fieldName;
    }

    public string FieldName { get; }
}

