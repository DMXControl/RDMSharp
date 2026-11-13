using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestDisplayInvertModule
{
    private DisplayInvertMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.DISPLAY_INVERT;
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
        generated = new DisplayInvertMockDevice(DEVCIE_UID);
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
        var curveModule = generated.Modules.OfType<DisplayInvertModule>().Single();
        Assert.That(curveModule, Is.Not.Null);
        Assert.That(curveModule.DisplayName, Is.EqualTo("Display Invert"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetDISPLAY_INVERT()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<DisplayInvertModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.AUTO));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DISPLAY_INVERT,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISPLAY_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.DisplayInvert));
        #endregion

        #region Test Display Invert changed (GET)
        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.AUTO));
        module.DisplayInvert = ERDM_DisplayInvert.ON;
        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.ON));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISPLAY_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.DisplayInvert));
        #endregion


        #region Test Display Invert changed (SET)
        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.ON));
        request.ParameterData = new byte[] { 0x00 }; // Requesting Display Invert OFF
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISPLAY_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.OFF));
        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISPLAY_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.DisplayInvert));

        request.ParameterData = new byte[] { 0x01 }; // Requesting Display Invert ON
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISPLAY_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.ON));

        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISPLAY_INVERT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(module.DisplayInvert));

        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetDISPLAY_INVERT))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<DisplayInvertModule>().Single();
        generatedModule.DisplayInvert = ERDM_DisplayInvert.AUTO;
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.AUTO));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DisplayInvertModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.AUTO));

        await module.SetDisplayInvert(ERDM_DisplayInvert.ON);
        await Task.Delay(1000);

        Assert.That(generatedModule.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.ON));
        Assert.That(module.DisplayInvert, Is.EqualTo(ERDM_DisplayInvert.ON));
    }
    class DisplayInvertMockDevice : MockGeneratedDevice1
    {
        public DisplayInvertMockDevice(UID uid) : base(uid, new IModule[] { new DisplayInvertModule() })
        {
        }
    }
}