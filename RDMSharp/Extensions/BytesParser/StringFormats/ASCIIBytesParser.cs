using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class ASCIIBytesParser : AbstractStringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "ascii";
    private static readonly int _nullDelimiterBytesLength = 1;
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.ASCII;
    public override int NullDelimiterBytesLength => _nullDelimiterBytesLength;
}