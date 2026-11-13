using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM;

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
                ParameterData = new DiscUniqueBranchRequest(UID.Empty, UID.Broadcast - 1).ToPayloadData()
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
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0xcba9, 0x87654321),
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
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0x02B0, 0x00112233),
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
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0xcba9, 0x87654321),
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
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0x02B0, 0x00112233),
            Command = ERDM_Command.DISCOVERY_COMMAND,
            Parameter = ERDM_Parameter.DISC_MUTE
        };


        byte[] erg = m.BuildMessage();


        byte[] expected = new byte[]
        {
            0xcc, 0x01, 0x18, 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0x02, 0xb0, 0x00, 0x11,
            0x22, 0x33, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x02, 0x00, 0x04, 0x7a
        };
        var eM = new RDMMessage(expected);
        Assert.That(m.DestUID, Is.EqualTo(eM.DestUID));
        Assert.That(m.SourceUID, Is.EqualTo(eM.SourceUID));
        Assert.That(m.Command, Is.EqualTo(eM.Command));
        Assert.That(m.Parameter, Is.EqualTo(eM.Parameter));
        Assert.That(m.PortID_or_Responsetype, Is.EqualTo(eM.PortID_or_Responsetype));
        Assert.That(m.Checksum, Is.EqualTo(eM.Checksum));

        Assert.That(expected, Is.EquivalentTo(erg));
    }


    [Test]
    public void RDMMessageNACKBuildMessage()
    {
        RDMMessage m = new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
        {
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0x02B0, 0x00112233),
            Command = ERDM_Command.GET_COMMAND_RESPONSE,
            Parameter = ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
        };


        byte[] erg = m.BuildMessage();

        byte[] expected = new byte[]
        {
            0xcc, 0x01, 0x1a, 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0x02, 0xb0, 0x00, 0x11,
            0x22, 0x33, 0x00, 0x02, 0x00, 0x00, 0x00, 0x21, 0x09, 0x03, 0x02, 0x00, 0x06,
            0x04, 0xa0
        };

        RDMMessage m2 = new RDMMessage(erg);

        Assert.That(m2.ResponseType, Is.EqualTo(m.ResponseType));
        Assert.That(m2.NackReason, Is.EqualTo(m.NackReason));
        Assert.That(m2.PDL, Is.EqualTo(m.PDL));
        Assert.That(m2.Command, Is.EqualTo(m.Command));
        Assert.That(erg, Is.EqualTo(expected));
    }



    [Test]
    public void RDMMessageAckBuildMessage()
    {
        var ackTimer = new AcknowledgeTimer(TimeSpan.FromMilliseconds(5000));
        RDMMessage m = new RDMMessage(ackTimer)
        {
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0x02B0, 0x00112233),
            Command = ERDM_Command.GET_COMMAND_RESPONSE,
            Parameter = ERDM_Parameter.LAMP_STATE,
        };


        byte[] erg = m.BuildMessage();

        byte[] expected = new byte[]
        {
            0xcc, 0x01, 0x1a, 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0x02, 0xb0, 0x00, 0x11,
            0x22, 0x33, 0x00, 0x01, 0x00, 0x00, 0x00, 0x21, 0x04, 0x03, 0x02, 0x00, 0x32,
            0x04, 0xc6
        };

        RDMMessage m2 = new RDMMessage(erg);

        Assert.That(m2.ResponseType, Is.EqualTo(m.ResponseType));
        Assert.That(m2.AcknowledgeTimer, Is.EqualTo(ackTimer));
        Assert.That(m2.PDL, Is.EqualTo(m.PDL));
        Assert.That(m2.Command, Is.EqualTo(m.Command));
        Assert.That(erg, Is.EqualTo(expected));
    }
    [Test]
    public void RDMMessageAckHighResBuildMessage()
    {
        var ackTimer = new AcknowledgeTimerHighRes(TimeSpan.FromMilliseconds(520));
        RDMMessage m = new RDMMessage(ackTimer)
        {
            DestUID = new UID(0x1234, 0x56789abc),
            SourceUID = new UID(0x02B0, 0x00112233),
            Command = ERDM_Command.GET_COMMAND_RESPONSE,
            Parameter = ERDM_Parameter.LAMP_STATE,
        };


        byte[] erg = m.BuildMessage();

        byte[] expected = new byte[]
        {
            0xcc, 0x01, 0x1a, 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0x02, 0xb0, 0x00, 0x11,
            0x22, 0x33, 0x00, 0x04, 0x00, 0x00, 0x00, 0x21, 0x04, 0x03, 0x02, 0x02, 0x08,
            0x04, 0xa1
        };

        RDMMessage m2 = new RDMMessage(erg);

        Assert.That(m2.ResponseType, Is.EqualTo(m.ResponseType));
        Assert.That(m2.AcknowledgeTimer, Is.EqualTo(ackTimer));
        Assert.That(m2.PDL, Is.EqualTo(m.PDL));
        Assert.That(m2.Command, Is.EqualTo(m.Command));
        Assert.That(erg, Is.EqualTo(expected));
    }
}
