using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class DataTreeObjectParameterAttribute : Attribute
{
    public readonly string Name;

    public readonly ERDM_Parameter? Parameter;

    public readonly bool IsArray;

    public DataTreeObjectParameterAttribute(string name)
    {
        Name = name;
    }
    public DataTreeObjectParameterAttribute(ERDM_Parameter parameter, string name) : this(name)
    {
        Parameter = parameter;
    }
    public DataTreeObjectParameterAttribute(ERDM_Parameter parameter, string name, bool isArray) : this(parameter, name)
    {
        IsArray = isArray;
    }
    public override string ToString()
    {
        return $"{Parameter} -> {Name}";
    }
}