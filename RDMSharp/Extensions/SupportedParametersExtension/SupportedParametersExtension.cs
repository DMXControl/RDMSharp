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

        if (!parameters.Contains(ERDM_Parameter.QUEUED_MESSAGE))
        {
            supportedParameters.Add(ERDM_Parameter.QUEUED_MESSAGE);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for QUEUED_MESSAGE Parameter.");
        }

        // Remote Device not send DMX_START_ADDRESS Parameter but uses it!
        if (deviceInfo.Dmx512StartAddress.HasValue &&
            deviceInfo.Dmx512StartAddress >= 1 &&
            deviceInfo.Dmx512StartAddress.Value <= 512 &&
            !parameters.Contains(ERDM_Parameter.DMX_START_ADDRESS))
        {
            supportedParameters.Add(ERDM_Parameter.DMX_START_ADDRESS);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for DMX_START_ADDRESS Parameter based on Start Address value {deviceInfo.Dmx512StartAddress.Value}.");
        }

        // Remote Device not send DMX_PERSONALITY Parameter but uses it!
        if (deviceInfo.Dmx512CurrentPersonality.HasValue &&
            !parameters.Contains(ERDM_Parameter.DMX_PERSONALITY))
        {
            supportedParameters.Add(ERDM_Parameter.DMX_PERSONALITY);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for DMX_PERSONALITY Parameter based on Current Personality value {deviceInfo.Dmx512CurrentPersonality.Value}.");
        }

        // Remote Device not send DMX_PERSONALITY Parameter but uses it!
        if (deviceInfo.Dmx512NumberOfPersonalities != 0 &&
            !parameters.Contains(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION))
        {
            supportedParameters.Add(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for DMX_PERSONALITY_DESCRIPTION Parameter based on Number of Personalities value {deviceInfo.Dmx512NumberOfPersonalities}.");
        }

        if (parameters.Any(p => ((ushort)p) >= 0x8000 &&
                                ((ushort)p) <= 0xFFDF))
        {
            // Remote Device not send PARAMETER_DESCRIPTION Parameter but has Manufacture speific Parameters it!
            if (!parameters.Contains(ERDM_Parameter.PARAMETER_DESCRIPTION))
            {
                supportedParameters.Add(ERDM_Parameter.PARAMETER_DESCRIPTION);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for PARAMETER_DESCRIPTION Parameter based on presence of Manufacturer Specific Parameters.");
            }

            // Remote Device not send METADATA_PARAMETER_VERSION Parameter but has Manufacture speific Parameters it!
            if (!parameters.Contains(ERDM_Parameter.METADATA_PARAMETER_VERSION))
            {
                supportedParameters.Add(ERDM_Parameter.METADATA_PARAMETER_VERSION);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for METADATA_PARAMETER_VERSION Parameter based on presence of Manufacturer Specific Parameters.");
            }
            // Remote Device not send METADATA_JSON Parameter but has Manufacture speific Parameters it!
            if (!parameters.Contains(ERDM_Parameter.METADATA_JSON))
            {
                supportedParameters.Add(ERDM_Parameter.METADATA_JSON);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for METADATA_JSON Parameter based on presence of Manufacturer Specific Parameters.");
            }
        }

        #region Assuming support for DESCRIPTION Parameters based on presence of related Parameters
        //Test it if the device supports CURVE Parameter, if not it will be labled as not supported later on
        if (parameters.Contains(ERDM_Parameter.CURVE) && !parameters.Contains(ERDM_Parameter.CURVE_DESCRIPTION))
        {
            supportedParameters.Add(ERDM_Parameter.CURVE_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for CURVE_DESCRIPTION Parameter.");
        }

        //Test it if the device supports OUTPUT_RESPONSE_TIME Parameter, if not it will be labled as not supported later on
        if (parameters.Contains(ERDM_Parameter.OUTPUT_RESPONSE_TIME) && !parameters.Contains(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION))
        {
            supportedParameters.Add(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for OUTPUT_RESPONSE_TIME_DESCRIPTION Parameter.");
        }

        //Test it if the device supports MODULATION_FREQUENCY Parameter, if not it will be labled as not supported later on
        if (parameters.Contains(ERDM_Parameter.MODULATION_FREQUENCY) && !parameters.Contains(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION))
        {
            supportedParameters.Add(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for MODULATION_FREQUENCY_DESCRIPTION Parameter.");
        }

        //Test it if the device supports LOCK_STATE Parameter, if not it will be labled as not supported later on
        if (parameters.Contains(ERDM_Parameter.LOCK_STATE) && !parameters.Contains(ERDM_Parameter.LOCK_STATE_DESCRIPTION))
        {
            supportedParameters.Add(ERDM_Parameter.LOCK_STATE_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for LOCK_STATE_DESCRIPTION Parameter.");
        }
        #endregion


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

        // Remote Device not send Required Tag Parameters but has at least one Tag Parameter!
        if (parameters.Any(p => ((ushort)p) >= 0x0651 &&
                                ((ushort)p) <= 0x0655))
        {
            if (!parameters.Contains(ERDM_Parameter.LIST_TAGS))
            {
                supportedParameters.Add(ERDM_Parameter.LIST_TAGS);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for LIST_TAGS Parameter based on presence of Tag Parameters.");
            }
            if (!parameters.Contains(ERDM_Parameter.ADD_TAG))
            {
                supportedParameters.Add(ERDM_Parameter.ADD_TAG);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for ADD_TAG Parameter based on presence of Tag Parameters.");
            }
            if (!parameters.Contains(ERDM_Parameter.REMOVE_TAG))
            {
                supportedParameters.Add(ERDM_Parameter.REMOVE_TAG);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for REMOVE_TAG Parameter based on presence of Tag Parameters.");
            }
            if (!parameters.Contains(ERDM_Parameter.CHECK_TAG))
            {
                supportedParameters.Add(ERDM_Parameter.CHECK_TAG);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for CHECK_TAG Parameter based on presence of Tag Parameters.");
            }
            if (!parameters.Contains(ERDM_Parameter.CLEAR_TAGS))
            {
                supportedParameters.Add(ERDM_Parameter.CLEAR_TAGS);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for CLEAR_TAGS Parameter based on presence of Tag Parameters.");
            }
        }

        // Remote Device not send LIST_INTERFACES Parameter but has Interface Parameters it!
        if (parameters.Any(p => ((ushort)p) > 0x7000 &&
                                ((ushort)p) <= 0x700D))
        {
            if (!parameters.Contains(ERDM_Parameter.LIST_INTERFACES))
            {
                supportedParameters.Add(ERDM_Parameter.LIST_INTERFACES);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for LIST_INTERFACES Parameter based on presence of Interface Parameters.");
            }
        }

        // Remote Device not send ENDPOINT_LIST Parameter but has Endpoint Parameters it!
        if (parameters.Any(p => ((ushort)p) > 0x9000 &&
                                ((ushort)p) <= 0x900D))
        {
            if (!parameters.Contains(ERDM_Parameter.ENDPOINT_LIST))
            {
                supportedParameters.Add(ERDM_Parameter.ENDPOINT_LIST);
                Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for ENDPOINT_LIST Parameter based on presence of Endpoint Parameters.");
            }
        }

        if (parameters.Contains(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY) &&
           !parameters.Contains(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION))
        {
            supportedParameters.Add(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION);
            Logger?.LogInformation($"Remote Device Model ID 0x{deviceInfo.DeviceModelId:X4} - Assuming support for BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION Parameter based on presence of BACKGROUND_QUEUED_STATUS_POLICY Parameter.");
        }

        await handler.Invoke(supportedParameters.ToArray());
    }
}