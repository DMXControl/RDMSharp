using RDMSharp.Metadata.JSON;

namespace RDMSharp.Metadata;

public readonly struct DataTreeObjectDependeciePropertyBag
{
    public readonly string Name;
    public readonly string Property;
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicate Command;

    internal DataTreeObjectDependeciePropertyBag(string name, string property, ERDM_Parameter parameter, Command.ECommandDublicate command)
    {
        Name = name;
        Property = property;
        Parameter = parameter;
        Command = command;
    }
}