using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class MACAddressBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 6;
    private static readonly string _formatIdentifyer = "mac-address";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for MAC-Address parsing.", nameof(data));

        MACAddress macAddress = new MACAddress(data.Take(_bytesLength));
        data = data.Skip(_bytesLength).ToArray();

        return macAddress;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is MACAddress macAddress)
            return macAddress;
        throw new ArgumentException($"Object is not of type {nameof(MACAddress)}", nameof(obj));
    }
}
