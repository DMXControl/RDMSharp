namespace RDMSharp.RDM.Device.Module;

public sealed class IdentifyDeviceModule : AbstractModule
{
    private const string _moduleName = "IdentifyDevice";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.IDENTIFY_DEVICE;

    private bool _identify;
    public bool Identify
    {
        get
        {
            if (ParentDevice is null)
                return _identify;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.IDENTIFY_DEVICE, out res))
                return (bool)res;
            return _identify;
        }
        internal set
        {
            _identify = value;
            if (ParentDevice is not null)
                ParentDevice.setParameterValue(ERDM_Parameter.IDENTIFY_DEVICE, value);
        }
    }

    public IdentifyDeviceModule() : base(
        _moduleName,
        _moduleParameter)
    {
    }
    public IdentifyDeviceModule(IRDMRemoteDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.Identify = _identify;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.IDENTIFY_DEVICE:
                OnPropertyChanged(nameof(Identify));
                break;
        }
    }
}