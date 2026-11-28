using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class UnicodeBytesParser : AbstractStringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "unicode";
    private static readonly int _nullDelimiterBytesLength = 2;
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.Unicode;
    public override int NullDelimiterBytesLength => _nullDelimiterBytesLength;
}