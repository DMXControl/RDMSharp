using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class Latin1BytesParser : AbstractStringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "latin1";
    private static readonly int _nullDelimiterBytesLength = 1;
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.Latin1;
    public override int NullDelimiterBytesLength => _nullDelimiterBytesLength;
}