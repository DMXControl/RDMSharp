using RDMSharp.RDM;
using System;
using System.Linq;
using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public abstract class AbstractStringFormatBytesParser : IBytesParser
{
    public abstract string FormatIdentifyer { get; }
    public abstract Encoding Encoding { get; }
    public abstract int NullDelimiterBytesLength { get; }
    public PDL? PayloadDataLength => null;


    public object ParseToObject(ref byte[] data)
    {
        return getNullDelimitetData(ref data);
    }

    public byte[] ParseToData(object obj)
    {
        if (obj is string _string)
            return Encoding.GetBytes(_string);
        throw new ArgumentException($"Object is not of type {nameof(String)}", nameof(obj));
    }
    string getNullDelimitetData(ref byte[] data)
    {
        int charSize = NullDelimiterBytesLength;
        int length = 0;

        while (length + charSize - 1 < data.Length)
        {
            bool isNull = true;

            for (int i = 0; i < charSize; i++)
            {
                if (data[length + i] != 0x00)
                {
                    isNull = false;
                    break;
                }
            }

            if (isNull)
                break;

            length += charSize;
        }

        var res = Encoding.GetString(data, 0, length);
        data = data.Skip(Math.Min(data.Length, length + charSize)).ToArray();
        return res;
    }
}
