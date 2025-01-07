using RDMSharp.Metadata.JSON;
using System;
using System.Threading;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DataTreeObjectPropertyAttribute : Attribute
{
    public readonly string Name;
    public readonly uint Index;

    public readonly ERDM_Parameter? Parameter;

    public DataTreeObjectPropertyAttribute(string name, uint index)
    {
        Name = name;
        Index = index;
    }
    public DataTreeObjectPropertyAttribute(ERDM_Parameter parameter, string name, uint index) : this(name, index)
    {
        Parameter = parameter;
    }
}