using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// <see cref="PresetsModule"/> of a device that only supports the E1.20 Presets (PRESET_PLAYBACK and CAPTURE_PRESET, E1.20 10.11),
/// without the E1.37-1 parameters PRESET_INFO, PRESET_STATUS and PRESET_MERGEMODE.
/// </summary>
public class TestPresetsModuleE1_20
{
    private PresetsE1_20MockDevice? generated;

    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)ERDM_Parameter.CAPTURE_PRESET, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)ERDM_Parameter.CAPTURE_PRESET, 555);
    private static readonly UID DEVCIE_WITHOUT_PRESETS_UID = new UID(0x4000 + 123 + (ushort)ERDM_Parameter.CAPTURE_PRESET, 555);

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
        generated = new PresetsE1_20MockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<PresetsModule>().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module, Is.Not.TypeOf<PresetsModule_E1_37_1_2012>());
            Assert.That(module.DisplayName, Is.EqualTo("Presets"));
            Assert.That(module.Presets.Select(p => p.ID), Is.EqualTo(new ushort[] { 1, 2, 3 }));
        }
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestSupportedParameters()
    {
        Assert.That(generated, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generated.Parameters, Does.Contain(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(generated.Parameters, Does.Contain(ERDM_Parameter.CAPTURE_PRESET));
            // E1.37-1 parameters are not part of an E1.20 Presets device
            Assert.That(generated.Parameters, Does.Not.Contain(ERDM_Parameter.PRESET_INFO));
            Assert.That(generated.Parameters, Does.Not.Contain(ERDM_Parameter.PRESET_STATUS));
            Assert.That(generated.Parameters, Does.Not.Contain(ERDM_Parameter.PRESET_MERGEMODE));
        }
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetPRESET_PLAYBACK()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<PresetsModule>().Single();

        #region Test Basic: OFF by default
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0x00));
        #endregion

        #region Test SET existing scene, ALL and OFF
        assertSetResponse(sendPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(2, 0x80)), ERDM_Parameter.PRESET_PLAYBACK, null);
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(2, 0x80)));
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(2, 0x80));

        assertSetResponse(sendPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0xFF)), ERDM_Parameter.PRESET_PLAYBACK, null);
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0xFF));

        assertSetResponse(sendPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0x00)), ERDM_Parameter.PRESET_PLAYBACK, null);
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0x00));
        #endregion

        #region Test scene the device does not have
        assertSetResponse(sendPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(4, 0xFF)), ERDM_Parameter.PRESET_PLAYBACK, ERDM_NackReason.DATA_OUT_OF_RANGE);
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0x00));
        #endregion

        #region Test invalid PDL, the parser can not build it, so the bytes are needed here
        assertSetResponse(sendPlayback(generated, DEVCIE_UID, new byte[] { 0x00, 0x02 }), ERDM_Parameter.PRESET_PLAYBACK, ERDM_NackReason.FORMAT_ERROR);
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0x00));
        #endregion

        #region Test set locally
        module.CurrentPlayback = new RDMPresetPlayback(3, 0xFF);
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(3, 0xFF));
        // the same value again changes nothing
        module.CurrentPlayback = new RDMPresetPlayback(3, 0xFF);
        assertGetPlayback(generated, DEVCIE_UID, new RDMPresetPlayback(3, 0xFF));
        #endregion
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestSetPRESET_PLAYBACK_WithoutPresets()
    {
        // without known Presets there is nothing to compare with, every scene is accepted
        using var device = new PresetsE1_20WithoutPresetsMockDevice(DEVCIE_WITHOUT_PRESETS_UID);
        var module = device.Modules.OfType<PresetsModule>().Single();
        Assert.That(module.Presets, Is.Empty);

        assertSetResponse(sendPlayback(device, DEVCIE_WITHOUT_PRESETS_UID, new RDMPresetPlayback(7, 0xFF)), ERDM_Parameter.PRESET_PLAYBACK, null, DEVCIE_WITHOUT_PRESETS_UID);
        assertGetPlayback(device, DEVCIE_WITHOUT_PRESETS_UID, new RDMPresetPlayback(7, 0xFF));
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(7, 0xFF)));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestCAPTURE_PRESET()
    {
        Assert.That(generated, Is.Not.Null);

        // CAPTURE_PRESET is SET only
        var request = createRequest(DEVCIE_UID, ERDM_Parameter.CAPTURE_PRESET, ERDM_Command.GET_COMMAND);
        var response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));

        var preset = generated.Modules.OfType<PresetsModule>().Single().Presets.OfType<GeneratedPreset>().Single(p => p.ID == 2);
        request = createRequest(DEVCIE_UID, ERDM_Parameter.CAPTURE_PRESET, ERDM_Command.SET_COMMAND);
        request.ParameterData = new RDMCapturePreset(2, 1.5, 2.5, 3.5).ToPayloadData();
        assertSetResponse(generated.ProcessRequestMessage_Internal(request), ERDM_Parameter.CAPTURE_PRESET, null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(preset.Programmed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
            Assert.That(preset.UpFadeTime, Is.EqualTo(1.5).Within(0.001));
            Assert.That(preset.DownFadeTime, Is.EqualTo(2.5).Within(0.001));
            Assert.That(preset.WaitTime, Is.EqualTo(3.5).Within(0.001));
        }

        // a scene the device does not have
        request.ParameterData = new RDMCapturePreset(4, 1.5, 2.5, 3.5).ToPayloadData();
        assertSetResponse(generated.ProcessRequestMessage_Internal(request), ERDM_Parameter.CAPTURE_PRESET, ERDM_NackReason.DATA_OUT_OF_RANGE);
    }

    [Test, DependsOnTest(nameof(TestGetSetPRESET_PLAYBACK))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PresetsModule>().Single();

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        // a device without the E1.37-1 parameters gets the E1.20 module
        var module = mockDevice.Modules.OfType<PresetsModule>().Single();
        Assert.That(module, Is.Not.TypeOf<PresetsModule_E1_37_1_2012>());
        await Wait.UntilAsync(() => Wait.Holds(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0))));

        Assert.That(await module.SetCurrentPreset(new RDMPresetPlayback(2, 0xFF)), Is.True);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(2, 0xFF)))
            && Wait.Holds(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(2, 0xFF))));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(2, 0xFF)));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(2, 0xFF)));
        }
    }

    private static RDMMessage? sendPlayback(AbstractMockGeneratedDevice device, UID deviceUid, RDMPresetPlayback playback)
    {
        return sendPlayback(device, deviceUid, PayloadTestTools.SetRequestData(ERDM_Parameter.PRESET_PLAYBACK, playback));
    }
    private static RDMMessage? sendPlayback(AbstractMockGeneratedDevice device, UID deviceUid, byte[] data)
    {
        var request = createRequest(deviceUid, ERDM_Parameter.PRESET_PLAYBACK, ERDM_Command.SET_COMMAND);
        request.ParameterData = data;
        return device.ProcessRequestMessage_Internal(request);
    }
    private static RDMMessage createRequest(UID deviceUid, ERDM_Parameter parameter, ERDM_Command command)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = deviceUid,
            SourceUID = CONTROLLER_UID,
            Parameter = parameter,
            SubDevice = SubDevice.Root,
        };
    }
    private static void assertGetPlayback(AbstractMockGeneratedDevice device, UID deviceUid, RDMPresetPlayback expected)
    {
        var response = device.ProcessRequestMessage_Internal(createRequest(deviceUid, ERDM_Parameter.PRESET_PLAYBACK, ERDM_Command.GET_COMMAND));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.EqualTo(expected));
        }
    }
    private static void assertSetResponse(RDMMessage? response, ERDM_Parameter parameter, ERDM_NackReason? nackReason, UID? deviceUid = null)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(deviceUid ?? DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            if (nackReason is null)
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            }
            else
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
                Assert.That(response.NackReason, Is.EqualTo(nackReason));
            }
        }
    }

    class PresetsE1_20MockDevice : MockGeneratedDevice1
    {
        public PresetsE1_20MockDevice(UID uid) : base(uid, new IModule[] { new PresetsModule(new GeneratedPreset(1), new GeneratedPreset(2), new GeneratedPreset(3)) })
        {
        }
    }
    class PresetsE1_20WithoutPresetsMockDevice : MockGeneratedDevice1
    {
        public PresetsE1_20WithoutPresetsMockDevice(UID uid) : base(uid, new IModule[] { new PresetsModule() })
        {
        }
    }
}
