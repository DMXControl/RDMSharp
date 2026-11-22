using RDMSharp.Metadata;
using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM;

/// <summary>
/// The value cache of <see cref="AbstractRDMCache"/>: a parameter that is polled again with the same data is no change.
/// Only a new value raises <c>ParameterValueAdded</c>, only a different value raises <c>ParameterValueChanged</c>.
/// </summary>
public class TestParameterValueChangeDetection
{
    private sealed class Cache : AbstractRDMCache
    {
        public readonly List<string> Events = new();
        public Cache()
        {
            ParameterValueAdded += (_, e) => Events.Add($"added {e.Parameter} {e.Index}");
            ParameterValueChanged += (_, e) => Events.Add($"changed {e.Parameter} {e.Index}");
        }
        public void Store(ERDM_Parameter parameter, object value, object? index = null)
        {
            var branch = DataTreeBranch.FromObject(value, null, new ParameterBag(parameter), ERDM_Command.GET_COMMAND_RESPONSE);
            updateParameterValuesDataTreeBranch(index is null ? new ParameterDataCacheBag(parameter) : new ParameterDataCacheBag(parameter, index), branch);
        }
    }

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [Test]
    public void TestSameNumberIsNoChange()
    {
        using var cache = new Cache();
        cache.Store(ERDM_Parameter.DMX_START_ADDRESS, (ushort)17);
        cache.Store(ERDM_Parameter.DMX_START_ADDRESS, (ushort)17); // a new boxed object with the same value
        cache.Store(ERDM_Parameter.DMX_START_ADDRESS, (ushort)18);
        Assert.That(cache.Events, Is.EqualTo(new[] { "added DMX_START_ADDRESS ", "changed DMX_START_ADDRESS " }));
        Assert.That(cache.ParameterValues[ERDM_Parameter.DMX_START_ADDRESS], Is.EqualTo((ushort)18));
    }

    [Test]
    public void TestSamePayloadObjectIsNoChange()
    {
        using var cache = new Cache();
        cache.Store(ERDM_Parameter.DEVICE_INFO, new RDMDeviceInfo(deviceModelId: 0x0042, softwareVersionId: 7));
        cache.Store(ERDM_Parameter.DEVICE_INFO, new RDMDeviceInfo(deviceModelId: 0x0042, softwareVersionId: 7));
        cache.Store(ERDM_Parameter.DEVICE_INFO, new RDMDeviceInfo(deviceModelId: 0x0042, softwareVersionId: 8));
        Assert.That(cache.Events, Is.EqualTo(new[] { "added DEVICE_INFO ", "changed DEVICE_INFO " }));
    }

    [Test]
    public void TestSameArrayIsNoChange()
    {
        using var cache = new Cache();
        cache.Store(ERDM_Parameter.SUPPORTED_PARAMETERS, new[] { ERDM_Parameter.DEVICE_LABEL, ERDM_Parameter.IDENTIFY_DEVICE });
        cache.Store(ERDM_Parameter.SUPPORTED_PARAMETERS, new[] { ERDM_Parameter.DEVICE_LABEL, ERDM_Parameter.IDENTIFY_DEVICE });
        cache.Store(ERDM_Parameter.SUPPORTED_PARAMETERS, new[] { ERDM_Parameter.DEVICE_LABEL });
        Assert.That(cache.Events, Is.EqualTo(new[] { "added SUPPORTED_PARAMETERS ", "changed SUPPORTED_PARAMETERS " }));
    }

    [Test]
    public void TestIndexedValues()
    {
        using var cache = new Cache();
        cache.Store(ERDM_Parameter.SENSOR_VALUE, new RDMSensorValue(0, 20), (byte)0);
        cache.Store(ERDM_Parameter.SENSOR_VALUE, new RDMSensorValue(1, 30), (byte)1); // a new index is added
        cache.Store(ERDM_Parameter.SENSOR_VALUE, new RDMSensorValue(0, 20), (byte)0); // polled again, same value
        cache.Store(ERDM_Parameter.SENSOR_VALUE, new RDMSensorValue(1, 31), (byte)1);
        Assert.That(cache.Events, Is.EqualTo(new[] { "added SENSOR_VALUE 0", "added SENSOR_VALUE 1", "changed SENSOR_VALUE 1" }));
        var values = (System.Collections.Concurrent.ConcurrentDictionary<object, object>)cache.ParameterValues[ERDM_Parameter.SENSOR_VALUE];
        Assert.That(((RDMSensorValue)values[(byte)1]).PresentValue, Is.EqualTo((short)31));
    }

    [Test]
    public void TestValuesEqual()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(AbstractRDMCache.ValuesEqual(null, null), Is.True);
            Assert.That(AbstractRDMCache.ValuesEqual(null, 1), Is.False);
            Assert.That(AbstractRDMCache.ValuesEqual((byte)1, (ushort)1), Is.False, "a different type is a change");
            Assert.That(AbstractRDMCache.ValuesEqual("label", "label"), Is.True);
            Assert.That(AbstractRDMCache.ValuesEqual(new ushort[] { 1, 2 }, new ushort[] { 1, 2 }), Is.True);
            Assert.That(AbstractRDMCache.ValuesEqual(new ushort[] { 1, 2 }, new ushort[] { 1, 2, 3 }), Is.False);
        }
    }
}
