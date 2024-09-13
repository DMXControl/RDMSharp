using RDMSharp.Metadata;

namespace RDMSharpTests
{
    public class TestJSONMetadataDefines
    {
        [SetUp]
        public void Setup()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
        }
        [TearDown]
        public void Teardown()
        {
            Console.OutputEncoding = System.Text.Encoding.Default;
        }

        [Test]
        public void TestJSONDefinesResources()
        {
            string[] resources = JSONDefinesResources.GetResources();
            Assert.That(resources, Is.Not.Empty);
            Assert.That(resources, Has.Length.EqualTo(130));
            string[] schemas = resources.Where(r => r.EndsWith("schema.json")).ToArray();
            Assert.That(schemas, Has.Length.EqualTo(1));
        }
        [Test]
        public void TestMetadataFactory()
        {
            var schemas = MetadataFactory.GetMetadataSchemaVersions();
            Assert.That(schemas, Has.Count.EqualTo(1));
            var defines = MetadataFactory.GetMetadataDefineVersions();
            Assert.That(defines, Has.Count.EqualTo(129));
        }
    }
}