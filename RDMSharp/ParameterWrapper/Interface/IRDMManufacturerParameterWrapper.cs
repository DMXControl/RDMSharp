namespace RDMSharp.ParameterWrapper
{
    public interface IRDMManufacturerParameterWrapper : IRDMParameterWrapper
    {
        EManufacturer Manufacturer { get; }
    }
}