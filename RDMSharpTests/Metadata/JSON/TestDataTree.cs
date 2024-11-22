using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestDataTree
    {
        [Test]
        public void TestMany()
        {
            Assert.Multiple(() =>
            {
                var dataTree = new DataTree();
                Assert.That(dataTree.ToString(), Is.Not.Null);
                Assert.Throws(typeof(ArgumentException), () => new DataTree("name", 1, value: new DataTree[0].ToList()));

                Assert.That(new DataTree() == new DataTree(), Is.True);
                Assert.That(new DataTree() != new DataTree(), Is.False);
                Assert.That(new DataTree().Equals(new DataTree()), Is.True);
                Assert.That(((object)new DataTree()).Equals(new DataTree()), Is.True);
                Assert.That(((object)new DataTree()).Equals((object)new DataTree()), Is.True);
                Assert.That(((object)new DataTree()).Equals(1), Is.False);

                Assert.That(new DataTree("Test", 3, true) == new DataTree("Test", 3, true), Is.True);
                Assert.That(new DataTree("Test", 3, true) != new DataTree("Test", 3, true), Is.False);
                Assert.That(new DataTree("Test", 3, true).Equals(new DataTree("Test", 3, true)), Is.True);

                Assert.That(new DataTree("Test", 3, true) == new DataTree("Test", 31, true), Is.False);
                Assert.That(new DataTree("Test", 3, true) != new DataTree("Test", 31, true), Is.True);
                Assert.That(new DataTree("Test", 3, true).Equals(new DataTree("Test", 31, true)), Is.False);
                Assert.That(new DataTree("Test", 3, true) == new DataTree("Test", 3, false), Is.False);
                Assert.That(new DataTree("Test", 3, true) != new DataTree("Test", 3, false), Is.True);
                Assert.That(new DataTree("Test", 3, true).Equals(new DataTree("Test", 3, false)), Is.False);
                Assert.That(new DataTree("Test", 3, true) == new DataTree("Test2", 3, true), Is.False);
                Assert.That(new DataTree("Test", 3, true) != new DataTree("Test3", 3, true), Is.True);
                Assert.That(new DataTree("Test", 3, true).Equals(new DataTree("Test4", 3, true)), Is.False);
                Assert.That(new DataTree("Test", 3, true) == new DataTree("Test", 3, null), Is.False);
                Assert.That(new DataTree("Test", 3, true) != new DataTree("Test", 3, null), Is.True);
                Assert.That(new DataTree("Test", 3, true).Equals(new DataTree("Test", 3, null)), Is.False);
            });

            HashSet<DataTree> dict1 = new HashSet<DataTree>();
            HashSet<DataTree> dict2 = new HashSet<DataTree>();
            HashSet<DataTree> dict3 = new HashSet<DataTree>();

            dict1.Add(new DataTree());
            dict2.Add(new DataTree());
            dict3.Add(new DataTree());

            dict1.Add(new DataTree("Test", 1, false));
            dict2.Add(new DataTree("Test", 1, false));
            dict3.Add(new DataTree("Test", 1, true));

            dict1.Add(new DataTree("Test2", 2, 33));
            dict2.Add(new DataTree("Test2", 2, 33));
            dict3.Add(new DataTree("Test2", 2, 33));

            Assert.Multiple(() =>
            {
                Assert.That(new DataTree("Test", 3, children: dict1.ToArray()) == new DataTree("Test", 3, children: dict2.ToArray()), Is.True);
                Assert.That(new DataTree("Test", 3, children: dict1.ToArray()) != new DataTree("Test", 3, children: dict2.ToArray()), Is.False);
                Assert.That(new DataTree("Test", 3, children: dict1.ToArray()).Equals(new DataTree("Test", 3, children: dict2.ToArray())), Is.True);

                Assert.That(new DataTree("Test", 3, children: null) == new DataTree("Test", 3, children: dict3.ToArray()), Is.False);
                Assert.That(new DataTree("Test", 3, children: dict1.ToArray()) == new DataTree("Test", 3, children: dict3.ToArray()), Is.False);
                Assert.That(new DataTree("Test", 3, children: dict1.ToArray()) != new DataTree("Test", 3, children: dict3.ToArray()), Is.True);
                Assert.That(new DataTree("Test", 3, children: dict1.ToArray()).Equals(new DataTree("Test", 3, children: dict3.ToArray())), Is.False);
            });

            DataTreeIssue[] issues1 = new DataTreeIssue[] { new DataTreeIssue("Test Issue") };
            DataTreeIssue[] issues2 = new DataTreeIssue[] { new DataTreeIssue("Test Issue"), new DataTreeIssue("Test Issue2") };

            Assert.Multiple(() =>
            {
                Assert.That(new DataTree("Test", 3, true, issues1) == new DataTree("Test", 3, true, issues1.ToList().ToArray()), Is.True);
                Assert.That(new DataTree("Test", 3, true, issues1) != new DataTree("Test", 3, true, issues1.ToList().ToArray()), Is.False);
                Assert.That(new DataTree("Test", 3, true, issues1).Equals(new DataTree("Test", 3, true, issues1.ToList().ToArray())), Is.True);

                Assert.That(new DataTree("Test", 3, true) == new DataTree("Test", 3, true, issues2), Is.False);
                Assert.That(new DataTree("Test", 3, true, issues1) == new DataTree("Test", 3, true, issues2), Is.False);
                Assert.That(new DataTree("Test", 3, true, issues1) != new DataTree("Test", 3, true, issues2), Is.True);
                Assert.That(new DataTree("Test", 3, true, issues1).Equals(new DataTree("Test", 3, true, issues2)), Is.False);
            });
        }
    }
}