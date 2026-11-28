using RDMSharp.Metadata;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.Devices;

/// <summary>
/// What a remote device does if a request gets no answer.
/// <para>
/// A device that does not answer can be two things, and they can not be told apart by the request alone:
/// a device with a faulty implementation that ignores a parameter it does not support (instead of a NACK with UNKNOWN_PID),
/// or a device that is too busy to answer (many devices, a lot of traffic). So a request without an answer must not be taken for
/// "not supported" for good: a parameter that got no answer once has to be asked for again later, the other data
/// of the device (and of its model, which all devices of the model share) must not be lost because of it.
/// </para>
/// Invented data only (manufacturer 0x7FF0 is the ESTA prototyping range). Every test has its own device model.
/// </summary>
public class TestRemoteDeviceTimeouts
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

    // a device model of its own for every test, devices of one model share what the model got
    private sealed class DeviceOfModel1 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5101; public DeviceOfModel1(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel2 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5102; public DeviceOfModel2(UID uid) : base(uid) { } }
    private sealed class DeviceOfModel3 : MockGeneratedDevice1 { public override ushort DeviceModelID => 0x5103; public DeviceOfModel3(UID uid) : base(uid) { } }

    /// <summary>
    /// The first request of a parameter gets no answer (the device is busy). The parameter is only asked for in the first pass,
    /// and only a parameter that has a value is updated later: it is never asked for again.
    /// </summary>
    [Test, MaxTime(60000)]
    public async Task TestParameterWithoutAnAnswerIsAskedForAgain()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00005101);
        using var timeouts = new ShortTimeouts();
        using var generated = new DeviceOfModel1(uid);
        GlobalTestSetup.DropRequest = m => m.DestUID == uid && m.Command == ERDM_Command.GET_COMMAND && m.Parameter == ERDM_Parameter.IDENTIFY_DEVICE;

        using MockDevice remote = new MockDevice(uid);
        await pulled(remote);
        Assert.That(remote.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE), "precondition: the request got no answer");

        // the device answers again (the traffic is over)
        GlobalTestSetup.DropRequest = null;

        await Wait.UntilAsync(() => remote.GetAllParameterValues().ContainsKey(ERDM_Parameter.IDENTIFY_DEVICE), 8000);
        Assert.That(remote.GetAllParameterValues(), Does.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE), "the parameter has to be asked for again");
    }

    /// <summary>
    /// The first device of a model gets no answer to SUPPORTED_PARAMETERS. The model keeps what it has (nearly nothing) as initialized and shares it
    /// with every device of the model: another device of the model that answers everything still has no parameters.
    /// </summary>
    [Test, MaxTime(60000)]
    public async Task TestModelIsNotLostBecauseTheFirstDeviceDidNotAnswer()
    {
        var uidFirst = new UID(PROTOTYPE_MANUFACTURER, 0x00005102);
        var uidSecond = new UID(PROTOTYPE_MANUFACTURER, 0x00005103);
        using var timeouts = new ShortTimeouts();
        using var generatedFirst = new DeviceOfModel2(uidFirst);
        using var generatedSecond = new DeviceOfModel2(uidSecond);

        GlobalTestSetup.DropRequest = m => m.DestUID == uidFirst && m.Command == ERDM_Command.GET_COMMAND && m.Parameter == ERDM_Parameter.SUPPORTED_PARAMETERS;
        using MockDevice remoteFirst = new MockDevice(uidFirst);

        // the device is busy for a moment: the model must not be taken as initialized without what it did not get
        await Task.Delay(1000);
        Assert.That(remoteFirst.IsInitialized, Is.False, "precondition: without the supported parameters nothing is initialized");

        // from now on the devices answer
        GlobalTestSetup.DropRequest = null;
        using MockDevice remoteSecond = new MockDevice(uidSecond);
        await pulled(remoteFirst);
        await pulled(remoteSecond);

        // the sensors are not one of the parameters that are assumed without being announced
        await Wait.UntilAsync(() => remoteSecond.DeviceModel.GetSupportedParameters().Any(p => p.Parameter == ERDM_Parameter.SENSOR_DEFINITION), 8000);
        Assert.That(remoteSecond.DeviceModel.GetSupportedParameters().Select(p => p.Parameter), Does.Contain(ERDM_Parameter.SENSOR_DEFINITION),
            "the model has to be asked for again, a device that answers everything has its parameters");
    }

    /// <summary>
    /// A list (here the definitions of the sensors) is asked for entry by entry. One entry without an answer is taken for the end of the list:
    /// the following entries are not asked for.
    /// </summary>
    [Test, MaxTime(60000)]
    public async Task TestListIsNotCutOffByAnEntryWithoutAnAnswer()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00005104);
        using var timeouts = new ShortTimeouts();
        using var generated = new DeviceOfModel3(uid);
        // the sensors 0 - 4 exist, the one with the id 3 does not answer
        GlobalTestSetup.DropRequest = m => m.DestUID == uid && m.Command == ERDM_Command.GET_COMMAND && m.Parameter == ERDM_Parameter.SENSOR_DEFINITION && m.Value is byte sensorId && sensorId == 3;

        using MockDevice remote = new MockDevice(uid);
        await pulled(remote);

        var definitions = remote.DeviceModel.GetSensorDefinitions().Select(d => d.SensorId).ToArray();
        Assert.That(definitions, Does.Contain((byte)0).And.Contain((byte)1).And.Contain((byte)2), "precondition: the entries in front of it are there");
        Assert.That(definitions, Does.Not.Contain((byte)3), "precondition: the entry itself got no answer");
        Assert.That(definitions, Does.Contain((byte)4), "the entry behind the one without an answer has to be asked for");
    }
}
