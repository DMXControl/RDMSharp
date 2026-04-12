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
            if (ParentDevice is null)
                return _deviceLabel;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DEVICE_LABEL, out res))
                return (string)res;
            return _deviceLabel;
        }
        set
        {
            _deviceLabel = value;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DEVICE_LABEL, value);

            if (ParentRemoteDevice is not null)
                _ = ParentRemoteDevice.SetParameter(ERDM_Parameter.DEVICE_LABEL, value);
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
}