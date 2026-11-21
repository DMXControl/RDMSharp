using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DevicePowerCyclesModule : AbstractSimpleNumericModule<uint>
{
    private const string _moduleName = "DevicePowerCycles";
    private const string _moduleDisplayName = "Device Power Cycles";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_POWER_CYCLES;

    public sealed override string DisplayName => _moduleDisplayName;

    public uint? DevicePowerCycles
    {
        get
        {
            return base.Value;
        }
        set
        {
            base.Value = value;
        }
    }

    public DevicePowerCyclesModule(uint initialDevicePowerCycles = 0) : base(
        initialDevicePowerCycles,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public DevicePowerCyclesModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(DevicePowerCycles));
    }


    public async Task<bool> SetDevicePowerCycles(uint devicePowerCycles)
    {
        return await base.SetValue(devicePowerCycles);
    }
}