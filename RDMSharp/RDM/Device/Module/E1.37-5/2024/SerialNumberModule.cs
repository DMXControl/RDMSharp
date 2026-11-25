using System;
using System.Text;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// SERIAL_NUMBER (E1.37-5 4.7): the manufacturer's serial number of the device.
/// GET only, a string of up to 231 bytes. A longer serial has to be truncated by the manufacturer.
/// </summary>
public sealed class SerialNumberModule : AbstractSimpleStringParameterIDModule
{
    private const string _moduleName = "SerialNumber";
    private const string _moduleDisplayName = "Serial Number";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.SERIAL_NUMBER;
    public const int MaximumLength = 231;

    public override string DisplayName => _moduleDisplayName;

    public string SerialNumber
    {
        get
        {
            return base.Value;
        }
    }

    public SerialNumberModule(string serialNumber) : base(
        validate(serialNumber),
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public SerialNumberModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }

    private static string validate(string serialNumber)
    {
        if (serialNumber is null)
            throw new ArgumentException("The serial number must not be null.", nameof(serialNumber));
        if (Encoding.UTF8.GetByteCount(serialNumber) > MaximumLength)
            throw new ArgumentException($"The serial number must not be longer than {MaximumLength} bytes, it has to be truncated to a meaningful string (E1.37-5 4.7).", nameof(serialNumber));
        return serialNumber;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(SerialNumber));
    }
}
