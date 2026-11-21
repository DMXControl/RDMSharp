using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.Devices;

/// <summary>
/// A fault inside of a generated device (E1.20 Table A-17: NR_HARDWARE_FAULT) is no fault of the request:
/// <list type="bullet">
/// <item>not UNKNOWN_PID, that would tell the controller that the parameter is not supported (and it stops asking for it),</item>
/// <item>not FORMAT_ERROR, the request was fine.</item>
/// </list>
/// Invented data only (manufacturer 0x7FF0 is the ESTA prototyping range).
/// </summary>
public class TestGeneratedDeviceInternalFaults
{
    private static readonly UID CONTROLLER_UID = new UID(0x1fff, 0x00000004);
    private static readonly UID DEVICE_UID = new UID(0x7FF0, 0x00000F03);
    private static readonly ERDM_Parameter THROWING = (ERDM_Parameter)0x9401;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    private static RDMMessage? send(AbstractMockGeneratedDevice device, ERDM_Command command, ERDM_Parameter parameter)
    {
        return device.ProcessRequestMessage_Internal(new RDMMessage()
        {
            Command = command,
            Parameter = parameter,
            DestUID = DEVICE_UID,
            SourceUID = CONTROLLER_UID,
            SubDevice = SubDevice.Root
        });
    }

    private static void assertHardwareFault(RDMMessage? response, ERDM_Command command, ERDM_Parameter parameter)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response!.Command, Is.EqualTo(command | ERDM_Command.RESPONSE), "the response to the command of the request");
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            Assert.That(response.SourceUID, Is.EqualTo(DEVICE_UID));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
        }
    }

    [Test]
    public void TestStoredValueThatDoesNotFitTheDefineIsAHardwareFault()
    {
        using var generated = new MockGeneratedDevice1(DEVICE_UID);
        generated.setParameterValue(ERDM_Parameter.DMX_START_ADDRESS, "not a number");

        assertHardwareFault(send(generated, ERDM_Command.GET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS), ERDM_Command.GET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS);

        // the device works again as soon as the value is valid
        generated.setParameterValue(ERDM_Parameter.DMX_START_ADDRESS, (ushort)7);
        var response = send(generated, ERDM_Command.GET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response!.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.EqualTo((ushort)7));
        }
    }

    [Test]
    public void TestAParameterThatIsNotSupportedIsStillUnknown()
    {
        // the fault is only for a parameter the device has, an unsupported parameter is not a fault of the device
        using var generated = new MockGeneratedDevice1(DEVICE_UID);
        var response = send(generated, ERDM_Command.GET_COMMAND, ERDM_Parameter.LAMP_HOURS);
        Assert.That(response!.NackReason, Is.EqualTo(ERDM_NackReason.UNKNOWN_PID));
    }

    [TestCase(ERDM_Command.GET_COMMAND)]
    [TestCase(ERDM_Command.SET_COMMAND)]
    public void TestModuleThatThrowsIsAHardwareFault(ERDM_Command command)
    {
        using var generated = new MockGeneratedDevice1(DEVICE_UID, new IModule[] { new ThrowingModule() });
        assertHardwareFault(send(generated, command, THROWING), command, THROWING);
    }

    [TestCase(ERDM_Command.GET_COMMAND)]
    [TestCase(ERDM_Command.SET_COMMAND)]
    public void TestModuleAnswersAFaultToTheCommandOfTheRequest(ERDM_Command command)
    {
        // directly at the module: a SET is not answered as a GET, and the response comes from the device the request was sent to
        var response = new ThrowingModule().HandleRequest(new RDMMessage()
        {
            Command = command,
            Parameter = THROWING,
            DestUID = DEVICE_UID,
            SourceUID = CONTROLLER_UID,
            SubDevice = SubDevice.Root
        });
        assertHardwareFault(response, command, THROWING);
    }

    /// <summary>A module whose parameter always fails.</summary>
    private sealed class ThrowingModule : AbstractModule
    {
        public override string DisplayName => "Throwing";

        public ThrowingModule() : base("Throwing", THROWING)
        {
        }

        public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
        {
            return parameter == THROWING;
        }

        protected override RDMMessage handleRequest(RDMMessage message)
        {
            throw new InvalidOperationException("fault of the device");
        }
    }
}
