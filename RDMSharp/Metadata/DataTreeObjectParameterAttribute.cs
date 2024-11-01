using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Parameter)]
public class DataTreeObjectParameterAttribute : Attribute
{
    public readonly string Name;

    public DataTreeObjectParameterAttribute(string name)
    {
        Name = name;
    }
}