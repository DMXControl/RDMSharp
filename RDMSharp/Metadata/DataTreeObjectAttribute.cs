using RDMSharp.Metadata.JSON;
using System;

namespace RDMSharp.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class DataTreeObjectAttribute : Attribute
{
    public readonly ERDM_Parameter Parameter;
    public readonly Command.ECommandDublicate Command;
    public readonly EManufacturer Manufacturer = EManufacturer.ESTA;

    public readonly bool IsArray;
    public readonly string Path;

    public DataTreeObjectAttribute(ERDM_Parameter parameter, Command.ECommandDublicate command, bool isArray = false, string path = null)
    {
        Parameter = parameter;
        Command = command;
        IsArray = isArray;
        Path = path;
    }
    public DataTreeObjectAttribute(EManufacturer manufacturer, ERDM_Parameter parameter, Command.ECommandDublicate command, bool isArray = false, string path = null)
        : this(parameter, command, isArray, path)
    {
        Manufacturer = manufacturer;
    }
}
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true)]
public class DataTreeEnumAttribute : DataTreeObjectAttribute
{
    public readonly string Name;
    public DataTreeEnumAttribute(ERDM_Parameter parameter, Command.ECommandDublicate command, string name, bool isArray = false, string path = null)
        : base(parameter, command, isArray, path)
    {
        Name = name;
    }
    public DataTreeEnumAttribute(EManufacturer manufacturer, ERDM_Parameter parameter, Command.ECommandDublicate command, string name, bool isArray = false, string path = null)
        : base(manufacturer, parameter, command, isArray, path)
    {
        Name = name;
    }
}