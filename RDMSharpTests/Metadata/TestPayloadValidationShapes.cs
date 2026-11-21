using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Text.Json;
using CommandType = RDMSharp.Metadata.JSON.Command.ECommandDublicate;

namespace RDMSharpTests.Metadata;

/// <summary>
/// <see cref="MetadataFactory.ValidatePayload"/> for shapes the built-in defines do not have (compound, list of compounds with a boolean,
/// layouts that can not be checked). The defines are invented (manufacturer 0x7FF0 is the ESTA prototyping range).
/// </summary>
public class TestPayloadValidationShapes
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    /// <summary>A define with the given fields as the response of a GET.</summary>
    private static MetadataJSONObjectDefine define(string getResponseFields)
    {
        string json = $$"""
        {
          "name": "FICTIONAL_SHAPE",
          "manufacturer_id": 32752,
          "pid": 37700,
          "version": 1,
          "get_request_subdevice_range": [ "root" ],
          "get_request": [],
          "get_response": {{getResponseFields}}
        }
        """;
        return JsonSerializer.Deserialize<MetadataJSONObjectDefine>(json)!;
    }

    private static EPayloadValidation validate(MetadataJSONObjectDefine define, out string reason, params byte[] data)
        => MetadataFactory.ValidatePayload(define, CommandType.GetResponse, data, out reason);

    [Test]
    public void TestListOfCompoundsWithBoolean()
    {
        var define = TestPayloadValidationShapes.define("""
            [ { "name": "entries", "type": "list", "itemType": { "type": "compound", "subtypes": [
                { "name": "id", "type": "uint8" },
                { "name": "enabled", "type": "boolean" } ] } } ]
            """);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(define, out _), Is.EqualTo(EPayloadValidation.Valid), "an empty list");
            Assert.That(validate(define, out _, 1, 0), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out _, 1, 0, 2, 1), Is.EqualTo(EPayloadValidation.Valid));

            Assert.That(validate(define, out var reason, 1, 0, 2), Is.EqualTo(EPayloadValidation.LengthMismatch), "the boolean of the last entry is missing");
            Assert.That(reason, Does.Contain("enabled"));
            Assert.That(reason, Does.Contain("missing"));

            Assert.That(validate(define, out reason, 1, 2), Is.EqualTo(EPayloadValidation.InvalidBoolean));
            Assert.That(reason, Does.Contain("enabled"));
            Assert.That(validate(define, out _, 1, 0, 2, 5), Is.EqualTo(EPayloadValidation.InvalidBoolean), "an invalid boolean in a later entry");
        }
    }

    [Test]
    public void TestCompound()
    {
        var define = TestPayloadValidationShapes.define("""
            [ { "name": "settings", "type": "compound", "subtypes": [
                { "name": "mode", "type": "uint8" },
                { "name": "enabled", "type": "boolean" },
                { "name": "level", "type": "uint16" } ] } ]
            """);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(define, out _, 1, 1, 0, 5), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out _, 1, 0, 0, 5), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out _, 1, 2, 0, 5), Is.EqualTo(EPayloadValidation.InvalidBoolean), "the boolean inside of the compound");
            Assert.That(validate(define, out _, 1, 1, 0), Is.EqualTo(EPayloadValidation.LengthMismatch), "too short");
            Assert.That(validate(define, out _, 1, 1, 0, 5, 9), Is.EqualTo(EPayloadValidation.LengthMismatch), "too long");
        }
    }

    /// <summary>
    /// The number of items of a list is limited by the PDL of the define (min/max items x size of an item).
    /// </summary>
    [Test]
    public void TestListLimitsAreCheckedByThePdl()
    {
        var define = TestPayloadValidationShapes.define("""
            [ { "name": "levels", "type": "list", "minItems": 1, "maxItems": 3, "itemType": { "type": "uint8" } } ]
            """);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(define, out _), Is.EqualTo(EPayloadValidation.LengthMismatch), "less than the minimum");
            Assert.That(validate(define, out _, 1), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out _, 1, 2, 3), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out var reason, 1, 2, 3, 4), Is.EqualTo(EPayloadValidation.LengthMismatch), "more than the maximum");
            Assert.That(reason, Does.Contain("PDL 4"));
        }
    }

    /// <summary>
    /// The length of a text is limited by the PDL of the define.
    /// </summary>
    [Test]
    public void TestStringLengthIsCheckedByThePdl()
    {
        var define = TestPayloadValidationShapes.define("""
            [ { "name": "label", "type": "string", "minLength": 2, "maxLength": 4 } ]
            """);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(define, out _, 0x41), Is.EqualTo(EPayloadValidation.LengthMismatch), "too short");
            Assert.That(validate(define, out _, 0x41, 0x42), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out _, 0x41, 0x42, 0x43, 0x44), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(validate(define, out _, 0x41, 0x42, 0x43, 0x44, 0x45), Is.EqualTo(EPayloadValidation.LengthMismatch), "too long");
        }
    }

    /// <summary>
    /// A variable field in front of other fields can not be assigned to bytes, only the PDL of the define is checked,
    /// the Payload is not rejected because of a layout that can not be checked.
    /// </summary>
    [Test]
    public void TestVariableFieldNotAtTheEndCanNotBeChecked()
    {
        var textFirst = TestPayloadValidationShapes.define("""
            [ { "name": "label", "type": "string", "maxLength": 8 },
              { "name": "level", "type": "uint8" } ]
            """);
        var listFirst = TestPayloadValidationShapes.define("""
            [ { "name": "levels", "type": "list", "itemType": { "type": "uint8" } },
              { "name": "level", "type": "uint8" } ]
            """);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validate(textFirst, out var reason, 0x41, 0x42, 0x07), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(reason, Is.Null);
            Assert.That(validate(textFirst, out _, 0x07), Is.EqualTo(EPayloadValidation.Valid), "an empty text");
            Assert.That(validate(textFirst, out _, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10), Is.EqualTo(EPayloadValidation.LengthMismatch), "the PDL is still checked");

            Assert.That(validate(listFirst, out reason, 1, 2, 3), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(reason, Is.Null);
        }
    }
}
