using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class GuidBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 16;
    private static readonly string _formatIdentifyer = "guid";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for Guid parsing.", nameof(data));

        Guid guid = new Guid(data.Take(_bytesLength).ToArray());
        data = data.Skip(_bytesLength).ToArray();

        return guid;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is Guid guid)
            return guid.ToByteArray();
        throw new ArgumentException($"Object is not of type {nameof(Guid)}", nameof(obj));
    }
}
