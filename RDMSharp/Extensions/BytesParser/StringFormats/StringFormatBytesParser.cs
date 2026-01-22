using RDMSharp.RDM;
using System;
using System.Linq;
using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public abstract class StringFormatBytesParser : IBytesParser
{
    public abstract string FormatIdentifyer { get; }
    public abstract Encoding Encoding { get; }
    public PDL? PayloadDataLength => null;

    public object ParseToObject(ref byte[] data)
    {
        return getNullDelimitetData(Encoding, ref data);
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is string _string)
            return Encoding.GetBytes(_string);
        throw new ArgumentException($"Object is not of type {nameof(String)}", nameof(obj));
    }
    string getNullDelimitetData(Encoding encoding, ref byte[] data)
    {
        string res = encoding.GetString(data);
        if (res.Contains('\0'))
        {
            res = res.Split('\0')[0];
            int count = encoding.GetByteCount(res + "\0");
            data = data.Skip(count).ToArray();
        }
        else
            data = new byte[0];
        return res;
    }
}
