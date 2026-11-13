using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class PanInvertModule : AbstractSimpleBooleanModule
{
    private const string _moduleName = "PanInvert";
    private const string _moduleDisplayName = "Pan Invert";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.PAN_INVERT;

    public sealed override string DisplayName => _moduleDisplayName;
    public bool? PanInvert
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

    public PanInvertModule() : base(
        false,
        _moduleName,
        _moduleParameter)
    {
    }
    public PanInvertModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(PanInvert));
    }


    public async Task<bool> SetPanInvert(bool panInvert)
    {
        return await base.SetValue(panInvert);
    }
}