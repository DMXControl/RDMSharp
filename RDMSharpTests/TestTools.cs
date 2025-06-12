namespace RDMSharpTests
{
    public class TestTools
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
        public void TestStatusMessages()
        {
            Assert.Multiple(() =>
            {
                Dictionary<string, ERDM_StatusMessage> results = new Dictionary<string, ERDM_StatusMessage>();
                var enums = new List<ERDM_StatusMessage>();
                enums.AddRange(Enum.GetValues(typeof(ERDM_StatusMessage)).Cast<ERDM_StatusMessage>());
                foreach (ERDM_StatusMessage e in enums)
                {
                    short val1 = 1;
                    if (e == ERDM_StatusMessage.PROXY_BROADCAST_DROPPED)
                        val1 = unchecked((short)(ushort)ERDM_Parameter.CURVE);
                    string str = e.GetStatusMessage(val1, 120);
                    Console.WriteLine($"{e} => {str}");

                    Assert.That(String.IsNullOrWhiteSpace(str), Is.False, e.ToString());
                    Assert.That(str, Does.EndWith("."), $"{e} => {str}");

                    Assert.That(results.TryAdd(str, e), Is.True, $"{e} => {str}");
                }
            });
        }
        [Test]
        public void TestSensorUnitSymbol()
        {
            Assert.Multiple(() =>
            {
                Dictionary<string, ERDM_SensorUnit> results = new Dictionary<string, ERDM_SensorUnit>();
                var enums = Enum.GetValues(typeof(ERDM_SensorUnit));
                foreach (ERDM_SensorUnit e in enums)
                {
                    string str = e.GetUnitSymbol();
                    Console.WriteLine($"{e} => \'{str}\'");
                    if (e == ERDM_SensorUnit.NONE)
                        Assert.That(String.IsNullOrWhiteSpace(str), Is.True, e.ToString());
                    else
                        Assert.That(String.IsNullOrWhiteSpace(str), Is.False, e.ToString());

                    Assert.That(results.TryAdd(str, e), Is.True, $"{e} => {str}");
                }
            });
        }
        [Test]
        public void TestUnitPrefixGetNormalizedValue()
        {
            Assert.Multiple(() =>
            {
                Dictionary<double, ERDM_UnitPrefix> results = new Dictionary<double, ERDM_UnitPrefix>();
                var enums = Enum.GetValues(typeof(ERDM_UnitPrefix));
                foreach (ERDM_UnitPrefix e in enums)
                {
                    short val = 1;
                    var ret = e.GetNormalizedValue(val);
                    Console.WriteLine($"{e} => {ret}");
                    if (e == ERDM_UnitPrefix.NONE)
                        Assert.That(val, Is.EqualTo(ret), e.ToString());
                    else
                        Assert.That(val, Is.Not.EqualTo(ret), e.ToString());

                    Assert.That(results.TryAdd(ret, e), Is.True, $"{e} => {ret}");
                }
            });
        }
        [Test]
        public void TestDataToValueExceptions()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = Array.Empty<byte>(); Tools.DataToBool(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = Array.Empty<byte>(); Tools.DataToBoolArray(ref data, 8); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = Array.Empty<byte>(); Tools.DataToByte(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = Array.Empty<byte>(); Tools.DataToSByte(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[1]; Tools.DataToUShort(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[1]; Tools.DataToShort(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[3]; Tools.DataToUInt(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[3]; Tools.DataToInt(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[5]; Tools.DataToULong(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[5]; Tools.DataToLong(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[5]; Tools.DataToRDMUID(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[3]; Tools.DataToIPAddressIPv4(ref data); });
                Assert.Throws(typeof(IndexOutOfRangeException), () => { var data = new byte[15]; Tools.DataToIPAddressIPv6(ref data); });
            });
        }
    }
}