namespace RDMSharpTests.RDM
{
    public class SerializeAndDeserializeTest
    {
        private static readonly Random rnd = new Random();
        private static byte[]? serialized = null;
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
            serialized = null;
        }
        [Test]
        public void _0TestBool()
        {
            Assert.Multiple(() =>
            {
                for (bool b = false; b != true; b = true)
                {
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(1));

                    bool desByte = Tools.DataToBool(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _0TestString()
        {
            string[] testStrings = new string[]
            {
                "jepmepnarmzsepbriantisewbglameid",
                "sd<sdvc<svcnsdjvvnjvdjlsjvmvcvsv",
                "kajdiuahduancdkahdiuajdlkadpojgl",
                "64sdfnlsdv5sdsdnsdv4sdvsvbsd4asf",
                "  ddd  ddd  ddqwdadada   dfafd a",
                "234568iefsfsafsfdsbdjfskfsfssfsf",
                "sdfsdfsfsf",
                "sdsfdsfdsf",
                "sdfsdfgggggwwwww"
            };

            Assert.Multiple(() =>
            {
                foreach (string str in testStrings)
                {
                    serialized = Tools.ValueToData(str);
                    Assert.That(str, Has.Length.EqualTo(serialized.Length));

                    string desByte = Tools.DataToString(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(str));
                }
            });
        }
        [Test]
        public void _0TestEnum()
        {
            Assert.Multiple(() =>
            {
                foreach (ERDM_Parameter eParameter in Enum.GetValues(typeof(ERDM_Parameter)))
                {
                    serialized = Tools.ValueToData(eParameter);
                    Assert.That(serialized, Has.Length.EqualTo(2));

                    ERDM_Parameter desByte = Tools.DataToEnum<ERDM_Parameter>(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(eParameter));
                }
                foreach (ERDM_SensorUnit eParameter in Enum.GetValues(typeof(ERDM_SensorUnit)))
                {
                    serialized = Tools.ValueToData(eParameter);
                    Assert.That(serialized, Has.Length.EqualTo(1));

                    ERDM_SensorUnit desByte = Tools.DataToEnum<ERDM_SensorUnit>(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(eParameter));
                }

                foreach (ERDM_ProductCategoryCoarse eParameter in Enum.GetValues(typeof(ERDM_ProductCategoryCoarse)))
                {
                    serialized = Tools.ValueToData(eParameter);
                    Assert.That(serialized, Has.Length.EqualTo(1));

                    ERDM_ProductCategoryCoarse desByte = Tools.DataToEnum<ERDM_ProductCategoryCoarse>(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(eParameter));
                }

                foreach (ERDM_ProductCategoryFine eParameter in Enum.GetValues(typeof(ERDM_ProductCategoryFine)))
                {
                    serialized = Tools.ValueToData(eParameter);
                    Assert.That(serialized, Has.Length.EqualTo(2));

                    ERDM_ProductCategoryFine desByte = Tools.DataToEnum<ERDM_ProductCategoryFine>(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(eParameter));
                }

                foreach (ERDM_ProductDetail eParameter in Enum.GetValues(typeof(ERDM_ProductDetail)))
                {
                    serialized = Tools.ValueToData(eParameter);
                    Assert.That(serialized, Has.Length.EqualTo(2));

                    ERDM_ProductDetail desByte = Tools.DataToEnum<ERDM_ProductDetail>(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(eParameter));
                }

                foreach (ERDM_UnitPrefix eParameter in Enum.GetValues(typeof(ERDM_UnitPrefix)))
                {
                    serialized = Tools.ValueToData(eParameter);
                    Assert.That(serialized, Has.Length.EqualTo(1));

                    ERDM_UnitPrefix desByte = Tools.DataToEnum<ERDM_UnitPrefix>(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(eParameter));
                }
            });
        }
        [Test]
        public void _0TestBoolArray()
        {
            bool[] b = new bool[] { true, false, true, false, false, true };
            serialized = Tools.ValueToData(b);
            Assert.Multiple(() =>
            {
                Assert.That(serialized, Has.Length.EqualTo(1));

                bool[] desByte = Tools.DataToBoolArray(ref serialized, b.Length);
                Assert.That(serialized, Is.Empty);
                Assert.That(desByte, Is.EqualTo(b));
            });
        }
        [Test]
        public void _1TestByte()
        {
            Assert.Multiple(() =>
            {
                for (byte b = byte.MinValue; b < byte.MaxValue; b++)
                {
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(1));

                    byte desByte = Tools.DataToByte(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _2TestSByte()
        {
            Assert.Multiple(() =>
            {
                for (sbyte b = sbyte.MinValue; b < sbyte.MaxValue; b++)
                {
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(1));

                    sbyte desByte = Tools.DataToSByte(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desByte, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _3TestShort()
        {
            Assert.Multiple(() =>
            {
                for (short b = short.MinValue; b < short.MaxValue; b++)
                {
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(2));

                    short desShort = Tools.DataToShort(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desShort, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _4TestUShort()
        {
            Assert.Multiple(() =>
            {
                for (ushort b = ushort.MinValue; b < ushort.MaxValue; b++)
                {
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(2));

                    ushort desShort = Tools.DataToUShort(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desShort, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _5TestInt()
        {
            Assert.Multiple(() =>
            {
                for (short s = short.MinValue; s < short.MaxValue; s++)
                {
                    int b = rnd.Next(int.MinValue, int.MaxValue);
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(4));

                    int desInt = Tools.DataToInt(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desInt, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _6TestUInt()
        {
            Assert.Multiple(() =>
            {
                for (short s = short.MinValue; s < short.MaxValue; s++)
                {
                    uint b = ((uint)int.MaxValue) + ((uint)rnd.Next(int.MinValue, int.MaxValue));
                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(4));

                    uint desInt = Tools.DataToUInt(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desInt, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _7TestLong()
        {
            Assert.Multiple(() =>
            {
                for (short s = short.MinValue; s < short.MaxValue; s++)
                {
                    long b = LongRandom(rnd);

                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(8));

                    long desLong = Tools.DataToLong(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desLong, Is.EqualTo(b));
                }
            });
        }
        [Test]
        public void _8TestULong()
        {
            Assert.Multiple(() =>
            {
                for (short s = short.MinValue; s < short.MaxValue; s++)
                {
                    ulong b = ULongRandom(rnd);

                    serialized = Tools.ValueToData(b);
                    Assert.That(serialized, Has.Length.EqualTo(8));

                    ulong desLong = Tools.DataToULong(ref serialized);
                    Assert.That(serialized, Is.Empty);
                    Assert.That(desLong, Is.EqualTo(b));
                }
            });
        }

        static long LongRandom(Random rand)
        {
            long result = rand.Next((int)(int.MinValue >> 32), (int)(int.MaxValue >> 32));
            result <<= 32;
            result |= (long)rand.Next((int)int.MinValue, (int)int.MaxValue);
            return result;
        }
        static ulong ULongRandom(Random rand)
        {
            uint result1 = (uint)rand.Next(int.MinValue, int.MaxValue);
            uint result2 = (uint)rand.Next(int.MinValue, int.MaxValue);
            return  (result1 << 32) + result2;
        }
    }
}