using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.Extensions.SupportedParametersExtension;

public sealed class SupportedParametersExtension : AbstractSupportedParametersExtension
{
    private const string _key = "BasicSupportedParametersExtension";
    private const EManufacturer _manufacturer = EManufacturer.ESTA;
    private static readonly ERDM_Parameter[] _manufacturerInternalParameters = new ERDM_Parameter[0];
    public sealed override string Key => _key;
    public sealed override EManufacturer Manufacturer => _manufacturer;

    public sealed override ERDM_Parameter[] BlueprintModelParameters => Constants.BLUEPRINT_MODEL_PARAMETERS;

    public sealed override ERDM_Parameter[] BlueprintModelPersonalityParameters => Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS;

    public sealed override ERDM_Parameter[] ManufacturerInternalParameters => _manufacturerInternalParameters;

    public sealed override bool TryGetParameterName(ERDM_Parameter parameter, out string name)
    {
        if (((ushort)parameter) >= 0x8000 && ((ushort)parameter) <= 0xFFDF)
        {
            name = null;
            return false;
        }

        var define = MetadataFactory.GetDefine(new ParameterBag(parameter));
        if (define is not MetadataJSONObjectDefine parameterDefine)
        {
            name = parameter.ToString();
            return true;
        }

        name = define.DisplayName ?? define.Name ?? parameter.ToString();
        return true;
    }

    public sealed override bool TryGetParameterUpdateTimeMilliseconds(ERDM_Parameter parameter, out int milliseconds)
    {
        if (((ushort)parameter) >= 0x8000 && ((ushort)parameter) <= 0xFFDF)
        {
            milliseconds = -1;
            return false;
        }
        if (parameter.GetAttribute<ParameterUpdateTimeAttribute>() is not ParameterUpdateTimeAttribute attribute)
        {
            milliseconds = -1;
            return false;
        }
        milliseconds = attribute.Milliseconds;
        return true;
    }

    protected override async Task registerAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler)
    {
        var parameters = deviceModel.GetSupportedParameters().Select(spm => spm.Parameter).ToArray();
        List<ERDM_Parameter> supportedParameters = new List<ERDM_Parameter>();
        var deviceInfo = deviceModel.DeviceInfo;

        // Remote Device not send DMX_START_ADDRESS Parameter but uses it!
        if (deviceInfo.Dmx512StartAddress.HasValue && deviceInfo.Dmx512StartAddress >= 1 && deviceInfo.Dmx512StartAddress.Value <= 512)
        {
            supportedParameters.Add(ERDM_Parameter.DMX_START_ADDRESS);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for DMX_START_ADDRESS Parameter based on Start Address value {deviceInfo.Dmx512StartAddress.Value}.");
        }

        // Remote Device not send DMX_PERSONALITY Parameter but uses it!
        if (deviceInfo.Dmx512CurrentPersonality.HasValue)
        {
            supportedParameters.Add(ERDM_Parameter.DMX_PERSONALITY);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for DMX_PERSONALITY Parameter based on Current Personality value {deviceInfo.Dmx512CurrentPersonality.Value}.");
        }

        // Remote Device not send PARAMETER_DESCRIPTION Parameter but has Manufacture speific Parameters it!
        if (!parameters.Contains(ERDM_Parameter.PARAMETER_DESCRIPTION) && parameters.Any(p => ((ushort)p) >= 0x8000 && ((ushort)p) <= 0xFFDF))
        {
            supportedParameters.Add(ERDM_Parameter.PARAMETER_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for PARAMETER_DESCRIPTION Parameter based on presence of Manufacturer Specific Parameters.");
        }
        //Test it if the device supports Identify Device Parameter, if not it will be labled as not supported later on
        if (!parameters.Contains(ERDM_Parameter.IDENTIFY_DEVICE))
        {
            supportedParameters.Add(ERDM_Parameter.IDENTIFY_DEVICE);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for IDENTIFY_DEVICE Parameter.");
        }

        //Test it if the device supports Software Version Lable Device Parameter, if not it will be labled as not supported later on
        if (!parameters.Contains(ERDM_Parameter.SOFTWARE_VERSION_LABEL))
        {
            supportedParameters.Add(ERDM_Parameter.SOFTWARE_VERSION_LABEL);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for SOFTWARE_VERSION_LABEL Parameter.");
        }

        //Test it if the device supports Factory Defaults Device Parameter, if not it will be labled as not supported later on
        if (!parameters.Contains(ERDM_Parameter.FACTORY_DEFAULTS))
        {
            supportedParameters.Add(ERDM_Parameter.FACTORY_DEFAULTS);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for FACTORY_DEFAULTS Parameter.");
        }

        // Remote Device not send ENDPOINT_LIST Parameter but has Endpoint Parameters it!
        if (parameters.Any(p => ((ushort)p) > 0x9000 && ((ushort)p) <= 0x900D))
        {
            supportedParameters.Add(ERDM_Parameter.ENDPOINT_LIST);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for ENDPOINT_LIST Parameter based on presence of Endpoint Parameters.");
        }

        await handler.Invoke(supportedParameters.ToArray());
    }
}