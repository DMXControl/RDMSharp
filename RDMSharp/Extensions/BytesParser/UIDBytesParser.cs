using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Extensions.BytesParser;

public class UIDBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 6;
    private static readonly string _formatIdentifyer = "uid";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for UID parsing.", nameof(data));

        UID uid = new UID(Tools.DataToUShort(ref data), Tools.DataToUInt(ref data));
        return uid;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is UID uid)
            return uid.ToBytes().ToArray();
        throw new ArgumentException($"Object is not of type {nameof(UID)}", nameof(obj));
    }
}
