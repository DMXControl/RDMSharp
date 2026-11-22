using RDMSharp.Metadata;
using RDMSharp.RDM.Device;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.Devices;

/// <summary>
/// A device that does not answer a parameter, while it answers everything else, ignores a parameter it does not support (a faulty implementation,
/// it should NACK with UNKNOWN_PID). A device that is not reachable or too busy does not answer either: that is no reason to give up a parameter.
/// <list type="bullet">
/// <item>After a request without an answer the device is asked for DEVICE_INFO: an answer means the parameter is ignored, none means
/// the device is not reachable (the parameter is asked for again later, nothing is flagged).</item>
/// <item>The model is flagged when it is confirmed: a second device of the model, or the same device the second time.
/// A flagged parameter is not asked for any more by the other devices of the model.</item>
/// <item>The flag is withdrawn as soon as a device answers the parameter, so a wrong flag does no harm.</item>
/// </list>
/// Invented data only (manufacturer 0x7FF0 is the ESTA prototyping range). Every test has its own device model.
/// </summary>
public class TestSilentParameters
{
    private const ushort PROTOTYPE_MANUFACTURER = 0x7FF0;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [TearDown]
    public void TearDown()
    {
        GlobalTestSetup.DropRequest = null;
    }

    private static async Task pulled(AbstractRemoteRDMDevice device)
    {
        while (!device.IsInitialized)
            await Task.Delay(100);
        while (!device.AllDataPulled)
            await Task.Delay(100);
    }

    private static bool isSilent(AbstractRemoteRDMDevice device, ERDM_Parameter parameter)
        => device.DeviceModel.GetSilentParameters().Any(p => p.Parameter == parameter);

