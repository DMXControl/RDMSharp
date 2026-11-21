using RDMSharp.Metadata;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.Devices;

/// <summary>
/// A generated device with sub devices only answers the sub devices it has (E1.20 6.2.2 / Table A-17: SUB_DEVICE_OUT_OF_RANGE),
/// and does not answer a GET to the broadcast sub device (several devices would answer on one line).
/// </summary>
public class TestGeneratedDeviceSubDeviceRequests
{
    private static readonly UID CONTROLLER_UID = new UID(0x1fff, 0x00000003);
    private static readonly UID DEVICE_UID = new UID(0x0a5a, 0x00000F02);
    private MockGeneratedDeviceWithSubDeviceMaster1? generated;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }
    [SetUp]
    public void Setup()
    {
        generated = new MockGeneratedDeviceWithSubDeviceMaster1(DEVICE_UID, 2);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    private RDMMessage? send(ERDM_Command command, ERDM_Parameter parameter, SubDevice subDevice)
    {
        return generated!.ProcessRequestMessage_Internal(new RDMMessage()
        {
            Command = command,
            Parameter = parameter,
            DestUID = DEVICE_UID,
            SourceUID = CONTROLLER_UID,
            SubDevice = subDevice
        });
    }

    [Test]
    public void TestKnownSubDevicesAreAnswered()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO, SubDevice.Root)!.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO, new SubDevice(1))!.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO, new SubDevice(2))!.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        }
    }

    [TestCase(ERDM_Command.GET_COMMAND)]
    [TestCase(ERDM_Command.SET_COMMAND)]
    public void TestUnknownSubDeviceIsNacked(ERDM_Command command)
    {
        // the device has the sub devices 1 and 2
        var response = send(command, ERDM_Parameter.DEVICE_INFO, new SubDevice(7));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response!.Command, Is.EqualTo(command | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_INFO));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.SUB_DEVICE_OUT_OF_RANGE));
        }
    }

    [Test]
    public void TestGetToTheBroadcastSubDeviceIsNotAnswered()
    {
        // several devices would answer on one line, so nothing is sent back
        Assert.That(send(ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO, SubDevice.Broadcast), Is.Null);
    }
}
