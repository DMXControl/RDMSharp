using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DataTreeObjectPropertyAttribute : Attribute
{
    public readonly string Name;
    public readonly uint Index;
    public readonly object NullValue;

    public readonly ERDM_Parameter? Parameter;

    public DataTreeObjectPropertyAttribute(string name, uint index, object nullValue = null)
    {
        Name = name;
        Index = index;
        NullValue = nullValue;
    }
    public DataTreeObjectPropertyAttribute(ERDM_Parameter parameter, string name, uint index, object nullValue = null) : this(name, index, nullValue)
    {
        Parameter = parameter;
    }

    public override string ToString()
    {
        return $"{Parameter} -> {Name}";
    }
}