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
            Assert.That(obj.ToPayloadData(), Is.EqualTo(testSubject.PayloadToParseBag.Payload));
    }
    [Test, DependsOnTest(nameof(ToPayloadDataTest))]
    public void ToStringTest()
    {
        if (dataTreeBranch.ParsedObject is IRDMPayloadObject obj)
            Assert.That(obj.ToString(), Is.Not.Null);
    }
    [Test, DependsOnTest(nameof(ToStringTest))]
    public void Reverse()
    {
        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, new ParameterBag(testSubject.PayloadToParseBag.Parameter), testSubject.PayloadToParseBag.Command);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
    }
}