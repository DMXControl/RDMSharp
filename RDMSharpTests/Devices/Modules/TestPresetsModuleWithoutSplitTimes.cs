using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// <see cref="PresetsModule_E1_37_1_2012"/> of a device that does not support split times
/// and has less Presets than its Maximum Scene Number (E1.37-1 5.2 / 5.3).
/// </summary>
public class TestPresetsModuleWithoutSplitTimes
{
    private PresetsWithoutSplitTimesMockDevice? generated;

    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)ERDM_Parameter.PRESET_STATUS, 333);
    private static readonly UID DEVCIE_UID = new UID(0x4000 + 123 + (ushort)ERDM_Parameter.PRESET_STATUS, 555);

    // Maximum Scene Number 5, but only the Presets 1 and 2 exist
    private static readonly RDMPresetInfo PRESET_INFO = new RDMPresetInfo(
            levelFieldSupported: true,
            presetSequenceSupported: false,
            splitTimesSupported: false,
            maximumSceneNumber: 5,
            minimumPresetFadeTimeSupported: 0.0,
            maximumPresetFadeTimeSupported: 100.0,
            minimumPresetWaitTimeSupported: 0.0,
            maximumPresetWaitTimeSupported: 100.0);

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
        generated = new PresetsWithoutSplitTimesMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestSetPRESET_STATUS_SceneWithoutPreset()
    {
        Assert.That(generated, Is.Not.Null);

        // Scene 3 is inside the Maximum Scene Number, but the device has no Preset for it
        assertSetResponse(send(ERDM_Parameter.PRESET_STATUS, PayloadTestTools.SetRequestData(ERDM_Parameter.PRESET_STATUS, new SetPresetStatus(3, 2.0, 2.0, 1.0, false))), ERDM_Parameter.PRESET_STATUS, ERDM_NackReason.DATA_OUT_OF_RANGE);
        // Scene 6 is above the Maximum Scene Number
        assertSetResponse(send(ERDM_Parameter.PRESET_STATUS, PayloadTestTools.SetRequestData(ERDM_Parameter.PRESET_STATUS, new SetPresetStatus(6, 2.0, 2.0, 1.0, false))), ERDM_Parameter.PRESET_STATUS, ERDM_NackReason.DATA_OUT_OF_RANGE);
    }

    [Test]
    public void TestSetPRESET_STATUS_WithoutSplitTimes()
    {
        Assert.That(generated, Is.Not.Null);
        var preset = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single().Presets.OfType<GeneratedPreset>().Single(p => p.ID == 2);

        // without split times the Up Fade Time is used for both fade times
        assertSetResponse(send(ERDM_Parameter.PRESET_STATUS, PayloadTestTools.SetRequestData(ERDM_Parameter.PRESET_STATUS, new SetPresetStatus(2, 4.0, 6.0, 1.0, false))), ERDM_Parameter.PRESET_STATUS, null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(preset.UpFadeTime, Is.EqualTo(4.0).Within(0.001));
            Assert.That(preset.DownFadeTime, Is.EqualTo(4.0).Within(0.001));
            Assert.That(preset.WaitTime, Is.EqualTo(1.0).Within(0.001));
        }
    }

    [Test]
    public void TestCAPTURE_PRESET_WithoutSplitTimes()
    {
        Assert.That(generated, Is.Not.Null);
        var preset = generated.Modules.OfType<PresetsModule_E1_37_1_2012>().Single().Presets.OfType<GeneratedPreset>().Single(p => p.ID == 1);

        assertSetResponse(send(ERDM_Parameter.CAPTURE_PRESET, PayloadTestTools.SetRequestData(ERDM_Parameter.CAPTURE_PRESET, new RDMCapturePreset(1, 2.0, 3.0, 1.0))), ERDM_Parameter.CAPTURE_PRESET, null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(preset.Programmed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED));
            Assert.That(preset.UpFadeTime, Is.EqualTo(2.0).Within(0.001));
            Assert.That(preset.DownFadeTime, Is.EqualTo(2.0).Within(0.001), "without split times the Up Fade Time is used for both fade times");
            Assert.That(preset.WaitTime, Is.EqualTo(1.0).Within(0.001));
        }
    }

    private RDMMessage? send(ERDM_Parameter parameter, byte[] data)
    {
        var request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = parameter,
            SubDevice = SubDevice.Root,
            ParameterData = data
        };
        return generated!.ProcessRequestMessage_Internal(request);
    }
    private static void assertSetResponse(RDMMessage? response, ERDM_Parameter parameter, ERDM_NackReason? nackReason)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            if (nackReason is null)
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            else
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
                Assert.That(response.NackReason, Is.EqualTo(nackReason));
            }
        }
    }

    class PresetsWithoutSplitTimesMockDevice : MockGeneratedDevice1
    {
        public PresetsWithoutSplitTimesMockDevice(UID uid) : base(uid, new IModule[]
        {
            new PresetsModule_E1_37_1_2012(PRESET_INFO, ERDM_MergeMode.DEFAULT, new GeneratedPreset(1), new GeneratedPreset(2, 1.0, 1.0, 1.0, ERDM_PresetProgrammed.PROGRAMMED))
        })
        {
        }
    }
}
