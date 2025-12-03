using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestProxiedDevicesModlue
{
    private ProxiedDevicesMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 3453);
    private static UID DEVCIE_UID = new UID(123, 5225);
    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new ProxiedDevicesMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }


    [Test, Retry(3), Order(1)]
    public void TestGetPROXIED_DEVICES_COUNT()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        var proxiedDevicesModule = generated.Modules.OfType<ProxiedDevicesModule>().FirstOrDefault();
        Assert.That(proxiedDevicesModule, Is.Not.Null);
        Assert.That(proxiedDevicesModule.DeviceUIDs, Is.Not.Null);
        Assert.That(proxiedDevicesModule.DeviceUIDs, Has.Count.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PROXIED_DEVICES_COUNT,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(0, false)));
        #endregion

        proxiedDevicesModule.AddProxiedDevices(new UID(12, 3456), new UID(12, 2345));

        for (int i = 0; i < 5; i++)
        {
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3));
            Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(2, true)));
        }

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PROXIED_DEVICES,
            SubDevice = SubDevice.Root,
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(12));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDevices(new UID(12, 3456), new UID(12, 2345))));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PROXIED_DEVICES_COUNT,
            SubDevice = SubDevice.Root,
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(2, false)));


        proxiedDevicesModule.RemoveProxiedDevices(new UID(12, 3456));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(1, true)));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PROXIED_DEVICES,
            SubDevice = SubDevice.Root,
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(6));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDevices(new UID(12, 2345))));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PROXIED_DEVICES_COUNT,
            SubDevice = SubDevice.Root,
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(1, false)));

        List<UID> uidsToAdd = new List<UID>();
        uidsToAdd.Add(new UID(12, 2345));
        for (int i = 0; i < 134; i++)
            uidsToAdd.Add(new UID(12, (ushort)(3456 + i)));

        proxiedDevicesModule.AddProxiedDevices(uidsToAdd.ToArray());

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(135, true)));

        var chunks = uidsToAdd.Chunk(38);
        int totalResponses = 0;
        for (int i = 0; i < chunks.Count(); i++)
        {
            request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.PROXIED_DEVICES,
                SubDevice = SubDevice.Root,
            };
            response = generated.ProcessRequestMessage_Internal(request);
            totalResponses++;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.MessageCounter, Is.EqualTo(chunks.Count() - totalResponses));
            Assert.That(response.ParameterData, Has.Length.EqualTo(chunks.ElementAt(i).Count() * 6));
            Assert.That(response.Value, Is.EqualTo(new RDMProxiedDevices(chunks.ElementAt(i).ToArray())));
        }
        Assert.That(totalResponses, Is.EqualTo(chunks.Count()));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PROXIED_DEVICES_COUNT,
            SubDevice = SubDevice.Root,
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PROXIED_DEVICES_COUNT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.EqualTo(new RDMProxiedDeviceCount(135, false)));
    }

    class ProxiedDevicesMockDevice : MockGeneratedDevice1
    {
        public ProxiedDevicesMockDevice(UID uid) : base(uid, new IModule[] { new ProxiedDevicesModule() })
        {
        }
    }
}