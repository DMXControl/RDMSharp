using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using CommandType = RDMSharp.Metadata.JSON.Command.ECommandDublicate;

namespace RDMSharpTests.RDM.PayloadObject;

/// <summary>
/// PRESET_INFO (E1.37-1 5.1): Devices not supporting DMX_FAIL_MODE / DMX_STARTUP_MODE report 0xFFFF for their minimum and maximum times.
/// <see cref="RDMPresetInfo"/> represents "not supported" as <c>null</c>.
/// </summary>
public class RDMPresetInfoTest
{
    // 6 booleans, Maximum Scene Number, 4 Preset times: the DMX_FAIL_MODE / DMX_STARTUP_MODE times start at byte 16
    private const int FAIL_AND_STARTUP_TIMES_OFFSET = 16;

    private static readonly RDMPresetInfo NOT_SUPPORTED = new RDMPresetInfo(
        levelFieldSupported: true,
        maximumSceneNumber: 10,
        minimumPresetFadeTimeSupported: 0.0,
        maximumPresetFadeTimeSupported: 100.0,
        minimumPresetWaitTimeSupported: 0.0,
        maximumPresetWaitTimeSupported: 100.0);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [Test]
    public void TestToPayloadDataNotSupported()
    {
        byte[] data = NOT_SUPPORTED.ToPayloadData();
        Assert.That(data, Has.Length.EqualTo(RDMPresetInfo.PDL));
        Assert.That(data.Skip(FAIL_AND_STARTUP_TIMES_OFFSET), Is.All.EqualTo((byte)0xFF), "not supported is 0xFFFF");
    }

    [Test]
    public void TestMetadataSerializationNotSupported()
    {
        // the generated device answers a GET with the metadata serialization, it has to be the same as ToPayloadData()
        var parameterBag = new ParameterBag(ERDM_Parameter.PRESET_INFO);
        var dataTreeBranch = DataTreeBranch.FromObject(NOT_SUPPORTED, null, parameterBag, ERDM_Command.GET_COMMAND_RESPONSE);
        byte[] data = MetadataFactory.GetResponseMessageData(parameterBag, dataTreeBranch).SelectMany(d => d).ToArray();
        Assert.That(data, Is.EqualTo(NOT_SUPPORTED.ToPayloadData()));
    }

    [Test]
    public void TestParseNotSupported()
    {
        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.PRESET_INFO));
        byte[] data = NOT_SUPPORTED.ToPayloadData();
        var parsed = MetadataFactory.ParseDataToPayload(define, CommandType.GetResponse, data).ParsedObject as RDMPresetInfo;
        Assert.That(parsed, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parsed!.MinimumDMX512FailDelayTimeSupported, Is.Null);
            Assert.That(parsed.MaximumDMX512FailDelayTimeSupported, Is.Null);
            Assert.That(parsed.MinimumDMX512FailDelayHoldSupported, Is.Null);
            Assert.That(parsed.MaximumDMX512FailDelayHoldSupported, Is.Null);
            Assert.That(parsed.MinimumStartupDelayTimeSupported, Is.Null);
            Assert.That(parsed.MaximumStartupDelayTimeSupported, Is.Null);
            Assert.That(parsed.MinimumStartupDelayHoldSupported, Is.Null);
            Assert.That(parsed.MaximumStartupDelayHoldSupported, Is.Null);
            Assert.That(parsed, Is.EqualTo(NOT_SUPPORTED));
        }
    }

    [Test]
    public void TestSupportedTimesAreKept()
    {
        // 0xFFFE (6553.4 s) is the largest real time, only 0xFFFF means not supported
        var info = new RDMPresetInfo(
            minimumDMX512FailDelayTimeSupported: 0.0,
            maximumDMX512FailDelayTimeSupported: 6553.4,
            minimumDMX512FailDelayHoldSupported: 0.1,
            maximumDMX512FailDelayHoldSupported: 6553.4,
            minimumStartupDelayTimeSupported: 0.1,
            maximumStartupDelayTimeSupported: 6553.4,
            minimumStartupDelayHoldSupported: 0.1,
            maximumStartupDelayHoldSupported: 6553.4);
        byte[] data = info.ToPayloadData();
        Assert.That(data.Skip(FAIL_AND_STARTUP_TIMES_OFFSET).Take(4), Is.EqualTo(new byte[] { 0x00, 0x00, 0xFF, 0xFE }));

        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.PRESET_INFO));
        var parsed = MetadataFactory.ParseDataToPayload(define, CommandType.GetResponse, data).ParsedObject as RDMPresetInfo;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parsed!.MinimumDMX512FailDelayTimeSupported, Is.EqualTo(0.0).Within(0.001));
            Assert.That(parsed.MaximumDMX512FailDelayTimeSupported, Is.EqualTo(6553.4).Within(0.001));
            Assert.That(parsed, Is.EqualTo(info));
        }
    }
}
