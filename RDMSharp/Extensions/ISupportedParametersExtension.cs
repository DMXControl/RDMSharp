using System;
using System.Threading.Tasks;

namespace RDMSharp.Extensions;

public interface ISupportedParametersExtension
{
    string Key { get; }
    EManufacturer Manufacturer { get; }

    ERDM_Parameter[] BlueprintModelParameters { get; }
    ERDM_Parameter[] BlueprintModelPersonalityParameters { get; }
    ERDM_Parameter[] ManufacturerInternalParameters { get; }

    Task RegisterAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler);

    bool TryGetParameterName(ERDM_Parameter parameter, out string name);
    bool TryGetParameterUpdateTimeMilliseconds(ERDM_Parameter parameter, out int milliseconds);
}