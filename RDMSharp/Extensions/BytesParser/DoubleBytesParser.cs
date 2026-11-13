using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class DoubleBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 8;
    private static readonly string _formatIdentifyer = "double";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for Double parsing.", nameof(data));

        double _double = BitConverter.ToDouble(data.Take(_bytesLength).ToArray(), 0);
        data = data.Skip(_bytesLength).ToArray();

        return _double;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is double _double)
            return BitConverter.GetBytes(_double);
        throw new ArgumentException($"Object is not of type Double", nameof(obj));
    }
}
