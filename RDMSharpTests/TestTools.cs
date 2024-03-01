using System.Collections.Concurrent;

namespace RDMSharpTest
{
    public class TestTools
    {
        [SetUp]
        public void Setup()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
        }

        [Test]
        public void TestStatusMessages()
        { 
            Dictionary<string, ERDM_StatusMessage> results = new Dictionary<string, ERDM_StatusMessage>();
            var enums = new List<ERDM_StatusMessage>();
            enums.AddRange(Enum.GetValues(typeof(ERDM_StatusMessage)).Cast<ERDM_StatusMessage>());
            enums.Add((ERDM_StatusMessage)0);
            foreach (ERDM_StatusMessage e in enums)
            {
                string str = Tools.GetStatusMessage(e);
                Console.WriteLine($"{e} => {str}");
                if (e==0)
                {
                    Assert.That(String.IsNullOrWhiteSpace(str), Is.True);
                    continue;
                }
                Assert.That(String.IsNullOrWhiteSpace(str), Is.False, e.ToString());
                Assert.That(str.EndsWith("."), Is.True, $"{e} => {str}");

                Assert.That(results.TryAdd(str, e), Is.True, $"{e} => {str}");
            }
        }
        [Test]
        public void TestSensorUnitSymbol()
        {
            Dictionary<string, ERDM_SensorUnit> results = new Dictionary<string, ERDM_SensorUnit>();
            var enums = Enum.GetValues(typeof(ERDM_SensorUnit));
            foreach (ERDM_SensorUnit e in enums)
            {
                string str = Tools.GetUnitSymbol(e);
                Console.WriteLine($"{e} => \'{str}\'");
                if (e == ERDM_SensorUnit.NONE)
                    Assert.That(String.IsNullOrWhiteSpace(str), Is.True, e.ToString());
                else
                    Assert.That(String.IsNullOrWhiteSpace(str), Is.False, e.ToString());

                Assert.That(results.TryAdd(str, e), Is.True, $"{e} => {str}");
            }
        }
        [Test]
        public void TestUnitPrefixGetNormalizedValue()
        {
            Dictionary<double, ERDM_UnitPrefix> results = new Dictionary<double, ERDM_UnitPrefix>();
            var enums = Enum.GetValues(typeof(ERDM_UnitPrefix));
            foreach (ERDM_UnitPrefix e in enums)
            {
                short val = 1;
                var ret = Tools.GetNormalizedValue(e, val);
                Console.WriteLine($"{e} => {ret}");
                if (e == ERDM_UnitPrefix.NONE)
                    Assert.That(val, Is.EqualTo(ret), e.ToString());
                else
                    Assert.That(val, Is.Not.EqualTo(ret), e.ToString());

                Assert.That(results.TryAdd(ret, e), Is.True, $"{e} => {ret}");
            }
        }
        [Test]
        public void TestDataToValueExceptions()
        {
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[0]; Tools.DataToBool(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[0]; Tools.DataToBoolArray(ref data, 8); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[0]; Tools.DataToByte(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[0]; Tools.DataToSByte(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[1]; Tools.DataToUShort(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[1]; Tools.DataToShort(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[3]; Tools.DataToUInt(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[3]; Tools.DataToInt(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[5]; Tools.DataToULong(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[5]; Tools.DataToLong(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[5]; Tools.DataToRDMUID(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[3]; Tools.DataToIPAddressIPv4(ref data); });
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[15]; Tools.DataToIPAddressIPv6(ref data); });
        }
    }
}