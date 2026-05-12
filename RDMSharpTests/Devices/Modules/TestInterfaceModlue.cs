using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestInterfaceModlue
{
    private InterfaceMockDevice? generated;
    private InterfaceModule? interfaceModule;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(9231, 555);
    private const string TEST_INTERFACE_NAME = "Test Interface";
    private const string TEST_INTERFACE_HARDWARE_ADDRESS = "e0:63:da:5a:c4:fb";
    private const EARP_HardwareTypes TEST_INTERFACE_HARDWARE_TYPE = EARP_HardwareTypes.Ethernet;
    private const string TEST_INTERFACE_IP_ADDRESS = "2.3.4.5";
    private static RDMMessage APPLY_CONFIG_MESSAGE = new RDMMessage()
    {
        Command = ERDM_Command.SET_COMMAND,
        DestUID = DEVCIE_UID,
        SourceUID = CONTROLLER_UID,
        Parameter = ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION,
        SubDevice = SubDevice.Root,
        ParameterData = Tools.ValueToData((uint)1)
    };

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }


    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new InterfaceMockDevice(DEVCIE_UID);
        interfaceModule = generated.Modules.OfType<InterfaceModule>().FirstOrDefault();
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
        interfaceModule = null;
    }

    [Test, Order(1)]
    public void TestGetLIST_INTERFACES()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LIST_INTERFACES,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LIST_INTERFACES));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(6));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(InterfaceDescriptor[])));
        var interfaces = (InterfaceDescriptor[])response.Value;
        Assert.That(interfaces, Has.Length.EqualTo(1));
        Assert.That(interfaces[0].InterfaceId, Is.EqualTo(1));
        Assert.That(interfaces[0].HardwareType, Is.EqualTo(EARP_HardwareTypes.Ethernet));

        #endregion

    }
    [Test, Order(2)]
    public void TestGetINTERFACE_LABEL()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.INTERFACE_LABEL,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(18));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetInterfaceNameResponse)));
        var interfaceNameResponse = (GetInterfaceNameResponse)response.Value;
        Assert.That(interfaceNameResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceNameResponse.Label, Is.EqualTo(TEST_INTERFACE_NAME));
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }
    [Test, Order(3)]
    public void TestGetINTERFACE_HARDWARE_ADDRESS_TYPE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(10));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetHardwareAddressResponse)));
        var interfaceHardwareAddressResponse = (GetHardwareAddressResponse)response.Value;
        Assert.That(interfaceHardwareAddressResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceHardwareAddressResponse.HardwareAddress, Is.EqualTo(new MACAddress(TEST_INTERFACE_HARDWARE_ADDRESS)));
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }
    [Test, Order(4)]
    public void TestGetIPV4_CURRENT_ADDRESS()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_CURRENT_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_CURRENT_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(10));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
        var interfaceIPv4CurrentAddressResponse = (GetIPv4CurrentAddressResponse)response.Value;
        Assert.That(interfaceIPv4CurrentAddressResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceIPv4CurrentAddressResponse.IPAddress, Is.EqualTo(new IPv4Address(TEST_INTERFACE_IP_ADDRESS)));
        Assert.That(interfaceIPv4CurrentAddressResponse.Netmask, Is.EqualTo(8));
        Assert.That(interfaceIPv4CurrentAddressResponse.DHCPStatus, Is.EqualTo(ERDM_DHCPStatusMode.INACTIVE));
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_CURRENT_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    [Test, Order(5)]
    public void TestGetIPV4_DHCP_MODE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_DHCP_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        var interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.False);

        interfaceModule.Interfaces.First().DHCP = true;
        interfaceModule.Interfaces.First().DHCP = true; //For Coverage

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.True);
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }
    [Test, Order(6)]
    public void TestSetIPV4_DHCP_MODE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_DHCP_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData(new GetSetIPV4_xxx_Mode(1, true));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        var interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.False);

        response = generated.ProcessRequestMessage_Internal(APPLY_CONFIG_MESSAGE);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.True);
        #endregion

        request.Command = ERDM_Command.SET_COMMAND;
        request.ParameterData = Tools.ValueToData(new GetSetIPV4_xxx_Mode(2, true));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_DHCP_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    [Test, Order(7)]
    public void TestGetIPV4_ZEROCONF_MODE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_ZEROCONF_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        var interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.True);


        interfaceModule.Interfaces.First().ZeroConf = false;
        interfaceModule.Interfaces.First().ZeroConf = false; //For Coverage

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.False);
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    [Test, Order(8)]
    public void TestSetIPV4_ZEROCONF_MODE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_ZEROCONF_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData(new GetSetIPV4_xxx_Mode(1, false));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        var interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.True);


        response = generated.ProcessRequestMessage_Internal(APPLY_CONFIG_MESSAGE);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
        interfaceResponse = (GetSetIPV4_xxx_Mode)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.Enabled, Is.False);

        #endregion

        request.Command = ERDM_Command.SET_COMMAND;
        request.ParameterData = Tools.ValueToData(new GetSetIPV4_xxx_Mode(2, true));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_ZEROCONF_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    [Test, Order(9)]
    public void TestGetIPV4_STATIC_ADDRESS()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_STATIC_ADDRESS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(9));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
        var interfaceIPv4StaticAddressResponse = (GetSetIPv4StaticAddress)response.Value;
        Assert.That(interfaceIPv4StaticAddressResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceIPv4StaticAddressResponse.IPAddress, Is.EqualTo(new IPv4Address(TEST_INTERFACE_IP_ADDRESS)));
        Assert.That(interfaceIPv4StaticAddressResponse.Netmask, Is.EqualTo(8));

        IPv4Address oneDotOneDotOneDotOne = new IPv4Address("1.1.1.1");
        var iface = interfaceModule.Interfaces.First();
        iface.SetStaticIP(oneDotOneDotOneDotOne, 24);
        Assert.That(iface.CurrentIP, Is.EqualTo(oneDotOneDotOneDotOne));
        Assert.That(iface.CurrentSubnetMask, Is.EqualTo(24));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(9));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
        interfaceIPv4StaticAddressResponse = (GetSetIPv4StaticAddress)response.Value;
        Assert.That(interfaceIPv4StaticAddressResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceIPv4StaticAddressResponse.IPAddress, Is.EqualTo(oneDotOneDotOneDotOne));
        Assert.That(interfaceIPv4StaticAddressResponse.Netmask, Is.EqualTo(24));
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }
    [Test, Order(10)]
    public void TestSetIPV4_STATIC_ADDRESS()
    {
        IPv4Address oneDotOneDotOneDotOne = new IPv4Address("1.1.1.1");
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IPV4_STATIC_ADDRESS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData(new GetSetIPv4StaticAddress(1, oneDotOneDotOneDotOne, 24));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(9));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
        var interfaceResponse = (GetSetIPv4StaticAddress)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.IPAddress, Is.EqualTo(new IPv4Address(TEST_INTERFACE_IP_ADDRESS)));
        Assert.That(interfaceResponse.Netmask, Is.EqualTo(8));


        response = generated.ProcessRequestMessage_Internal(APPLY_CONFIG_MESSAGE);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(9));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
        interfaceResponse = (GetSetIPv4StaticAddress)response.Value;
        Assert.That(interfaceResponse.InterfaceId, Is.EqualTo(1));
        Assert.That(interfaceResponse.IPAddress, Is.EqualTo(oneDotOneDotOneDotOne));
        Assert.That(interfaceResponse.Netmask, Is.EqualTo(24));

        #endregion

        request.Command = ERDM_Command.SET_COMMAND;
        request.ParameterData = Tools.ValueToData(new GetSetIPv4StaticAddress(2, oneDotOneDotOneDotOne, 31));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IPV4_STATIC_ADDRESS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    [Test, Order(11)]
    public void TestSetINTERFACE_APPLY_CONFIGURATION()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);

        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    [Test, Order(11)]
    public void TestSetINTERFACE_RENEW_DHCP()
    {
        var iface = interfaceModule.Interfaces.First();
        iface.DHCP = true;
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.INTERFACE_RENEW_DHCP,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RENEW_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RENEW_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.ACTION_NOT_SUPPORTED));
        Assert.That(response.Value, Is.Null);

        iface.SetStaticIP(IPv4Address.Empty, 0);

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RENEW_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);

        Assert.That(iface.CurrentIP, Is.EqualTo(InterfaceMock.DHCP_ADDRESS_1));

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RENEW_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);

        Assert.That(iface.CurrentIP, Is.EqualTo(InterfaceMock.DHCP_ADDRESS_2));

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RENEW_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);

        Assert.That(iface.CurrentIP, Is.EqualTo(InterfaceMock.DHCP_ADDRESS_3));

        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RENEW_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }
    [Test, Order(12)]
    public void TestSetINTERFACE_RELEASE_DHCP()
    {
        var iface = interfaceModule.Interfaces.First();
        iface.DHCP = true;
        iface.SetStaticIP(IPv4Address.Empty, 0);
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(interfaceModule, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Is.Not.Null);
        Assert.That(interfaceModule.Interfaces, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.INTERFACE_RENEW_DHCP,
            SubDevice = SubDevice.Root,
            ParameterData = Tools.ValueToData((uint)1)
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(iface.CurrentIP, Is.EqualTo(InterfaceMock.DHCP_ADDRESS_1));

        request.Parameter = ERDM_Parameter.INTERFACE_RELEASE_DHCP;
        request.ParameterData = null;

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RELEASE_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((uint)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RELEASE_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);

        Assert.That(iface.CurrentIP, Is.EqualTo(IPv4Address.Empty));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RELEASE_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.ACTION_NOT_SUPPORTED));
        Assert.That(response.Value, Is.Null);
        #endregion

        request.ParameterData = Tools.ValueToData((uint)2);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.INTERFACE_RELEASE_DHCP));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }

    class InterfaceMockDevice : MockGeneratedDevice1
    {
        public InterfaceMockDevice(UID uid) : base(uid, new IModule[] { new InterfaceModule(new Interface[] { new InterfaceMock() }) })
        {
        }
    }
    class InterfaceMock : Interface
    {
        public static IPv4Address DHCP_ADDRESS_1 = new IPv4Address("2.5.5.5");
        public static IPv4Address DHCP_ADDRESS_2 = new IPv4Address("2.6.6.6");
        public static IPv4Address DHCP_ADDRESS_3 = new IPv4Address("2.7.7.7");
        public InterfaceMock()
            : base(1,
                  TEST_INTERFACE_HARDWARE_TYPE,
                  TEST_INTERFACE_NAME,
                  new IPv4Address(TEST_INTERFACE_IP_ADDRESS),
                  8,
                  new MACAddress(TEST_INTERFACE_HARDWARE_ADDRESS))
        {
        }
        public override void RenewDHCP()
        {
            switch (this.CurrentIP.B2)
            {
                default:
                case 0:
                    this.SetCurrentIP(DHCP_ADDRESS_1, 8, true);
                    break;
                case 5:
                    this.SetCurrentIP(DHCP_ADDRESS_2, 8, true);
                    break;
                case 6:
                    this.SetCurrentIP(DHCP_ADDRESS_3, 8, true);
                    break;
            }
        }
        public override void ReleaseDHCP()
        {
            this.SetCurrentIP(IPv4Address.Empty, 8, false);
            base.ReleaseDHCP();
        }
    }
}