using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class UTF8BytesParser : StringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "utf8";
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.UTF8;
}