    private sealed class DeviceOfModel4 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5204; public DeviceOfModel4(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel5 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5205; public DeviceOfModel5(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel6 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5206; public DeviceOfModel6(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel7 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5207; public DeviceOfModel7(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel8 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5208; public DeviceOfModel8(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel9 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5209; public DeviceOfModel9(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel10 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x520A; public DeviceOfModel10(UID uid) : base(uid) { } }

    #region The confirmation
    [Test]
    public void TestOneReportIsNotEnough()
    {
        var metadata = SupportedParameterMetadata.Create(ERDM_Parameter.IDENTIFY_DEVICE);
        metadata.ReportSilent(new UID(PROTOTYPE_MANUFACTURER, 1));
        Assert.That(metadata.IsSilent, Is.False, "a device that is busy does not answer as well");
    }

    [Test]
    public void TestASecondDeviceConfirms()
    {
        var metadata = SupportedParameterMetadata.Create(ERDM_Parameter.IDENTIFY_DEVICE);
        metadata.ReportSilent(new UID(PROTOTYPE_MANUFACTURER, 1));
        metadata.ReportSilent(new UID(PROTOTYPE_MANUFACTURER, 2));
        Assert.That(metadata.IsSilent, Is.True);
    }

    [Test]
    public void TestTheSameDeviceTheSecondTimeConfirms()
    {
        // a model with one device: it is asked again later and did not answer again
        var metadata = SupportedParameterMetadata.Create(ERDM_Parameter.IDENTIFY_DEVICE);
        var uid = new UID(PROTOTYPE_MANUFACTURER, 1);
        metadata.ReportSilent(uid);
        Assert.That(metadata.IsSilent, Is.False);
        metadata.ReportSilent(uid);
        Assert.That(metadata.IsSilent, Is.True);
    }

    [Test]
    public void TestAnAnswerWithdrawsTheFlagAndTheReports()
    {
        var metadata = SupportedParameterMetadata.Create(ERDM_Parameter.IDENTIFY_DEVICE);
        metadata.ReportSilent(new UID(PROTOTYPE_MANUFACTURER, 1));
        metadata.ReportSilent(new UID(PROTOTYPE_MANUFACTURER, 2));
        Assert.That(metadata.IsSilent, Is.True, "precondition");

        metadata.ReportAnswered();
        Assert.That(metadata.IsSilent, Is.False);

        // what was reported before is forgotten: it needs a confirmation again
        metadata.ReportSilent(new UID(PROTOTYPE_MANUFACTURER, 1));
        Assert.That(metadata.IsSilent, Is.False);
    }
    #endregion

    #region The devices
    /// <summary>
    /// The first two devices of a model ignore a parameter. The model is flagged when the second one confirms it, the third device
    /// is not even asked for the parameter.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestParameterIgnoredByTwoDevicesIsNotAskedForAgain()
    {
        var uid1 = new UID(PROTOTYPE_MANUFACTURER, 0x00005201);
        var uid2 = new UID(PROTOTYPE_MANUFACTURER, 0x00005202);
        var uid3 = new UID(PROTOTYPE_MANUFACTURER, 0x00005203);
        using var timeouts = new ShortTimeouts(silentParameterRetryInterval: 60000);
        using var generated1 = new DeviceOfModel4(uid1);
        using var generated2 = new DeviceOfModel4(uid2);
        using var generated3 = new DeviceOfModel4(uid3);

        int askedThirdDevice = 0;
        GlobalTestSetup.DropRequest = m =>
        {
            if (m.Command != ERDM_Command.GET_COMMAND || m.Parameter != ERDM_Parameter.IDENTIFY_DEVICE)
                return false;
            if (m.DestUID == uid3)
            {
                Interlocked.Increment(ref askedThirdDevice);
                return false;
            }
            return true; // the first two devices ignore the parameter
        };

        using MockDevice remote1 = new MockDevice(uid1);
        await pulled(remote1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(remote1.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(isSilent(remote1, ERDM_Parameter.IDENTIFY_DEVICE), Is.False, "one device is not enough, it can be too busy");
        }

        using MockDevice remote2 = new MockDevice(uid2);
        await pulled(remote2);
        Assert.That(isSilent(remote2, ERDM_Parameter.IDENTIFY_DEVICE), Is.True, "a second device confirms it");

        using MockDevice remote3 = new MockDevice(uid3);
        await pulled(remote3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(askedThirdDevice, Is.Zero, "the third device is not asked for a parameter the model does not get answered");
            Assert.That(remote3.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(remote3.DeviceModel.GetKnownNotSupportedParameters().Select(p => p.Parameter), Does.Contain(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(remote3.DeviceModel.GetSupportedNonBlueprintParameters().Select(p => p.Parameter), Does.Not.Contain(ERDM_Parameter.IDENTIFY_DEVICE));
        }
    }

    /// <summary>
    /// The flag is not for good: a parameter is asked for again now and then, and the first answer withdraws the flag.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestFlagIsWithdrawnWhenADeviceAnswers()
    {
        var uid1 = new UID(PROTOTYPE_MANUFACTURER, 0x00005211);
        var uid2 = new UID(PROTOTYPE_MANUFACTURER, 0x00005212);
        using var timeouts = new ShortTimeouts(silentParameterRetryInterval: 600);
        using var generated1 = new DeviceOfModel5(uid1);
        using var generated2 = new DeviceOfModel5(uid2);

        bool answering = false;
        GlobalTestSetup.DropRequest = m => !answering && m.Command == ERDM_Command.GET_COMMAND && m.Parameter == ERDM_Parameter.IDENTIFY_DEVICE;

        using MockDevice remote1 = new MockDevice(uid1);
        await pulled(remote1);
        using MockDevice remote2 = new MockDevice(uid2);
        await pulled(remote2);
        Assert.That(isSilent(remote2, ERDM_Parameter.IDENTIFY_DEVICE), Is.True, "precondition: the model is flagged");

        // the devices answer (the faulty firmware is updated, the traffic is over)
        answering = true;
        await Wait.UntilAsync(() => !isSilent(remote1, ERDM_Parameter.IDENTIFY_DEVICE), 8000);
        await Wait.UntilAsync(() => remote1.GetAllParameterValues().ContainsKey(ERDM_Parameter.IDENTIFY_DEVICE), 8000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isSilent(remote1, ERDM_Parameter.IDENTIFY_DEVICE), Is.False, "an answer withdraws the flag");
            Assert.That(remote1.GetAllParameterValues(), Does.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(remote1.DeviceModel.GetSupportedNonBlueprintParameters().Select(p => p.Parameter), Does.Contain(ERDM_Parameter.IDENTIFY_DEVICE));
        }
    }

    /// <summary>
    /// A device that does not answer at all (not reachable, or too busy) is no device that ignores a parameter:
    /// the parameter is not flagged, it is asked for again when the device answers.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestDeviceThatIsNotReachableDoesNotFlagTheParameter()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00005221);
        using var timeouts = new ShortTimeouts();
        using var generated = new DeviceOfModel6(uid);

        bool unreachable = true;
        bool probeLost = false;
        GlobalTestSetup.DropRequest = m =>
        {
            if (!unreachable || m.DestUID != uid || m.Command != ERDM_Command.GET_COMMAND)
                return false;
            if (m.Parameter == ERDM_Parameter.IDENTIFY_DEVICE)
            {
                probeLost = true;
                return true;
            }
            // the question after the first lost request is lost as well: the device does not answer at all
            return m.Parameter == ERDM_Parameter.DEVICE_INFO && probeLost;
        };

        using MockDevice remote = new MockDevice(uid);
        await pulled(remote);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(remote.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE), "precondition: no answer");
            Assert.That(isSilent(remote, ERDM_Parameter.IDENTIFY_DEVICE), Is.False, "a device that does not answer at all does not ignore the parameter");
        }

        unreachable = false;
        await Wait.UntilAsync(() => remote.GetAllParameterValues().ContainsKey(ERDM_Parameter.IDENTIFY_DEVICE), 8000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(remote.GetAllParameterValues(), Does.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE), "asked for again when the device is reachable");
            Assert.That(isSilent(remote, ERDM_Parameter.IDENTIFY_DEVICE), Is.False);
        }
    }

    /// <summary>
    /// A device that ignores SUPPORTED_PARAMETERS, but answers everything else, is faulty: after a few attempts the model is initialized
    /// without them (the parameters that are assumed), as before.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestDeviceThatIgnoresSupportedParametersIsInitializedAnyway()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00005231);
        using var timeouts = new ShortTimeouts();
        using var generated = new DeviceOfModel7(uid);
        GlobalTestSetup.DropRequest = m => m.DestUID == uid && m.Command == ERDM_Command.GET_COMMAND && m.Parameter == ERDM_Parameter.SUPPORTED_PARAMETERS;

        using MockDevice remote = new MockDevice(uid);
        await pulled(remote);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(remote.DeviceModel.IsInitialized, Is.True);
            Assert.That(remote.DeviceModel.GetSupportedParameters(), Is.Not.Empty, "the parameters that are assumed");
            Assert.That(remote.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.SUPPORTED_PARAMETERS));
        }
    }
    #endregion

    #region A lost answer is no reason to give a parameter up
    /// <summary>
    /// A parameter the device answered before is no parameter the device ignores: one request that gets no answer (a busy bus) must not park it.
    /// It is asked for again soon.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestLostAnswerOfAnAnsweredParameterDoesNotParkIt()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00005241);
        // a parameter that was classified as ignored would only be asked for again after a minute
        using var timeouts = new ShortTimeouts(silentParameterRetryInterval: 60000);
        using var generated = new DeviceOfModel8(uid);
        using MockDevice remote = new MockDevice(uid);
        await pulled(remote);
        Assert.That(remote.GetAllParameterValues(), Does.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE), "precondition: the device answered the parameter");

        // one update request gets no answer, afterwards the requests are counted
        int dropped = 0;
        int askedAgain = 0;
        bool dropping = true;
        GlobalTestSetup.DropRequest = m =>
        {
            if (m.DestUID != uid || m.Command != ERDM_Command.GET_COMMAND || m.Parameter != ERDM_Parameter.IDENTIFY_DEVICE)
                return false;
            if (dropping)
            {
                Interlocked.Increment(ref dropped);
                return true;
            }
            Interlocked.Increment(ref askedAgain);
            return false;
        };
        await Wait.UntilAsync(() => dropped > 0, 8000);
        Assert.That(dropped, Is.GreaterThan(0), "precondition: a request got no answer");
        await Task.Delay(800); // the request is given up (400 ms) and the device is asked if it is alive
        dropping = false;

        // the value of this device also comes in with its queued messages, so the proof that the parameter is not parked is the request
        await Wait.UntilAsync(() => askedAgain > 0, 8000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(askedAgain, Is.GreaterThan(0), "the parameter is asked for again soon, it is not parked");
            Assert.That(isSilent(remote, ERDM_Parameter.IDENTIFY_DEVICE), Is.False);
        }
    }

