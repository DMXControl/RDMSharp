using RDMSharp;

namespace RDMSharpTest.RDM
{
    public class RDMMessageTest
    {
        [Test]
        public void RDMMessageParameterData_Exception()
        {
            RDMMessage m = new RDMMessage();

            Assert.AreEqual(0, m.ParameterData.Length);
            Assert.AreEqual(0, m.PDL);
            Assert.AreEqual(24, m.MessageLength);

            m.ParameterData = new byte[5];
            Assert.AreEqual(5, m.ParameterData.Length);
            Assert.AreEqual(5, m.PDL);
            Assert.AreEqual(29, m.MessageLength);

            m.ParameterData = new byte[231];
            Assert.AreEqual(231, m.ParameterData.Length);
            Assert.AreEqual(231, m.PDL);
            Assert.AreEqual(255, m.MessageLength);

            Assert.Throws<ArgumentException>(() => m.ParameterData = new byte[232]);
        }

        [Test]
        public void RDMMessageLengthTest()
        {
            RDMMessage m = new RDMMessage();

            m.ParameterData = new byte[17];

            Assert.AreEqual(17, m.PDL);
            Assert.AreEqual(24 + 17, m.MessageLength);
        }

        [Test]
        public void RDMMessageChecksumTest()
        {
            //Example taken from RDM Spec
            RDMMessage m = new RDMMessage
            {
                DestUID = new RDMUID(0x1234, 0x56789abc),
                SourceUID = new RDMUID(0xcba9, 0x87654321),
                PortID_or_Responsetype = 1,
                Command = ERDM_Command.GET_COMMAND,
                Parameter = ERDM_Parameter.STATUS_MESSAGES,
                ParameterData = new byte[] {0x04}
            };

            Assert.AreEqual(0x66A, m.Checksum);
        }

        [Test]
        public void RDMMessageChecksumTest2()
        {
            RDMMessage m = new RDMMessage
            {
                DestUID = new RDMUID(0x1234, 0x56789abc),
                SourceUID = new RDMUID(0x02B0, 0x00112233),
                PortID_or_Responsetype = 1,
                Command = ERDM_Command.SET_COMMAND,
                Parameter = ERDM_Parameter.DMX_START_ADDRESS,
                ParameterData = new byte[] {0x00, 0x42}
            };

            Assert.AreEqual(0x5CE, m.Checksum);
        }

        [Test]
        public void RDMMessageChecksumTest3()
        {
            RDMMessage m = new RDMMessage
            {
                DestUID = new RDMUID(0x1234, 0x56789abc),
                SourceUID = new RDMUID(0xcba9, 0x87654321),
                PortID_or_Responsetype = 1,
                Command = ERDM_Command.SET_COMMAND,
                Parameter = ERDM_Parameter.DMX_START_ADDRESS,
                ParameterData = Enumerable.Range(0, 200).Select(c => (byte)0xFE).ToArray()
            };

            Assert.AreEqual(0xCF34, m.Checksum);
        }

        [Test]
        public void RDMMessageDiscoveryBuildMessage()
        {
            RDMMessage m = new RDMMessage
            {
                DestUID = new RDMUID(0x1234, 0x56789abc),
                SourceUID = new RDMUID(0x02B0, 0x00112233),
                Command = ERDM_Command.DISCOVERY_COMMAND,
                Parameter = ERDM_Parameter.DISC_MUTE
            };


            byte[] erg = m.BuildMessage();

            byte[] expected = new byte[]
            {
                0xcc, 0x01, 0x18, 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0x02, 0xb0, 0x00, 0x11, 
                0x22, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x02, 0x00, 0x04, 0x79
            };

            Assert.IsTrue(erg.SequenceEqual(expected));
        }

        [Test]
        public void RDMUID_ToString()
        {
            Assert.AreEqual("FFFF:ABCDEF98", new RDMUID(0xFFFF, 0xABCDEF98).ToString());
            Assert.AreEqual("A053:00335612", new RDMUID(0xA053, 0x00335612).ToString());
            Assert.AreEqual("0000:12345678", new RDMUID(0x0000, 0x12345678).ToString());
            Assert.AreEqual("0001:00000002", new RDMUID(1, 2).ToString());
            Assert.AreEqual("0000:00000000", new RDMUID(0, 0).ToString());
        }

        [Test]
        public void RDMUID_FromULong()
        {
            Assert.AreEqual(new RDMUID(0xFFFF, 0xABCDEF98), RDMUID.FromULong(0xFFFFABCDEF98));
            Assert.AreEqual(new RDMUID(0xFF1F, 0xABCD0F98), RDMUID.FromULong(0xFF1FABCD0F98));

            Assert.AreEqual(new RDMUID(0, 1), RDMUID.FromULong(0x0001));
        }

        [Test]
        public void RDMUID_CastToULong()
        {
            Assert.AreEqual(0xFFFFABCDEF98u, (ulong)new RDMUID(0xFFFF, 0xABCDEF98));
            Assert.AreEqual(0xFF1FABCD0F98u, (ulong)new RDMUID(0xFF1F, 0xABCD0F98));

            Assert.AreEqual(0x0001u, (ulong)new RDMUID(0, 1));
        }

        [Test]
        public void RDMUID_Equals()
        {
            Assert.AreEqual(RDMUID.Empty, new RDMUID());
            Assert.AreEqual(RDMUID.Empty, new RDMUID(0, 0));
            Assert.AreEqual(RDMUID.Broadcast, new RDMUID(ushort.MaxValue, uint.MaxValue));

        }
    }
}
