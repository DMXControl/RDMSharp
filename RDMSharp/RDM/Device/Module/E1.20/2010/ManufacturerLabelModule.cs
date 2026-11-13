using System;

namespace RDMSharp.RDM.Device.Module;

public sealed class ManufacturerLabelModule : AbstractSimpleStringModule
{
    private const string _moduleName = "ManufacturerLabel";
    private const string _moduleDisplayName = "Manufacturer Label";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.MANUFACTURER_LABEL;

    public override string DisplayName => _moduleDisplayName;

    public string ManufacturerLabel
    {
        get
        {
            return base.Value;
        }
        private set
        {
            base.Value = value;
        }
    }
    public ManufacturerLabelModule(string manufacturerLabel = null) : base(
        manufacturerLabel,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public ManufacturerLabelModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (string.IsNullOrWhiteSpace(ManufacturerLabel))
            ManufacturerLabel = Enum.GetName(typeof(EManufacturer), (EManufacturer)device.UID.ManufacturerID);

        base.OnParentGeneratedDeviceChanged(device);
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(ManufacturerLabel));
    }
}