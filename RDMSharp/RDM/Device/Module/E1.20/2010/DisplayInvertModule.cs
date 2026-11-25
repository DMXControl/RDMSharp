using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DisplayInvertModule : AbstractSimpleEnumParameterIDModule<ERDM_DisplayInvert>
{
    private const string _moduleName = "DisplayInvert";
    private const string _moduleDisplayName = "Display Invert";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DISPLAY_INVERT;

    public sealed override string DisplayName => _moduleDisplayName;

    /// <summary>
    /// Robe devices (e.g. MegaPointe FW 2.9, Spiider 4.0, iEsprite LTL 1.7) report Auto as 0x03 instead of 0x02.
    /// </summary>
    private const ERDM_DisplayInvert ROBE_AUTO = (ERDM_DisplayInvert)0x03;
    private bool robeAutoLogged = false;

    public ERDM_DisplayInvert? DisplayInvert
    {
        get
        {
            var value = base.Value;
            if (value == ROBE_AUTO && ParentRemoteDevice is not null && ParentRemoteDevice.UID.ManufacturerID == (ushort)EManufacturer.Robe_Show_Lighting_sro)
            {
                if (!robeAutoLogged)
                {
                    robeAutoLogged = true;
                    Logger?.LogInformation($"{ParentRemoteDevice.UID} reports DISPLAY_INVERT 0x03, which Robe uses for Auto (0x02)");
                }
                return ERDM_DisplayInvert.AUTO;
            }
            return value;
        }
        set
        {
            base.Value = value;
        }
    }

    public DisplayInvertModule(ERDM_DisplayInvert initialDisplayInvert = ERDM_DisplayInvert.AUTO) : base(
        initialDisplayInvert,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public DisplayInvertModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(DisplayInvert));
    }


    public async Task<bool> SetDisplayInvert(ERDM_DisplayInvert displayInvert)
    {
        return await base.SetValue(displayInvert);
    }
}