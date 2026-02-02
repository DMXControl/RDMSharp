using System;
using System.Threading.Tasks;

namespace RDMSharp.Extensions.SupportedParametersExtension;

public sealed class SupportedParametersExtension : AbstractSupportedParametersExtension
{
    private const string _key = "BasicSupportedParametersExtension";
    private const EManufacturer _manufacturer = EManufacturer.ESTA;
    public sealed override string Key => _key;
    public sealed override EManufacturer Manufacturer => _manufacturer;

    public sealed override ERDM_Parameter[] BlueprintModelParameters => Constants.BLUEPRINT_MODEL_PARAMETERS;

    public sealed override ERDM_Parameter[] BlueprintModelPersonalityParameters => Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS;

    protected override async Task registerAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler)
    {
        await handler.Invoke(new ERDM_Parameter[0]);
    }
}