namespace RDMSharpTests.RDM;

/// <summary>
/// Parsing of faulty frames and the properties of <see cref="RDMMessage"/> that depend on the kind of the message.
/// The frames are bytes on purpose: the parser is what is tested here.
/// </summary>
public class TestRDMMessageParsing
{
    private static readonly UID SOURCE = new UID(0x1234, 0x56789ABC);
    private static readonly UID DESTINATION = new UID(0x1fff, 0x00000001);

    /// <summary>
    /// A frame of a response of DMX_START_ADDRESS with <paramref name="parameterData"/>. It is built as an ACK, then the response type and the
    /// message counter are set in the bytes (and the checksum is calculated again), so the frame says what the test needs it to say,
    /// even if it does not fit the PDL.
    /// </summary>
    private static byte[] frame(ERDM_ResponseType responseType, byte messageCounter, params byte[] parameterData)
    {
        var message = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND_RESPONSE,
            Parameter = ERDM_Parameter.DMX_START_ADDRESS,
            SourceUID = SOURCE,
            DestUID = DESTINATION,
            SubDevice = SubDevice.Root,
            ParameterData = parameterData
        };
        byte[] data = message.BuildMessage();
        Assert.That(data[23], Is.EqualTo(parameterData.Length), "byte 23 is the PDL");
        data[16] = (byte)responseType;
        data[17] = messageCounter;
        int length = data[2];
        ushort checksum = (ushort)data.Take(length).Sum(b => b);
        data[length] = (byte)(checksum >> 8);
        data[length + 1] = (byte)checksum;
        return data;
    }

    #region Faulty frames
    [Test]
    public void TestNullFrame()
    {
        Assert.Throws<ArgumentNullException>(() => new RDMMessage((byte[])null!));
    }

    [Test]
    public void TestFrameTooShort()
    {
        Assert.Throws<IndexOutOfRangeException>(() => new RDMMessage(new byte[10]));
    }

    [Test]
    public void TestDiscoveryResponseWithoutPreambleSeparator()
    {
        // looks like a discovery response (starts with the preamble), but the preamble separator 0xAA is missing
        var data = Enumerable.Repeat((byte)0xFE, 20).ToArray();
        var ex = Assert.Throws<Exception>(() => new RDMMessage(data));
        Assert.That(ex!.Message, Does.Contain("Preamble"));
    }

    [Test]
    public void TestDiscoveryResponseIsCutOff()
    {
        // the preamble separator is there, but less than the 16 bytes of the encoded UID and checksum follow
        var data = Enumerable.Repeat((byte)0xFE, 10).Append((byte)0xAA).Concat(Enumerable.Repeat((byte)0x55, 9)).ToArray();
        Assert.That(data, Has.Length.EqualTo(20));
        var ex = Assert.Throws<Exception>(() => new RDMMessage(data));
        Assert.That(ex!.Message, Does.Contain("Data Missing"));
    }

    [Test]
    public void TestDiscoveryResponseWithTooLongPreamble()
    {
        // a preamble has 0 - 7 bytes
        var data = Enumerable.Repeat((byte)0xFE, 8).Append((byte)0xAA).Concat(Enumerable.Repeat((byte)0x55, 16)).ToArray();
        Assert.That(data, Has.Length.EqualTo(25));
        var ex = Assert.Throws<Exception>(() => new RDMMessage(data));
        Assert.That(ex!.Message, Does.Contain("Data Missing"));
    }

    [Test]
    public void TestWrongStartCode()
    {
        var data = frame(ERDM_ResponseType.ACK, 0, 0x00, 0x2A);
        data[1] = 0x02;
        var ex = Assert.Throws<Exception>(() => new RDMMessage(data));
        Assert.That(ex!.Message, Does.Contain("Start-Code"));
    }

    [Test]
    public void TestFrameCutOff()
    {
        // the header says how long the message is, the frame is shorter
        var data = frame(ERDM_ResponseType.ACK, 0, 0x00, 0x2A);
        data = data.Take(data.Length - 1).ToArray();
        var ex = Assert.Throws<Exception>(() => new RDMMessage(data));
        Assert.That(ex!.Message, Does.Contain("DataLength"));
    }
    #endregion

    #region Response type that does not fit the PDL
    [TestCase(ERDM_ResponseType.NACK_REASON, Description = "a NACK has a PDL of 2 (the reason)")]
    [TestCase(ERDM_ResponseType.ACK_TIMER, Description = "an ACK_TIMER has a PDL of 2 (the time)")]
    [TestCase(ERDM_ResponseType.ACK_TIMER_HI_RES, Description = "an ACK_TIMER_HI_RES has a PDL of 2 (the time)")]
    public void TestResponseTypeNotMatchingThePdlIsAnAck(ERDM_ResponseType responseType)
    {
        // 5 bytes of Parameter Data, so it can not be what the response type says
        var message = new RDMMessage(frame(responseType, 0, 1, 2, 3, 4, 5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(message.ParameterData, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5 }));
        }
    }

    [Test]
    public void TestOverflowWithoutMessageCounterAndWrongPdlIsAnAck()
    {
        var message = new RDMMessage(frame(ERDM_ResponseType.ACK_OVERFLOW, 0, 1, 2, 3, 4, 5));
        Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
    }

    [Test]
    public void TestOverflowWithMessageCounterStaysOverflow()
    {
        var message = new RDMMessage(frame(ERDM_ResponseType.ACK_OVERFLOW, 1, 1, 2, 3, 4, 5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK_OVERFLOW));
            Assert.That(message.MessageCounter, Is.EqualTo((byte)1));
        }
    }

    [Test]
    public void TestNackWithItsPdlIsKept()
    {
        var message = new RDMMessage(frame(ERDM_ResponseType.NACK_REASON, 0, 0x00, (byte)ERDM_NackReason.FORMAT_ERROR));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(message.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        }
    }
    #endregion

    #region Properties
    [Test]
    public void TestAcknowledgeTimer()
    {
        var timer = new RDMMessage(frame(ERDM_ResponseType.ACK_TIMER, 0, 0x00, 0x0A));
        var ack = new RDMMessage(frame(ERDM_ResponseType.ACK, 0, 0x00, 0x2A));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(timer.AcknowledgeTimer, Is.Not.Null);
            Assert.That(ack.AcknowledgeTimer, Is.Null, "only a timer response has a timer");
        }
    }

    [Test]
    public void TestAcknowledgeTimerWithWrongPdlIsNull()
    {
        // an ACK_TIMER has a PDL of 2, the response type is only kept for that PDL, so the message is built directly
        var message = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND_RESPONSE,
            Parameter = ERDM_Parameter.DMX_START_ADDRESS,
            ParameterData = new byte[] { 0x00, 0x0A, 0x00 }
        };
        message.PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK_TIMER;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK_TIMER));
            Assert.That(message.AcknowledgeTimer, Is.Null, "a timer with a wrong PDL is not parsed");
        }
    }

    [Test]
    public void TestControllerFlagsOnlyOfARequest()
    {
        var request = new RDMMessage() { Command = ERDM_Command.GET_COMMAND, ControllerFlags_or_MessageCounter = 0x01 };
        var response = new RDMMessage() { Command = ERDM_Command.GET_COMMAND_RESPONSE, ControllerFlags_or_MessageCounter = 0x01 };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(request.ControllerFlags, Is.EqualTo((ERDM_ControllerFlags)0x01));
            Assert.That(response.ControllerFlags, Is.Null, "a response has a message counter instead");
            Assert.That(response.MessageCounter, Is.EqualTo((byte)0x01));
            Assert.That(request.MessageCounter, Is.Null);
        }
    }

    [Test]
    public void TestPreambleCountOnlyOfADiscoveryResponse()
    {
        var discovery = new RDMMessage() { Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE, Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH };
        discovery.PreambleCount = 4;
        discovery.PreambleCount = 4; // the same value again
        Assert.That(discovery.PreambleCount, Is.EqualTo((byte)4));

        var otherCommand = new RDMMessage() { Command = ERDM_Command.GET_COMMAND, Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH };
        otherCommand.PreambleCount = 4;
        var otherParameter = new RDMMessage() { Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE, Parameter = ERDM_Parameter.DISC_MUTE };
        otherParameter.PreambleCount = 4;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(otherCommand.PreambleCount, Is.Null);
            Assert.That(otherParameter.PreambleCount, Is.Null);
        }
    }

    [Test]
    public void TestSettingTheSameCommandAndParameterAgain()
    {
        var message = new RDMMessage() { Command = ERDM_Command.GET_COMMAND, Parameter = ERDM_Parameter.DEVICE_LABEL };
        message.Command = ERDM_Command.GET_COMMAND;
        message.Parameter = ERDM_Parameter.DEVICE_LABEL;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.Command, Is.EqualTo(ERDM_Command.GET_COMMAND));
            Assert.That(message.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
        }
    }
    #endregion
}
