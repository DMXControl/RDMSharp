using RDMSharp.Metadata.JSON;
using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DataTreeObjectDependeciePropertyAttribute : Attribute
{
    public readonly string Name;
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicate Command;
    public readonly DataTreeObjectDependeciePropertyBag Bag;

    public readonly string Property;

    public DataTreeObjectDependeciePropertyAttribute(string name, ERDM_Parameter parameter, Command.ECommandDublicate command) : this(name, null, parameter, command)
    {
    }
    public DataTreeObjectDependeciePropertyAttribute(string name, string property, ERDM_Parameter parameter, Command.ECommandDublicate command) : base()
    {
        Name = name;
        Parameter = parameter;
        Command = command;
        Property = property;
        Bag = new DataTreeObjectDependeciePropertyBag(name, property, parameter, command);
    }
}