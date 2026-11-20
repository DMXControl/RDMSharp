using System;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// MANUFACTURER_URL (E1.37-5 4.2): URL of the manufacturer's website.
/// GET only. A URL longer than one packet is sent with ACK_OVERFLOW.
/// </summary>
public sealed class ManufacturerUrlModule : AbstractSimpleStringModule
{
    private const string _moduleName = "ManufacturerUrl";
    private const string _moduleDisplayName = "Manufacturer URL";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.MANUFACTURER_URL;

    public override string DisplayName => _moduleDisplayName;

    public string ManufacturerUrl
    {
        get
        {
            return base.Value;
        }
    }

    public ManufacturerUrlModule(string manufacturerUrl) : base(
        validate(manufacturerUrl),
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public ManufacturerUrlModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }

    private static string validate(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("The URL must not be empty.", nameof(url));
        return url;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(ManufacturerUrl));
    }
}
