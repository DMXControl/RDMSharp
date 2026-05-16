using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DeviceLabelModule : AbstractModule
{
    private const string _moduleName = "DeviceLabel";
    private const string _moduleDisplayName = "Device Label";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_LABEL;

    public override string DisplayName => _moduleDisplayName;

    private string _deviceLabel;
    public string DeviceLabel
    {
        get
        {
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DEVICE_LABEL, out object res) && res is string label)
                return label;
            return null;
        }
        set
        {
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DEVICE_LABEL, value);

            if (ParentRemoteDevice is not null)
                _ = SetLabel(value);
        }
    }
    public DeviceLabelModule(string deviceLabel) : base(
        _moduleName,
        _moduleParameter)
    {
        _deviceLabel = deviceLabel;
    }
    public DeviceLabelModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.DeviceLabel = _deviceLabel;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.DEVICE_LABEL:
                OnPropertyChanged(nameof(DeviceLabel));
                break;
        }
    }
    public async Task<bool> SetLabel(string label)
    {
        if (ParentGeneratedDevice is not null)
            DeviceLabel = label;
        if (ParentRemoteDevice is not null)
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.DEVICE_LABEL, label);

        return true;
    }
}