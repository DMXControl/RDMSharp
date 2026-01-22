using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class ASCIIBytesParser : StringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "ascii";
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.ASCII;
}