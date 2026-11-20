using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// SHIPPING_LOCK (E1.37-5 4.5): GET returns UNLOCKED, LOCKED or PARTIALLY_LOCKED (Table A-2).
/// Devices with a remotely operable lock also support SET with Unlock (0x00) / Lock (0x01),
/// devices with only a sensor support GET only.
/// </summary>
public class TestShippingLockModule
{
    private ShippingLockMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.SHIPPING_LOCK;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);
    private static readonly UID SENSOR_ONLY_UID = new UID(123 + 0x4000 + (ushort)MODULE_PARAMETER, 555);

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
        generated = new ShippingLockMockDevice(DEVCIE_UID, remotelyOperable: true);
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
        var module = generated.Modules.OfType<ShippingLockModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Shipping Lock"));
        Assert.That(module.RemotelyOperable, Is.True);
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSHIPPING_LOCK()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<ShippingLockModule>().Single();

        Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, DEVCIE_UID)), ERDM_ShippingLockState.UNLOCKED);

        // the device reports the state of its sensor, PARTIALLY_LOCKED is only reported, never set
        foreach (var state in new[] { ERDM_ShippingLockState.PARTIALLY_LOCKED, ERDM_ShippingLockState.LOCKED, ERDM_ShippingLockState.UNLOCKED })
        {
            module.ShippingLockState = state;
            assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, DEVCIE_UID)), state);
        }
    }

    [Test, DependsOnTest(nameof(TestGetSHIPPING_LOCK))]
    public void TestSetSHIPPING_LOCK()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<ShippingLockModule>().Single();

        #region Test lock and unlock
        assertSetResponse(send(generated, DEVCIE_UID, ERDM_ShippingLockState.LOCKED), null);
        Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.LOCKED));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, DEVCIE_UID)), ERDM_ShippingLockState.LOCKED);

        assertSetResponse(send(generated, DEVCIE_UID, ERDM_ShippingLockState.UNLOCKED), null);
        Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED));
        #endregion

        #region Test invalid values: SET only knows Unlock (0x00) / Lock (0x01)
        // PARTIALLY_LOCKED can only be reported
        assertSetResponse(send(generated, DEVCIE_UID, ERDM_ShippingLockState.PARTIALLY_LOCKED), ERDM_NackReason.DATA_OUT_OF_RANGE);
        Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED), "rejected request must not change the state");
        // undefined values are no ERDM_ShippingLockState, so the bytes are needed here
        foreach (byte invalid in new byte[] { 0x03, 0xFF })
        {
            assertSetResponse(send(generated, DEVCIE_UID, new byte[] { invalid }), ERDM_NackReason.DATA_OUT_OF_RANGE);
            Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED), "rejected request must not change the state");
        }
        #endregion

        #region Test invalid PDL, the parser can not build it, so the bytes are needed here
        foreach (byte[] data in new[] { Array.Empty<byte>(), new byte[] { 0x01, 0x00 } })
        {
            var request = createRequest(ERDM_Command.SET_COMMAND, DEVCIE_UID);
            request.ParameterData = data;
            assertSetResponse(generated.ProcessRequestMessage_Internal(request), ERDM_NackReason.FORMAT_ERROR);
            Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED), "rejected request must not change the state");
        }
        #endregion
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestSensorOnlySHIPPING_LOCK()
    {
        using var device = new ShippingLockMockDevice(SENSOR_ONLY_UID, remotelyOperable: false);
        var module = device.Modules.OfType<ShippingLockModule>().Single();
        Assert.That(module.RemotelyOperable, Is.False);

        module.ShippingLockState = ERDM_ShippingLockState.LOCKED;
        var getResponse = device.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, SENSOR_ONLY_UID));
        Assert.That(getResponse, Is.Not.Null);
        Assert.That(getResponse.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(getResponse.Value, Is.EqualTo(ERDM_ShippingLockState.LOCKED));

        var setResponse = send(device, SENSOR_ONLY_UID, ERDM_ShippingLockState.UNLOCKED);
        Assert.That(setResponse, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setResponse.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(setResponse.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
            Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.LOCKED));
        }
    }

    [Test, DependsOnTest(nameof(TestSetSHIPPING_LOCK))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<ShippingLockModule>().Single();
        generatedModule.ShippingLockState = ERDM_ShippingLockState.PARTIALLY_LOCKED;

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<ShippingLockModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.PARTIALLY_LOCKED)));
        Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.PARTIALLY_LOCKED));

        await module.SetShippingLock(true);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.LOCKED))
            && Wait.Holds(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.LOCKED)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.LOCKED));
            Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.LOCKED));
        }

        await module.SetShippingLock(false);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED))
            && Wait.Holds(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED));
            Assert.That(module.ShippingLockState, Is.EqualTo(ERDM_ShippingLockState.UNLOCKED));
        }
    }

    private static RDMMessage createRequest(ERDM_Command command, UID destination)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = destination,
            SourceUID = CONTROLLER_UID,
            Parameter = MODULE_PARAMETER,
            SubDevice = SubDevice.Root,
        };
    }
    private static RDMMessage? send(ShippingLockMockDevice device, UID destination, ERDM_ShippingLockState state)
    {
        return send(device, destination, PayloadTestTools.SetRequestData(MODULE_PARAMETER, state));
    }
    private static RDMMessage? send(ShippingLockMockDevice device, UID destination, byte[] data)
    {
        var request = createRequest(ERDM_Command.SET_COMMAND, destination);
        request.ParameterData = data;
        return device.ProcessRequestMessage_Internal(request);
    }
    private static void assertGetResponse(RDMMessage? response, ERDM_ShippingLockState expected)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.EqualTo(expected));
        }
    }
    private static void assertSetResponse(RDMMessage? response, ERDM_NackReason? nackReason)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
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

    class ShippingLockMockDevice : MockGeneratedDevice1
    {
        public ShippingLockMockDevice(UID uid, bool remotelyOperable) : base(uid, new IModule[] { new ShippingLockModule(remotelyOperable) })
        {
        }
    }
}
