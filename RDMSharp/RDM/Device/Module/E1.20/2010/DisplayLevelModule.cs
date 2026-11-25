using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// DISPLAY_LEVEL (E1.20 10.9.2): intensity of the display of the user interface.
/// 0x00 = display off, 0xFF = full, values in between are a relative intensity.
/// A device without relative intensity interprets any non-zero value as on.
/// </summary>
public sealed class DisplayLevelModule : AbstractSimpleNumericParameterIDModule<byte>
{
    private const string _moduleName = "DisplayLevel";
    private const string _moduleDisplayName = "Display Level";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DISPLAY_LEVEL;
    public const byte Off = 0x00;
    public const byte Full = 0xFF;

    public sealed override string DisplayName => _moduleDisplayName;

    public byte? DisplayLevel
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

    public DisplayLevelModule(byte initialDisplayLevel = Full) : base(
        initialDisplayLevel,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public DisplayLevelModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(DisplayLevel));
    }

    public async Task<bool> SetDisplayLevel(byte displayLevel)
    {
        return await base.SetValue(displayLevel);
    }
}
