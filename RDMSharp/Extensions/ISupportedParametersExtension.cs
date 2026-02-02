using System;
using System.Threading.Tasks;

namespace RDMSharp.Extensions;

public interface ISupportedParametersExtension
{
    string Key { get; }
    EManufacturer Manufacturer { get; }

    ERDM_Parameter[] BlueprintModelParameters { get; }
    ERDM_Parameter[] BlueprintModelPersonalityParameters { get; }

    Task RegisterAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler);
}