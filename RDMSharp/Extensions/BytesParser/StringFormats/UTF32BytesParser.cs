using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class UTF32BytesParser : AbstractStringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "utf32";
    private static readonly int _nullDelimiterBytesLength = 4;
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.UTF32;
    public override int NullDelimiterBytesLength => _nullDelimiterBytesLength;
}