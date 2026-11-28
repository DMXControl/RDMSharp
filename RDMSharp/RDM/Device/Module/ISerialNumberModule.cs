namespace RDMSharp.RDM.Device.Module;

public interface ISerialNumberModule : IModule
{
    string SerialNumber { get; }
}