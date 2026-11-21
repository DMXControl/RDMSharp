using RDMSharp.Metadata;
using System.ComponentModel;

namespace RDMSharpTests.Metadata;

/// <summary>
/// How a <see cref="PeerToPeerProcess"/> fails: an exception, a disposed helper, a NACK, and that a process runs only once.
/// </summary>
public class TestPeerToPeerProcessFailures
{
    private static readonly UID DEVICE_UID = new UID(0x4C73, 0x00001E1E);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    private static PeerToPeerProcess newGet(ERDM_Parameter parameter = ERDM_Parameter.DEVICE_LABEL)
        => new PeerToPeerProcess(ERDM_Command.GET_COMMAND, DEVICE_UID, SubDevice.Root, new ParameterBag(parameter));

    /// <summary>A helper whose device answers every request with <paramref name="responseType"/> (and <paramref name="data"/>).</summary>
    private static AsyncRDMRequestHelper answering(ERDM_ResponseType responseType, byte[] data, Action? onRequest = null)
    {
        AsyncRDMRequestHelper? helper = null;
        helper = new AsyncRDMRequestHelper(async message =>
        {
            onRequest?.Invoke();
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
        return helper;
    }

    [Test, MaxTime(10000)]
    public async Task TestExceptionBeforeSendFailsTheProcess()
    {
        var process = newGet();
        var changed = new List<string?>();
        process.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        process.BeforeSendMessage = _ => throw new InvalidOperationException("boom");
        using var helper = answering(ERDM_ResponseType.ACK, Array.Empty<byte>());

        await process.Run(helper);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed));
            Assert.That(process.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(process.Exception!.Message, Is.EqualTo("boom"));
            Assert.That(process.RequestResponseHistory, Is.Empty, "nothing was sent");
            Assert.That(changed, Does.Contain(nameof(PeerToPeerProcess.Exception)));
            Assert.That(changed, Does.Contain(nameof(PeerToPeerProcess.State)));
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestExceptionAfterTheHistoryEntryMarksItAsFailed()
    {
        var process = newGet();
        // the handler of a new history entry throws, the request was not sent
        process.OnRequestResponseHistoryAdded += (_, _) => throw new InvalidOperationException("boom");
        using var helper = answering(ERDM_ResponseType.ACK, Array.Empty<byte>());

        await process.Run(helper);

        var entry = process.RequestResponseHistory.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed));
            Assert.That(process.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(entry.State, Is.EqualTo("Fail"), "an entry without a response is marked as failed");
            Assert.That(entry.Response, Is.Null);
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestDisposedHelperFailsTheProcess()
    {
        var process = newGet();
        // nobody answers, the helper is disposed while the process waits for the response
        var helper = new AsyncRDMRequestHelper(_ => Task.CompletedTask);

        var run = process.Run(helper);
        await Task.Delay(100);
        helper.Dispose();
        await run;

        var entry = process.RequestResponseHistory.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed));
            Assert.That(entry.State, Is.EqualTo("Disposing"));
            Assert.That(process.Exception, Is.Null, "a disposed helper is no error of the process");
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestNackFailsTheProcessWithTheReason()
    {
        var process = newGet();
        var changed = new List<string?>();
        process.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        int responseMessages = 0;
        process.ResponseMessage = _ =>
        {
            responseMessages++;
            return Task.CompletedTask;
        };
        // a NACK carries its reason as the Parameter Data
        using var helper = answering(ERDM_ResponseType.NACK_REASON, new byte[] { 0x00, (byte)ERDM_NackReason.DATA_OUT_OF_RANGE });

        await process.Run(helper);

        var entry = process.RequestResponseHistory.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed));
            Assert.That(process.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
            Assert.That(process.ResponsePayloadObject.IsUnset, Is.True);
            Assert.That(entry.State, Is.EqualTo("NACK"));
            Assert.That(responseMessages, Is.EqualTo(1), "the NACK is passed on");
            Assert.That(changed, Does.Contain(nameof(PeerToPeerProcess.NackReason)));
        }
    }

    [Test, MaxTime(10000)]
    public async Task TestProcessRunsOnlyOnce()
    {
        var process = newGet();
        int requests = 0;
        using var helper = answering(ERDM_ResponseType.ACK, new byte[] { 0x41, 0x42 }, () => Interlocked.Increment(ref requests));

        await process.Run(helper);
        Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
        Assert.That(requests, Is.EqualTo(1));

        // a finished process is not sent again
        await process.Run(helper);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(requests, Is.EqualTo(1));
            Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
            Assert.That(process.RequestResponseHistory, Has.Exactly(1).Items);
        }
    }
}
