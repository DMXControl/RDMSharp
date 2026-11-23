using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>
/// Builds a <see cref="IGenericModule"/> from the JSON define of a parameter, if the define fits a simple module:
/// <list type="bullet">
/// <item>GET without request data (an indexed parameter is not simple) and/or SET,</item>
/// <item>one field: boolean, integer or string; an integer restricted to its labels is a choice (<see cref="GenericLabeledIntegerModule{T}"/>),</item>
/// <item>for an ESTA parameter no C# type is defined for it (then the device stores that object, not the raw value).</item>
/// </list>
/// Everything else (several fields, bytes, lists, ...) needs a module written for it.
/// </summary>
public static class GenericModuleFactory
{
    /// <summary>
    /// Unique per manufacturer and PID: a manufacturer specific PID means something different for every manufacturer.
    /// The manufacturer is the one of the define (0x0000 for an ESTA parameter), e.g. <c>Generic_0x5753_0x8031</c>.
    /// </summary>
    internal static string ModuleName(MetadataJSONObjectDefine define, ERDM_Parameter parameter) => $"Generic_0x{define.ManufacturerID:X4}_0x{(ushort)parameter:X4}";
    internal static string DisplayNameOf(MetadataJSONObjectDefine define) => string.IsNullOrWhiteSpace(define.DisplayName) ? define.Name : define.DisplayName;

    /// <summary>
    /// Creates the generic modules of <paramref name="device"/> for its supported parameters that no module in <paramref name="existingModules"/> handles.
    /// </summary>
    public static IReadOnlyCollection<IModule> CreateMissing(AbstractRemoteRDMDevice device, IEnumerable<IModule> existingModules)
    {
        var result = new List<IModule>();
        var supported = device?.DeviceModel?.GetSupportedParameters();
        if (supported is null)
            return result;

        var covered = new HashSet<ERDM_Parameter>(existingModules.SelectMany(m => m.SupportedParameters));
        foreach (var metadata in supported)
        {
            if (!metadata.IsNonBlueprint || metadata.IsManufacturerInternal || covered.Contains(metadata.Parameter))
                continue;
            if (TryCreate(device, metadata.Parameter, out IGenericModule module))
            {
                result.Add(module);
                covered.Add(metadata.Parameter);
            }
        }
        return result;
    }

    /// <summary>Creates the generic module for <paramref name="parameter"/> of <paramref name="device"/>, if its define fits a simple module.</summary>
    public static bool TryCreate(AbstractRemoteRDMDevice device, ERDM_Parameter parameter, out IGenericModule module)
    {
        module = null;
        if (device is null)
            return false;
        var info = device.DeviceInfo;
        var bag = new ParameterBag(parameter, device.UID.ManufacturerID, info?.DeviceModelId, info?.SoftwareVersionId);
        MetadataJSONObjectDefine define;
        try
        {
            define = MetadataFactory.GetDefine(bag);
        }
        catch (DefineNotFoundException)
        {
            return false;
        }
        if (define is null)
            return false;

        return TryCreate(device, parameter, define, out module);
    }

    internal static bool TryCreate(AbstractRemoteRDMDevice device, ERDM_Parameter parameter, MetadataJSONObjectDefine define, out IGenericModule module)
    {
        module = null;
        bool get = define.GetRequest.HasValue;
        bool set = define.SetRequest.HasValue;
        if (!get && !set)
            return false;
        if (get && !define.GetRequest.Value.GetIsEmpty())
            return false; // indexed (or with request data): not a simple parameter

        Command.ECommandDublicate valueCommand = get ? Command.ECommandDublicate.GetResponse : Command.ECommandDublicate.SetRequest;
        if (!tryGetSingleField(define, valueCommand, out OneOfTypes field))
            return false;
        if (define.ManufacturerID == 0 && MetadataFactory.GetDefinedDataTreeObjectType(define, valueCommand) is not null)
            return false; // RDMSharp stores the defined object for it, not the raw value

        ERDM_CommandClass commandClass = 0;
        if (get)
            commandClass |= ERDM_CommandClass.GET;
        if (set)
            commandClass |= ERDM_CommandClass.SET;

        if (field.BooleanType is BooleanType booleanType)
            module = new GenericBooleanModule(device, define, parameter, commandClass, booleanType);
        else if (field.StringType is StringType stringType)
            module = new GenericStringModule(device, define, parameter, commandClass, stringType);
        else
            module = createNumeric(device, define, parameter, commandClass, field);

        return module is not null;
    }

    private static IGenericModule createNumeric(AbstractRemoteRDMDevice device, MetadataJSONObjectDefine define, ERDM_Parameter parameter, ERDM_CommandClass commandClass, OneOfTypes field)
    {
        return field switch
        {
            { IntegerType_UInt8: { } t } => numeric(t),
            { IntegerType_Int8: { } t } => numeric(t),
            { IntegerType_UInt16: { } t } => numeric(t),
            { IntegerType_Int16: { } t } => numeric(t),
            { IntegerType_UInt32: { } t } => numeric(t),
            { IntegerType_Int32: { } t } => numeric(t),
            { IntegerType_UInt64: { } t } => numeric(t),
            { IntegerType_Int64: { } t } => numeric(t),
            _ => null
        };

        IGenericModule numeric<T>(IntegerType<T> integerType) where T : struct, IComparable, IConvertible, IFormattable
            // only labels the value is restricted to make a choice; otherwise they name special values of a number
            => integerType.Labels is { Length: > 0 } && integerType.RestrictToLabeled == true
                ? new GenericLabeledIntegerModule<T>(device, define, parameter, commandClass, integerType)
                : new GenericNumericModule<T>(device, define, parameter, commandClass, integerType);
    }

    /// <summary>The one field of the command, also if the command refers to another one (e.g. SET request = GET response).</summary>
    private static bool tryGetSingleField(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType, out OneOfTypes field)
    {
        field = default;
        define.GetCommand(commandType, out Command? command);
        for (int i = 0; i < 3 && command?.EnumValue is Command.ECommandDublicate referenced; i++)
            define.GetCommand(referenced, out command);
        if (command is not Command cmd)
            return false;

        if (cmd.SingleField is OneOfTypes single)
            field = single;
        else if (cmd.ListOfFields is { Length: 1 } list)
            field = list[0];
        else
            return false;

        return field.BooleanType is not null
            || field.StringType is not null
            || field.IntegerType_UInt8 is not null || field.IntegerType_Int8 is not null
            || field.IntegerType_UInt16 is not null || field.IntegerType_Int16 is not null
            || field.IntegerType_UInt32 is not null || field.IntegerType_Int32 is not null
            || field.IntegerType_UInt64 is not null || field.IntegerType_Int64 is not null;
    }
}
