namespace RDMSharp.RDM.Device.Module;

public sealed class PresetsModule : AbstractModule
{
    private const string _moduleName = "Presets";
    private const string _moduleDisplayName = "Presets";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.PRESET_PLAYBACK,
        ERDM_Parameter.CAPTURE_PRESET
    };
    public override string DisplayName => _moduleDisplayName;


    public PresetsModule() : base(
        _moduleName,
        _moduleParameters)
    {
    }
    public PresetsModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        throw new System.NotImplementedException();
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        throw new System.NotImplementedException();
    }

    //protected override RDMMessage handleRequest(RDMMessage message)
    //{
    //    return null;
    //}
}