using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class TiltInvertModule : AbstractSimpleBooleanModule
{
    private const string _moduleName = "TiltInvert";
    private const string _moduleDisplayName = "Tilt Invert";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.TILT_INVERT;

    public sealed override string DisplayName => _moduleDisplayName;
    public bool? TiltInvert
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

    public TiltInvertModule() : base(
        false,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public TiltInvertModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(TiltInvert));
    }


    public async Task<bool> SetTiltInvert(bool tiltInvert)
    {
        return await base.SetValue(tiltInvert);
    }
}