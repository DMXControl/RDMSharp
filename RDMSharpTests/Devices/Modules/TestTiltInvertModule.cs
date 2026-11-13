using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestTiltInvertModule
{
    private TiltInvertMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.TILT_INVERT;
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
        generated = new TiltInvertMockDevice(DEVCIE_UID);
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
        var curveModule = generated.Modules.OfType<TiltInvertModule>().Single();
        Assert.That(curveModule, Is.Not.Null);
        Assert.That(curveModule.DisplayName, Is.EqualTo("Tilt Invert"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetTILT_INVERT()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<TiltInvertModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.TiltInvert, Is.False);
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.TILT_INVERT,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.TILT_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.TiltInvert));
        #endregion

        #region Test Tilt Invert changed (GET)
        Assert.That(module.TiltInvert, Is.False);
        module.TiltInvert = true;
        Assert.That(module.TiltInvert, Is.True);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.TILT_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.TiltInvert));
        #endregion


        #region Test Tilt Invert changed (SET)
        Assert.That(module.TiltInvert, Is.True);
        request.ParameterData = new byte[] { 0x00 }; // Requesting Tilt Invert OFF
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.TILT_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(module.TiltInvert, Is.False);
        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.TILT_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.TiltInvert));

        request.ParameterData = new byte[] { 0x01 }; // Requesting Tilt Invert ON
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.TILT_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(module.TiltInvert, Is.True);

        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.TILT_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.TiltInvert));

        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetTILT_INVERT))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<TiltInvertModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.TiltInvert, Is.False);

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<TiltInvertModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.TiltInvert, Is.False);

        await module.SetTiltInvert(true);
        await Task.Delay(1000);

        Assert.That(generatedModule.TiltInvert, Is.True);
        Assert.That(module.TiltInvert, Is.True);
    }
    class TiltInvertMockDevice : MockGeneratedDevice1
    {
        public TiltInvertMockDevice(UID uid) : base(uid, new IModule[] { new TiltInvertModule() })
        {
        }
    }
}