using System;
using System.Linq;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class DeviceInfoModule : AbstractModule
    {
        public RDMDeviceInfo DeviceInfo
        {
            get
            {
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DEVICE_INFO, out res))
                    return (RDMDeviceInfo)res;
                return null;
            }
            internal set
            {
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.DEVICE_INFO, value);
            }
        }
        private SoftwareVersionModule softwareVersionModule;
        private DMX_StartAddressModule dmxStartAddressModule;
        private DMX_PersonalityModule dmxPersonalityModule;
        private SensorsModule sensorsModule;

        public DeviceInfoModule() : base(
            "DeviceInfo",
            ERDM_Parameter.DEVICE_INFO)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            softwareVersionModule = device.Modules.OfType<SoftwareVersionModule>().FirstOrDefault();
            dmxStartAddressModule = device.Modules.OfType<DMX_StartAddressModule>().FirstOrDefault();
            dmxPersonalityModule = device.Modules.OfType<DMX_PersonalityModule>().FirstOrDefault();
            sensorsModule = device.Modules.OfType<SensorsModule>().FirstOrDefault();
            updateParameterValues();
        }
        private void updateParameterValues()
        {
            if (ParentDevice is null)
                return;
            this.DeviceInfo = new RDMDeviceInfo(1,
                                           0,
                                           ParentDevice.DeviceModelID,
                                           ParentDevice.ProductCategoryCoarse,
                                           ParentDevice.ProductCategoryFine,
                                           softwareVersionModule?.SoftwareVersionId ?? 0,
                                           dmx512Footprint: dmxPersonalityModule?.CurrentPersonalityFootprint ?? 0,
                                           dmx512CurrentPersonality: dmxPersonalityModule?.CurrentPersonality ?? 0,
                                           dmx512NumberOfPersonalities: dmxPersonalityModule?.PersonalitiesCount ?? 0,
                                           dmx512StartAddress: (dmxStartAddressModule?.DMXAddress ?? ushort.MaxValue),
                                           subDeviceCount: (ushort)(ParentDevice.SubDevices?.Where(sd => !sd.Subdevice.IsRoot).Count() ?? 0),
                                           sensorCount: (byte)(sensorsModule?.Sensors?.Count ?? 0));

            OnPropertyChanged(nameof(DeviceInfo));
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_INFO:
                    OnPropertyChanged(nameof(DeviceInfo));
                    break;
                case ERDM_Parameter.DMX_PERSONALITY:
                case ERDM_Parameter.DMX_START_ADDRESS:
                    updateParameterValues();
                    break;
            }
        }
    }
}