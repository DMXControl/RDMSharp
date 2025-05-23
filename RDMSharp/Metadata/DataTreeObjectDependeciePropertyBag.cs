using RDMSharp.Metadata.JSON;

namespace RDMSharp.Metadata;

public readonly struct DataTreeObjectDependeciePropertyBag
{
    public readonly string Name;
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicate Command;
    public readonly object Value;

    internal DataTreeObjectDependeciePropertyBag(string name, ERDM_Parameter parameter, Command.ECommandDublicate command)
    {
        Name = name;
        Parameter = parameter;
        Command = command;
    }
}