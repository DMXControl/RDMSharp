using System;

namespace RDMSharp.RDM.Device.Module;

public sealed class ManufacturerLabelModule : AbstractModule
{
    private const string _moduleName = "ManufacturerLabel";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.MANUFACTURER_LABEL;

    private string _manufacturerLabel;
    public string ManufacturerLabel
    {
        get
        {
            if (ParentDevice is null)
                return _manufacturerLabel;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.MANUFACTURER_LABEL, out res))
                return (string)res;
            return _manufacturerLabel;
        }
        internal set
        {
            _manufacturerLabel = value;
            if (ParentDevice is not null)
                ParentDevice.setParameterValue(ERDM_Parameter.MANUFACTURER_LABEL, value);
        }
    }
    public ManufacturerLabelModule(string manufacturerLabel) : base(
        _moduleName,
        _moduleParameter)
    {
        _manufacturerLabel = manufacturerLabel;
    }
    public ManufacturerLabelModule(IRDMRemoteDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (string.IsNullOrWhiteSpace(_manufacturerLabel))
            _manufacturerLabel = Enum.GetName(typeof(EManufacturer), (EManufacturer)device.UID.ManufacturerID);
        this.ManufacturerLabel = _manufacturerLabel;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.MANUFACTURER_LABEL:
                OnPropertyChanged(nameof(ManufacturerLabel));
                break;
        }
    }
}