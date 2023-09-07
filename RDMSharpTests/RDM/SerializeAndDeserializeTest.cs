using RDMSharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharpTest.RDM
{
    public class SerializeAndDeserializeTest
    {
        private static ERDM_Parameter[] GET_NOT_ALLOWED_PARAMETERS = new ERDM_Parameter[]
        {
            //E1.20
            ERDM_Parameter.DISC_UNIQUE_BRANCH,
            ERDM_Parameter.DISC_MUTE,
            ERDM_Parameter.DISC_UN_MUTE,
            ERDM_Parameter.CLEAR_STATUS_ID,
            ERDM_Parameter.RECORD_SENSORS,
            ERDM_Parameter.RESET_DEVICE,
            ERDM_Parameter.CAPTURE_PRESET,

            //E1.37-2
            ERDM_Parameter.INTERFACE_RENEW_DHCP,
            ERDM_Parameter.INTERFACE_RELEASE_DHCP,
            ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION,
        };
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void _0TestBool()
        {
            byte[] serialized = null;
            for (bool b = false; b != true; b=true)
            {
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(1, serialized.Length);

                bool desByte = Tools.DataToBool(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desByte);
            }
        }
        [Test]
        public void _0TestString()
        {
            byte[] serialized = null;
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

            foreach (string str in testStrings)
            {
                serialized = Tools.ValueToData(str);
                Assert.AreEqual(str.Length, serialized.Length);

                string desByte = Tools.DataToString(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(str, desByte);
            }
        }
        [Test]
        public void _0TestEnum()
        {
            byte[] serialized = null;

            foreach(ERDM_Parameter eParameter in Enum.GetValues(typeof(ERDM_Parameter)))
            {
                serialized = Tools.ValueToData(eParameter);
                Assert.AreEqual(2, serialized.Length);

                ERDM_Parameter desByte = Tools.DataToEnum<ERDM_Parameter>(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(eParameter, desByte);
            }
            foreach (ERDM_SensorUnit eParameter in Enum.GetValues(typeof(ERDM_SensorUnit)))
            {
                serialized = Tools.ValueToData(eParameter);
                Assert.AreEqual(1, serialized.Length);

                ERDM_SensorUnit desByte = Tools.DataToEnum<ERDM_SensorUnit>(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(eParameter, desByte);
            }

            foreach (ERDM_ProductCategoryCoarse eParameter in Enum.GetValues(typeof(ERDM_ProductCategoryCoarse)))
            {
                serialized = Tools.ValueToData(eParameter);
                Assert.AreEqual(1, serialized.Length);

                ERDM_ProductCategoryCoarse desByte = Tools.DataToEnum<ERDM_ProductCategoryCoarse>(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(eParameter, desByte);
            }

            foreach (ERDM_ProductCategoryFine eParameter in Enum.GetValues(typeof(ERDM_ProductCategoryFine)))
            {
                serialized = Tools.ValueToData(eParameter);
                Assert.AreEqual(2, serialized.Length);

                ERDM_ProductCategoryFine desByte = Tools.DataToEnum<ERDM_ProductCategoryFine>(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(eParameter, desByte);
            }

            foreach (ERDM_ProductDetail eParameter in Enum.GetValues(typeof(ERDM_ProductDetail)))
            {
                serialized = Tools.ValueToData(eParameter);
                Assert.AreEqual(2, serialized.Length);

                ERDM_ProductDetail desByte = Tools.DataToEnum<ERDM_ProductDetail>(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(eParameter, desByte);
            }

            foreach (ERDM_UnitPrefix eParameter in Enum.GetValues(typeof(ERDM_UnitPrefix)))
            {
                serialized = Tools.ValueToData(eParameter);
                Assert.AreEqual(1, serialized.Length);

                ERDM_UnitPrefix desByte = Tools.DataToEnum<ERDM_UnitPrefix>(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(eParameter, desByte);
            }
        }
        [Test]
        public void _0TestBoolArray()
        {
            byte[] serialized = null;
            bool[] b = new bool[] { true, false, true, false, false, true };
            serialized = Tools.ValueToData(b);
            Assert.AreEqual(1, serialized.Length);

            bool[] desByte = Tools.DataToBoolArray(ref serialized, b.Length);
            Assert.AreEqual(0, serialized.Length);
            Assert.AreEqual(b, desByte);
        }
        [Test]
        public void _1TestByte()
        {
            byte[] serialized = null;
            for (byte b = byte.MinValue; b < byte.MaxValue; b++)
            {
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(1, serialized.Length);

                byte desByte = Tools.DataToByte(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desByte);
            }
        }
        [Test]
        public void _2TestSByte()
        {
            byte[] serialized = null;
            for (sbyte b = sbyte.MinValue; b < sbyte.MaxValue; b++)
            {
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(1, serialized.Length);

                sbyte desByte = Tools.DataToSByte(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desByte);
            }
        }
        [Test]
        public void _3TestShort()
        {
            byte[] serialized = null;
            for (short b = short.MinValue; b < short.MaxValue; b++)
            {
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(2, serialized.Length);

                short desShort = Tools.DataToShort(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desShort);
            }
        }
        [Test]
        public void _4TestUShort()
        {
            byte[] serialized = null;
            for (ushort b = ushort.MinValue; b < ushort.MaxValue; b++)
            {
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(2, serialized.Length);

                ushort desShort = Tools.DataToUShort(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desShort);
            }
        }
        [Test]
        public void _5TestInt()
        {
            Random rnd = new Random();
            byte[] serialized = null;
            for (short s = short.MinValue; s < short.MaxValue; s++)
            {
                int b = rnd.Next(int.MinValue, int.MaxValue);
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(4, serialized.Length);

                int desInt = Tools.DataToInt(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desInt);
            }
        }
        [Test]
        public void _6TestUInt()
        {
            Random rnd = new Random();
            byte[] serialized = null;
            for (short s = short.MinValue; s < short.MaxValue; s++)
            {
                uint b = ((uint)int.MaxValue) + ((uint)rnd.Next(int.MinValue, int.MaxValue));
                serialized = Tools.ValueToData(b);
                Assert.AreEqual(4, serialized.Length);

                uint desInt = Tools.DataToUInt(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desInt);
            }
        }
        [Test]
        public void _7TestLong()
        {
            Random rnd = new Random();
            byte[] serialized = null;
            for (short s = short.MinValue; s < short.MaxValue; s++)
            {
                long b = this.LongRandom(rnd);

                serialized = Tools.ValueToData(b);
                Assert.AreEqual(8, serialized.Length);

                long desLong = Tools.DataToLong(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desLong);
            }
        }
        [Test]
        public void _8TestULong()
        {
            Random rnd = new Random();
            byte[] serialized = null;
            for (short s = short.MinValue; s < short.MaxValue; s++)
            {
                ulong b = this.ULongRandom(rnd);

                serialized = Tools.ValueToData(b);
                Assert.AreEqual(8, serialized.Length);

                ulong desLong = Tools.DataToULong(ref serialized);
                Assert.AreEqual(0, serialized.Length);
                Assert.AreEqual(b, desLong);
            }
        }

        long LongRandom(Random rand)
        {
            long result = rand.Next((int)(int.MinValue >> 32), (int)(int.MaxValue >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((int)int.MinValue, (int)int.MaxValue);
            return result;
        }
        ulong ULongRandom(Random rand)
        {
            ulong result = (uint)rand.Next((int)(int.MinValue >> 32), (int)(int.MaxValue >> 32));
            result = (result << 32);
            result = result | (ulong)rand.Next((int)int.MinValue, (int)int.MaxValue);
            return result;
        }
    }
}