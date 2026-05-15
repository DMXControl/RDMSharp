using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module;

public sealed class InterfaceModule : AbstractModule
{
    private const string _moduleName = "Interface";
    private const string _moduleDisplayName = "Interface";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.LIST_INTERFACES,
        ERDM_Parameter.INTERFACE_LABEL,
        ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
        ERDM_Parameter.IPV4_DHCP_MODE,
        ERDM_Parameter.IPV4_ZEROCONF_MODE,
        ERDM_Parameter.IPV4_CURRENT_ADDRESS,
        ERDM_Parameter.IPV4_STATIC_ADDRESS,
        ERDM_Parameter.INTERFACE_RENEW_DHCP,
        ERDM_Parameter.INTERFACE_RELEASE_DHCP,
        ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION
    };

    public override string DisplayName => _moduleDisplayName;

    private ConcurrentDictionary<uint, Interface> _interfaces = new ConcurrentDictionary<uint, Interface>();
    public IReadOnlyCollection<Interface> Interfaces
    {
        get
        {
            return _interfaces.Values.ToList();
        }
    }
    private ConcurrentDictionary<object, object> lableDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> hardwareAddressDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> currentAddressDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> staticAddressDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> dhcpModeDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> zeroConfModeDict = new ConcurrentDictionary<object, object>();

    private ConcurrentDictionary<uint, ConcurrentQueue<RDMMessage>> applyConfigQueueDict = new ConcurrentDictionary<uint, ConcurrentQueue<RDMMessage>>();
    public InterfaceModule(IReadOnlyCollection<Interface> interfaces) : base(
        _moduleName,
        _moduleParameters)
    {
        foreach (var _interface in interfaces)
            _interfaces.TryAdd(_interface.InterfaceId, _interface);
    }
    public InterfaceModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        fillFromRemoteCache();
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {

        foreach (var iface in _interfaces.Values)
        {
            lableDict.TryAdd(iface.InterfaceId, new GetInterfaceNameResponse(iface.InterfaceId, iface.Lable));
            hardwareAddressDict.TryAdd(iface.InterfaceId, new GetHardwareAddressResponse(iface.InterfaceId, iface.MACAddress));
            currentAddressDict.TryAdd(iface.InterfaceId, new GetIPv4CurrentAddressResponse(iface.InterfaceId, iface.CurrentIP, iface.CurrentSubnetMask, iface.CurrentIP_DHCPStatus));
            staticAddressDict.TryAdd(iface.InterfaceId, new GetSetIPv4StaticAddress(iface.InterfaceId, iface.StaticIP, iface.StaticSubnetMask));
            dhcpModeDict.TryAdd(iface.InterfaceId, new GetSetIPV4_xxx_Mode(iface.InterfaceId, iface.DHCP));
            zeroConfModeDict.TryAdd(iface.InterfaceId, new GetSetIPV4_xxx_Mode(iface.InterfaceId, iface.ZeroConf));
            iface.PropertyChanged += Iface_PropertyChanged;
        }

        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.INTERFACE_LABEL, lableDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE, hardwareAddressDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IPV4_CURRENT_ADDRESS, currentAddressDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IPV4_STATIC_ADDRESS, staticAddressDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IPV4_DHCP_MODE, dhcpModeDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IPV4_ZEROCONF_MODE, zeroConfModeDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.LIST_INTERFACES, _interfaces.Values.Select(i => new InterfaceDescriptor(i.InterfaceId, i.HardwareType)).ToArray());
    }

    private void fillFromRemoteCache()
    {
        var values = ParentRemoteDevice.GetAllParameterValues();

        object value = null;
        values.TryGetValue(ERDM_Parameter.LIST_INTERFACES, out value);
        if (value is not InterfaceDescriptor[] interfaceDescriptors)
            return;

        if (values.TryGetValue(ERDM_Parameter.INTERFACE_LABEL, out value) && value is ConcurrentDictionary<object, object> _lableDict)
            lableDict = _lableDict;

        if (values.TryGetValue(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE, out value) && value is ConcurrentDictionary<object, object> _hardwareAddressTypeDict)
            hardwareAddressDict = _hardwareAddressTypeDict;

        if (values.TryGetValue(ERDM_Parameter.IPV4_CURRENT_ADDRESS, out value) && value is ConcurrentDictionary<object, object> _currentAddressDict)
            currentAddressDict = _currentAddressDict;

        if (values.TryGetValue(ERDM_Parameter.IPV4_STATIC_ADDRESS, out value) && value is ConcurrentDictionary<object, object> _staticAddressDict)
            staticAddressDict = _staticAddressDict;

        if (values.TryGetValue(ERDM_Parameter.IPV4_DHCP_MODE, out value) && value is ConcurrentDictionary<object, object> _dhcpModeDict)
            dhcpModeDict = _dhcpModeDict;

        if (values.TryGetValue(ERDM_Parameter.IPV4_ZEROCONF_MODE, out value) && value is ConcurrentDictionary<object, object> _zeroConfModeDict)
            zeroConfModeDict = _zeroConfModeDict;


        foreach (InterfaceDescriptor interfaceDescriptor in interfaceDescriptors)
        {
            uint interfaceId = interfaceDescriptor.InterfaceId;
            Interface _interface = null;
            if (!_interfaces.TryGetValue(interfaceId, out _interface))
            {
                _interface = new Interface(interfaceId, interfaceDescriptor.HardwareType);
                _interfaces.TryAdd(interfaceId, _interface);
            }

            if (lableDict.TryGetValue(interfaceId, out object lableValue) && lableValue is GetInterfaceNameResponse getInterfaceNameResponse)
                _interface.Lable = getInterfaceNameResponse.Label;

            if (hardwareAddressDict.TryGetValue(interfaceId, out object hardwareAddressValue) && hardwareAddressValue is GetHardwareAddressResponse getHardwareAddressResponse)
                _interface.MACAddress = getHardwareAddressResponse.HardwareAddress;

            if (currentAddressDict.TryGetValue(interfaceId, out object currentAddressValue) && currentAddressValue is GetIPv4CurrentAddressResponse getIPv4CurrentAddressResponse)
            {
                _interface.CurrentIP = getIPv4CurrentAddressResponse.IPAddress;
                _interface.CurrentSubnetMask = getIPv4CurrentAddressResponse.Netmask;
                _interface.CurrentIP_DHCPStatus = getIPv4CurrentAddressResponse.DHCPStatus;
            }

            if (staticAddressDict.TryGetValue(interfaceId, out object staticAddressValue) && staticAddressValue is GetSetIPv4StaticAddress getSetIPv4StaticAddress)
            {
                _interface.StaticIP = getSetIPv4StaticAddress.IPAddress;
                _interface.StaticSubnetMask = getSetIPv4StaticAddress.Netmask;
            }

            if (dhcpModeDict.TryGetValue(interfaceId, out object dhcpModeValue) && dhcpModeValue is GetSetIPV4_xxx_Mode getSetIPV4_DHCP_Mode)
                _interface.DHCP = getSetIPV4_DHCP_Mode.Enabled;

            if (zeroConfModeDict.TryGetValue(interfaceId, out object zeroConfModeValue) && zeroConfModeValue is GetSetIPV4_xxx_Mode getSetIPV4_ZeroConf_Mode)
                _interface.ZeroConf = getSetIPV4_ZeroConf_Mode.Enabled;
        }
    }

    private void Iface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not Interface iface || this.ParentGeneratedDevice is null)
            return;

        switch (e.PropertyName)
        {
            case (nameof(Interface.DHCP)):
                var newDHCPValue = new GetSetIPV4_xxx_Mode(iface.InterfaceId, iface.DHCP);
                this.dhcpModeDict.AddOrUpdate(iface.InterfaceId, (_) => newDHCPValue, (_, _) => newDHCPValue);
                this.ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.IPV4_DHCP_MODE, this.dhcpModeDict, iface.InterfaceId);
                break;
            case (nameof(Interface.ZeroConf)):
                var newZeroConfValue = new GetSetIPV4_xxx_Mode(iface.InterfaceId, iface.ZeroConf);
                this.zeroConfModeDict.AddOrUpdate(iface.InterfaceId, (_) => newZeroConfValue, (_, _) => newZeroConfValue);
                this.ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.IPV4_ZEROCONF_MODE, this.zeroConfModeDict, iface.InterfaceId);
                break;

            case (nameof(Interface.StaticIP)):
            case (nameof(Interface.StaticSubnetMask)):
                var newIPv4StaticAddressValue = new GetSetIPv4StaticAddress(iface.InterfaceId, iface.StaticIP, iface.StaticSubnetMask);
                this.staticAddressDict.AddOrUpdate(iface.InterfaceId, (_) => newIPv4StaticAddressValue, (_, _) => newIPv4StaticAddressValue);
                this.ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.IPV4_STATIC_ADDRESS, this.staticAddressDict, iface.InterfaceId);
                break;
            case (nameof(Interface.CurrentIP)):
            case (nameof(Interface.CurrentSubnetMask)):
            case (nameof(Interface.CurrentIP_DHCPStatus)):
                var newIPv4CurrentAddressValue = new GetIPv4CurrentAddressResponse(iface.InterfaceId, iface.CurrentIP, iface.CurrentSubnetMask, iface.CurrentIP_DHCPStatus);
                this.currentAddressDict.AddOrUpdate(iface.InterfaceId, (_) => newIPv4CurrentAddressValue, (_, _) => newIPv4CurrentAddressValue);
                this.ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.IPV4_CURRENT_ADDRESS, this.currentAddressDict, iface.InterfaceId);
                break;
        }
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        Interface? iface = null;
        if (index is not null && index is uint ui)
            iface = _interfaces?.Values.FirstOrDefault(iface => iface.InterfaceId == ui);

        if (newValue is ConcurrentDictionary<object, object> dict && index is not null)
            newValue = dict.GetValueOrDefault(index);

        switch (parameter)
        {
            case ERDM_Parameter.LIST_INTERFACES:
                OnPropertyChanged(nameof(Interfaces));
                fillFromRemoteCache();
                break;

            case ERDM_Parameter.INTERFACE_LABEL:
                if (iface is not null)
                    iface.Lable = ((GetInterfaceNameResponse)newValue).Label;
                break;
            case ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE:
                if (iface is not null)
                    iface.MACAddress = ((GetHardwareAddressResponse)newValue).HardwareAddress;
                break;
            case ERDM_Parameter.IPV4_CURRENT_ADDRESS:
                if (iface is not null)
                {
                    iface.CurrentIP = ((GetIPv4CurrentAddressResponse)newValue).IPAddress;
                    iface.CurrentSubnetMask = ((GetIPv4CurrentAddressResponse)newValue).Netmask;
                    iface.CurrentIP_DHCPStatus = ((GetIPv4CurrentAddressResponse)newValue).DHCPStatus;
                }
                break;
            case ERDM_Parameter.IPV4_STATIC_ADDRESS:
                if (iface is not null)
                {
                    iface.StaticIP = ((GetSetIPv4StaticAddress)newValue).IPAddress;
                    iface.StaticSubnetMask = ((GetSetIPv4StaticAddress)newValue).Netmask;
                }
                break;
            case ERDM_Parameter.IPV4_DHCP_MODE:
                if (iface is not null)
                    iface.DHCP = ((GetSetIPV4_xxx_Mode)newValue).Enabled;
                break;
            case ERDM_Parameter.IPV4_ZEROCONF_MODE:
                if (iface is not null)
                    iface.ZeroConf = ((GetSetIPV4_xxx_Mode)newValue).Enabled;
                break;
        }
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        switch (message.Parameter)
        {
            case ERDM_Parameter.IPV4_ZEROCONF_MODE when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.IPV4_DHCP_MODE when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.IPV4_STATIC_ADDRESS when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION when message.Command is ERDM_Command.SET_COMMAND:
                if (message.Value is null)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };

                if (message.PDL >= 4)
                {
                    byte[] parameterData = message.ParameterData.Take(4).ToArray();
                    uint interfaceId = message.ParameterData.Length > 0 ? Tools.DataToUInt(ref parameterData) : 0;

                    if (_interfaces.Values.FirstOrDefault(iface => iface.InterfaceId == interfaceId) is not Interface iface)
                        return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                            Parameter = message.Parameter
                        };
                    try
                    {
#if DEBUG
                        if (message.SourceUID.Equals(new UID(0xeeee, 0xf0f0f0f0)))
                            throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                        if (message.Parameter != ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION)
                            applyConfigQueueDict.GetOrAdd(interfaceId, (_) => { return new ConcurrentQueue<RDMMessage>(); }).Enqueue(new RDMMessage(message.BuildMessage()));
                        else
                        {
                            if (applyConfigQueueDict.TryGetValue(interfaceId, out ConcurrentQueue<RDMMessage> queue))
                                while (queue.TryDequeue(out RDMMessage applyMessage))
                                {
                                    switch (applyMessage.Parameter)
                                    {
                                        case ERDM_Parameter.IPV4_DHCP_MODE when applyMessage.Value is GetSetIPV4_xxx_Mode dhcpMode:
                                            iface.DHCP = dhcpMode.Enabled;
                                            break;
                                        case ERDM_Parameter.IPV4_ZEROCONF_MODE when applyMessage.Value is GetSetIPV4_xxx_Mode zeroConfMode:
                                            iface.ZeroConf = zeroConfMode.Enabled;
                                            break;
                                        case ERDM_Parameter.IPV4_STATIC_ADDRESS when applyMessage.Value is GetSetIPv4StaticAddress staticAddress:
                                            iface.SetStaticIP(staticAddress.IPAddress, staticAddress.Netmask);
                                            break;
                                    }
                                }
                        }

                        return new RDMMessage()
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                            Parameter = message.Parameter
                        };
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex);
                    }
                }
                break;
            case ERDM_Parameter.INTERFACE_RENEW_DHCP when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.INTERFACE_RELEASE_DHCP when message.Command is ERDM_Command.SET_COMMAND:
                if (message.Value is not uint ifaceId)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                if (_interfaces.Values.FirstOrDefault(iface => iface.InterfaceId == ifaceId) is not Interface iface2)
                    return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                try
                {
                    switch (message.Parameter)
                    {
                        case ERDM_Parameter.INTERFACE_RENEW_DHCP:
                            if (iface2.StaticIP != IPv4Address.Empty || !iface2.DHCP)
                                return new RDMMessage(ERDM_NackReason.ACTION_NOT_SUPPORTED)
                                {
                                    SourceUID = message.DestUID,
                                    DestUID = message.SourceUID,
                                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                    Parameter = message.Parameter
                                };
                            iface2.RenewDHCP();
                            break;
                        case ERDM_Parameter.INTERFACE_RELEASE_DHCP:
                            if (iface2.CurrentIP_DHCPStatus != ERDM_DHCPStatusMode.ACTIVE)
                                return new RDMMessage(ERDM_NackReason.ACTION_NOT_SUPPORTED)
                                {
                                    SourceUID = message.DestUID,
                                    DestUID = message.SourceUID,
                                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                    Parameter = message.Parameter
                                };
                            iface2.ReleaseDHCP();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex);
                }
                return new RDMMessage()
                {
                    SourceUID = message.DestUID,
                    DestUID = message.SourceUID,
                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
                    Parameter = message.Parameter
                };
                break;
            default:
                break;
        }
        return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
        {
            SourceUID = message.DestUID,
            DestUID = message.SourceUID,
            Command = message.Command | ERDM_Command.RESPONSE,
            Parameter = message.Parameter
        };
    }
    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        switch (parameter)
        {
            case ERDM_Parameter.IPV4_DHCP_MODE when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.IPV4_ZEROCONF_MODE when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.IPV4_STATIC_ADDRESS when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.INTERFACE_RENEW_DHCP when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.INTERFACE_RELEASE_DHCP when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION when command is ERDM_Command.SET_COMMAND:
                return true;
        }
        return base.IsHandlingParameter(parameter, command);
    }
}
