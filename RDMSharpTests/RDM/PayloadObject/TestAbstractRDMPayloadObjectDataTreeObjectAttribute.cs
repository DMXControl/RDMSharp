using Newtonsoft.Json.Linq;
using RDMSharp.Metadata;
using System.Reflection;

namespace RDMSharpTests.RDM.PayloadObject.Attribute
{
    [TestFixtureSource(typeof(TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject), nameof(TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject.TestSubjects))]
    public class TestAbstractRDMPayloadObjectDataTreeObjectAttribute
    {
        private readonly TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject testSubject;

        public TestAbstractRDMPayloadObjectDataTreeObjectAttribute(TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject _TestSubject)
        {
            testSubject = _TestSubject;
        }
        [Test]
        public void TestDataTreeObjectParameter_And_DataTreeObjectProperty()
        {
            Assert.That(testSubject.Type.GetCustomAttributes<DataTreeObjectAttribute>().ToArray(), Has.Length.AtLeast(1));

            List<DataTreeObjectPropertyAttribute> propertyAttributes = new List<DataTreeObjectPropertyAttribute>();
            List<DataTreeObjectParameterAttribute> parameterAttributes = new List<DataTreeObjectParameterAttribute>();
            var properties = testSubject.Type.GetProperties().Where(p => p.GetCustomAttributes<DataTreeObjectPropertyAttribute>().Count() != 0).ToArray();
            foreach (var prop in properties)
                if (prop.GetCustomAttributes<DataTreeObjectPropertyAttribute>() is IEnumerable<DataTreeObjectPropertyAttribute> pAttributes)
                    propertyAttributes.AddRange(pAttributes);

            foreach (var constructor in testSubject.Type.GetConstructors())
                foreach (var param in constructor.GetParameters())
                {
                    if (param.GetCustomAttributes<DataTreeObjectParameterAttribute>() is IEnumerable<DataTreeObjectParameterAttribute> pAttributes)
                        parameterAttributes.AddRange(pAttributes);
                }
            Assert.That(parameterAttributes, Has.Count.EqualTo(propertyAttributes.Count));

            foreach(var para in parameterAttributes)
            {
                var prop = propertyAttributes.FirstOrDefault(prop=>string.Equals(prop.Name, para.Name));
                Assert.That(prop, Is.Not.Null, $"No Property found using{nameof(DataTreeObjectPropertyAttribute)} with {nameof(DataTreeObjectPropertyAttribute.Name)}: {para.Name}");
            }
            foreach (var prop in propertyAttributes)
            {
                var para = parameterAttributes.FirstOrDefault(para => string.Equals(prop.Name, para.Name));
                Assert.That(para, Is.Not.Null, $"No Parameter found using{nameof(DataTreeObjectParameterAttribute)} with {nameof(DataTreeObjectParameterAttribute.Name)}: {prop.Name}");
            }
            foreach (var prop in propertyAttributes)
            {
                var para = parameterAttributes.FirstOrDefault(para => string.Equals(prop.Name, para.Name));
                Assert.That(para, Is.Not.Null, $"No Parameter found using{nameof(DataTreeObjectParameterAttribute)} with {nameof(DataTreeObjectParameterAttribute.Name)}: {prop.Name}");
            }
            foreach (var item in propertyAttributes.Where(p => !p.Name.Contains('/')).GroupBy(p => p.Parameter))
            {
                foreach (var item1 in item)
                    Assert.That(item.Where(i => i.Index == item1.Index).ToList(), Has.Count.EqualTo(1));
            }
        }
    }
}