using RDMSharp.Metadata.JSON;
using RDMSharp.ParameterWrapper;
using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true)]
public class DataTreeObjectAttribute : Attribute
{
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicte Command;
    public readonly EManufacturer Manufacturer = EManufacturer.ESTA;

    public readonly bool IsArray;
    public readonly string Path;

    public DataTreeObjectAttribute(ERDM_Parameter parameter, Command.ECommandDublicte command, bool isArray = false, string path=null)
    {
        Parameter = parameter;
        Command = command;
        IsArray = isArray;
        Path = path;
    }
    public DataTreeObjectAttribute(EManufacturer manufacturer, ERDM_Parameter parameter, Command.ECommandDublicte command, bool isArray = false, string path = null)
        : this(parameter, command, isArray, path)
    {
        Manufacturer = manufacturer;
    }
}