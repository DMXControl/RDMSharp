using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class UTF32BytesParser : StringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "utf32";
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.UTF32;
}