using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DeviceHoursModule : AbstractSimpleNumericModule<uint>
{
    private const string _moduleName = "DeviceHours";
    private const string _moduleDisplayName = "Device Hours";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_HOURS;

    public sealed override string DisplayName => _moduleDisplayName;

    public uint? DeviceHours
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

    public DeviceHoursModule(uint initialDeviceHours = 0) : base(
        initialDeviceHours,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public DeviceHoursModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(DeviceHours));
    }


    public async Task<bool> SetDeviceHours(uint deviceHours)
    {
        return await base.SetValue(deviceHours);
    }
}