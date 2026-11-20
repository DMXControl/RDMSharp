using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// DMX_FAIL_MODE (E1.37-1 3.4): behavior of the device when the DMX512 signal is lost.
/// <list type="bullet">
/// <item>Scene # 0x0000 (PRESET_PLAYBACK_OFF) uses the Level field, a scene the device does not support is NACKed with DATA_OUT_OF_RANGE.</item>
/// <item>Loss of Signal Delay and Hold Time outside of the limits (PRESET_INFO) are set to the minimum or maximum.</item>
/// <item>0xFFFF is infinite, if the device does not support infinite (PRESET_INFO) the maximum is used.</item>
/// </list>
/// </summary>
public class TestDMXFailModeModule
{
    private DMXFailModeMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.DMX_FAIL_MODE;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);
    private static readonly UID DEVCIE_WITHOUT_PRESETS_UID = new UID(0x4000 + 123 + (ushort)MODULE_PARAMETER, 555);

    // Fail Delay 2.0 - 100.0 (infinite not supported), Fail Hold 3.0 - 100.0 (infinite supported)
    private static readonly RDMPresetInfo PRESET_INFO = new RDMPresetInfo(
            levelFieldSupported: true,
            presetSequenceSupported: true,
            splitTimesSupported: true,
            dmx512FailInfiniteDelayTimeSupported: false,
            dmx512FailInfiniteHoldTimeSupported: true,
            startupInfiniteHoldTimeSupported: false,
            maximumSceneNumber: 3,
            minimumPresetFadeTimeSupported: 0.0,
            maximumPresetFadeTimeSupported: 100.0,
            minimumPresetWaitTimeSupported: 0.0,
            maximumPresetWaitTimeSupported: 100.0,
            minimumDMX512FailDelayTimeSupported: 2.0,
            maximumDMX512FailDelayTimeSupported: 100.0,
            minimumDMX512FailDelayHoldSupported: 3.0,
            maximumDMX512FailDelayHoldSupported: 100.0,
            minimumStartupDelayTimeSupported: 5.0,
            maximumStartupDelayTimeSupported: 100.0,
            minimumStartupDelayHoldSupported: 4.0,
            maximumStartupDelayHoldSupported: 100.0);

    private static readonly RDMDMX_xxxx_Mode INITIAL_MODE = new RDMDMX_xxxx_Mode(1, 5.0, 10.0, 255);

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
        generated = new DMXFailModeMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<DMXFailModeModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("DMX512 Fail Mode"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetDMX_FAIL_MODE()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<DMXFailModeModule>().Single();

        #region Test Basic: initial mode
        Assert.That(module.Mode, Is.EqualTo(INITIAL_MODE));
        assertGetResponse(generated, modeData(1, 50, 100, 255));
        #endregion

        #region Test SET valid values
        assertSetResponse(send(generated, modeData(2, 100, 200, 128)), null);
        Assert.That(module.Mode, Is.EqualTo(new RDMDMX_xxxx_Mode(2, 10.0, 20.0, 128)));
        assertGetResponse(generated, modeData(2, 100, 200, 128));

        // PRESET_PLAYBACK_OFF, the Level field is used
        assertSetResponse(send(generated, modeData(0x0000, 20, 30, 77)), null);
        assertGetResponse(generated, modeData(0x0000, 20, 30, 77));

        // PRESET_PLAYBACK_ALL
        assertSetResponse(send(generated, modeData(0xFFFF, 20, 30, 255)), null);
        assertGetResponse(generated, modeData(0xFFFF, 20, 30, 255));
        #endregion

        #region Test scene the device does not support
        foreach (ushort scene in new ushort[] { 4, 11, 0xFFFE })
        {
            assertSetResponse(send(generated, modeData(scene, 20, 30, 255)), ERDM_NackReason.DATA_OUT_OF_RANGE);
            assertGetResponse(generated, modeData(0xFFFF, 20, 30, 255), "rejected request must not change the mode");
        }
        #endregion

        #region Test times outside of the limits are set to the minimum or maximum
        assertSetResponse(send(generated, modeData(3, 5, 10, 255)), null);
        assertGetResponse(generated, modeData(3, 20, 30, 255), "times below the minimum");

        assertSetResponse(send(generated, modeData(3, 2000, 5000, 255)), null);
        assertGetResponse(generated, modeData(3, 1000, 1000, 255), "times above the maximum");

        // infinite delay is not supported => maximum, infinite hold is supported => infinite
        assertSetResponse(send(generated, modeData(3, 0xFFFF, 0xFFFF, 255)), null);
        assertGetResponse(generated, modeData(3, 1000, 0xFFFF, 255), "infinite");
        Assert.That(module.Mode, Is.EqualTo(new RDMDMX_xxxx_Mode(3, 100.0, 6553.5, 255)));
        #endregion

        #region Test set locally after a SET by RDM
        module.Mode = new RDMDMX_xxxx_Mode(1, 5.0, 10.0, 255);
        assertSetResponse(send(generated, modeData(2, 100, 200, 128)), null);
        module.Mode = new RDMDMX_xxxx_Mode(1, 5.0, 10.0, 255);
        Assert.That(module.Mode, Is.EqualTo(new RDMDMX_xxxx_Mode(1, 5.0, 10.0, 255)), "setting the mode locally after a SET must not be ignored");
        assertGetResponse(generated, modeData(1, 50, 100, 255));
        #endregion

        #region Test invalid PDL
        foreach (byte[] data in new[] { Array.Empty<byte>(), new byte[] { 0x00, 0x02, 0x00, 0x20 }, new byte[] { 0x00, 0x02, 0x00, 0x20, 0x00, 0x30, 0xFF, 0x00 } })
        {
            assertSetResponse(send(generated, data), ERDM_NackReason.FORMAT_ERROR);
            assertGetResponse(generated, modeData(1, 50, 100, 255), "rejected request must not change the mode");
        }
        #endregion
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestSetDMX_FAIL_MODE_WithoutPresets()
    {
        // a device without presets supports only PRESET_PLAYBACK_OFF, without PRESET_INFO there are no limits for the times
        using var device = new DMXFailModeWithoutPresetsMockDevice(DEVCIE_WITHOUT_PRESETS_UID);
        var module = device.Modules.OfType<DMXFailModeModule>().Single();
        Assert.That(module.Mode, Is.EqualTo(new RDMDMX_xxxx_Mode()));
        assertGetResponse(device, modeData(0, 0, 0, 0), deviceUid: DEVCIE_WITHOUT_PRESETS_UID);

        assertSetResponse(send(device, modeData(0x0000, 0xFFFF, 0xFFFF, 128), DEVCIE_WITHOUT_PRESETS_UID), null, DEVCIE_WITHOUT_PRESETS_UID);
        assertGetResponse(device, modeData(0x0000, 0xFFFF, 0xFFFF, 128), deviceUid: DEVCIE_WITHOUT_PRESETS_UID);

        foreach (ushort scene in new ushort[] { 1, 0xFFFF })
        {
            assertSetResponse(send(device, modeData(scene, 1, 1, 128), DEVCIE_WITHOUT_PRESETS_UID), ERDM_NackReason.DATA_OUT_OF_RANGE, DEVCIE_WITHOUT_PRESETS_UID);
            assertGetResponse(device, modeData(0x0000, 0xFFFF, 0xFFFF, 128), "rejected request must not change the mode", DEVCIE_WITHOUT_PRESETS_UID);
        }
    }

    [Test, DependsOnTest(nameof(TestGetSetDMX_FAIL_MODE))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<DMXFailModeModule>().Single();

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DMXFailModeModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.Mode, Is.EqualTo(INITIAL_MODE)));
        Assert.That(module.Mode, Is.EqualTo(INITIAL_MODE));

        var newMode = new RDMDMX_xxxx_Mode(2, 10.0, 20.0, 128);
        Assert.That(await module.SetMode(newMode), Is.True);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.Mode, Is.EqualTo(newMode))
            && Wait.Holds(module.Mode, Is.EqualTo(newMode)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.Mode, Is.EqualTo(newMode));
            Assert.That(module.Mode, Is.EqualTo(newMode));
        }
    }

    private static byte[] modeData(ushort scene, ushort delay, ushort hold, byte level)
    {
        return new byte[] { (byte)(scene >> 8), (byte)scene, (byte)(delay >> 8), (byte)delay, (byte)(hold >> 8), (byte)hold, level };
    }
    private static RDMMessage? send(AbstractMockGeneratedDevice device, byte[] data, UID? deviceUid = null)
    {
        var request = createRequest(ERDM_Command.SET_COMMAND, deviceUid ?? DEVCIE_UID);
        request.ParameterData = data;
        return device.ProcessRequestMessage_Internal(request);
    }
    private static RDMMessage createRequest(ERDM_Command command, UID deviceUid)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = deviceUid,
            SourceUID = CONTROLLER_UID,
            Parameter = MODULE_PARAMETER,
            SubDevice = SubDevice.Root,
        };
    }
    private static void assertGetResponse(AbstractMockGeneratedDevice device, byte[] expected, string? message = null, UID? deviceUid = null)
    {
        RDMMessage? response = device.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, deviceUid ?? DEVCIE_UID));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(expected), message);
            Assert.That(response.Value, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
        }
    }
    private static void assertSetResponse(RDMMessage? response, ERDM_NackReason? nackReason, UID? deviceUid = null)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(deviceUid ?? DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
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

    class DMXFailModeMockDevice : MockGeneratedDevice1
    {
        public DMXFailModeMockDevice(UID uid) : base(uid, new IModule[]
        {
            new PresetsModule_E1_37_1_2012(PRESET_INFO, ERDM_MergeMode.DEFAULT, new GeneratedPreset(1), new GeneratedPreset(2), new GeneratedPreset(3)),
            new DMXFailModeModule(INITIAL_MODE)
        })
        {
        }
    }
    class DMXFailModeWithoutPresetsMockDevice : MockGeneratedDevice1
    {
        public DMXFailModeWithoutPresetsMockDevice(UID uid) : base(uid, new IModule[] { new DMXFailModeModule() })
        {
        }
    }
}
