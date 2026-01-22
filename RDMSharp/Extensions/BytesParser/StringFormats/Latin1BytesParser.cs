using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class Latin1BytesParser : StringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "latin1";
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.Latin1;
}