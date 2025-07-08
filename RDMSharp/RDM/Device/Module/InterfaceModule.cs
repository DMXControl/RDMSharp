using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class InterfaceModule : AbstractModule
    {
        public IReadOnlyCollection<Interface> _interfaces;
        public IReadOnlyCollection<Interface> Interfaces
        {
            get
            {
                if (ParentDevice is null)
                    return _interfaces;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.LIST_INTERFACES, out res))
                    return (IReadOnlyCollection<Interface>)res;
                return new List<Interface>();
            }
        }
        public InterfaceModule(IReadOnlyCollection<Interface> interfaces) : base(
            "Interface",
            ERDM_Parameter.LIST_INTERFACES,
            ERDM_Parameter.INTERFACE_LABEL,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            ERDM_Parameter.IPV4_DHCP_MODE,
            ERDM_Parameter.IPV4_ZEROCONF_MODE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            ERDM_Parameter.INTERFACE_RENEW_DHCP,
            ERDM_Parameter.INTERFACE_RELEASE_DHCP,
            ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION)
        {
            _interfaces = interfaces;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            foreach (var iface in _interfaces)
            {
                ParentDevice.setParameterValue(ERDM_Parameter.INTERFACE_LABEL, new GetInterfaceNameResponse(iface.InterfaceId, iface.Lable), iface.InterfaceId);
                ParentDevice.setParameterValue(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE, new GetHardwareAddressResponse(iface.InterfaceId, iface.MACAddress), iface.InterfaceId);
                ParentDevice.setParameterValue(ERDM_Parameter.IPV4_CURRENT_ADDRESS, new GetIPv4CurrentAddressResponse(iface.InterfaceId, iface.CurrentIP,iface.SubnetMask,(byte)(iface.DHCP?0:1)), iface.InterfaceId);
                ParentDevice.setParameterValue(ERDM_Parameter.IPV4_STATIC_ADDRESS, new GetSetIPv4StaticAddress(iface.InterfaceId, iface.CurrentIP, iface.SubnetMask), iface.InterfaceId);
                ParentDevice.setParameterValue(ERDM_Parameter.IPV4_DHCP_MODE, new GetSetIPV4_xxx_Mode(iface.InterfaceId, iface.DHCP), iface.InterfaceId);
                ParentDevice.setParameterValue(ERDM_Parameter.IPV4_ZEROCONF_MODE, new GetSetIPV4_xxx_Mode(iface.InterfaceId, iface.ZeroConf), iface.InterfaceId);
            }
            ParentDevice.setParameterValue(ERDM_Parameter.LIST_INTERFACES, new GetInterfaceListResponse(_interfaces.Select(i => new InterfaceDescriptor(i.InterfaceId, i.HardwareType)).ToArray()));
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.LIST_INTERFACES:
                    OnPropertyChanged(nameof(Interfaces));
                    break;
            }
        }
        protected override RDMMessage handleRequest(RDMMessage message)
        {
            return null;
        }
        public override bool IsHandlingParameter(ERDM_Parameter parameter)
        {
            switch (parameter)
            {
                case ERDM_Parameter.INTERFACE_RENEW_DHCP:
                case ERDM_Parameter.INTERFACE_RELEASE_DHCP:
                case ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION:
                    return true;
            }
            return base.IsHandlingParameter(parameter);
        }
    }
}
