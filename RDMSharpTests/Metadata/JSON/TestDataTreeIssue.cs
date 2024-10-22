using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestDataTreeIssue
    {
        [Test]
        public void TestMany()
        {
            Assert.Multiple(() =>
            {
                var issue = new DataTreeIssue();
                Assert.That(issue.ToString(), Is.Null);
                issue = new DataTreeIssue("Test");
                Assert.That(issue.ToString(), Is.EqualTo("Test"));
                Assert.Throws(typeof(ArgumentNullException), () => new DataTreeIssue(""));
                Assert.Throws(typeof(ArgumentNullException), () => new DataTreeIssue(null));
            });
        }
    }
}