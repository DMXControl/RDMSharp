using RDMSharp.Metadata;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.Devices;

/// <summary>
/// A generated device answers a request with Parameter Data that does not fit the define (E1.20 6.3.3 / Table A-17):
/// <list type="bullet">
/// <item>length does not fit: NACK FORMAT_ERROR,</item>
/// <item>boolean other than 0x00/0x01: NACK DATA_OUT_OF_RANGE,</item>
/// <item>a not supported parameter is still NACKed with UNKNOWN_PID.</item>
/// </list>
/// </summary>
public class TestGeneratedDeviceFaultyRequests
{
    private static readonly UID CONTROLLER_UID = new UID(0x1fff, 0x00000002);
    private static readonly UID DEVICE_UID = new UID(0x0a5a, 0x00000F01);
    private MockGeneratedDevice1? generated;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }
    [SetUp]
    public void Setup()
    {
        generated = new MockGeneratedDevice1(DEVICE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    private RDMMessage? send(ERDM_Command command, ERDM_Parameter parameter, params byte[] data)
    {
        return generated!.ProcessRequestMessage_Internal(new RDMMessage()
        {
            Command = command,
            Parameter = parameter,
            DestUID = DEVICE_UID,
            SourceUID = CONTROLLER_UID,
            SubDevice = SubDevice.Root,
            ParameterData = data
        });
    }
    private static void assertNack(RDMMessage? response, ERDM_Command command, ERDM_Parameter parameter, ERDM_NackReason reason)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response!.Command, Is.EqualTo(command | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(parameter));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(reason));
        }
    }

    [Test]
    public void TestSetTooShortAndTooLong()
    {
        assertNack(send(ERDM_Command.SET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS, 0x00), ERDM_Command.SET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS, ERDM_NackReason.FORMAT_ERROR);
        assertNack(send(ERDM_Command.SET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS, 0x00, 0x05, 0x00), ERDM_Command.SET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS, ERDM_NackReason.FORMAT_ERROR);

        var response = send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS);
        Assert.That(response!.Value, Is.EqualTo((ushort)1), "a rejected request must not change the value");
    }

    [Test]
    public void TestValidSet()
    {
        var response = send(ERDM_Command.SET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS, 0x00, 0x05);
        Assert.That(response!.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DMX_START_ADDRESS)!.Value, Is.EqualTo((ushort)5));
    }

    [Test]
    public void TestGetWithParameterData()
    {
        // GET DEVICE_INFO has no Parameter Data
        assertNack(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO, 0x00), ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO, ERDM_NackReason.FORMAT_ERROR);
    }

    [Test]
    public void TestInvalidBoolean()
    {
        assertNack(send(ERDM_Command.SET_COMMAND, ERDM_Parameter.IDENTIFY_DEVICE, 0x02), ERDM_Command.SET_COMMAND, ERDM_Parameter.IDENTIFY_DEVICE, ERDM_NackReason.DATA_OUT_OF_RANGE);
        Assert.That(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.IDENTIFY_DEVICE)!.Value, Is.EqualTo(false), "a rejected request must not change the value");
    }

    [Test]
    public void TestUnknownPidHasPriority()
    {
        // LAMP_HOURS is not supported by the mock, the wrong length does not matter
        assertNack(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.LAMP_HOURS, 0x00), ERDM_Command.GET_COMMAND, ERDM_Parameter.LAMP_HOURS, ERDM_NackReason.UNKNOWN_PID);
    }

    /// <summary>
    /// "The last message" (E1.20 10.3.2) can only be asked for after a message was sent, otherwise there is nothing to repeat.
    /// </summary>
    [TestCase(ERDM_Parameter.QUEUED_MESSAGE)]
    [TestCase(ERDM_Parameter.STATUS_MESSAGES)]
    public void TestLastMessageWithoutAMessage(ERDM_Parameter parameter)
    {
        var data = PayloadTestTools.GetRequestData(parameter, ERDM_Status.GET_LAST_MESSAGE);
        assertNack(send(ERDM_Command.GET_COMMAND, parameter, data), ERDM_Command.GET_COMMAND, parameter, ERDM_NackReason.DATA_OUT_OF_RANGE);
    }
}
