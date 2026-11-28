using RDMSharp.Metadata;

namespace RDMSharpTests.Devices;

/// <summary>
/// The transaction lock lets one request at a time run to a device (a device answers one request at a time).
/// <list type="bullet">
/// <item>A wait that timed out goes on without the lock: it must not release it, that would let two transactions run at once from then on.</item>
/// <item>The lock is released whatever happens in the transaction (an exception of a handler).</item>
/// </list>
/// </summary>
public class TestTransactionLock
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [Test, MaxTime(10000)]
    public async Task TestSecondWaitTimesOutWhileTheLockIsHeld()
    {
        var uid = new UID(0x7FF0, 0x00005301);
        var instance = RDMSharp.RDMSharp.Instance;

        Assert.That(await instance.lockTransaktion(uid, TimeSpan.FromSeconds(1)), Is.True, "the lock is free");
        Assert.That(await instance.lockTransaktion(uid, TimeSpan.FromMilliseconds(100)), Is.False, "the lock is held, the wait timed out");

        instance.unlockTransaktion(uid);
        Assert.That(await instance.lockTransaktion(uid, TimeSpan.FromSeconds(1)), Is.True, "released");
        instance.unlockTransaktion(uid);
    }

    [Test, MaxTime(10000)]
    public async Task TestWaitThatTimedOutAddsNoRelease()
    {
        var uid = new UID(0x7FF0, 0x00005302);
        var instance = RDMSharp.RDMSharp.Instance;

        Assert.That(await instance.lockTransaktion(uid), Is.True);
        Assert.That(await instance.lockTransaktion(uid, TimeSpan.FromMilliseconds(50)), Is.False);

        // only the holder releases: one release frees it for one waiter, not for two
        instance.unlockTransaktion(uid);
        Assert.That(await instance.lockTransaktion(uid, TimeSpan.FromMilliseconds(500)), Is.True, "the first waiter gets it");
        Assert.That(await instance.lockTransaktion(uid, TimeSpan.FromMilliseconds(50)), Is.False, "the second one has to wait");
        instance.unlockTransaktion(uid);
    }

    [Test, MaxTime(10000)]
    public async Task TestLockIsReleasedAfterAnExceptionBeforeTheRequest()
    {
        var uid = new UID(0x7FF0, 0x00005303);
        var process = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, SubDevice.Root, new ParameterBag(ERDM_Parameter.DEVICE_LABEL));
        process.BeforeSendMessage = _ => throw new InvalidOperationException("boom");
        using var helper = new AsyncRDMRequestHelper(_ => Task.CompletedTask);

        await process.Run(helper);

        Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed), "precondition");
        Assert.That(await RDMSharp.RDMSharp.Instance.lockTransaktion(uid, TimeSpan.FromMilliseconds(300)), Is.True, "the lock was released");
        RDMSharp.RDMSharp.Instance.unlockTransaktion(uid);
    }

    [Test, MaxTime(10000)]
    public async Task TestLockIsReleasedAfterAnExceptionOfTheHistoryHandler()
    {
        var uid = new UID(0x7FF0, 0x00005304);
        var process = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, SubDevice.Root, new ParameterBag(ERDM_Parameter.DEVICE_LABEL));
        process.OnRequestResponseHistoryAdded += (_, _) => throw new InvalidOperationException("boom");
        using var helper = new AsyncRDMRequestHelper(_ => Task.CompletedTask);

        await process.Run(helper);

        Assert.That(process.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Failed), "precondition");
        Assert.That(await RDMSharp.RDMSharp.Instance.lockTransaktion(uid, TimeSpan.FromMilliseconds(300)), Is.True, "the lock was released");
        RDMSharp.RDMSharp.Instance.unlockTransaktion(uid);
    }

    /// <summary>
    /// Two requests to one device run one after the other, never at the same time.
    /// </summary>
    [Test, MaxTime(20000)]
    public async Task TestRequestsToOneDeviceRunOneAfterTheOther()
    {
        var uid = new UID(0x7FF0, 0x00005305);
        int running = 0;
        int mostAtOnce = 0;
        AsyncRDMRequestHelper? helper = null;
        helper = new AsyncRDMRequestHelper(async message =>
        {
            int now = Interlocked.Increment(ref running);
            lock (this)
                mostAtOnce = Math.Max(mostAtOnce, now);
            await Task.Delay(100);
            var response = new RDMMessage()
            {
                Command = message.Command | ERDM_Command.RESPONSE,
                DestUID = message.SourceUID,
                SourceUID = message.DestUID,
                Parameter = message.Parameter,
                SubDevice = message.SubDevice,
                TransactionCounter = message.TransactionCounter,
                ParameterData = new byte[] { 0x41 }
            };
            Interlocked.Decrement(ref running);
            helper?.ReceiveMessage(response);
        });
        try
        {
            var first = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, SubDevice.Root, new ParameterBag(ERDM_Parameter.DEVICE_LABEL));
            var second = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, SubDevice.Root, new ParameterBag(ERDM_Parameter.MANUFACTURER_LABEL));
            await Task.WhenAll(first.Run(helper), second.Run(helper));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
                Assert.That(second.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Finished));
                Assert.That(mostAtOnce, Is.EqualTo(1), "one request at a time to a device");
            }
        }
        finally
        {
            helper.Dispose();
        }
    }
}