    /// <summary>
    /// A lost answer on two devices of a model, which both answered the parameter before, says nothing about the model:
    /// it is not flagged.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestLostAnswersOfTwoDevicesDoNotFlagTheModel()
    {
        var uid1 = new UID(PROTOTYPE_MANUFACTURER, 0x00005251);
        var uid2 = new UID(PROTOTYPE_MANUFACTURER, 0x00005252);
        using var timeouts = new ShortTimeouts(silentParameterRetryInterval: 60000);
        using var generated1 = new DeviceOfModel9(uid1);
        using var generated2 = new DeviceOfModel9(uid2);
        using MockDevice remote1 = new MockDevice(uid1);
        await pulled(remote1);
        using MockDevice remote2 = new MockDevice(uid2);
        await pulled(remote2);

        int dropped1 = 0, dropped2 = 0;
        bool dropping = true;
        GlobalTestSetup.DropRequest = m =>
        {
            if (!dropping || m.Command != ERDM_Command.GET_COMMAND || m.Parameter != ERDM_Parameter.IDENTIFY_DEVICE)
                return false;
            if (m.DestUID == uid1)
                Interlocked.Increment(ref dropped1);
            else if (m.DestUID == uid2)
                Interlocked.Increment(ref dropped2);
            else
                return false;
            return true;
        };
        await Wait.UntilAsync(() => dropped1 > 0 && dropped2 > 0, 8000);
        Assert.That(dropped1 > 0 && dropped2 > 0, Is.True, "precondition: both devices lost a request");
        await Task.Delay(1000); // the requests are given up and the devices are asked if they are alive
        dropping = false;

        Assert.That(isSilent(remote1, ERDM_Parameter.IDENTIFY_DEVICE), Is.False, "the devices answered the parameter before, a lost answer is no confirmation");
    }

