using RDMSharp.Metadata.JSON;
using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true)]
public class DataTreeObjectAttribute : Attribute
{
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicte Command;

    public DataTreeObjectAttribute(ERDM_Parameter parameter, Command.ECommandDublicte command)
    {
        Parameter = parameter;
        Command = command;
    }
}