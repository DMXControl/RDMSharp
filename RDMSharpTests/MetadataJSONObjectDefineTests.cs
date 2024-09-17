using Json.Schema;
using RDMSharp.Metadata;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace RDMSharpTests
{
    [TestFixtureSource(typeof(MetadataJSONObjectDefineTestSubject), nameof(MetadataJSONObjectDefineTestSubject.TestSubjects))]
    public class MetadataJSONObjectDefineTests
    {
        private readonly MetadataJSONObjectDefineTestSubject testSubject;

        public MetadataJSONObjectDefineTests(MetadataJSONObjectDefineTestSubject _TestSubject)
        {
            testSubject = _TestSubject;
        }


        [Test]
        public void TestValidateAgainstSchema()
        {
            JsonSchema jsonSchema = JsonSchema.FromText(testSubject.Schema.Content);
            var result = jsonSchema.Evaluate(JsonNode.Parse(testSubject.Define.Content));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.True);
        }
        [Test]
        public void TestDeseriaizeAndSerialize()
        {
            MetadataJSONObjectDefine deserialized = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(testSubject.Define.Content);
            Assert.That(deserialized.Version, Is.AtLeast(1));
            Assert.That(deserialized.Name, Is.Not.WhiteSpace);
            Assert.That(deserialized.Name, Is.Not.Empty);
            string serialized = JsonSerializer.Serialize(deserialized);

            var original = JToken.Parse(PrittyJSON(testSubject.Define.Content));
            var smashed = JToken.Parse(PrittyJSON(serialized));

            Assert.That(JToken.DeepEquals(smashed, original));


           Warn.Unless(PrittyJSON(serialized),Is.EqualTo(PrittyJSON(testSubject.Define.Content)));
        }

        private static string PrittyJSON(string jsonString)
        {
            var jsonObject = JsonSerializer.Deserialize<object>(jsonString);

            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
            };

            string formattedJson = JsonSerializer.Serialize(jsonObject, options);
            return formattedJson;
        }
    }
}