using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.Extensions.ModulesExtension;

public abstract class ModulesExtension : AbstractModulesExtension
{
    private static readonly string _key = "BasicModulesExtension";
    private const EManufacturer _manufacturer = EManufacturer.ESTA;
    public sealed override string Key => _key;
    public sealed override EManufacturer Manufacturer => _manufacturer;

    public sealed override bool TryGetModules(ERDM_Parameter[] parameters, out IReadOnlyCollection<Type> modules)
    {
        var _modules = new List<Type>();

        if (parameters.Contains(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID) &&
            parameters.Contains(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL))
            _modules.Add(typeof(BootSoftwareVersionModule));

        if (parameters.Contains(ERDM_Parameter.DEVICE_INFO))
            _modules.Add(typeof(DeviceInfoModule));

        if (parameters.Contains(ERDM_Parameter.DEVICE_LABEL))
            _modules.Add(typeof(DeviceLabelModule));

        if (parameters.Contains(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION))
            _modules.Add(typeof(DeviceModelDescriptionModule));

        if (parameters.Contains(ERDM_Parameter.DMX_PERSONALITY) &&
            parameters.Contains(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION))
            _modules.Add(typeof(DMX_PersonalityModule));

        if (parameters.Contains(ERDM_Parameter.DMX_START_ADDRESS))
            _modules.Add(typeof(DMX_StartAddressModule));

        if (parameters.Contains(ERDM_Parameter.IDENTIFY_DEVICE))
            _modules.Add(typeof(IdentifyDeviceModule));

        if (parameters.Contains(ERDM_Parameter.LIST_INTERFACES))
            _modules.Add(typeof(InterfaceModule));

        if (parameters.Contains(ERDM_Parameter.MANUFACTURER_LABEL))
            _modules.Add(typeof(ManufacturerLabelModule));

        if (parameters.Contains(ERDM_Parameter.PRESET_PLAYBACK))
            _modules.Add(typeof(PresetsModule));

        if (parameters.Contains(ERDM_Parameter.PROXIED_DEVICES))
            _modules.Add(typeof(ProxiedDevicesModule));

        if (parameters.Contains(ERDM_Parameter.REAL_TIME_CLOCK))
            _modules.Add(typeof(RealTimeClockModule));

        if (parameters.Contains(ERDM_Parameter.PERFORM_SELFTEST))
            _modules.Add(typeof(SelfTestsModule));

        if (parameters.Contains(ERDM_Parameter.SENSOR_DEFINITION))
            _modules.Add(typeof(SensorsModule));

        if (parameters.Contains(ERDM_Parameter.SLOT_DESCRIPTION))
            _modules.Add(typeof(SlotsModule));

        if (parameters.Contains(ERDM_Parameter.SOFTWARE_VERSION_LABEL))
            _modules.Add(typeof(SoftwareVersionModule));

        if (parameters.Contains(ERDM_Parameter.STATUS_MESSAGES))
            _modules.Add(typeof(ProxiedDevicesModule));

        if (parameters.Contains(ERDM_Parameter.LIST_TAGS))
            _modules.Add(typeof(TagsModule));

        modules = _modules;
        return true;
    }
}