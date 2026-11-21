using RDMSharp.Metadata;
using CommandType = RDMSharp.Metadata.JSON.Command.ECommandDublicate;

namespace RDMSharpTests.Metadata;

/// <summary>
/// The <see cref="DataTreeBranch"/>: building it from a define, comparing it, and what it does if a Payload can not be turned into an object.
/// The defines are invented (manufacturer 0x7FF0 is the ESTA prototyping range).
/// </summary>
public class TestDataTreeBranch
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    private static DataTreeBranch branch(params object[] values)
        => new DataTreeBranch(values.Select((v, i) => new DataTree("value" + i, (uint)i, v)).ToArray());

    #region Building
    [Test]
    public void TestDefineIsRequired()
    {
        Assert.Throws<ArgumentNullException>(() => new DataTreeBranch(null!, CommandType.GetResponse));
    }

    [Test]
    public void TestWithoutChildrenTheBranchIsEmpty()
    {
        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.DEVICE_INFO));
        var empty = new DataTreeBranch(define, CommandType.GetRequest);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(empty.IsEmpty, Is.True);
            Assert.That(empty.IsUnset, Is.False);
            Assert.That(empty.ParsedObject, Is.Null, "there is nothing to parse");
        }
    }

    [Test]
    public void TestChildrenThatDoNotFitTheDefineHaveNoParsedObject()
    {
        // DEVICE_INFO needs its fields, one unknown value can not be turned into the object: no exception, but no object
        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.DEVICE_INFO));
        var faulty = new DataTreeBranch(define, CommandType.GetResponse, new DataTree("unexpected", 0, 1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(faulty.IsEmpty, Is.False);
            Assert.That(faulty.ParsedObject, Is.Null);
            Assert.That(faulty.Children, Has.Length.EqualTo(1));
        }
    }

    [Test]
    public void TestListOfValuesIsAnArray()
    {
        var define = TestPayloadValidationShapes.define("""
            [ { "name": "levels", "type": "list", "itemType": { "type": "uint16" } } ]
            """);

        var branch = MetadataFactory.ParseDataToPayload(define, CommandType.GetResponse, new byte[] { 0x00, 0x01, 0x00, 0x02 });
        Assert.That(branch.ParsedObject, Is.EqualTo(new ushort[] { 1, 2 }));
    }

    [Test]
    public void TestEmptyListHasNoParsedObject()
    {
        var define = TestPayloadValidationShapes.define("""
            [ { "name": "levels", "type": "list", "itemType": { "type": "uint16" } } ]
            """);

        var branch = MetadataFactory.ParseDataToPayload(define, CommandType.GetResponse, Array.Empty<byte>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(branch.IsUnset, Is.False);
            Assert.That(branch.ParsedObject, Is.Null, "an empty list is not turned into an array");
            Assert.That(branch.Children, Has.Length.EqualTo(1));
            Assert.That(branch.Children[0].Children, Is.Empty);
        }
    }
    #endregion

    #region Comparing
    [Test]
    public void TestEqualBranches()
    {
        var a = branch((byte)1, "text");
        var b = branch((byte)1, "text");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.Equals((object)b), Is.True);
            Assert.That(DataTreeBranch.Unset == DataTreeBranch.Unset, Is.True);
            Assert.That(DataTreeBranch.Empty == DataTreeBranch.Empty, Is.True);
        }
    }

    [Test]
    public void TestDifferentBranches()
    {
        var a = branch((byte)1, "text");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a != branch((byte)2, "text"), Is.True, "another value");
            Assert.That(a != branch((byte)1), Is.True, "the other one has less children");
            Assert.That(branch((byte)1) != a, Is.True, "this one has less children");
            Assert.That(a != DataTreeBranch.Unset, Is.True);
            Assert.That(DataTreeBranch.Unset != DataTreeBranch.Empty, Is.True);
            Assert.That(a != default(DataTreeBranch), Is.True, "a branch without a list of children");
            Assert.That(default(DataTreeBranch) != a, Is.True);
            Assert.That(a.Equals("not a branch"), Is.False);
            Assert.That(a.Equals((object?)null), Is.False);
        }
    }

    [Test]
    public void TestEqualBranchesHaveTheSameHashCode()
    {
        var a = branch((byte)1, "text");
        var b = branch((byte)1, "text");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()), "equal objects need equal hash codes");
            Assert.That(new HashSet<DataTreeBranch> { a }, Does.Contain(b), "so they can be found in a set");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(branch((byte)2, "text").GetHashCode()));
            Assert.That(DataTreeBranch.Unset.GetHashCode(), Is.Not.EqualTo(DataTreeBranch.Empty.GetHashCode()));
            Assert.That(default(DataTreeBranch).GetHashCode(), Is.EqualTo(default(DataTreeBranch).GetHashCode()), "a branch without children");
        }
    }

    [Test]
    public void TestEqualDataTreesHaveTheSameHashCode()
    {
        var a = new DataTree("level", 0, (byte)5);
        var b = new DataTree("level", 0, (byte)5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
            Assert.That(new HashSet<DataTree> { a }, Does.Contain(b));
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(new DataTree("level", 1, (byte)5).GetHashCode()));
        }
    }
    #endregion
}
