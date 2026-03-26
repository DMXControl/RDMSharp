using RDMSharp.PayloadObject;

namespace RDMSharp.RDM.Device.Module;

public sealed class SoftwareVersionModule : AbstractModule
{
    private const string _moduleName = "SoftwareVersion";
    private const string _moduleDisplayName = "Software Version";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.SOFTWARE_VERSION_LABEL;

    public override string DisplayName => _moduleDisplayName;

    private uint _softwareVersionId;
    private string _softwareVersionLabel;
    public uint SoftwareVersionId
    {
        get
        {
            if (ParentRemoteDevice is not null)
            {
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DEVICE_INFO, out res))
                    return ((RDMDeviceInfo)res).SoftwareVersionId;
            }
            return _softwareVersionId;
        }
        internal set
        {
            _softwareVersionId = value;
        }
    }
    public string SoftwareVersionLabel
    {
        get
        {
            if (ParentGeneratedDevice is null && ParentRemoteDevice is null)
                return _softwareVersionLabel;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.SOFTWARE_VERSION_LABEL, out res))
                return (string)res;
            return _softwareVersionLabel;
        }
        internal set
        {
            _softwareVersionLabel = value;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.SOFTWARE_VERSION_LABEL, value);
        }
    }
    public SoftwareVersionModule(uint softwareVersionId, string softwareVersionLabel) : base(
        _moduleName,
        _moduleParameter)
    {
        _softwareVersionId = softwareVersionId;
        _softwareVersionLabel = softwareVersionLabel;
    }
    public SoftwareVersionModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.SoftwareVersionId = _softwareVersionId;
        this.SoftwareVersionLabel = _softwareVersionLabel;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.SOFTWARE_VERSION_LABEL:
                OnPropertyChanged(nameof(SoftwareVersionLabel));
                break;
        }
    }
}