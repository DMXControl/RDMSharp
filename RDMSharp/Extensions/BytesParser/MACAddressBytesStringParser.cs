using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class MACAddressBytesStringParser : IBytesParser
{
    private static readonly byte _bytesLength = 18;
    private static readonly string _formatIdentifyer = "mac-address.string";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for MAC-Address parsing.", nameof(data));

        var macString = Tools.DataToString(ref data, _bytesLength); // Skip the string representation of the MAC address
        MACAddress macAddress = new MACAddress(macString);
        data = data.Skip(_bytesLength).ToArray();

        return macAddress;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is MACAddress macAddress)
            return Tools.ValueToData(macAddress.ToString());
        throw new ArgumentException($"Object is not of type {nameof(MACAddress)}", nameof(obj));
    }
}
