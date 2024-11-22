using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Constructor)]
public class DataTreeObjectConstructorAttribute : Attribute
{
    public DataTreeObjectConstructorAttribute()
    {
    }
}