using RDMSharp.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>
/// The module of a supported parameter that neither a written nor a generic module (<see cref="IGenericModule"/>) handles:
/// its define is not simple enough, no define is known, or it is a manufacturer internal parameter.
/// It only holds the value, read only, so every parameter of a device is reachable through <see cref="AbstractRemoteRDMDevice.Modules"/>
/// and an application can show it, marked as "no module yet".
/// </summary>
public sealed class FallbackParameterModule : AbstractModule
{
    /// <summary>
    /// Parameters the device itself processes (discovery, status, sensors, slots, metadata): they need no module.
    /// </summary>
    public static readonly IReadOnlySet<ERDM_Parameter> HandledByDevice = new HashSet<ERDM_Parameter>
    {
        ERDM_Parameter.QUEUED_MESSAGE, ERDM_Parameter.STATUS_MESSAGES, ERDM_Parameter.STATUS_ID_DESCRIPTION,
        ERDM_Parameter.CLEAR_STATUS_ID, ERDM_Parameter.SUB_DEVICE_STATUS_REPORT_THRESHOLD,
        ERDM_Parameter.SUPPORTED_PARAMETERS, ERDM_Parameter.PARAMETER_DESCRIPTION, ERDM_Parameter.DEVICE_INFO,
        ERDM_Parameter.METADATA_PARAMETER_VERSION, ERDM_Parameter.METADATA_JSON, ERDM_Parameter.METADATA_JSON_URL,
        ERDM_Parameter.SENSOR_DEFINITION, ERDM_Parameter.SENSOR_VALUE, ERDM_Parameter.RECORD_SENSORS,
        ERDM_Parameter.SLOT_INFO, ERDM_Parameter.SLOT_DESCRIPTION, ERDM_Parameter.DEFAULT_SLOT_VALUE,
        ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
    };

    public override string DisplayName { get; }
    public ERDM_Parameter Parameter { get; }
    /// <summary>The define of the parameter, <c>null</c> if none is known.</summary>
    public MetadataJSONObjectDefine Define { get; }
    /// <summary>A parameter the manufacturer marks as internal (service, test, firmware update, ...).</summary>
    public bool IsManufacturerInternal { get; }

    /// <summary>
    /// The value as the device stores it: an object, a raw value, or for an indexed parameter a dictionary index → value.
    /// <c>null</c> until the device answered (or for a SET only parameter).
    /// </summary>
    public object Value => ParentDevice is not null && ParentDevice.GetAllParameterValues().TryGetValue(Parameter, out object value) ? value : null;

    internal FallbackParameterModule(AbstractRemoteRDMDevice remoteDevice, ERDM_Parameter parameter, MetadataJSONObjectDefine define, string name, bool isManufacturerInternal)
        : base(remoteDevice, ModuleName(remoteDevice, parameter, define), parameter)
    {
        Parameter = parameter;
        Define = define;
        IsManufacturerInternal = isManufacturerInternal;
        DisplayName = define is not null
            ? GenericModuleFactory.DisplayNameOf(define)
            // without a define the metadata name is the PID as decimal number (e.g. "32724"), the hex PID reads better
            : !string.IsNullOrWhiteSpace(name) && !name.All(char.IsDigit) ? name
            : System.Enum.IsDefined(parameter) ? parameter.ToString()
            : $"0x{(ushort)parameter:X4}";
    }

    /// <summary>Unique per manufacturer and PID, like the generic modules, e.g. <c>Fallback_0x5347_0xA108</c>.</summary>
    private static string ModuleName(AbstractRemoteRDMDevice device, ERDM_Parameter parameter, MetadataJSONObjectDefine define)
    {
        ushort manufacturer = define?.ManufacturerID ?? ((ushort)parameter >= 0x8000 ? device?.UID.ManufacturerID ?? 0 : (ushort)0);
        return $"Fallback_0x{manufacturer:X4}_0x{(ushort)parameter:X4}";
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (parameter == Parameter)
            OnPropertyChanged(nameof(Value));
    }

    public override string ToString() => $"{DisplayName}: {Value}";
}
