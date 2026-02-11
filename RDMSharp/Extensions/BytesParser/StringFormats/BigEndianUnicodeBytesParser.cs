using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class BigEndianUnicodeBytesParser : AbstractStringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "big_edian_unicode";
    private static readonly int _nullDelimiterBytesLength = 2;
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.BigEndianUnicode;
    public override int NullDelimiterBytesLength => _nullDelimiterBytesLength;
}