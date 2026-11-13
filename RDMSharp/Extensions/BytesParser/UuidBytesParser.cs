using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class UuidBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 16;
    private static readonly string _formatIdentifyer = "uuid";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for Uuid parsing.", nameof(data));

        Guid uuid = new Guid(data.Take(_bytesLength).ToArray());
        data = data.Skip(_bytesLength).ToArray();

        return uuid;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is Guid uuid)
            return uuid.ToByteArray();
        throw new ArgumentException($"Object is not of type {nameof(Guid)}", nameof(obj));
    }
}
