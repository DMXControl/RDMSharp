using Json.Schema;
using RDMSharp.Metadata;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using RDMSharp.Metadata.JSON;
using NUnit.Framework.Internal.Commands;
using RDMSharp.Metadata.OneOfTypes;

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
            try
            {
                MetadataJSONObjectDefine deserialized = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(testSubject.Define.Content);
                Assert.That(deserialized.Version, Is.AtLeast(1));
                Assert.That(deserialized.Name, Is.Not.WhiteSpace);
                Assert.That(deserialized.Name, Is.Not.Empty);
                string serialized = JsonSerializer.Serialize(deserialized);

                var original = JToken.Parse(PrittyJSON(testSubject.Define.Content));
                var smashed = JToken.Parse(PrittyJSON(serialized));

                Assert.Multiple(() =>
                {
                    Assert.That(JToken.DeepEquals(smashed, original));


                    Warn.Unless(PrittyJSON(serialized), Is.EqualTo(PrittyJSON(testSubject.Define.Content)));
                });
            }
            catch (JsonException ex)
            {
#if !NET7_0_OR_GREATER
                Warn.If(ex.Message, Is.EqualTo("Unexpected JSON format Type: int128 for FieldContainer.").Or.EqualTo("Unexpected JSON format Type: uint128 for FieldContainer."), "Due to .NET6 limitations");
                return;
#else
                throw;
#endif
            }
        }
        [Test]
        public void TestDeseriaizedObject()
        {
            MetadataJSONObjectDefine deserialized = new MetadataJSONObjectDefine();
            testString(testSubject.Define.ToString());
            try
            {
                deserialized = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(testSubject.Define.Content);
            }
            catch (JsonException ex)
            {
#if !NET7_0_OR_GREATER
                Warn.If(ex.Message, Is.EqualTo("Unexpected JSON format Type: int128 for FieldContainer.").Or.EqualTo("Unexpected JSON format Type: uint128 for FieldContainer."), "Due to .NET6 limitations");
                return;
#else
                throw;
#endif
            }
            Assert.That(deserialized.Version, Is.AtLeast(1));
            Assert.That(deserialized.Name, Is.Not.WhiteSpace);
            Assert.That(deserialized.Name, Is.Not.Empty);

            testString(deserialized.ToString());

            if (deserialized.GetRequestSubdeviceRange != null)
            {
                testString(string.Join("; ", deserialized.GetRequestSubdeviceRange.Select(r => r.ToString()))!);
            }
            if (deserialized.GetResponseSubdeviceRange != null)
            {
                testString(string.Join("; ", deserialized.GetResponseSubdeviceRange.Select(r => r.ToString()))!);
            }
            if (deserialized.SetRequestsSubdeviceRange != null)
            {
                testString(string.Join("; ", deserialized.SetRequestsSubdeviceRange.Select(r => r.ToString()))!);
            }
            if (deserialized.SetResponseSubdeviceRange != null)
            {
                testString(string.Join("; ", deserialized.SetResponseSubdeviceRange.Select(r => r.ToString()))!);
            }

            if (deserialized.GetRequest != null)
                testCommand(deserialized.GetRequest.Value);

            if (deserialized.GetResponse != null)
                testCommand(deserialized.GetResponse.Value);

            if (deserialized.SetRequest != null)
                testCommand(deserialized.SetRequest.Value);

            if (deserialized.SetResponse != null)
                testCommand(deserialized.SetResponse.Value);


            static void testString(string str)
            {
                Assert.That(str, Is.Not.WhiteSpace);
                Assert.That(str, Is.Not.Empty);
                Assert.That(str, Does.Not.Contain("{"));
                Assert.That(str, Does.Not.Contain("}"));
            }
            static void testCommand(Command command)
            {
                testString(command.ToString()!);
                if(command.EnumValue is Command.ECommandDublicte _enum)
                {
                    Assert.That(command.GetIsEmpty(), Is.False);
                    testString(_enum.ToString()!);
                    return;
                }
                else if (command.SingleField is OneOfTypes singleField)
                {
                    Assert.That(command.GetIsEmpty(), Is.False);
                    testString(singleField.ToString()!);
                    if (singleField.ObjectType is CommonPropertiesForNamed common)
                        testCommon(common);
                    return;
                }
                else if (command.ListOfFields is OneOfTypes[] listOfFields)
                {
                    if (listOfFields.Length != 0)
                    {
                        Assert.That(command.GetIsEmpty(), Is.False);
                        testString(string.Join("; ", listOfFields.Select(r => r.ToString()))!);
                        foreach (var field in listOfFields)
                        {
                            if (field.ObjectType is CommonPropertiesForNamed common)
                                testCommon(common);
                        }
                        return;
                    }
                }
                Assert.That(command.GetIsEmpty(), Is.True);
            }
            static void testCommon(CommonPropertiesForNamed common)
            {
                testString(common.ToString()!);
                if (common is IntegerType<byte> integerByte)
                    testIntegerType(integerByte);
                else if (common is IntegerType<sbyte> integerSByte)
                    testIntegerType(integerSByte);
                else if (common is IntegerType<short> integerShort)
                    testIntegerType(integerShort);
                else if (common is IntegerType<ushort> integerUShort)
                    testIntegerType(integerUShort);
                else if (common is IntegerType<int> integerInt)
                    testIntegerType(integerInt);
                else if (common is IntegerType<uint> integerUInt)
                    testIntegerType(integerUInt);
                else if (common is IntegerType<long> integerLong)
                    testIntegerType(integerLong);
                else if (common is IntegerType<ulong> integerULong)
                    testIntegerType(integerULong);
#if NET7_0_OR_GREATER
                else if (common is IntegerType<Int128> integerInt128)
                    testIntegerType(integerInt128);
                else if (common is IntegerType<UInt128> integerUInt128)
                    testIntegerType(integerUInt128);
#endif
            }
            static void testIntegerType<T>(IntegerType<T> integerType)
            {
                testString(integerType.ToString()!);
                if (integerType.Ranges != null)
                    foreach (Range<T> range in integerType.Ranges)
                        testString(range.ToString()!);
            }
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