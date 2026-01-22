using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class BigEndianUnicodeBytesParser : StringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "big_edian_unicode";
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.BigEndianUnicode;
}