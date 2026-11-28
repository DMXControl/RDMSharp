using System;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// FIRMWARE_URL (E1.37-5 4.4): URL of the product firmware on the manufacturer's website.
/// GET only. A URL longer than one packet is sent with ACK_OVERFLOW.
/// </summary>
public sealed class FirmwareUrlModule : AbstractSimpleStringParameterIDModule
{
    private const string _moduleName = "FirmwareUrl";
    private const string _moduleDisplayName = "Firmware URL";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.FIRMWARE_URL;

    public override string DisplayName => _moduleDisplayName;

    public string FirmwareUrl
    {
        get
        {
            return base.Value;
        }
    }

    public FirmwareUrlModule(string firmwareUrl) : base(
        validate(firmwareUrl),
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public FirmwareUrlModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(FirmwareUrl));
    }
}
