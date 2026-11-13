using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class PanTiltSwapModule : AbstractSimpleBooleanModule
{
    private const string _moduleName = "PanTiltSwap";
    private const string _moduleDisplayName = "Pan Tilt Swap";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.PAN_TILT_SWAP;

    public sealed override string DisplayName => _moduleDisplayName;
    public bool? PanTiltSwap
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

    public PanTiltSwapModule() : base(
        false,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public PanTiltSwapModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(PanTiltSwap));
    }


    public async Task<bool> SetPanTiltSwap(bool panTiltSwap)
    {
        return await base.SetValue(panTiltSwap);
    }
}