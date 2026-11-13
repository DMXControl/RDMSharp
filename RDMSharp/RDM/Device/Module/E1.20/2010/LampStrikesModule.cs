using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class LampStrikesModule : AbstractSimpleNumericModule<uint>
{
    private const string _moduleName = "LampStrikes";
    private const string _moduleDisplayName = "Lamp Strikes";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.LAMP_STRIKES;

    public sealed override string DisplayName => _moduleDisplayName;

    public uint? LampStrikes
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

    public LampStrikesModule(uint initialLampStrikes = 0) : base(
        initialLampStrikes,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public LampStrikesModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(LampStrikes));
    }


    public async Task<bool> SetLampStrikes(uint lampStrikes)
    {
        return await base.SetValue(lampStrikes);
    }
}