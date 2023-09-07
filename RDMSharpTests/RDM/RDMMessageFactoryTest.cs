using RDMSharp;
using NUnit.Framework;
using System.Linq;

namespace RDMSharpTest.RDM
{
    public class RDMMessageFactoryTest
    {

        [Test]
        public void TestBuildDIscUniqueBranchResponse_Corrupt()
        {
            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(null));
            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(new byte[12]));
            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(new byte[23]));

            var b = BuildMessage();
            //Corrupt Checksum
            b[18]++;

            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(b));

            b = BuildMessage();
            //Corrupt Data
            b[14]++;
            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(b));

            b = BuildMessage();
            //Remove Preample seperator
            b[3] = 0;
            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(b));

            //Cut Message
            b = BuildMessage();
            b = b.Take(b.Length - 3).ToArray();
            Assert.IsNull(RDMMessageFactory.BuildDiscUniqueBranchResponse(b));
        }

        [Test]
        public void TestBuildDIscUniqueBranchResponse_Correct()
        {
            var b = BuildMessage();
            RDMMessage m = RDMMessageFactory.BuildDiscUniqueBranchResponse(b);

            Assert.IsNotNull(m);
            Assert.AreEqual(ERDM_Command.DISCOVERY_COMMAND_RESPONSE, m.Command);
            Assert.AreEqual(ERDM_Parameter.DISC_UNIQUE_BRANCH, (ERDM_Parameter)m.Parameter);
            Assert.AreEqual((ushort)0xACBD, m.SourceUID.ManufacturerID);
            Assert.AreEqual(0x12345678u, m.SourceUID.DeviceID);
        }

        [Test]
        public void TestBuildDIscUniqueBranchResponse_CorrectNoPreample()
        {
            var b = BuildMessage().Skip(3).ToArray();
            RDMMessage m = RDMMessageFactory.BuildDiscUniqueBranchResponse(b);

            Assert.IsNotNull(m);
            Assert.AreEqual(ERDM_Command.DISCOVERY_COMMAND_RESPONSE, m.Command);
            Assert.AreEqual(ERDM_Parameter.DISC_UNIQUE_BRANCH, (ERDM_Parameter)m.Parameter);
            Assert.AreEqual((ushort)0xACBD, m.SourceUID.ManufacturerID);
            Assert.AreEqual(0x12345678u, m.SourceUID.DeviceID);
        }

        private byte[] BuildMessage()
        {
            var b = new byte[]
            {
                0xFE, 0xFE, 0xFE,
                0xAA,
                0xAC | 0xAA,
                0xAC | 0x55,
                0xBD | 0xAA,
                0xBD | 0x55,
                0x12 | 0xAA,
                0x12 | 0x55,
                0x34 | 0xAA,
                0x34 | 0x55,
                0x56 | 0xAA,
                0x56 | 0x55,
                0x78 | 0xAA,
                0x78 | 0x55,
                0, 0, 0, 0
            };

            //Calculate Checksum
            ushort cs = (ushort)b.Skip(4).Take(12).Sum(c => c);
            b[16] = (byte)((cs >> 8) | 0xAA);
            b[17] = (byte)((cs >> 8) | 0x55);
            b[18] = (byte)((cs & 0xFF) | 0xAA);
            b[19] = (byte)((cs & 0xFF) | 0x55);

            return b;
        }
    }
}