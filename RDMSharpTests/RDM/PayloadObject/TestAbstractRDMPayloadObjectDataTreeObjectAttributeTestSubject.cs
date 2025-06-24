using RDMSharp.Metadata;
using System.Reflection;

namespace RDMSharpTests.RDM.PayloadObject.Attribute
{
    public class TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject
    {
        public static readonly object[] TestSubjects = getTestSubjects();
        private static object[] getTestSubjects()
        {
            var baseType = typeof(AbstractRDMPayloadObject);
            var assembly = Assembly.GetAssembly(baseType);

            var types = assembly.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

            List<object> result = new List<object>();
            foreach (var type in types)
            {
                if (type.GetCustomAttributes<DataTreeObjectAttribute>().Count() != 0)
                    result.Add(new TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject(type, type.Name));
            }
            return result.ToArray();

        }

        public readonly Type Type;
        public readonly string Name;
        public TestAbstractRDMPayloadObjectDataTreeObjectAttributeTestSubject(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public override string ToString() => Name;
    }
}