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
    private static Task<PeerToPeerProcess> runGet(ERDM_Parameter parameter, params (ERDM_ResponseType ResponseType, byte[] Data)[] responses)
        => runGet(parameter, null, responses);

    /// <summary>
    /// Runs a GET of <paramref name="parameter"/> with <paramref name="requestValue"/> as Parameter Data (e.g. an endpoint ID).
    /// </summary>
    private static async Task<PeerToPeerProcess> runGet(ERDM_Parameter parameter, object? requestValue, params (ERDM_ResponseType ResponseType, byte[] Data)[] responses)
    {
        var bag = new ParameterBag(parameter);
        DataTreeBranch? payload = requestValue is null ? null : DataTreeBranch.FromObject(requestValue, null, bag, ERDM_Command.GET_COMMAND);
        var process = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, DEVICE_UID, SubDevice.Root, bag, payload);
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
    public async Task TestEndpointRespondersWithoutDataMeansNoResponders()
    {
        // Luminex LumiSplit 1.6 FW 7 (capture 2026-11-22): GET ENDPOINT_RESPONDERS for endpoint 3 is answered with an ACK without Parameter Data,
        // E1.37-7 needs the endpoint ID and the list change number. Unambiguous: the endpoint has no responders.
        var process = await runGet(ERDM_Parameter.ENDPOINT_RESPONDERS, (ushort)3, (ERDM_ResponseType.ACK, Array.Empty<byte>()));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
            Assert.That(process.ResponsePayloadObject.IsUnset, Is.False, "the empty ACK is taken as 'no responders'");
            var responders = process.ResponsePayloadObject.ParsedObject as RDMSharp.PayloadObject.GetEndpointRespondersResponse;
            Assert.That(responders, Is.Not.Null);
            Assert.That(responders!.EndpointId, Is.EqualTo((ushort)3), "the endpoint is the requested one");
            Assert.That(responders.ListChangedNumber, Is.Zero);
            Assert.That(responders.UIDs, Is.Empty);
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestEndpointRespondersTruncatedStaysIgnored()
    {
        // only the empty ACK is unambiguous: a truncated answer is still not interpreted
        var process = await runGet(ERDM_Parameter.ENDPOINT_RESPONDERS, (ushort)3, (ERDM_ResponseType.ACK, new byte[] { 0x00, 0x03, 0x00 }));
        Assert.That(process.ResponsePayloadObject.IsUnset, Is.True);
    }

    [Test]
    public void TestRepairOnlyForEndpointResponders()
    {
        var curve = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.CURVE));
        var responders = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.ENDPOINT_RESPONDERS));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(MetadataFactory.TryRepairPayload(curve, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse, Array.Empty<byte>(), Array.Empty<byte>(), out _, out _), Is.False);
            Assert.That(MetadataFactory.TryRepairPayload(responders, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse, null!, Array.Empty<byte>(), out _, out _), Is.False, "without the requested endpoint nothing can be completed");
            Assert.That(MetadataFactory.TryRepairPayload(responders, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse, new byte[] { 0x00, 0x07 }, Array.Empty<byte>(), out var repaired, out _), Is.True);
            Assert.That(repaired, Is.EqualTo(new byte[] { 0x00, 0x07, 0, 0, 0, 0 }));
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

    /// <summary>
    /// A device that answers every request (and every following QUEUED_MESSAGE) with ACK_TIMER again must not keep the
    /// process running forever. Seen live: a node answered for UID 0000:00000000 with ACK_TIMER, more than 1000 requests in 3 minutes.
    /// </summary>
    [Test, MaxTime(20000)]
    public async Task TestEndlessAckTimerIsBounded()
    {
        int requests = 0;
        AsyncRDMRequestHelper? helper = null;
        helper = new AsyncRDMRequestHelper(async message =>
        {
            Interlocked.Increment(ref requests);
            var response = new RDMMessage()
            {
                Command = message.Command | ERDM_Command.RESPONSE,
                DestUID = message.SourceUID,
                SourceUID = message.DestUID,
                Parameter = message.Parameter,
                SubDevice = message.SubDevice,
                TransactionCounter = message.TransactionCounter,
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK_TIMER,
                ParameterData = new byte[] { 0x00, 0x00 }, // estimated response time 0 -> no delay, so the loop runs as fast as possible
            };
            await Task.Delay(1);
            helper?.ReceiveMessage(response);
        });
        var process = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, DEVICE_UID, SubDevice.Root, new ParameterBag(ERDM_Parameter.DEVICE_INFO));
        try
        {
            await process.Run(helper);
        }
        finally
        {
            helper.Dispose();
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed), "endless ACK_TIMER has to fail the process");
            // the first request plus at most MAX_ACK_TIMER_RESPONSES QUEUED_MESSAGE requests
            Assert.That(requests, Is.LessThanOrEqualTo(PeerToPeerProcess.MAX_ACK_TIMER_RESPONSES + 1), "the number of requests must be bounded");
            Assert.That(process.ResponsePayloadObject.IsUnset, Is.True);
        }
    }
}
