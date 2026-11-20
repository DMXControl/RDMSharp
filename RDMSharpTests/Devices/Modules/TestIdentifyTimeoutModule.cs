using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;
using System.Diagnostics;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// IDENTIFY_TIMEOUT (E1.37-5 4.1): GET/SET of the time in seconds after which IDENTIFY_DEVICE stops by itself.
/// - 0x0000 disables the timeout, Identify stays on until it is switched off.
/// - GET returns the configured timeout, not the remaining time.
/// - After the timeout Identify is switched off (and queued as IDENTIFY_DEVICE off).
/// - A change during a running Identify takes effect immediately: a reduced timeout that is already over ends Identify,
///   an increased timeout extends it.
/// </summary>
public class TestIdentifyTimeoutModule
{
    private IdentifyTimeoutMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.IDENTIFY_TIMEOUT;
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
        generated = new IdentifyTimeoutMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    private IdentifyTimeoutModule timeoutModule => generated!.Modules.OfType<IdentifyTimeoutModule>().Single();
    private IdentifyDeviceModule identifyModule => generated!.Modules.OfType<IdentifyDeviceModule>().Single();

    [Test]
    public void TestDisplayName()
    {
        Assert.That(generated, Is.Not.Null);
        Assert.That(timeoutModule.DisplayName, Is.EqualTo("Identify Timeout"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetIDENTIFY_TIMEOUT()
    {
        Assert.That(generated, Is.Not.Null);

        // disabled by default (backwards compatible)
        Assert.That(timeoutModule.TimeoutSeconds, Is.Zero);
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, MODULE_PARAMETER)), 0);

        foreach (ushort seconds in new ushort[] { 30, 0xFFFF, 0 })
        {
            assertSetAck(setTimeout(seconds));
            Assert.That(timeoutModule.TimeoutSeconds, Is.EqualTo(seconds));
            assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, MODULE_PARAMETER)), seconds);
        }
    }

    [Test, DependsOnTest(nameof(TestGetSetIDENTIFY_TIMEOUT))]
    public async Task TestTimeoutEndsIdentify()
    {
        Assert.That(generated, Is.Not.Null);
        assertSetAck(setTimeout(1));

        Stopwatch stopwatch = Stopwatch.StartNew();
        assertSetAck(setIdentify(true), ERDM_Parameter.IDENTIFY_DEVICE);
        Assert.That(identifyModule.Identify, Is.True);

        // GET returns the configured timeout, not the remaining time
        await Task.Delay(300);
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, MODULE_PARAMETER)), 1);
        Assert.That(identifyModule.Identify, Is.True);

        await Wait.UntilAsync(() => identifyModule.Identify == false, 3000);
        stopwatch.Stop();
        Assert.That(identifyModule.Identify, Is.False);
        Assert.That(stopwatch.ElapsedMilliseconds, Is.InRange(900, 2500));

        // a GET of IDENTIFY_DEVICE reports Off
        var response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, ERDM_Parameter.IDENTIFY_DEVICE));
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ParameterData, Is.EqualTo(new byte[] { 0x00 }));
    }

    [Test, DependsOnTest(nameof(TestGetSetIDENTIFY_TIMEOUT))]
    public async Task TestTimeoutAlsoForLocalIdentify()
    {
        Assert.That(generated, Is.Not.Null);
        timeoutModule.TimeoutSeconds = 1;
        identifyModule.Identify = true;

        await Wait.UntilAsync(() => identifyModule.Identify == false, 3000);
        Assert.That(identifyModule.Identify, Is.False);
    }

    [Test, DependsOnTest(nameof(TestGetSetIDENTIFY_TIMEOUT))]
    public async Task TestNoTimeout()
    {
        Assert.That(generated, Is.Not.Null);
        assertSetAck(setTimeout(0));
        assertSetAck(setIdentify(true), ERDM_Parameter.IDENTIFY_DEVICE);

        await Task.Delay(1500);
        Assert.That(identifyModule.Identify, Is.True, "a timeout of 0 keeps Identify on");

        assertSetAck(setIdentify(false), ERDM_Parameter.IDENTIFY_DEVICE);
        Assert.That(identifyModule.Identify, Is.False);
    }

    [Test, DependsOnTest(nameof(TestTimeoutEndsIdentify))]
    public async Task TestReducedTimeoutEndsIdentifyImmediately()
    {
        Assert.That(generated, Is.Not.Null);
        assertSetAck(setTimeout(0));
        assertSetAck(setIdentify(true), ERDM_Parameter.IDENTIFY_DEVICE);
        await Task.Delay(1200);
        Assert.That(identifyModule.Identify, Is.True);

        // Identify already runs longer than the new timeout of 1 second
        Stopwatch stopwatch = Stopwatch.StartNew();
        assertSetAck(setTimeout(1));
        await Wait.UntilAsync(() => identifyModule.Identify == false, 3000);
        Assert.That(identifyModule.Identify, Is.False);
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(500));
    }

    [Test, DependsOnTest(nameof(TestTimeoutEndsIdentify))]
    public async Task TestIncreasedTimeoutExtendsIdentify()
    {
        Assert.That(generated, Is.Not.Null);
        assertSetAck(setTimeout(1));
        Stopwatch stopwatch = Stopwatch.StartNew();
        assertSetAck(setIdentify(true), ERDM_Parameter.IDENTIFY_DEVICE);
        await Task.Delay(400);

        assertSetAck(setTimeout(3));
        await Task.Delay(1400);
        Assert.That(identifyModule.Identify, Is.True, "the increased timeout extends Identify beyond the old timeout of 1 second");

        await Wait.UntilAsync(() => identifyModule.Identify == false, 4000);
        stopwatch.Stop();
        Assert.That(identifyModule.Identify, Is.False);
        Assert.That(stopwatch.ElapsedMilliseconds, Is.InRange(2900, 4500));
    }

    [Test, DependsOnTest(nameof(TestTimeoutEndsIdentify))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        timeoutModule.TimeoutSeconds = 1;

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<IdentifyTimeoutModule>().Single();
        var remoteIdentify = mockDevice.Modules.OfType<IdentifyDeviceModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.TimeoutSeconds, Is.EqualTo(1)));
        Assert.That(module.TimeoutSeconds, Is.EqualTo(1));

        await module.SetTimeoutSeconds(2);
        await Wait.UntilAsync(() =>
            Wait.Holds(timeoutModule.TimeoutSeconds, Is.EqualTo(2))
            && Wait.Holds(module.TimeoutSeconds, Is.EqualTo(2)));
        Assert.That(timeoutModule.TimeoutSeconds, Is.EqualTo(2));

        // Identify started by the remote device ends by itself, the remote device learns it from the queued messages
        await remoteIdentify.SetIdentify(true);
        await Wait.UntilAsync(() => remoteIdentify.Identify == true && identifyModule.Identify == true);
        Assert.That(identifyModule.Identify, Is.True);

        await Wait.UntilAsync(() => identifyModule.Identify == false && remoteIdentify.Identify == false, 5000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(identifyModule.Identify, Is.False);
            Assert.That(remoteIdentify.Identify, Is.False);
        }
    }

    private RDMMessage? setTimeout(ushort seconds)
    {
        var request = createRequest(ERDM_Command.SET_COMMAND, MODULE_PARAMETER);
        request.ParameterData = Tools.ValueToData(seconds);
        return generated!.ProcessRequestMessage_Internal(request);
    }
    private RDMMessage? setIdentify(bool identify)
    {
        var request = createRequest(ERDM_Command.SET_COMMAND, ERDM_Parameter.IDENTIFY_DEVICE);
        request.ParameterData = new byte[] { identify ? (byte)0x01 : (byte)0x00 };
        return generated!.ProcessRequestMessage_Internal(request);
    }
    private static RDMMessage createRequest(ERDM_Command command, ERDM_Parameter parameter)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = parameter,
            SubDevice = SubDevice.Root,
        };
    }
    private static void assertGetResponse(RDMMessage? response, ushort expected)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(Tools.ValueToData(expected)));
        }
    }
    private static void assertSetAck(RDMMessage? response, ERDM_Parameter parameter = MODULE_PARAMETER)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        }
    }

    class IdentifyTimeoutMockDevice : MockGeneratedDevice1
    {
        public IdentifyTimeoutMockDevice(UID uid) : base(uid, new IModule[] { new IdentifyTimeoutModule() })
        {
        }
    }
}
