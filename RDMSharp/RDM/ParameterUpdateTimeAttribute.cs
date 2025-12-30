using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ParameterUpdateTimeAttribute : Attribute
{
    public readonly int Milliseconds;

    public ParameterUpdateTimeAttribute(int milliseconds)
    {
        Milliseconds = milliseconds;
    }

    public override string ToString()
    {
        return $"{Milliseconds}ms";
    }
}