namespace RDMSharpTest.RDM
{
    public class RDMMessageFactoryTest
    {

        [Test]
        public void TestBuildDIscUniqueBranchResponse_Corrupt()
        {
            var b = BuildMessage();
            //Corrupt Checksum
            b[18]++;

            var m = new RDMMessage(b);
            Assert.That(m.ChecksumValid, Is.False);
            Assert.That(m.DeserializedChecksum1, Is.Not.Null);
            Assert.That(m.DeserializedChecksum2, Is.Not.Null);

            b = BuildMessage();
            //Corrupt Data
            b[14]++;

            m = new RDMMessage(b);
            Assert.That(m.ChecksumValid, Is.False);
            Assert.That(m.DeserializedChecksum1, Is.Not.Null);
            Assert.That(m.DeserializedChecksum2, Is.Not.Null);

            b = BuildMessage();
            //Remove Preample seperator
            b[3] = 0;

            Assert.Throws(typeof(Exception), ()=> new RDMMessage(b));

            //Cut Message
            b = BuildMessage();
            b = b.Take(b.Length - 3).ToArray();

            Assert.Throws(typeof(Exception), () => new RDMMessage(b));
        }

        [Test]
        public void TestBuildDIscUniqueBranchResponse_Correct()
        {
            var b = BuildMessage();
            RDMMessage m = new RDMMessage(b);

            Assert.That(m, Is.Not.Null);
            Assert.That(m.Command, Is.EqualTo(ERDM_Command.DISCOVERY_COMMAND_RESPONSE));
            Assert.That(m.Parameter, Is.EqualTo(ERDM_Parameter.DISC_UNIQUE_BRANCH));
            Assert.That(m.SourceUID.ManufacturerID, Is.EqualTo((ushort)0xACBD));
            Assert.That(m.SourceUID.DeviceID, Is.EqualTo(0x12345678u));
        }

        [Test]
        public void TestBuildDIscUniqueBranchResponse_CorrectNoPreample()
        {
            var b = BuildMessage().Skip(3).ToArray();
            RDMMessage m = new RDMMessage(b);

            Assert.That(m, Is.Not.Null);
            Assert.That(m.Command, Is.EqualTo(ERDM_Command.DISCOVERY_COMMAND_RESPONSE));
            Assert.That(m.Parameter, Is.EqualTo(ERDM_Parameter.DISC_UNIQUE_BRANCH));
            Assert.That(m.SourceUID.ManufacturerID, Is.EqualTo((ushort)0xACBD));
            Assert.That(m.SourceUID.DeviceID, Is.EqualTo(0x12345678u));
        }
        [Test]
        public void TestBuildDiscUniqueBranchResponseMessage()
        {
            var b = BuildMessage();
            RDMMessage m = new RDMMessage(b);
            var rebuildBytes = m.BuildMessage();
            var rebuildMessage = new RDMMessage(rebuildBytes);

            Assert.That(rebuildBytes, Is.EquivalentTo(b));
            Assert.That(rebuildMessage, Is.EqualTo(m));

            Assert.That(m, Is.Not.Null);
            Assert.That(m.Command, Is.EqualTo(ERDM_Command.DISCOVERY_COMMAND_RESPONSE));
            Assert.That(m.Parameter, Is.EqualTo(ERDM_Parameter.DISC_UNIQUE_BRANCH));
            Assert.That(m.SourceUID.ManufacturerID, Is.EqualTo((ushort)0xACBD));
            Assert.That(m.SourceUID.DeviceID, Is.EqualTo(0x12345678u));
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