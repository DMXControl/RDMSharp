using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// DEVICE_UNIT_NUMBER (E1.37-5 4.15): a 32-bit unit number stored in the device to identify it in the rig.
/// Valid unit numbers are 0x00000001 - 0xFFFFFFFF, 0x00000000 means "un-set".
/// </summary>
public sealed class DeviceUnitNumberModule : AbstractSimpleNumericParameterIDModule<uint>, IDeviceUnitNumberModule
{
    private const string _moduleName = "DeviceUnitNumber";
    private const string _moduleDisplayName = "Device Unit Number";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_UNIT_NUMBER;
    public const uint Unset = 0;

    public sealed override string DisplayName => _moduleDisplayName;

    public uint? DeviceUnitNumber
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

    public DeviceUnitNumberModule(uint initialDeviceUnitNumber = Unset) : base(
        initialDeviceUnitNumber,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public DeviceUnitNumberModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(DeviceUnitNumber));
    }

    public async Task<bool> SetDeviceUnitNumber(uint deviceUnitNumber)
    {
        return await base.SetValue(deviceUnitNumber);
    }
}
