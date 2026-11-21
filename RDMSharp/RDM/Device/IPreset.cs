namespace RDMSharp;

public interface IPreset
{
    ushort ID { get; }
    double UpFadeTime { get; }
    double DownFadeTime { get; }
    double WaitTime { get; }
}
