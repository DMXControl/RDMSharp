using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class UTF8BytesParser : AbstractStringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "utf8";
    private static readonly int _nullDelimiterBytesLength = 1;
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.UTF8;
    public override int NullDelimiterBytesLength => _nullDelimiterBytesLength;
}