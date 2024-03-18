﻿namespace RDMSharpTests.RDM
{
    public class RDMMessageTest
    {
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public void RDMMessageParameterData_Exception()
        {
            RDMMessage? m = new RDMMessage();

            Assert.Multiple(() =>
            {
                Assert.That(m.ParameterData, Is.Empty);
                Assert.That(m.PDL, Is.EqualTo(0));
                Assert.That(m.MessageLength, Is.EqualTo(24));

                m.ParameterData = new byte[5];
                Assert.That(m.ParameterData, Has.Length.EqualTo(5));
                Assert.That(m.PDL, Is.EqualTo(5));
                Assert.That(m.MessageLength, Is.EqualTo(29));

                m.ParameterData = new byte[231];
                Assert.That(m.ParameterData, Has.Length.EqualTo(231));
                Assert.That(m.PDL, Is.EqualTo(231));
                Assert.That(m.MessageLength, Is.EqualTo(255));

                Assert.Throws<ArgumentException>(() => m.ParameterData = new byte[232]);

                m = new RDMMessage
                {
                    Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                    Command = ERDM_Command.DISCOVERY_COMMAND,
                    ParameterData = new DiscUniqueBranchRequest(RDMUID.Empty, RDMUID.Broadcast - 1).ToPayloadData()
                };

                Assert.That(m.Equals(m), Is.True);
                Assert.That(m.Equals((object)m), Is.True);
                Assert.That(m.GetHashCode(), Is.EqualTo(m!.GetHashCode()));
                var m2 = new RDMMessage(m.BuildMessage());
                Assert.That(m2, Is.EqualTo(m));
                Assert.That(m2.GetHashCode(), Is.EqualTo(m.GetHashCode()));
            });
        }

        [Test]
        public void RDMMessageLengthTest()
        {
            RDMMessage m = new RDMMessage
            {
                ParameterData = new byte[17]
            };

            Assert.Multiple(() =>
            {
                Assert.That(m.PDL, Is.EqualTo(17));
                Assert.That(m.MessageLength, Is.EqualTo(24 + 17));
            });
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
                ParameterData = new byte[] { 0x04 }
            };

            Assert.That(m.Checksum, Is.EqualTo(0x66A));
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
                ParameterData = new byte[] { 0x00, 0x42 }
            };

            Assert.That(m.Checksum, Is.EqualTo(0x5CE));
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

            Assert.That(m.Checksum, Is.EqualTo(0xCF34));
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

            Assert.That(erg.SequenceEqual(expected), Is.True);
        }
    }
}
