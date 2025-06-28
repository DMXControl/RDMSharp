using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ParameterGroupAttribute : Attribute
{
    public readonly string Name;

    public ParameterGroupAttribute(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}