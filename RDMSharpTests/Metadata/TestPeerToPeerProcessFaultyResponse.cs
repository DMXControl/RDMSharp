using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata;

/// <summary>
/// A response of a remote device that does not fit the define is not parsed:
/// <see cref="PeerToPeerProcess.ResponsePayloadObject"/> stays Unset, so the cache keeps its value.
/// ACK_OVERFLOW responses are checked after they are put together.
/// </summary>
public class TestPeerToPeerProcessFaultyResponse
{
    private static readonly UID DEVICE_UID = new UID(0x4C73, 0x00001E1D);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    /// <summary>
    /// Runs a GET of <paramref name="parameter"/>, the device answers with <paramref name="responses"/> (Response Type, Parameter Data) one after the other.
    /// </summary>
    private static async Task<PeerToPeerProcess> runGet(ERDM_Parameter parameter, params (ERDM_ResponseType ResponseType, byte[] Data)[] responses)
    {
        var process = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, DEVICE_UID, SubDevice.Root, new ParameterBag(parameter));
        int index = 0;
        AsyncRDMRequestHelper? helper = null;
        helper = new AsyncRDMRequestHelper(async message =>
        {
            var (responseType, data) = responses[Math.Min(index++, responses.Length - 1)];
            var response = new RDMMessage()
            {
                Command = message.Command | ERDM_Command.RESPONSE,
                DestUID = message.SourceUID,
                SourceUID = message.DestUID,
                Parameter = message.Parameter,
                SubDevice = message.SubDevice,
                TransactionCounter = message.TransactionCounter,
                ParameterData = data
            };
            response.PortID_or_Responsetype = (byte)responseType;
            await Task.Delay(5);
            helper?.ReceiveMessage(response);
        });
        try
        {
            await process.Run(helper);
        }
        finally
        {
            helper.Dispose();
        }
        return process;
    }

    [Test, MaxTime(10000)]
    public async Task TestTooShort()
    {
        // GenVI Dimmer v1.08: CURVE with 1 instead of 2 bytes
        var process = await runGet(ERDM_Parameter.CURVE, (ERDM_ResponseType.ACK, new byte[] { 0x03 }));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
            Assert.That(process.Exception, Is.Null);
            Assert.That(process.ResponsePayloadObject.IsUnset, Is.True, "a response that does not fit the define must not be parsed");
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestInvalidBoolean()
    {
        // JBL P10: FACTORY_DEFAULTS 0x02
        var process = await runGet(ERDM_Parameter.FACTORY_DEFAULTS, (ERDM_ResponseType.ACK, new byte[] { 0x02 }));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
            Assert.That(process.ResponsePayloadObject.IsUnset, Is.True, "a boolean other than 0x00/0x01 must not be parsed");
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestValidResponse()
    {
        var process = await runGet(ERDM_Parameter.CURVE, (ERDM_ResponseType.ACK, new byte[] { 0x02, 0x03 }));
        Assert.That(process.ResponsePayloadObject.IsUnset, Is.False);
        Assert.That(process.ResponsePayloadObject.ParsedObject, Is.Not.Null);
    }

    [Test, MaxTime(10000)]
    public async Task TestOverflowIsCheckedPutTogether()
    {
        // the first part alone (3 bytes) is no complete list of uint16, but both parts together are
        var process = await runGet(ERDM_Parameter.SUPPORTED_PARAMETERS,
            (ERDM_ResponseType.ACK_OVERFLOW, new byte[] { 0x00, 0x60, 0x00 }),
            (ERDM_ResponseType.ACK, new byte[] { 0xF0 }));
        Assert.That(process.ResponsePayloadObject.IsUnset, Is.False);
        Assert.That(process.ResponsePayloadObject.ParsedObject, Is.EqualTo(new ERDM_Parameter[] { ERDM_Parameter.DEVICE_INFO, ERDM_Parameter.DMX_START_ADDRESS }));
    }

    [Test, MaxTime(10000)]
    public async Task TestOverflowPutTogetherDoesNotFit()
    {
        var process = await runGet(ERDM_Parameter.SUPPORTED_PARAMETERS,
            (ERDM_ResponseType.ACK_OVERFLOW, new byte[] { 0x00, 0x60 }),
            (ERDM_ResponseType.ACK, new byte[] { 0x00 }));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
            Assert.That(process.ResponsePayloadObject.IsUnset, Is.True, "3 bytes are no list of uint16");
        }
    }
}
