using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public interface IDeviceUnitNumberModule : IModule
{
    uint? DeviceUnitNumber { get; }

    Task<bool> SetDeviceUnitNumber(uint value);
}
