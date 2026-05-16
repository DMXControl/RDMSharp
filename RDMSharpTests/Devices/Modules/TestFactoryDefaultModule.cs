using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestFactoryDefaultModule
{
    private FactoryDefaultMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1f1f, 412312401);
    private static UID DEVCIE_UID = new UID(13214, 801461847);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new FactoryDefaultMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }
    [Test, Order(6)]
    public async Task TestGetFACTORY_DEFAULTS()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var factoryDefaultsModule = generated.Modules.OfType<FactoryDefaultsModule>().Single();
        Assert.That(factoryDefaultsModule, Is.Not.Null);
        Assert.That(factoryDefaultsModule.FactoryDefaults, Is.False);
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.FACTORY_DEFAULTS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.FACTORY_DEFAULTS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.False);
        #endregion

        #region Test Factory Defaults changed (GET)
        Assert.That(factoryDefaultsModule.FactoryDefaults, Is.False);
        await factoryDefaultsModule.SetFactoryDefaults();
        Assert.That(factoryDefaultsModule.FactoryDefaults, Is.True);

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.FACTORY_DEFAULTS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.True);
        #endregion


        #region Test Factory Defaults changed (SET)
        generated.DMXAddress = 200;
        Assert.That(factoryDefaultsModule.FactoryDefaults, Is.False);
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.FACTORY_DEFAULTS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(factoryDefaultsModule.FactoryDefaults, Is.True);
        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.FACTORY_DEFAULTS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.True);
        #endregion
    }

    [Test, Order(301)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<FactoryDefaultsModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.FactoryDefaults, Is.False);

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<FactoryDefaultsModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.FactoryDefaults, Is.False);

        await module.SetFactoryDefaults();
        await Task.Delay(1000);

        Assert.That(generatedModule.FactoryDefaults, Is.True);
        Assert.That(module.FactoryDefaults, Is.True);
    }
    class FactoryDefaultMockDevice : MockGeneratedDevice1
    {
        public FactoryDefaultMockDevice(UID uid) : base(uid, new IModule[] { new FactoryDefaultsModule() })
        {
        }
    }
}