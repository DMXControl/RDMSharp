namespace RDMSharp.ParameterWrapper
{
    public interface IRDMDeviceModelIdParameterWrapper : IRDMManufacturerParameterWrapper
    {
        ushort[] DeviceModelIds { get; }
    }
}