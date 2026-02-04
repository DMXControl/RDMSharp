namespace RDMSharp.RDM.Device.Module;

public sealed class SoftwareVersionModule : AbstractModule
{
    private const string _moduleName = "SoftwareVersion";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.SOFTWARE_VERSION_LABEL;

    private uint _softwareVersionId;
    private string _softwareVersionLabel;
    public uint SoftwareVersionId
    {
        get
        {
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
            if (ParentDevice is null)
                return _softwareVersionLabel;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.SOFTWARE_VERSION_LABEL, out res))
                return (string)res;
            return _softwareVersionLabel;
        }
        internal set
        {
            _softwareVersionLabel = value;
            if (ParentDevice is not null)
                ParentDevice.setParameterValue(ERDM_Parameter.SOFTWARE_VERSION_LABEL, value);
        }
    }
    public SoftwareVersionModule(uint softwareVersionId, string softwareVersionLabel) : base(
        _moduleName,
        _moduleParameter)
    {
        _softwareVersionId = softwareVersionId;
        _softwareVersionLabel = softwareVersionLabel;
    }
    public SoftwareVersionModule(IRDMRemoteDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
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