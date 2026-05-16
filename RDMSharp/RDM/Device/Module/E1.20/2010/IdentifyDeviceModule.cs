using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class IdentifyDeviceModule : AbstractModule
{
    private const string _moduleName = "IdentifyDevice";
    private const string _moduleDisplayName = "Identify Device";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.IDENTIFY_DEVICE;

    public override string DisplayName => _moduleDisplayName;

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
        set
        {
            _identify = value;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IDENTIFY_DEVICE, value);

            if (ParentRemoteDevice is not null)
                _ = SetIdentify(value);
        }
    }

    public IdentifyDeviceModule() : base(
        _moduleName,
        _moduleParameter)
    {
    }
    public IdentifyDeviceModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
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

    public async Task<bool> SetIdentify(bool value)
    {
        return await ParentRemoteDevice.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE, value);
    }
}