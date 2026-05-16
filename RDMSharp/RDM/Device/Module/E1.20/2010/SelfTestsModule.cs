namespace RDMSharp.RDM.Device.Module;

public sealed class SelfTestsModule : AbstractModule
{
    private const string _moduleName = "SelfTests";
    private const string _moduleDisplayName = "Self Tests";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.PERFORM_SELFTEST,
        ERDM_Parameter.SELF_TEST_DESCRIPTION
    };

    public override string DisplayName => _moduleDisplayName;

    public SelfTestsModule() : base(
        _moduleName,
        _moduleParameters)
    {
    }
    public SelfTestsModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
        //throw new System.NotImplementedException();
    }

    //protected override RDMMessage handleRequest(RDMMessage message)
    //{
    //    return null;
    //}
}