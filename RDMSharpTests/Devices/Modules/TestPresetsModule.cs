using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestPresetsModule
{
    private PresetsMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.PRESET_PLAYBACK;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);

    private static readonly RDMPresetInfo PRESET_INFO = new RDMPresetInfo(
            levelFieldSupported: true,
            presetSequenceSupported: true,
            splitTimesSupported: true,
            dmx512FailInfiniteDelayTimeSupported: false,
            dmx512FailInfiniteHoldTimeSupported: true,
            startupInfiniteHoldTimeSupported: false,
            maximumSceneNumber: 10,
            minimumPresetFadeTimeSupported: 22.3,
            maximumPresetFadeTimeSupported: 100.0,
            minimumPresetWaitTimeSupported: 1.0,
            maximumPresetWaitTimeSupported: 33.3,
            minimumDMX512FailDelayTimeSupported: 2.0,
            maximumDMX512FailDelayTimeSupported: 100.0,
            minimumDMX512FailDelayHoldSupported: 3.0,
            maximumDMX512FailDelayHoldSupported: 100.0,
            minimumStartupDelayHoldSupported: 4.0,
            maximumStartupDelayHoldSupported: 100.0,
            minimumStartupDelayTimeSupported: 5.0,
            maximumStartupDelayTimeSupported: 100.0);

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
        generated = new PresetsMockDevice(DEVCIE_UID);
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
        var presetsModule = generated.Modules.OfType<PresetsModule>().Single();
        Assert.That(presetsModule, Is.Not.Null);
        Assert.That(presetsModule.DisplayName, Is.EqualTo("Presets"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetPRESET_PLAYBACK()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PRESET_PLAYBACK,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3));
            Assert.That(response.Value, Is.EqualTo(module.CurrentPlayback));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));
        }
        #endregion

        #region Test CurrentPreset changed (GET)
        module.CurrentPlayback = new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0);
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3));
            Assert.That(response.Value, Is.EqualTo(module.CurrentPlayback));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)));
        }
        #endregion


        #region Test CurrentPreset changed (SET)
        request.ParameterData = new byte[] { 0x00, 0x00, 0x00 }; // Requesting CurrentPreset OFF
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));
        }
        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Array.Empty<byte>(); // GET has no Parameter Data
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3));
            Assert.That(response.Value, Is.EqualTo(module.CurrentPlayback));
        }

        request.ParameterData = new byte[] { 0xff, 0xff, 0xff }; // Requesting Preset Playback ALL
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0xff)));
        }

        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Array.Empty<byte>(); // GET has no Parameter Data
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3));
            Assert.That(response.Value, Is.EqualTo(module.CurrentPlayback));
        }

        // set locally back to the value set before the RDM SET, this has to be applied
        module.CurrentPlayback = new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)));
        }

        #endregion
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestSetCAPTURE_PRESET()
    {
        #region Test GET
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule>().Single();
        Assert.That(module, Is.Not.Null);
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CAPTURE_PRESET,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CAPTURE_PRESET));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
        }
        #endregion

        #region Test SET
        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CAPTURE_PRESET,
            SubDevice = SubDevice.Root,
            ParameterData = Tools.ValueToData(new RDMCapturePreset(4, 30, 40, 5))
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CAPTURE_PRESET));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        }
        #endregion


        //#region Test CurrentPreset changed (SET)
        //request.ParameterData = new byte[] { 0x00, 0x00, 0x00 }; // Requesting CurrentPreset OFF
        //request.Command = ERDM_Command.SET_COMMAND;
        //response = generated.ProcessRequestMessage_Internal(request);
        //Assert.That(response, Is.Not.Null);
        //using (Assert.EnterMultipleScope())
        //{
        //    Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        //    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        //    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        //    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
        //    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        //    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        //    Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        //    Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));
        //}
        //request.Command = ERDM_Command.GET_COMMAND;
        //response = generated.ProcessRequestMessage_Internal(request);
        //Assert.That(response, Is.Not.Null);
        //using (Assert.EnterMultipleScope())
        //{
        //    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        //    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        //    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        //    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
        //    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        //    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        //    Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        //    Assert.That(response.Value, Is.EqualTo(module.CurrentPlayback));
        //}

        //request.ParameterData = new byte[] { 0xff, 0xff, 0xff }; // Requesting Preset Playback ALL
        //request.Command = ERDM_Command.SET_COMMAND;
        //response = generated.ProcessRequestMessage_Internal(request);
        //Assert.That(response, Is.Not.Null);
        //using (Assert.EnterMultipleScope())
        //{
        //    Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        //    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        //    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        //    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
        //    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        //    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        //    Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        //    Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0xff)));
        //}

        //request.Command = ERDM_Command.GET_COMMAND;
        //response = generated.ProcessRequestMessage_Internal(request);
        //Assert.That(response, Is.Not.Null);
        //using (Assert.EnterMultipleScope())
        //{
        //    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        //    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        //    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        //    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_PLAYBACK));
        //    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        //    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        //    Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        //    Assert.That(response.Value, Is.EqualTo(module.CurrentPlayback));
        //}

        //#endregion
    }

    /// <summary>
    /// CAPTURE_PRESET (E1.20 10.11.6): PDL 2 (scene only) or PDL 8 (scene + up/down fade + wait time in 0.1s), SET only.
    /// The captured Preset becomes PROGRAMMED. Scene range is 0x0001 - 0xFFFE and limited by PRESET_INFO.
    /// The time limits of PRESET_INFO are checked like for SET PRESET_STATUS (not demanded by the standard, but consistent).
    /// </summary>
    [Test, DependsOnTest(nameof(TestSetCAPTURE_PRESET))]
    public void TestSetCAPTURE_PRESET_Details()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        const ERDM_Command SET = ERDM_Command.SET_COMMAND;
        const ERDM_Parameter CP = ERDM_Parameter.CAPTURE_PRESET;

        #region Test full capture (PDL 8)
        {
            Assert.That(getPresetStatus(3).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
            assertSetAck(send(SET, CP, Tools.ValueToData(new RDMCapturePreset(3, 30.0, 40.0, 5.0))), CP);

            var status = getPresetStatus(3);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
                Assert.That(status.UpFadeTime, Is.EqualTo(30.0).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(40.0).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(5.0).Within(0.01));
            }
            var preset = module._generatedPresets.Single(p => p.ID == 3);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(preset.Programmed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
                Assert.That(preset.UpFadeTime, Is.EqualTo(30.0).Within(0.01));
                Assert.That(preset.DownFadeTime, Is.EqualTo(40.0).Within(0.01));
                Assert.That(preset.WaitTime, Is.EqualTo(5.0).Within(0.01));
            }
        }
        #endregion

        #region Test capture without times (PDL 2)
        {
            Assert.That(getPresetStatus(4).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
            assertSetAck(send(SET, CP, Tools.ValueToData((ushort)4)), CP);
            Assert.That(getPresetStatus(4).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
        }
        #endregion

        #region Test invalid Scenes
        foreach (ushort invalidScene in new ushort[] { 0, (ushort)(PRESET_INFO.MaximumSceneNumber + 1), 0xFFFE, 0xFFFF })
        {
            assertSetNack(send(SET, CP, Tools.ValueToData(invalidScene)), ERDM_NackReason.DATA_OUT_OF_RANGE, CP);
            assertSetNack(send(SET, CP, Tools.ValueToData(new RDMCapturePreset(invalidScene, 30.0, 40.0, 5.0))), ERDM_NackReason.DATA_OUT_OF_RANGE, CP);
        }
        #endregion

        #region Test invalid PDL
        foreach (int length in new[] { 0, 1, 3, 4, 7, 9, 10 })
        {
            var data = new byte[length];
            if (length >= 2)
                data[1] = 0x05; // valid Scene 5, only the length is wrong
            assertSetNack(send(SET, CP, data), ERDM_NackReason.FORMAT_ERROR, CP);
        }
        Assert.That(getPresetStatus(5).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
        #endregion

        #region Test read only Preset
        {
            var before = getPresetStatus(10);
            assertSetNack(send(SET, CP, Tools.ValueToData(new RDMCapturePreset(10, 30.0, 40.0, 5.0))), ERDM_NackReason.WRITE_PROTECT, CP);
            assertSetNack(send(SET, CP, Tools.ValueToData((ushort)10)), ERDM_NackReason.WRITE_PROTECT, CP);
            var after = getPresetStatus(10);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(after.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY));
                Assert.That(after.UpFadeTime, Is.EqualTo(before.UpFadeTime).Within(0.01));
                Assert.That(after.DownFadeTime, Is.EqualTo(before.DownFadeTime).Within(0.01));
                Assert.That(after.WaitTime, Is.EqualTo(before.WaitTime).Within(0.01));
            }
        }
        #endregion

        #region Test times out of PRESET_INFO range
        {
            assertSetNack(send(SET, CP, Tools.ValueToData(new RDMCapturePreset(5, 10.0, 40.0, 5.0))), ERDM_NackReason.DATA_OUT_OF_RANGE, CP);
            assertSetNack(send(SET, CP, Tools.ValueToData(new RDMCapturePreset(5, 30.0, 120.0, 5.0))), ERDM_NackReason.DATA_OUT_OF_RANGE, CP);
            assertSetNack(send(SET, CP, Tools.ValueToData(new RDMCapturePreset(5, 30.0, 40.0, 40.0))), ERDM_NackReason.DATA_OUT_OF_RANGE, CP);
            Assert.That(getPresetStatus(5).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED), "rejected capture must not program the Preset");
        }
        #endregion
    }

    /// <summary>
    /// PRESET_PLAYBACK (E1.20 10.11.7 / Table A-7): mode 0x0000 OFF, 0xFFFF ALL, 0x0001 - 0xFFFE Scene #, level 0x00 - 0xFF, PDL 3.
    /// </summary>
    [Test, DependsOnTest(nameof(TestGetPRESET_PLAYBACK))]
    public void TestSetPRESET_PLAYBACK_Validation()
    {
        Assert.That(generated, Is.Not.Null);
        const ERDM_Parameter PB = ERDM_Parameter.PRESET_PLAYBACK;

        #region Test valid modes
        foreach (var (mode, level) in new (ushort, byte)[] { (1, 0x80), (2, 0x00), (1, 0xFF), (0xFFFF, 0xFF), (0, 0x00) })
        {
            assertSetAck(send(ERDM_Command.SET_COMMAND, PB, PayloadTestTools.SetRequestData(PB, new RDMPresetPlayback(mode, level))), PB);
            var playback = getPlayback();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(playback.Mode, Is.EqualTo(mode));
                Assert.That(playback.Level, Is.EqualTo(level));
            }
        }
        #endregion

        #region Test invalid Scenes
        assertSetAck(send(ERDM_Command.SET_COMMAND, PB, PayloadTestTools.SetRequestData(PB, new RDMPresetPlayback(2, 0x40))), PB);
        foreach (ushort invalidScene in new ushort[] { (ushort)(PRESET_INFO.MaximumSceneNumber + 1), 99, 0xFFFE })
        {
            assertSetNack(send(ERDM_Command.SET_COMMAND, PB, PayloadTestTools.SetRequestData(PB, new RDMPresetPlayback(invalidScene, 0x80))), ERDM_NackReason.DATA_OUT_OF_RANGE, PB);
            var playback = getPlayback();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(playback.Mode, Is.EqualTo(2), "rejected request must not change the playback");
                Assert.That(playback.Level, Is.EqualTo(0x40));
            }
        }
        #endregion

        #region Test invalid PDL
        foreach (int length in new[] { 0, 1, 2, 4 })
        {
            var data = new byte[length];
            if (length >= 2)
                data[1] = 0x01; // valid Scene 1, only the length is wrong
            assertSetNack(send(ERDM_Command.SET_COMMAND, PB, data), ERDM_NackReason.FORMAT_ERROR, PB);
            var playback = getPlayback();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(playback.Mode, Is.EqualTo(2), "rejected request must not change the playback");
                Assert.That(playback.Level, Is.EqualTo(0x40));
            }
        }
        #endregion
    }

    private RDMPresetPlayback getPlayback()
    {
        var response = send(ERDM_Command.GET_COMMAND, ERDM_Parameter.PRESET_PLAYBACK, new byte[0]);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.Value, Is.TypeOf<RDMPresetPlayback>());
        return (RDMPresetPlayback)response.Value;
    }
    private static void assertCleared(GetPresetStatus status)
    {
        // E1.37-1 5.3: not programmed Presets report all timing information as zero
        using (Assert.EnterMultipleScope())
        {
            Assert.That(status.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
            Assert.That(status.UpFadeTime, Is.Zero);
            Assert.That(status.DownFadeTime, Is.Zero);
            Assert.That(status.WaitTime, Is.Zero);
        }
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetPRESET_INFO()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.PresetInfo, Is.EqualTo(PRESET_INFO));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PRESET_INFO,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0x20));
            Assert.That(response.Value, Is.EqualTo(module.PresetInfo));
            Assert.That(module.PresetInfo, Is.EqualTo(PRESET_INFO));
        }
        #endregion
    }

    /// <summary>
    /// PRESET_INFO is GET only (E1.37-1 5.2), a SET shall be answered with NR_UNSUPPORTED_COMMAND_CLASS and must not harm the stored value.
    /// </summary>
    [Test, DependsOnTest(nameof(TestGetPRESET_INFO))]
    public void TestSetPRESET_INFO()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        const ERDM_Parameter PI = ERDM_Parameter.PRESET_INFO;

        foreach (int length in new[] { 0, 0x20 })
            assertSetNack(send(ERDM_Command.SET_COMMAND, PI, new byte[length]), ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS, PI);

        var response = send(ERDM_Command.GET_COMMAND, PI, new byte[0]);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0x20));
            Assert.That(response.Value, Is.EqualTo(PRESET_INFO));
            Assert.That(module.PresetInfo, Is.EqualTo(PRESET_INFO));
        }
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetPRESET_STATUS()
    {
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);

        #region Test invalid Scene (0)
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PRESET_STATUS,
            SubDevice = SubDevice.Root,
            ParameterData = Tools.ValueToData((ushort)0) // Requesting invalid Scene 0
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_STATUS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        }
        #endregion

        #region Test invalid requests
        foreach (ushort invalidScene in new ushort[] { (ushort)(PRESET_INFO.MaximumSceneNumber + 1), 0xFFFE, 0xFFFF })
        {
            request.ParameterData = Tools.ValueToData(invalidScene);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
                Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
            }
        }
        foreach (int length in new[] { 0, 1, 3 })
        {
            request.ParameterData = new byte[length];
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
                Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
            }
        }
        #endregion

        #region Test every generated Preset (GET)
        Assert.That(module._generatedPresets, Is.Not.Null);
        Assert.That(module._generatedPresets, Has.Count.EqualTo(PRESET_INFO.MaximumSceneNumber));
        foreach (var expected in module._generatedPresets)
        {
            request.ParameterData = Tools.ValueToData(expected.ID);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_STATUS));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(GetPresetStatus.PDL));
                Assert.That(response.Value, Is.TypeOf<GetPresetStatus>());
            }
            var status = (GetPresetStatus)response.Value;

            // E1.37-1 5.3: if a Preset is NOT_PROGRAMMED all timing information in the response shall be zero
            bool programmed = expected.Programmed != ERDM_PresetProgrammed.NOT_PROGRAMMED;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.SceneId, Is.EqualTo(expected.ID));
                Assert.That(status.UpFadeTime, Is.EqualTo(programmed ? expected.UpFadeTime : 0).Within(0.01), $"Scene {expected.ID}");
                Assert.That(status.DownFadeTime, Is.EqualTo(programmed ? expected.DownFadeTime : 0).Within(0.01), $"Scene {expected.ID}");
                Assert.That(status.WaitTime, Is.EqualTo(programmed ? expected.WaitTime : 0).Within(0.01), $"Scene {expected.ID}");
                Assert.That(status.EProgrammed, Is.EqualTo(expected.Programmed), $"Scene {expected.ID}");
            }
        }

        // the mock has to cover all three states, including a not programmed Preset that still carries times
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module._generatedPresets.Single(p => p.ID == 1).Programmed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
            Assert.That(module._generatedPresets.Single(p => p.ID == 9).Programmed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
            Assert.That(module._generatedPresets.Single(p => p.ID == 9).UpFadeTime, Is.Not.Zero);
            Assert.That(module._generatedPresets.Single(p => p.ID == 10).Programmed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY));
        }
        #endregion
    }

    /// <summary>
    /// SET PRESET_STATUS according to E1.37-1:
    /// PD = scene_num (uint16), up_fade_time, down_fade_time, wait_time (uint16, 0.1s), clear_preset (uint8 0/1) → PDL 9.
    /// Response is an empty ACK.
    /// - scene 0, 0xFFFF or above PRESET_INFO.MaximumSceneNumber → NACK DATA_OUT_OF_RANGE
    /// - times outside the limits announced in PRESET_INFO → NACK DATA_OUT_OF_RANGE
    /// - clear_preset = 1 clears the preset (NOT_PROGRAMMED), the time fields are ignored
    /// - clear_preset = 0 only changes the times, the programmed state stays untouched
    /// </summary>
    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestSetPRESET_STATUS()
    {
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);

        #region Test invalid Scenes
        foreach (ushort invalidScene in new ushort[] { 0, (ushort)(PRESET_INFO.MaximumSceneNumber + 1), 0xFFFF })
        {
            var response = setPresetStatus(invalidScene, 25.0, 50.0, 10.0, false);
            assertSetNack(response, ERDM_NackReason.DATA_OUT_OF_RANGE);
        }
        #endregion

        #region Test invalid PDL
        foreach (int length in new[] { 0, 8, 10 })
        {
            var data = new byte[length];
            if (length >= 2)
                data[1] = 0x01; // valid Scene 1, only the length is wrong
            assertSetNack(send(ERDM_Command.SET_COMMAND, ERDM_Parameter.PRESET_STATUS, data), ERDM_NackReason.FORMAT_ERROR);
        }
        #endregion

        #region Test change times (clear_preset = 0) of a programmed Preset
        {
            var response = setPresetStatus(1, 25.0, 50.0, 10.0, false);
            assertSetAck(response);

            var status = getPresetStatus(1);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.SceneId, Is.EqualTo(1));
                Assert.That(status.UpFadeTime, Is.EqualTo(25.0).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(50.0).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(10.0).Within(0.01));
                Assert.That(status.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED), "clear_preset = 0 must not change the programmed state");
            }

            var preset = module._generatedPresets.Single(p => p.ID == 1);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(preset.UpFadeTime, Is.EqualTo(25.0).Within(0.01));
                Assert.That(preset.DownFadeTime, Is.EqualTo(50.0).Within(0.01));
                Assert.That(preset.WaitTime, Is.EqualTo(10.0).Within(0.01));
            }

            // other presets must not be touched
            var other = getPresetStatus(2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(other.UpFadeTime, Is.EqualTo(35.0).Within(0.01));
                Assert.That(other.DownFadeTime, Is.EqualTo(45.0).Within(0.01));
                Assert.That(other.WaitTime, Is.EqualTo(10.0).Within(0.01));
            }
        }
        #endregion

        #region Test time limits of PRESET_INFO (tenths of a second)
        {
            // Fade limits 22.3 - 100.0, Wait limits 1.0 - 33.3: the limits themselves are valid
            assertSetAck(setPresetStatus(1, 22.3, 22.3, 1.0, false));
            assertSetAck(setPresetStatus(1, 100.0, 100.0, 33.3, false));
            var accepted = getPresetStatus(1);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(accepted.UpFadeTime, Is.EqualTo(100.0).Within(0.01));
                Assert.That(accepted.DownFadeTime, Is.EqualTo(100.0).Within(0.01));
                Assert.That(accepted.WaitTime, Is.EqualTo(33.3).Within(0.01));
            }

            // one tenth outside of the limits and far outside of the limits
            assertSetNack(setPresetStatus(1, 22.2, 50.0, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 50.0, 22.2, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 100.1, 50.0, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 50.0, 100.1, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 50.0, 50.0, 0.9, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 50.0, 50.0, 33.4, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 10.0, 50.0, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 25.0, 120.0, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertSetNack(setPresetStatus(1, 25.0, 50.0, 40.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);

            // rejected requests must not change the preset
            var status = getPresetStatus(1);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.UpFadeTime, Is.EqualTo(100.0).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(100.0).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(33.3).Within(0.01));
            }
        }
        #endregion

        #region Test clear Preset (clear_preset = 1)
        {
            // program Preset 4 via CAPTURE_PRESET
            var captureResponse = send(ERDM_Command.SET_COMMAND, ERDM_Parameter.CAPTURE_PRESET, Tools.ValueToData(new RDMCapturePreset(4, 30.0, 40.0, 5.0)));
            assertSetAck(captureResponse, ERDM_Parameter.CAPTURE_PRESET);
            Assert.That(getPresetStatus(4).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));

            // changing only the times keeps it programmed
            assertSetAck(setPresetStatus(4, 35.0, 45.0, 6.0, false));
            var changed = getPresetStatus(4);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(changed.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
                Assert.That(changed.UpFadeTime, Is.EqualTo(35.0).Within(0.01));
                Assert.That(changed.DownFadeTime, Is.EqualTo(45.0).Within(0.01));
                Assert.That(changed.WaitTime, Is.EqualTo(6.0).Within(0.01));
            }

            // E1.37-1 5.3: clearing resets all timing information to zero, the times of the SET are ignored (even if out of range)
            assertSetAck(setPresetStatus(4, 0, 0, 0, true));
            assertCleared(getPresetStatus(4));

            assertSetAck(setPresetStatus(1, 999.0, 999.0, 999.0, true));
            assertCleared(getPresetStatus(1));
        }
        #endregion

        #region Test read only Preset
        {
            var before = getPresetStatus(10);
            Assert.That(before.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY));

            assertSetNack(setPresetStatus(10, 25.0, 50.0, 10.0, false), ERDM_NackReason.WRITE_PROTECT);
            assertSetNack(setPresetStatus(10, 0, 0, 0, true), ERDM_NackReason.WRITE_PROTECT);

            var after = getPresetStatus(10);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(after.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY));
                Assert.That(after.UpFadeTime, Is.EqualTo(before.UpFadeTime).Within(0.01));
                Assert.That(after.DownFadeTime, Is.EqualTo(before.DownFadeTime).Within(0.01));
                Assert.That(after.WaitTime, Is.EqualTo(before.WaitTime).Within(0.01));
            }
        }
        #endregion

        #region Test Broadcast (Sub-Device 0xFFFF is allowed for SET)
        {
            // no response is sent on Broadcast, so only the result is checked
            send(ERDM_Command.SET_COMMAND, ERDM_Parameter.PRESET_STATUS, buildSetPresetStatusData(2, 40.0, 41.0, 12.0, false), SubDevice.Broadcast);
            var status = getPresetStatus(2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.UpFadeTime, Is.EqualTo(40.0).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(41.0).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(12.0).Within(0.01));
                Assert.That(status.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
            }
        }
        #endregion
    }

    private RDMMessage createPresetStatusRequest(ERDM_Command command, byte[] parameterData)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PRESET_STATUS,
            SubDevice = SubDevice.Root,
            ParameterData = parameterData
        };
    }
    private static byte[] buildSetPresetStatusData(ushort scene, double upFade, double downFade, double wait, bool clear)
    {
        var data = new List<byte>();
        data.AddRange(Tools.ValueToData(scene));
        data.AddRange(Tools.ValueToData((ushort)Math.Round(upFade * 10)));
        data.AddRange(Tools.ValueToData((ushort)Math.Round(downFade * 10)));
        data.AddRange(Tools.ValueToData((ushort)Math.Round(wait * 10)));
        data.Add(clear ? (byte)0x01 : (byte)0x00);
        return data.ToArray();
    }
    private RDMMessage? setPresetStatus(ushort scene, double upFade, double downFade, double wait, bool clear)
    {
        return generated!.ProcessRequestMessage_Internal(createPresetStatusRequest(ERDM_Command.SET_COMMAND, buildSetPresetStatusData(scene, upFade, downFade, wait, clear)));
    }
    private GetPresetStatus getPresetStatus(ushort scene)
    {
        var response = generated!.ProcessRequestMessage_Internal(createPresetStatusRequest(ERDM_Command.GET_COMMAND, Tools.ValueToData(scene)));
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.Value, Is.TypeOf<GetPresetStatus>());
        return (GetPresetStatus)response.Value;
    }
    private RDMMessage? send(ERDM_Command command, ERDM_Parameter parameter, byte[] parameterData, SubDevice? subDevice = null)
    {
        return generated!.ProcessRequestMessage_Internal(new RDMMessage()
        {
            Command = command,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = parameter,
            SubDevice = subDevice ?? SubDevice.Root,
            ParameterData = parameterData
        });
    }
    private static void assertSetAck(RDMMessage? response, ERDM_Parameter parameter = ERDM_Parameter.PRESET_STATUS)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        }
    }
    private static void assertSetNack(RDMMessage? response, ERDM_NackReason reason, ERDM_Parameter parameter = ERDM_Parameter.PRESET_STATUS)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(reason));
        }
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetPRESET_MERGEMODE()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PRESET_MERGEMODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_MERGEMODE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(module.CurrentMergeMode));
            Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DEFAULT));
        }
        #endregion

        #region Test MergeMode changed (GET)
        module.CurrentMergeMode = ERDM_MergeMode.DMX_ONLY;
        Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DMX_ONLY));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_MERGEMODE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(module.CurrentMergeMode));
            Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DMX_ONLY));
        }
        #endregion


        #region Test MergeMode changed (SET)
        request.ParameterData = new byte[] { 0x02 }; // Requesting CurrentPreset OFF
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_MERGEMODE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.LTP));
        }
        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Array.Empty<byte>(); // GET has no Parameter Data
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_MERGEMODE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(module.CurrentMergeMode));
        }

        request.ParameterData = new byte[] { 0x01 }; // Requesting Preset Playback ALL
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_MERGEMODE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.HTP));
        }

        request.Command = ERDM_Command.GET_COMMAND;
        request.ParameterData = Array.Empty<byte>(); // GET has no Parameter Data
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_MERGEMODE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(module.CurrentMergeMode));
        }

        // set locally back to the value set before the RDM SET, this has to be applied
        module.CurrentMergeMode = ERDM_MergeMode.DMX_ONLY;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.EqualTo(ERDM_MergeMode.DMX_ONLY));
            Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DMX_ONLY));
        }

        #endregion
    }

    /// <summary>
    /// PRESET_MERGEMODE (E1.37-1 5.4 / Table A-3): defined values are 0x00-0x03 and 0xFF (OTHER).
    /// Undefined values are out of range, the PDL has to be exactly 1.
    /// </summary>
    [Test, DependsOnTest(nameof(TestGetPRESET_MERGEMODE))]
    public void TestSetPRESET_MERGEMODE_Validation()
    {
        Assert.That(generated, Is.Not.Null);
        const ERDM_Parameter MM = ERDM_Parameter.PRESET_MERGEMODE;

        #region Test defined values
        foreach (var mode in new[] { ERDM_MergeMode.HTP, ERDM_MergeMode.LTP, ERDM_MergeMode.DMX_ONLY, ERDM_MergeMode.DEFAULT, ERDM_MergeMode.DMX_ONLY })
        {
            assertSetAck(send(ERDM_Command.SET_COMMAND, MM, PayloadTestTools.SetRequestData(MM, mode)), MM);
            Assert.That(getMergeMode(), Is.EqualTo(mode));
        }
        #endregion

        #region Test undefined values, they are no ERDM_MergeMode, so the bytes are needed here
        foreach (byte invalid in new byte[] { 0x04, 0x7F, 0xFE })
        {
            assertSetNack(send(ERDM_Command.SET_COMMAND, MM, new byte[] { invalid }), ERDM_NackReason.DATA_OUT_OF_RANGE, MM);
            Assert.That(getMergeMode(), Is.EqualTo(ERDM_MergeMode.DMX_ONLY), "rejected request must not change the Merge Mode");
        }
        #endregion

        #region Test invalid PDL, the parser can not build it, so the bytes are needed here
        foreach (int length in new[] { 0, 2, 3 })
        {
            assertSetNack(send(ERDM_Command.SET_COMMAND, MM, new byte[length]), ERDM_NackReason.FORMAT_ERROR, MM);
            Assert.That(getMergeMode(), Is.EqualTo(ERDM_MergeMode.DMX_ONLY), "rejected request must not change the Merge Mode");
        }
        #endregion
    }

    private ERDM_MergeMode getMergeMode()
    {
        var response = send(ERDM_Command.GET_COMMAND, ERDM_Parameter.PRESET_MERGEMODE, new byte[0]);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.Value, Is.TypeOf<ERDM_MergeMode>());
        return (ERDM_MergeMode)response.Value;
    }

    [Test, DependsOnTest(nameof(TestGetPRESET_PLAYBACK))]
    public async Task TestRemoteDevicePRESET_PLAYBACK()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PresetsModule>().Single();
        generatedModule.CurrentPlayback = new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0);
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PresetsModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));

        await module.SetCurrentPreset(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0));
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)))
            && Wait.Holds(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0))));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)));
            Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0)));
        }
    }

    [Test, DependsOnTest(nameof(TestSetCAPTURE_PRESET))]
    public async Task TestRemoteDeviceCAPTURE_PRESET()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<MockPresetsModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PresetsModule>().Single();
        Assert.That(module, Is.Not.Null);

        RDMCapturePreset result = null;
        generatedModule.CapturePresetEvent += (s, e) =>
        {
            Assert.That(e, Is.Not.Null);
            result = e;
        };

        await module.CapturePreset(new RDMCapturePreset(4, 30, 40, 5));
        await Wait.UntilAsync(() => Wait.Holds(result, Is.Not.Null));

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            var expected = new RDMCapturePreset(4, 30, 40, 5);
            Assert.That(result.SceneId, Is.EqualTo(expected.SceneId));
            Assert.That(result.WaitTime, Is.EqualTo(expected.WaitTime).Within(0.01));
            Assert.That(result.UpFadeTime, Is.EqualTo(expected.UpFadeTime).Within(0.01));
            Assert.That(result.DownFadeTime, Is.EqualTo(expected.DownFadeTime).Within(0.01));

        }
    }

    [Test, DependsOnTest(nameof(TestGetPRESET_INFO))]
    public async Task TestRemoteDevicePRESET_INFO()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.PresetInfo, Is.EqualTo(PRESET_INFO));

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.PresetInfo, Is.EqualTo(PRESET_INFO));
    }

    [Test, DependsOnTest(nameof(TestGetPRESET_MERGEMODE))]
    public async Task TestRemoteDevicePRESET_MERGEMODE()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        generatedModule.CurrentMergeMode = ERDM_MergeMode.DEFAULT;
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DEFAULT));

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DEFAULT));

        await module.SetCurrentMergeMode(ERDM_MergeMode.OTHER);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.OTHER))
            && Wait.Holds(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.OTHER)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.OTHER));
            Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.OTHER));
        }
    }
    class PresetsMockDevice : MockGeneratedDevice1
    {
        public PresetsMockDevice(UID uid) : base(uid, new IModule[] { new MockPresetsModule() })
        {
        }
    }
    sealed class MockPresetsModule : PresetsModule_E1_37_1_2012
    {
        // created per instance, SET/CAPTURE tests modify the presets
        // All times are inside the limits of PRESET_INFO (Fade 22.3 - 100.0, Wait 1.0 - 33.3)
        private static GeneratedPreset[] createPresets() => new GeneratedPreset[]
        {
             new GeneratedPreset(1, 30, 40, 5, ERDM_PresetProgrammed.PROGRAMMED),
             new GeneratedPreset(2, 35, 45, 10, ERDM_PresetProgrammed.PROGRAMMED),
             new GeneratedPreset(3),
             new GeneratedPreset(4),
             new GeneratedPreset(5),
             new GeneratedPreset(6),
             new GeneratedPreset(7),
             new GeneratedPreset(8),
             // not programmed, but still carries times: the response has to report zero (E1.37-1 5.3)
             new GeneratedPreset(9, 30, 40, 5, ERDM_PresetProgrammed.NOT_PROGRAMMED),
             new GeneratedPreset(10, 50, 60, 20, ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY)
        };
        public event EventHandler<RDMCapturePreset>? CapturePresetEvent;
        public MockPresetsModule() : base(PRESET_INFO, ERDM_MergeMode.DEFAULT, createPresets())
        {
        }

        protected override void OnCapturePreset(RDMCapturePreset preset)
        {
            CapturePresetEvent?.InvokeFailSafe(this, preset);
        }
    }
}