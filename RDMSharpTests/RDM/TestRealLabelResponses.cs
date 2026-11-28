using RDMSharp.Metadata;

namespace RDMSharpTests.RDM;

/// <summary>
/// Label responses of real devices with edge cases, captured on 2026-11-22 from a LumiNode12 (Art-Net) in a lab network:
/// full RDM frames (start code to checksum), all ACK with a valid checksum.
/// <list type="bullet">
/// <item>A NUL at the end inside the PDL: labels must not be NUL terminated (E1.20), the text ends at the NUL.</item>
/// <item>A space at the end: it is part of the text and is kept (a label written with a SET comes back the same).</item>
/// <item>A space and NUL padding: the text ends at the first NUL, the space is kept.</item>
/// <item>Devices of one model with a different format of the software version label: both are read as they are.</item>
/// </list>
/// </summary>
public class TestRealLabelResponses
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    public static IEnumerable<TestCaseData> Responses()
    {
        // NUL terminator inside the PDL
        yield return new TestCaseData("cc012302b00000301b5377000b180f01000000002100810b53574953534f4e204147000675", ERDM_Parameter.MANUFACTURER_LABEL, "SWISSON AG", false)
            .SetName("Swisson XPD-28 MANUFACTURER_LABEL with NUL");
        yield return new TestCaseData("cc011f02b00000301b5377000b180f0200000000210080075850442d3238000512", ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, "XPD-28", false)
            .SetName("Swisson XPD-28 DEVICE_MODEL_DESCRIPTION with NUL");
        yield return new TestCaseData("cc013302b00000301b5377000b180f03000000002100c01b5630312e30382e322e3834643636332e3531393136323337373600097f", ERDM_Parameter.SOFTWARE_VERSION_LABEL, "V01.08.2.84d663.5191623776", false)
            .SetName("Swisson XPD-28 SOFTWARE_VERSION_LABEL with NUL");
        yield return new TestCaseData("cc011e02b00000301b4c550000d5aa27000000002100c006322e352e30000609", ERDM_Parameter.SOFTWARE_VERSION_LABEL, "2.5.0", false)
            .SetName("LumenRadio CRMX Nova FX SOFTWARE_VERSION_LABEL with NUL");

        // a space and 13 NUL as padding (PDL 32): more than one NUL is reported as an issue, the value is read
        yield return new TestCaseData("cc013802b00000301b57536e9230912b000000002100c020572d444d582047345320322e31302e383136200000000000000000000000000009c2", ERDM_Parameter.SOFTWARE_VERSION_LABEL, "W-DMX G4S 2.10.816 ", true)
            .SetName("W-DMX G4S L5 Pro SOFTWARE_VERSION_LABEL with a space and NUL padding");

        // a space at the end only
        yield return new TestCaseData("cc012902b00000301b57536e9230912a0000000021008011572d444d5820473453204c352050726f2009a7", ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, "W-DMX G4S L5 Pro ", false)
            .SetName("W-DMX G4S L5 Pro DEVICE_MODEL_DESCRIPTION with a space");
        yield return new TestCaseData("cc012202b00000301b5347a5d3ac251a000000002100800a512d32205768697465200885", ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, "Q-2 White ", false)
            .SetName("SGM Q-2 White DEVICE_MODEL_DESCRIPTION with a space");
        yield return new TestCaseData("cc012902b00000301b53479471af2f1e0000000021008011492d322053696e676c6520436f6c6f72200aa9", ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, "I-2 Single Color ", false)
            .SetName("SGM I-2 Single Color DEVICE_MODEL_DESCRIPTION with a space");
        yield return new TestCaseData("cc011c02b00000301b5347166199f5220000000021008004502d3220061b", ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, "P-2 ", false)
            .SetName("SGM P-2 DEVICE_MODEL_DESCRIPTION with a space");

        // one model, two formats of the software version label
        yield return new TestCaseData("cc012202b00000301b676c000fffff0f000000002100c00a46522d312056312e323007f3", ERDM_Parameter.SOFTWARE_VERSION_LABEL, "FR-1 V1.20", false)
            .SetName("GLP FR-1 SOFTWARE_VERSION_LABEL long format");
        yield return new TestCaseData("cc011c02b00000301b676c000ffffd17000000002100c004312e32330684", ERDM_Parameter.SOFTWARE_VERSION_LABEL, "1.23", false)
            .SetName("GLP FR-1 SOFTWARE_VERSION_LABEL short format");
    }

    [TestCaseSource(nameof(Responses))]
    public void TestLabelResponse(string frame, ERDM_Parameter parameter, string expected, bool moreThanOneNul)
    {
        var message = new RDMMessage(Convert.FromHexString(frame));
        var define = MetadataFactory.GetDefine(new ParameterBag(parameter));
        var branch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse, message.ParameterData);
        var issues = (branch.Children ?? Array.Empty<DataTree>()).SelectMany(c => c.Issues ?? Array.Empty<DataTreeIssue>()).Select(i => i.ToString()).ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(message.ChecksumValid, Is.True);
            Assert.That(message.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
            Assert.That(message.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(message.Parameter, Is.EqualTo(parameter));
            Assert.That(MetadataFactory.ValidatePayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse, message.ParameterData, out _), Is.EqualTo(EPayloadValidation.Valid));
            Assert.That(message.Value, Is.EqualTo(expected), "the text ends at the first NUL, a space at the end is kept");
            if (moreThanOneNul)
                Assert.That(issues, Has.Some.Contains("Null-Delimiter"), "NUL padding is reported");
            else
                Assert.That(issues, Is.Empty);
        }
    }
}
