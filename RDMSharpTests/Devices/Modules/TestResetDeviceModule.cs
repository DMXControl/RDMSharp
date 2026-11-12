using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestResetDeviceModule
{
    private ResetDeviceMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x7f1f, 412354401);
    private static UID DEVCIE_UID = new UID(13234, 801411117);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new ResetDeviceMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public async Task TestSetRESET_DEVICE()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var resetDeviceModule = generated.Modules.OfType<ResetDeviceModule>().Single();
        Assert.That(resetDeviceModule, Is.Not.Null);
        #endregion

        ERDM_ResetType? received = null;
        resetDeviceModule.ResetRequested += (o, e) =>
        {
            received = e;
        };
        #region Test Reset Device (SET)
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.RESET_DEVICE,
            SubDevice = SubDevice.Root,
            ParameterData = Tools.ValueToData(ERDM_ResetType.Warm)
        };

        Assert.That(received, Is.Null);
        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(received, Is.EqualTo(ERDM_ResetType.Warm));

        request.ParameterData = Tools.ValueToData(ERDM_ResetType.Cold);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(received, Is.EqualTo(ERDM_ResetType.Cold));
        #endregion


        #region Test Reset Device (SET) NACKs
        request.ParameterData = Tools.ValueToData("Warm");
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));

        request.ParameterData = Tools.ValueToData((int)0xeeee);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));

        request.ParameterData = Tools.ValueToData((byte)0x09);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        request.ParameterData = Tools.ValueToData(ERDM_ResetType.Cold);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
        #endregion
    }

    [Test, DependsOnTest(nameof(TestSetRESET_DEVICE))]
    public async Task TestGetRESET_DEVICE()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var resetDeviceModule = generated.Modules.OfType<ResetDeviceModule>().Single();
        Assert.That(resetDeviceModule, Is.Not.Null);
        #endregion

        #region Test Reset Device (GET)
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.RESET_DEVICE,
            SubDevice = SubDevice.Root
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RESET_DEVICE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetRESET_DEVICE))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<ResetDeviceModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        Assert.That(generatedModule.SupportedParameters, Contains.Item(ERDM_Parameter.RESET_DEVICE));

        var module = mockDevice.Modules.OfType<ResetDeviceModule>().Single();

        ERDM_ResetType? received = null;
        generatedModule.ResetRequested += (o, e) =>
        {
            received = e;
        };

        Assert.That(module, Is.Not.Null);

        Assert.That(received, Is.Null);
        await module.SetReset(ERDM_ResetType.Warm);
        await Task.Delay(1000);

        Assert.That(received, Is.EqualTo(ERDM_ResetType.Warm));

        await module.SetReset(ERDM_ResetType.Cold);
        await Task.Delay(1000);

        Assert.That(received, Is.EqualTo(ERDM_ResetType.Cold));

        received = null;

        Assert.That(received, Is.Null);
        await module.SetReset(ERDM_ResetType.Warm);
        await Task.Delay(1000);

        Assert.That(received, Is.EqualTo(ERDM_ResetType.Warm));

        await generatedModule.SetReset(ERDM_ResetType.Cold);
        await Task.Delay(1000);

        Assert.That(received, Is.EqualTo(ERDM_ResetType.Cold));
    }

    class ResetDeviceMockDevice : MockGeneratedDevice1
    {
        public ResetDeviceMockDevice(UID uid) : base(uid, new IModule[] { new ResetDeviceModule() })
        {
        }
    }
}