    /// <summary>
    /// The answer of any device of the model withdraws what was reported about the parameter, not only the one of the device that reported it.
    /// </summary>
    [Test, MaxTime(90000)]
    public async Task TestAnswerOfAnotherDeviceWithdrawsTheFlag()
    {
        var uidAnswering = new UID(PROTOTYPE_MANUFACTURER, 0x00005261);
        var uidIgnoring1 = new UID(PROTOTYPE_MANUFACTURER, 0x00005262);
        var uidIgnoring2 = new UID(PROTOTYPE_MANUFACTURER, 0x00005263);
        using var timeouts = new ShortTimeouts(silentParameterRetryInterval: 60000);
        using var generatedAnswering = new DeviceOfModel10(uidAnswering);
        using var generatedIgnoring1 = new DeviceOfModel10(uidIgnoring1);
        using var generatedIgnoring2 = new DeviceOfModel10(uidIgnoring2);

        // the first device answers (and its update asks for the parameter again and again), two others never answer it
        using MockDevice remoteAnswering = new MockDevice(uidAnswering);
        await pulled(remoteAnswering);
        GlobalTestSetup.DropRequest = m => (m.DestUID == uidIgnoring1 || m.DestUID == uidIgnoring2) && m.Command == ERDM_Command.GET_COMMAND && m.Parameter == ERDM_Parameter.IDENTIFY_DEVICE;
        using MockDevice remoteIgnoring1 = new MockDevice(uidIgnoring1);
        await pulled(remoteIgnoring1);
        using MockDevice remoteIgnoring2 = new MockDevice(uidIgnoring2);
        await pulled(remoteIgnoring2);

        // the next update of the first device is answered: that withdraws the reports of the other two
        await Wait.UntilAsync(() => !isSilent(remoteAnswering, ERDM_Parameter.IDENTIFY_DEVICE), 8000);
        await Task.Delay(1000);
        Assert.That(isSilent(remoteAnswering, ERDM_Parameter.IDENTIFY_DEVICE), Is.False, "a device of the model answers the parameter, so it is not ignored");
    }
    #endregion
}
