using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class IPv6AddressBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 16;
    private static readonly string _formatIdentifyer = "ipv6";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for IPv6 parsing.", nameof(data));

        IPv6Address ipv6Address = new IPv6Address(data.Take(_bytesLength));
        data = data.Skip(_bytesLength).ToArray();

        return ipv6Address;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is IPv6Address ipv6Address)
            return ipv6Address;
        throw new ArgumentException($"Object is not of type {nameof(IPv6Address)}", nameof(obj));
    }
}
