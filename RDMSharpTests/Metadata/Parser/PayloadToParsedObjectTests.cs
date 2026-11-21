using RDMSharp.Metadata;
using RDMSharp.PayloadObject;

namespace RDMSharpTests.Metadata.Parser;

[TestFixtureSource(typeof(PayloadToParsedObjectTestSubject), nameof(PayloadToParsedObjectTestSubject.TestSubjects))]
public class PayloadToParsedObjectTests
{
    private readonly PayloadToParsedObjectTestSubject testSubject;

    private DataTreeBranch dataTreeBranch;

    public PayloadToParsedObjectTests(PayloadToParsedObjectTestSubject _TestSubject)
    {
        testSubject = _TestSubject;
    }

    [Test]
    public void PayloadDataToObject()
    {
        Assert.DoesNotThrow(() =>
        {
            dataTreeBranch = MetadataFactory.ParseDataToPayload(testSubject.Define, testSubject.PayloadToParseBag.CommandDublicate, testSubject.PayloadToParseBag.Payload);
        });


    }
    [Test, DependsOnTest(nameof(PayloadDataToObject))]
    public void TestPayloadAction()
    {
        testSubject.PayloadToParseBag.TestPayload.Invoke(dataTreeBranch);
    }

    [Test, DependsOnTest(nameof(TestPayloadAction))]
    public void ToPayloadDataTest()
    {
        if (dataTreeBranch.ParsedObject is IRDMPayloadObject obj)
            Assert.That(obj.ToPayloadData(), Is.EqualTo(testSubject.PayloadToParseBag.RoundTripPayload));
    }
    [Test, DependsOnTest(nameof(ToPayloadDataTest))]
    public void ToStringTest()
    {
        if (dataTreeBranch.ParsedObject is IRDMPayloadObject obj)
            Assert.That(obj.ToString(), Is.Not.Null);
    }
    /// <summary>
    /// Every Payload of these test subjects is valid, the validation before parsing must not reject it.
    /// </summary>
    [Test]
    public void PayloadIsValid()
    {
        var result = MetadataFactory.ValidatePayload(testSubject.Define, testSubject.PayloadToParseBag.CommandDublicate, testSubject.PayloadToParseBag.Payload, out string reason);
        Assert.That(result, Is.EqualTo(EPayloadValidation.Valid), reason);
    }

    [Test, DependsOnTest(nameof(ToStringTest))]
    public void Reverse()
    {
        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, new ParameterBag(testSubject.PayloadToParseBag.Parameter), testSubject.PayloadToParseBag.Command);
        // a not normalized Payload (e.g. a NUL padded String) can not be reproduced, it is compared with the normalized Payload instead
        var expected = testSubject.PayloadToParseBag.NormalizedPayload is byte[] normalized
            ? MetadataFactory.ParseDataToPayload(testSubject.Define, testSubject.PayloadToParseBag.CommandDublicate, normalized)
            : dataTreeBranch;
        Assert.That(reversed, Is.EqualTo(expected));
    }
}