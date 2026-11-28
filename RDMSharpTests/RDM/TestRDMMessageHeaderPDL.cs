namespace RDMSharpTests.RDM;

/// <summary>
/// The PDL in the header of a received message has to fit the Message Length (E1.20 6.2.3: Message Length = 24 + PDL).
/// If it does not fit, the real Parameter Data between the header and the checksum is used.
/// </summary>
public class TestRDMMessageHeaderPDL
{
    private static readonly byte[] PARAMETER_DATA = new byte[] { 0x00, 0x2A };

    /// <summary>
    /// A valid GET_COMMAND_RESPONSE of DMX_START_ADDRESS (42), the PDL byte is replaced by <paramref name="pdl"/>
    /// and the checksum is calculated again, so only the header is inconsistent.
    /// </summary>
    private static byte[] buildWithPdl(byte pdl)
    {
        var message = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND_RESPONSE,
            Parameter = ERDM_Parameter.DMX_START_ADDRESS,
            SourceUID = new UID(0x1234, 0x56789ABC),
            DestUID = new UID(0x1fff, 0x00000001),
            SubDevice = SubDevice.Root,
            ParameterData = PARAMETER_DATA
        };
        byte[] data = message.BuildMessage();
        Assert.That(data[23], Is.EqualTo(PARAMETER_DATA.Length), "byte 23 is the PDL");
        data[23] = pdl;
        int length = data[2];
        ushort checksum = (ushort)data.Take(length).Sum(b => b);
        data[length] = (byte)(checksum >> 8);
        data[length + 1] = (byte)checksum;
        return data;
    }

    [Test]
    public void TestPdlFitsMessageLength()
    {
        var message = new RDMMessage(buildWithPdl((byte)PARAMETER_DATA.Length));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ChecksumValid, Is.True);
            Assert.That(message.ParameterData, Is.EqualTo(PARAMETER_DATA));
            Assert.That(message.PDL, Is.EqualTo(PARAMETER_DATA.Length));
            Assert.That(message.Value, Is.EqualTo((ushort)42));
        }
    }

    [Test]
    public void TestPdlLargerThanMessageLength()
    {
        // the header claims 5 bytes, but the Message Length only has room for 2
        var message = new RDMMessage(buildWithPdl(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ChecksumValid, Is.True);
            Assert.That(message.ParameterData, Is.EqualTo(PARAMETER_DATA), "the real Parameter Data is used, not the checksum behind it");
            Assert.That(message.PDL, Is.EqualTo(PARAMETER_DATA.Length));
            Assert.That(message.Value, Is.EqualTo((ushort)42));
        }
    }

    [Test]
    public void TestPdlSmallerThanMessageLength()
    {
        // the header claims 1 byte, but the Message Length contains 2
        var message = new RDMMessage(buildWithPdl(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ChecksumValid, Is.True);
            Assert.That(message.ParameterData, Is.EqualTo(PARAMETER_DATA), "the real Parameter Data is used");
            Assert.That(message.PDL, Is.EqualTo(PARAMETER_DATA.Length));
            Assert.That(message.Value, Is.EqualTo((ushort)42));
        }
    }
}
