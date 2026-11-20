using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// IDENTIFY_MODE (E1.37-1 3.2): how the device identifies itself with IDENTIFY_DEVICE.
/// 0x00 = Quiet Identify (e.g. a front panel indicator), 0xFF = Loud Identify (e.g. strobing),
/// the values 0x01-0xFE are not defined and rejected.
/// </summary>
public sealed class IdentifyModeModule : AbstractSimpleEnumModule<ERDM_IdentifyMode>
{
    private const string _moduleName = "IdentifyMode";
    private const string _moduleDisplayName = "Identify Mode";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.IDENTIFY_MODE;

    public sealed override string DisplayName => _moduleDisplayName;

    public ERDM_IdentifyMode? IdentifyMode
    {
        get
        {
            return base.Value;
        }
        set
        {
            base.Value = value;
        }
    }

    public IdentifyModeModule(ERDM_IdentifyMode initialIdentifyMode = ERDM_IdentifyMode.QUIET) : base(
        initialIdentifyMode,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public IdentifyModeModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(IdentifyMode));
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (ParentGeneratedDevice is not null && parameter == ModuleParameter && command == ERDM_Command.SET_COMMAND)
            return true;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (ParentGeneratedDevice is not null && message.Parameter == ModuleParameter && message.Command == ERDM_Command.SET_COMMAND)
        {
            byte[] data = message.ParameterData;
            if (data is null || data.Length != 1)
                return createNack(message, ERDM_NackReason.FORMAT_ERROR);
            if (data[0] != (byte)ERDM_IdentifyMode.QUIET && data[0] != (byte)ERDM_IdentifyMode.LOUD)
                return createNack(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
            return null; // valid, the generic handling stores the new mode
        }
        return base.handleRequest(message);
    }
    private static RDMMessage createNack(RDMMessage request, ERDM_NackReason nackReason)
    {
        return new RDMMessage(nackReason)
        {
            SourceUID = request.DestUID,
            DestUID = request.SourceUID,
            SubDevice = request.SubDevice,
            Command = request.Command | ERDM_Command.RESPONSE,
            Parameter = request.Parameter
        };
    }

    public async Task<bool> SetIdentifyMode(ERDM_IdentifyMode identifyMode)
    {
        return await base.SetValue(identifyMode);
    }
}
