using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestLampOnModeModule
{
    private LampOnModeMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.LAMP_ON_MODE;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        Assert.That(defines, Is.Not.Null);
        generated = new LampOnModeMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestDisplayName()
    {
        Assert.That(generated, Is.Not.Null);
        var curveModule = generated.Modules.OfType<LampOnModeModule>().Single();
        Assert.That(curveModule, Is.Not.Null);
        Assert.That(curveModule.DisplayName, Is.EqualTo("Lamp On Mode"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetLAMP_ON_MODE()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<LampOnModeModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_OFF));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LAMP_ON_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LAMP_ON_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.LampOnMode));
        #endregion

        #region Test Lamp State changed (GET)
        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_OFF));
        module.LampOnMode = ERDM_LampMode.ON_MODE_ON;
        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_ON));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LAMP_ON_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.LampOnMode));
        #endregion


        #region Test Lamp State changed (SET)
        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_ON));
        request.ParameterData = new byte[] { 0x00 }; // Requesting Lamp State OFF
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LAMP_ON_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_OFF));
        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LAMP_ON_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.LampOnMode));

        request.ParameterData = new byte[] { 0x01 }; // Requesting Lamp State ON
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LAMP_ON_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_DMX));

        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LAMP_ON_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.LampOnMode));

        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetLAMP_ON_MODE))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<LampOnModeModule>().Single();
        generatedModule.LampOnMode = ERDM_LampMode.ON_MODE_OFF;
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_OFF));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<LampOnModeModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_OFF));

        await module.SetLampOnMode(ERDM_LampMode.ON_MODE_AFTER_CAL);
        await Task.Delay(1000);

        Assert.That(generatedModule.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_AFTER_CAL));
        Assert.That(module.LampOnMode, Is.EqualTo(ERDM_LampMode.ON_MODE_AFTER_CAL));
    }
    class LampOnModeMockDevice : MockGeneratedDevice1
    {
        public LampOnModeMockDevice(UID uid) : base(uid, new IModule[] { new LampOnModeModule() })
        {
        }
    }
}