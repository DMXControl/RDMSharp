using RDMSharp.Metadata;

namespace RDMSharpTests
{
    public class TestMetadataFactoryStuff
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
        public void TestMetadataFactory()
        {
            var schemas = MetadataFactory.GetMetadataSchemaVersions();
            Assert.That(schemas, Has.Count.EqualTo(1));
            var defines = MetadataFactory.GetMetadataDefineVersions();
            Assert.That(defines, Has.Count.EqualTo(129));
        }
    }
}