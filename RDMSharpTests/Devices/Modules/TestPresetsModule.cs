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
            presetSequenceSupported: false,
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
            ParameterData = Tools.ValueToData(new RDMCapturePreset(4, 3, 6, 7))
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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.SceneId, Is.EqualTo(expected.ID));
                Assert.That(status.UpFadeTime, Is.EqualTo(expected.UpFadeTime).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(expected.DownFadeTime).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(expected.WaitTime).Within(0.01));
                Assert.That(status.EProgrammed, Is.EqualTo(expected.Programmed));
            }
        }
        Assert.That(module._generatedPresets.Single(p => p.ID == 10).Programmed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY));
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
    [Test, DependsOnTest(nameof(TestGetPRESET_STATUS))]
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
        {
            var request = createPresetStatusRequest(ERDM_Command.SET_COMMAND, buildSetPresetStatusData(3, 25.0, 50.0, 10.0, false).Take(8).ToArray());
            var response = generated.ProcessRequestMessage_Internal(request);
            assertSetNack(response, ERDM_NackReason.FORMAT_ERROR);
        }
        #endregion

        #region Test change times (clear_preset = 0)
        {
            var response = setPresetStatus(3, 25.0, 50.0, 10.0, false);
            assertSetAck(response);

            var status = getPresetStatus(3);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.SceneId, Is.EqualTo(3));
                Assert.That(status.UpFadeTime, Is.EqualTo(25.0).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(50.0).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(10.0).Within(0.01));
                Assert.That(status.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
            }

            var preset = module._generatedPresets.Single(p => p.ID == 3);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(preset.UpFadeTime, Is.EqualTo(25.0).Within(0.01));
                Assert.That(preset.DownFadeTime, Is.EqualTo(50.0).Within(0.01));
                Assert.That(preset.WaitTime, Is.EqualTo(10.0).Within(0.01));
            }

            // other presets must not be touched
            var other = getPresetStatus(2);
            var expectedOther = module._generatedPresets.Single(p => p.ID == 2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(other.UpFadeTime, Is.EqualTo(expectedOther.UpFadeTime).Within(0.01));
                Assert.That(other.DownFadeTime, Is.EqualTo(expectedOther.DownFadeTime).Within(0.01));
                Assert.That(other.WaitTime, Is.EqualTo(expectedOther.WaitTime).Within(0.01));
            }
        }
        #endregion

        #region Test times out of PRESET_INFO range
        {
            // up fade below minimum (22.3s)
            assertSetNack(setPresetStatus(3, 10.0, 50.0, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            // down fade above maximum (100.0s)
            assertSetNack(setPresetStatus(3, 25.0, 120.0, 10.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);
            // wait time above maximum (33.3s)
            assertSetNack(setPresetStatus(3, 25.0, 50.0, 40.0, false), ERDM_NackReason.DATA_OUT_OF_RANGE);

            // rejected requests must not change the preset
            var status = getPresetStatus(3);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status.UpFadeTime, Is.EqualTo(25.0).Within(0.01));
                Assert.That(status.DownFadeTime, Is.EqualTo(50.0).Within(0.01));
                Assert.That(status.WaitTime, Is.EqualTo(10.0).Within(0.01));
            }
        }
        #endregion

        #region Test clear Preset (clear_preset = 1)
        {
            // program Preset 4 via CAPTURE_PRESET
            var captureRequest = new RDMMessage()
            {
                Command = ERDM_Command.SET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.CAPTURE_PRESET,
                SubDevice = SubDevice.Root,
                ParameterData = Tools.ValueToData(new RDMCapturePreset(4, 30.0, 40.0, 5.0))
            };
            var captureResponse = generated.ProcessRequestMessage_Internal(captureRequest);
            Assert.That(captureResponse, Is.Not.Null);
            Assert.That(captureResponse.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(getPresetStatus(4).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));

            // changing only the times keeps it programmed
            assertSetAck(setPresetStatus(4, 35.0, 45.0, 6.0, false));
            Assert.That(getPresetStatus(4).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));

            // clear it, time fields are ignored (even if out of range)
            assertSetAck(setPresetStatus(4, 0, 0, 0, true));
            Assert.That(getPresetStatus(4).EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.NOT_PROGRAMMED));
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
    private static void assertSetAck(RDMMessage? response)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_STATUS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        }
    }
    private static void assertSetNack(RDMMessage? response, ERDM_NackReason reason)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.PRESET_STATUS));
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

        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetPRESET_PLAYBACK))]
    public async Task TestRemoteDevicePRESET_PLAYBACK()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PresetsModule>().Single();
        generatedModule.CurrentPlayback = new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0);
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PresetsModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentPlayback, Is.EqualTo(new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0)));

        await module.SetCurrentPreset(new RDMPresetPlayback(ERDM_PresetPlayback.ALL, 0));
        await Task.Delay(1000);

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

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
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

        await module.CapturePreset(new RDMCapturePreset(4, 3, 6, 7));
        await Task.Delay(1000);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            var expected = new RDMCapturePreset(4, 3, 6, 7);
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

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
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

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentMergeMode, Is.EqualTo(ERDM_MergeMode.DEFAULT));

        await module.SetCurrentMergeMode(ERDM_MergeMode.OTHER);
        await Task.Delay(1000);

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
        private static GeneratedPreset[] createPresets() => new GeneratedPreset[]
        {
             new GeneratedPreset(1, 10, 20, 30),
             new GeneratedPreset(2, 20, 30, 40),
             new GeneratedPreset(3, 30, 40, 50),
             new GeneratedPreset(4, 40, 50, 60),
             new GeneratedPreset(5, 50, 60, 70),
             new GeneratedPreset(6, 60, 70, 80),
             new GeneratedPreset(7, 70, 80, 90),
             new GeneratedPreset(8, 80, 90, 100),
             new GeneratedPreset(9, 90, 100, 110),
             new GeneratedPreset(10, 100, 110, 120, ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY)
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