using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata.Parser;

/// <summary>
/// Faulty Payloads of real devices must not be interpreted:
/// <list type="bullet">
/// <item>If the PDL does not fit the define (too short, too long, incomplete list item), the Payload is not parsed.</item>
/// <item>A boolean field may only be 0x00 or 0x01, otherwise the Payload is not parsed.</item>
/// </list>
/// In both cases <see cref="RDMMessage.Value"/> has to be <c>null</c>.
/// The Payloads are generated from Wireshark captures (tools/RdmDumpTests/faulty_gen.py), see TestFaultyDevicePayloads.Dumps.cs.
/// </summary>
public partial class TestFaultyDevicePayloads
{
    private static readonly UID CONTROLLER_UID = new UID(0x1fff, 0x00000001);

    public readonly record struct FaultyPayload(ERDM_Command Command, ERDM_Parameter Parameter, ushort Manufacturer, byte[] Payload, string Reason)
    {
        public override string ToString() => $"{Parameter} ({Command}) 0x{Manufacturer:X4}: {Reason}";
    }

    private static IEnumerable<TestCaseData> testCases()
    {
        foreach (var payload in getDumpPayloads())
            yield return new TestCaseData(payload).SetArgDisplayNames(payload.ToString());
    }

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [TestCaseSource(nameof(testCases))]
    public void TestValueIsNull(FaultyPayload faulty)
    {
        bool response = faulty.Command.HasFlag(ERDM_Command.RESPONSE);
        UID device = new UID(faulty.Manufacturer, 0x12345678);
        var message = new RDMMessage()
        {
            Command = faulty.Command,
            Parameter = faulty.Parameter,
            SourceUID = response ? device : CONTROLLER_UID,
            DestUID = response ? CONTROLLER_UID : device,
            SubDevice = SubDevice.Root,
            ParameterData = faulty.Payload
        };
        if (response)
            Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));

        object? value = null;
        var task = Task.Run(() => value = message.Value);
        Assert.That(task.Wait(TimeSpan.FromSeconds(5)), Is.True, "parsing must not hang");
        Assert.That(value, Is.Null, faulty.Reason);
    }
}
