using RDMSharp.RDM;

namespace RDMSharp.Extensions;

public interface IBytesParser
{
    string FormatIdentifyer { get; }
    PDL? PayloadDataLength { get; }

    object ParseToObject(ref byte[] data);
    byte[] ParseToData(object obj);
}