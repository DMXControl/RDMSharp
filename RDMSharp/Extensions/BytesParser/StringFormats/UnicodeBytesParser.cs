using System.Text;

namespace RDMSharp.Extensions.BytesParser.StringFormats;

public sealed class UnicodeBytesParser : StringFormatBytesParser
{
    private static readonly string _formatIdentifyer = "unicode";
    public override sealed string FormatIdentifyer => _formatIdentifyer;
    public override sealed Encoding Encoding => Encoding.Unicode;
}