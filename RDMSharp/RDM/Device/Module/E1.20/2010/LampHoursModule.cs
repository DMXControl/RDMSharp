using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class LampHoursModule : AbstractSimpleNumericModule<uint>
{
    private const string _moduleName = "LampHours";
    private const string _moduleDisplayName = "Lamp Hours";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.LAMP_HOURS;

    public sealed override string DisplayName => _moduleDisplayName;

    public uint? LampHours
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

    public LampHoursModule(uint initialLampHours = 0) : base(
        initialLampHours,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public LampHoursModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(LampHours));
    }


    public async Task<bool> SetLampHours(uint lampHours)
    {
        return await base.SetValue(lampHours);
    }
}