using RDMSharp.Metadata;
using System.Data;
using System.Reflection;
using System.Text;

namespace RDMSharpTests.Metadata.Parser;

public class TestDefinedDataTreeObjects
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }
    [Test]
    public void Test_AllDefinedDataTreeObjectsForValidility()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var type in MetadataFactory.DefinedDataTreeObjects)
        {
            if (type.IsEnum)
                continue;

            StringBuilder stringBuilder2 = new StringBuilder();

            var constructors = type.GetConstructors().Where(c => c.GetCustomAttributes<DataTreeObjectConstructorAttribute>().Count() != 0);
            if (constructors.Count() == 0)
                stringBuilder2.AppendLine($"{type} not defines a {nameof(DataTreeObjectConstructorAttribute)}");

            foreach (var constructor in constructors)
            {
                StringBuilder stringBuilder3 = new StringBuilder();
                var parameters = constructor.GetParameters();
                foreach (var para in parameters.Where(p => !p.GetCustomAttributes<DataTreeObjectParameterAttribute>().Any(a => a is DataTreeObjectParameterAttribute)))
                    stringBuilder3.AppendLine($"\t{para.Name}");
                if (stringBuilder3.Length > 0)
                {
                    stringBuilder2.AppendLine($"{type} Constructor not defines {nameof(DataTreeObjectParameterAttribute)} for the Parameters:");
                    stringBuilder2.AppendLine(stringBuilder3.ToString().TrimEnd());
                }
            }
            if (stringBuilder2.Length > 0)
                stringBuilder.AppendLine(stringBuilder2.ToString().Trim());
        }
        if (stringBuilder.Length > 0)
            Assert.Fail(stringBuilder.ToString().Trim());
    }
}