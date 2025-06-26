using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata
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

        //[Test]
        //public void TestMetadataFactory()
        //{
        //    var schemas = MetadataFactory.GetMetadataSchemaVersions();
        //    Assert.That(schemas, Has.Count.EqualTo(1));
        //    var defines = MetadataFactory.GetMetadataDefineVersions();
        //    Assert.That(defines, Has.Count.EqualTo(122));
        //    foreach (var define in defines)
        //        testString(define.ToString());
        //}
        [Test]
        public void TestMetadataFactoryESTAParameterHaseDefine()
        {
            var parameters = Enum.GetValues<ERDM_Parameter>().Where(p => (ushort)p >= 30).ToList().AsReadOnly();
            Assert.Multiple(() =>
            {
                foreach (var parameter in parameters)
                {
                    switch (parameter)
                    {
                        case ERDM_Parameter.COMPONENT_SCOPE:
                        case ERDM_Parameter.SEARCH_DOMAIN:
                        case ERDM_Parameter.TCP_COMMS_STATUS:
                        case ERDM_Parameter.BROKER_STATUS:
                            Assert.Warn("Implement E1.33 Defines!!!");
                            continue;
                    }
                    var define = MetadataFactory.GetDefine(new ParameterBag(parameter));
                    Assert.That(define, Is.Not.Null, $"Parameter: {parameter} has no matchig Define");
                }
            });
        }

        [Test]
        public void TestMetadataVersion()
        {
            var mv = new MetadataVersion("RDMSharp.Resources.JSON_Defines._1._0._0.Defines.e1._20.BOOT_SOFTWARE_VERSION_ID.json", typeof(MetadataFactory).Assembly);
            testString(mv.ToString());
            Assert.Multiple(() =>
            {
                Assert.Throws(typeof(ArgumentNullException), () => MetadataVersion.getVersion(null));
                Assert.Throws(typeof(ArgumentNullException), () => MetadataVersion.getName(null));
                Assert.Throws(typeof(ArgumentNullException), () => MetadataVersion.getIsSchema(null));

                Assert.Throws(typeof(FormatException), () => MetadataVersion.getVersion("RDMSharp.Resources.JSON_Defines.1.0.0.Defines.e1._20.BOOT_SOFTWARE_VERSION_ID.json"));
                Assert.Throws(typeof(FormatException), () => MetadataVersion.getVersion("RDMSharp.Resources.JSON_Defines._1._X._0.Defines.e1._20.BOOT_SOFTWARE_VERSION_ID.json"));
                Assert.Throws(typeof(ArgumentException), () => MetadataVersion.getVersion("RDMSharp.Resources.JSON_Defines._1._0._0.Defines.e1._20.BOOT_SOFTWARE_VERSION_ID.jsan"));
                Assert.Throws(typeof(FormatException), () => MetadataVersion.getName("RDMSharp.Resources.JSON_Defines._1._0._0.Defines.e1._20.BOOT_SOFTWARE_VERSION_ID.jsan"));
                Assert.Throws(typeof(ArgumentException), () => MetadataVersion.getIsSchema("RDMSharp.Resources.JSON_Defines._1._0._0.Defines.e1._20.BOOT_SOFTWARE_VERSION_ID.jsan"));
            });
        }

        [Test]
        public void TestMetadataBag()
        {
            var bag = new MetadataBag("1.0.2", "NAME.json", false, "content", "Path");
            testString(bag.ToString());
            Assert.Throws(typeof(ArgumentNullException), () => MetadataBag.getContent(null, typeof(MetadataFactory).Assembly));
        }
        static void testString(string str)
        {
            Assert.That(str, Is.Not.WhiteSpace);
            Assert.That(str, Is.Not.Empty);
            Assert.That(str, Does.Not.Contain("{"));
            Assert.That(str, Does.Not.Contain("}"));
        }
    }
}