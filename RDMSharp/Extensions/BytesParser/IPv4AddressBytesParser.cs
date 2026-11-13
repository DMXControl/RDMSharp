using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class IPv4AddressBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 4;
    private static readonly string _formatIdentifyer = "ipv4";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for IPv4 parsing.", nameof(data));

        IPv4Address ipv4Address = new IPv4Address(data.Take(_bytesLength));
        data = data.Skip(_bytesLength).ToArray();

        return ipv4Address;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is IPv4Address ipv4Address)
            return ipv4Address;
        throw new ArgumentException($"Object is not of type {nameof(IPv4Address)}", nameof(obj));
    }
}
