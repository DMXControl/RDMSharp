using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class DataTreeObjectParameterAttribute : Attribute
{
    public readonly string Name;

    public readonly ERDM_Parameter? Parameter;

    public readonly bool IsArray;

    public DataTreeObjectParameterAttribute(string name, bool isArray = false)
    {
        Name = name;
        IsArray = isArray;
    }
    public DataTreeObjectParameterAttribute(ERDM_Parameter parameter, string name, bool isArray = false) : this(name, isArray)
    {
        Parameter = parameter;
    }
}