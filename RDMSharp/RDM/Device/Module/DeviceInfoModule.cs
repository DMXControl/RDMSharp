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

        public DeviceInfoModule() : base(
            "DeviceInfo",
            ERDM_Parameter.DEVICE_INFO)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            var softwareVersionModule = device.Modules.OfType<SoftwareVersionModule>().FirstOrDefault();
            var dmxStartAddressModule = device.Modules.OfType<DMX_StartAddressModule>().FirstOrDefault();
            var dmxPersonalityModule = device.Modules.OfType<DMX_PersonalityModule>().FirstOrDefault();
            this.DeviceInfo = new RDMDeviceInfo(1,
                                           0,
                                           device.DeviceModelID,
                                           device.ProductCategoryCoarse,
                                           device.ProductCategoryFine,
                                           softwareVersionModule?.SoftwareVersionId ?? 0,
                                           dmx512Footprint: dmxPersonalityModule?.CurrentPersonalityFootprint ?? 0,
                                           dmx512CurrentPersonality: dmxPersonalityModule?.CurrentPersonality ?? 0,
                                           dmx512NumberOfPersonalities: dmxPersonalityModule?.PersonalitiesCount ?? 0,
                                           dmx512StartAddress: (dmxStartAddressModule.DMXAddress ?? ushort.MaxValue),
                                           subDeviceCount: (ushort)(device.SubDevices?.Where(sd => !sd.Subdevice.IsRoot).Count() ?? 0),
                                           sensorCount: (byte)(device.Sensors?.Count ?? 0));
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_INFO:
                    OnPropertyChanged(nameof(DeviceInfo));
                    break;
            }
        }
    }
}