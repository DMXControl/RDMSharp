using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class FloatBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 4;
    private static readonly string _formatIdentifyer = "float";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for Float parsing.", nameof(data));

        float _float = BitConverter.ToSingle(data.Take(_bytesLength).ToArray(), 0);
        data = data.Skip(_bytesLength).ToArray();

        return _float;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is float _float)
            return BitConverter.GetBytes(_float);
        throw new ArgumentException($"Object is not of type Float", nameof(obj));
    }
}
