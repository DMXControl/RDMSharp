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
            Assert.That(defines, Has.Count.EqualTo(122));
            foreach (var define in defines)
                testString(define.ToString());
        }
        void testString(string str)
        {
            Assert.That(str, Is.Not.WhiteSpace);
            Assert.That(str, Is.Not.Empty);
            Assert.That(str, Does.Not.Contain("{"));
            Assert.That(str, Does.Not.Contain("}"));
        }
    }
}