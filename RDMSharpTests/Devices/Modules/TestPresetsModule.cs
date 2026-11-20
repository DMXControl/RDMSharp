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
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single();
        Assert.That(module, Is.Not.Null);
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.PRESET_STATUS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response;
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
        private static GeneratedPreset[] PRESETS = new GeneratedPreset[]
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
             new GeneratedPreset(10, 100, 110, 120)
        };
        public event EventHandler<RDMCapturePreset>? CapturePresetEvent;
        public MockPresetsModule() : base(PRESET_INFO, ERDM_MergeMode.DEFAULT, PRESETS)
        {
        }

        protected override void OnCapturePreset(RDMCapturePreset preset)
        {
            CapturePresetEvent?.InvokeFailSafe(this, preset);
        }
    }
}