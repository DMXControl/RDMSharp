using RDMSharp.Metadata.JSON;
using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DataTreeObjectDependeciePropertyAttribute : Attribute
{
    public readonly string Name;
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicte Command;
    public readonly DataTreeObjectDependeciePropertyBag Bag;

    public DataTreeObjectDependeciePropertyAttribute(string name, ERDM_Parameter parameter, Command.ECommandDublicte command)
    {
        Name = name;
        Parameter = parameter;
        Command = command;
        Bag = new DataTreeObjectDependeciePropertyBag(name, parameter, command);
    }
}
