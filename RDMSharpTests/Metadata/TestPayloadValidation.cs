using RDMSharp.Metadata;
using CommandType = RDMSharp.Metadata.JSON.Command.ECommandDublicate;

namespace RDMSharpTests.Metadata;

/// <summary>
/// <see cref="MetadataFactory.ValidatePayload"/> checks the Parameter Data against the define before it is parsed:
/// <list type="bullet">
/// <item>the length has to fit the fields of the define (fixed sizes, PDL range, complete list items, text length),</item>
/// <item>a boolean may only be 0x00 or 0x01.</item>
/// </list>
/// </summary>
public class TestPayloadValidation
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    private static EPayloadValidation validate(ERDM_Parameter parameter, CommandType commandType, params byte[] data)
    {
        var define = MetadataFactory.GetDefine(new ParameterBag(parameter));
        return MetadataFactory.ValidatePayload(define, commandType, data, out _);
    }

    [Test]
    public void TestFixedSize()
    {
        // CURVE: current curve (uint8) + number of curves (uint8)
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(ERDM_Parameter.CURVE, CommandType.GetResponse, 0x02, 0x03), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.CURVE, CommandType.GetResponse, 0x03), Is.EqualTo(EPayloadValidation.LengthMismatch), "too short");
            Assert.That(validate(ERDM_Parameter.CURVE, CommandType.GetResponse, 0x02, 0x03, 0x04), Is.EqualTo(EPayloadValidation.LengthMismatch), "too long");
            Assert.That(validate(ERDM_Parameter.CURVE, CommandType.GetResponse), Is.EqualTo(EPayloadValidation.LengthMismatch), "empty");

            // PRESET_PLAYBACK: mode (uint16) + level (uint8)
            Assert.That(validate(ERDM_Parameter.PRESET_PLAYBACK, CommandType.GetResponse, 0x00, 0x00, 0xFF), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.PRESET_PLAYBACK, CommandType.GetResponse, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00), Is.EqualTo(EPayloadValidation.LengthMismatch));
        }
    }

    [Test]
    public void TestEmptyCommand()
    {
        // GET DEVICE_INFO has no Parameter Data
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(ERDM_Parameter.DEVICE_INFO, CommandType.GetRequest), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.DEVICE_INFO, CommandType.GetRequest, 0x00), Is.EqualTo(EPayloadValidation.LengthMismatch));
        }
    }

    [Test]
    public void TestList()
    {
        using (Assert.EnterMultipleScope())
        {
            // SUPPORTED_PARAMETERS: list of uint16
            Assert.That(validate(ERDM_Parameter.SUPPORTED_PARAMETERS, CommandType.GetResponse, 0x00, 0x60, 0x00, 0xF0), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.SUPPORTED_PARAMETERS, CommandType.GetResponse, 0x00, 0x60, 0x00), Is.EqualTo(EPayloadValidation.LengthMismatch), "incomplete item");

            // STATUS_MESSAGES: list of 9 byte status messages, empty if there is no message
            Assert.That(validate(ERDM_Parameter.STATUS_MESSAGES, CommandType.GetResponse), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.STATUS_MESSAGES, CommandType.GetResponse, 0x00, 0x00, 0x02, 0x00, 0x50, 0x00, 0x01, 0x00, 0x00), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.STATUS_MESSAGES, CommandType.GetResponse, 0x00), Is.EqualTo(EPayloadValidation.LengthMismatch), "a single byte is no status message");
            Assert.That(validate(ERDM_Parameter.STATUS_MESSAGES, CommandType.GetResponse, 0x00, 0x00, 0x02, 0x00, 0x50, 0x00, 0x01, 0x00, 0x00, 0x00), Is.EqualTo(EPayloadValidation.LengthMismatch), "incomplete second message");
        }
    }

    [Test]
    public void TestText()
    {
        byte[] text(int length) => Enumerable.Repeat((byte)'A', length).ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(ERDM_Parameter.DEVICE_LABEL, CommandType.GetResponse, text(0)), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.DEVICE_LABEL, CommandType.GetResponse, text(32)), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.DEVICE_LABEL, CommandType.GetResponse, text(33)), Is.EqualTo(EPayloadValidation.LengthMismatch), "more than 32 characters");

            // CURVE_DESCRIPTION: curve (uint8) + up to 32 characters
            Assert.That(validate(ERDM_Parameter.CURVE_DESCRIPTION, CommandType.GetResponse, new byte[] { 0x01 }.Concat(text(32)).ToArray()), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.CURVE_DESCRIPTION, CommandType.GetResponse, new byte[] { 0x01 }.Concat(text(33)).ToArray()), Is.EqualTo(EPayloadValidation.LengthMismatch));
        }
    }

    [Test]
    public void TestBoolean()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(ERDM_Parameter.FACTORY_DEFAULTS, CommandType.GetResponse, 0x00), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.FACTORY_DEFAULTS, CommandType.GetResponse, 0x01), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.FACTORY_DEFAULTS, CommandType.GetResponse, 0x02), Is.EqualTo(EPayloadValidation.InvalidBoolean));
            Assert.That(validate(ERDM_Parameter.FACTORY_DEFAULTS, CommandType.GetResponse, 0xFF), Is.EqualTo(EPayloadValidation.InvalidBoolean));

            // MINIMUM_LEVEL: increasing (uint16) + decreasing (uint16) + on below minimum (boolean)
            Assert.That(validate(ERDM_Parameter.MINIMUM_LEVEL, CommandType.GetResponse, 0x00, 0x10, 0x00, 0x20, 0x01), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.MINIMUM_LEVEL, CommandType.GetResponse, 0x00, 0x10, 0x00, 0x20, 0x02), Is.EqualTo(EPayloadValidation.InvalidBoolean));
        }
    }

    [Test]
    public void TestAdditionalLengthOfTheSpecification()
    {
        // E1.20 10.11.6: CAPTURE_PRESET has a PDL of 0x02 (Scene # only) or 0x08, the define only describes 0x08
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(ERDM_Parameter.CAPTURE_PRESET, CommandType.SetRequest, 0x00, 0x01), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.CAPTURE_PRESET, CommandType.SetRequest, 0x00, 0x01, 0x00, 0x0A, 0x00, 0x0A, 0x00, 0x00), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(ERDM_Parameter.CAPTURE_PRESET, CommandType.SetRequest, 0x00, 0x01, 0x00, 0x0A), Is.EqualTo(EPayloadValidation.LengthMismatch));
        }
    }

    [Test]
    public void TestReason()
    {
        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.CURVE));
        Assert.That(MetadataFactory.ValidatePayload(define, CommandType.GetResponse, new byte[] { 0x03 }, out string reason), Is.EqualTo(EPayloadValidation.LengthMismatch));
        Assert.That(reason, Is.Not.Null.And.Not.Empty);
    }
}
