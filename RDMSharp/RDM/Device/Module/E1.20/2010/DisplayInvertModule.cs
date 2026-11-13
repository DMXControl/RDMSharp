using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DisplayInvertModule : AbstractSimpleEnumModule<ERDM_DisplayInvert>
{
    private const string _moduleName = "DisplayInvert";
    private const string _moduleDisplayName = "Display Invert";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DISPLAY_INVERT;

    public sealed override string DisplayName => _moduleDisplayName;

    public ERDM_DisplayInvert? DisplayInvert
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

    public DisplayInvertModule(ERDM_DisplayInvert initialDisplayInvert = ERDM_DisplayInvert.AUTO) : base(
        initialDisplayInvert,
        _moduleName,
        _moduleParameter)
    {
    }
    public DisplayInvertModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(DisplayInvert));
    }


    public async Task<bool> SetDisplayInvert(ERDM_DisplayInvert displayInvert)
    {
        return await base.SetValue(displayInvert);
    }
}