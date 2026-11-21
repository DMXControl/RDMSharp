using RDMSharp.RDM;
using System;

namespace RDMSharp.Extensions.BytesParser;

public class PIDBytesParser : IBytesParser
{
    private static readonly byte _bytesLength = 2;
    private static readonly string _formatIdentifyer = "pid";
    private static readonly PDL _payloadDataLength = new PDL(_bytesLength);
    public string FormatIdentifyer => _formatIdentifyer;
    public PDL? PayloadDataLength => _payloadDataLength;

    public object ParseToObject(ref byte[] data)
    {
        if (data.Length < _bytesLength)
            throw new ArgumentException("Data length is less than required for PID parsing.", nameof(data));

        ERDM_Parameter pid = Tools.DataToEnum<ERDM_Parameter>(ref data);
        return pid;
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is ERDM_Parameter pid)
            return Tools.ValueToData(pid);
        throw new ArgumentException($"Object is not of type {nameof(ERDM_Parameter)}", nameof(obj));
    }
}
