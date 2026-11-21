using RDMSharp.Metadata;
using RDMSharp.PayloadObject;

namespace RDMSharpTests.Metadata.Parser;

/// <summary>
/// Payloads of real devices, captured with Wireshark (Art-Net RDM and others).
/// The expected values are the values decoded by the Wireshark RDM dissector, not by RDMSharp,
/// so the parser is checked against an independent implementation. ACK_OVERFLOW responses are reassembled.
/// Payloads the dissector warns about, or that are not valid by the specification, are not part of these tests.
/// Generated, see the comment of every Payload for the capture and the frame.
/// </summary>
public partial class PayloadToParsedObjectTestSubject
{
    private static IEnumerable<DataTreeIssue> allIssues(DataTreeBranch branch)
        => (branch.Children ?? Array.Empty<DataTree>()).SelectMany(allIssues);
    private static IEnumerable<DataTreeIssue> allIssues(DataTree tree)
        => (tree.Issues ?? Array.Empty<DataTreeIssue>()).Concat((tree.Children ?? Array.Empty<DataTree>()).SelectMany(allIssues));

    private static IEnumerable<PayloadToParseBagData> getDumpPayloadList()
    {
        #region BOOT_SOFTWARE_VERSION_ID GET_COMMAND_RESPONSE
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 411
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x12, 0x32, 0x78, 0xa5 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x123278A5u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 5230
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x00, 0x01, 0x00, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x000100FFu));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 84
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x00000001u));
                }
            }));
        // DIHK 2025 1.pcapng, frame 2591
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x00, 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x00000100u));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 6899
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x00000000u));
                }
            }));
        // KLSTR Luminode Buddy Dump Wireless Solution.pcapng, frame 670
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x02, 0x00, 0x00, 0x7e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x0200007Eu));
                }
            }));
        // DZP Esprit Creos.pcapng, frame 244
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x5d, 0x8b, 0xdb, 0x71 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x5D8BDB71u));
                }
            }));
        // GLP Creos O FW 1.0.0-rc.10.pcapng, frame 60
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x03, 0x00, 0x01, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0x030001FFu));
                }
            }));
        #endregion

        #region BOOT_SOFTWARE_VERSION_LABEL GET_COMMAND_RESPONSE
        // Lollapalooza 2026 1.pcapng, frame 7792, characters after the NUL terminator
        // Known deviation of manufacturer 0x6A6A: ROXX (Cluster B4 FC FW V1.6) sends a character after the NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x31, 0x2e, 0x36, 0x00, 0x43 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: a NUL terminates the String, the characters after it are ignored and reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Trailing Characters"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V1.6"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x31, 0x2e, 0x36 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 425, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x30, 0x31, 0x2e, 0x30, 0x34, 0x2e, 0x31, 0x2e, 0x38, 0x31, 0x31, 0x35, 0x33, 0x31, 0x2e, 0x33, 0x39, 0x39, 0x37, 0x33, 0x37, 0x32, 0x35, 0x36, 0x31, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V01.04.1.811531.3997372561"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x30, 0x31, 0x2e, 0x30, 0x34, 0x2e, 0x31, 0x2e, 0x38, 0x31, 0x31, 0x35, 0x33, 0x31, 0x2e, 0x33, 0x39, 0x39, 0x37, 0x33, 0x37, 0x32, 0x35, 0x36, 0x31 });
        // Blackcoffe 1.pcapng, frame 5685
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x30, 0x2e, 0x31, 0x2e, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("0.1.0"));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 86
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x35, 0x2e, 0x34 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V5.4"));
                }
            }));
        // DIHK 2025 2.pcapng, frame 327
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x30, 0x2e, 0x30, 0x2e, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("0.0.0"));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 7049, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x31, 0x2e, 0x35, 0x20, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V1.5 "));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x31, 0x2e, 0x35, 0x20 });
        // Innotrans 1 elation KL und Litekraft wash XT.pcapng, frame 364
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x31, 0x2e, 0x31, 0x2e, 0x33 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V1.1.3"));
                }
            }));
        // KLSTR Luminode Buddy Dump Wireless Solution.pcapng, frame 672, String padded with NUL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x42, 0x6f, 0x6f, 0x74, 0x6c, 0x6f, 0x61, 0x64, 0x65, 0x72, 0x20, 0x32, 0x2e, 0x30, 0x2e, 0x31, 0x32, 0x36, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: the PDL has to match the String, but a NUL terminates it, the padding is reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Null-Delimiter"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Bootloader 2.0.126 "));
                }
            }), normalizedPayload: new byte[] { 0x42, 0x6f, 0x6f, 0x74, 0x6c, 0x6f, 0x61, 0x64, 0x65, 0x72, 0x20, 0x32, 0x2e, 0x30, 0x2e, 0x31, 0x32, 0x36, 0x20 });
        #endregion

        #region CAPTURE_PRESET SET_COMMAND
        // Lumisplit 1.6 FW 7.0.pcapng, frame 251
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CAPTURE_PRESET,
            new byte[] { 0x00, 0x02, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCapturePreset)));
                var obj = (RDMCapturePreset)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SceneId, Is.EqualTo((ushort)2));
                    Assert.That(obj.UpFadeTime, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.DownFadeTime, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.WaitTime, Is.EqualTo(6553.5).Within(0.001));
                }
            }));
        #endregion

        #region COMMS_STATUS GET_COMMAND_RESPONSE
        // Estrel 14_01_2026_2.pcapng, frame 6062
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.COMMS_STATUS,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCommunicationStatus)));
                var obj = (RDMCommunicationStatus)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ShortMessage, Is.EqualTo((ushort)0));
                    Assert.That(obj.LengthMismatch, Is.EqualTo((ushort)0));
                    Assert.That(obj.ChecksumFail, Is.EqualTo((ushort)0));
                }
            }));
        #endregion

        #region CURVE GET_COMMAND_RESPONSE
        // ACME Pixel Line IP V22 Curve Bug.pcapng, frame 96
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x03, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)3));
                    Assert.That(obj.Curves, Is.EqualTo((byte)4));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 160
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x01, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)1));
                    Assert.That(obj.Curves, Is.EqualTo((byte)5));
                }
            }));
        // Coemar Relite MK2 v0.87.pcapng, frame 548
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x03, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)3));
                    Assert.That(obj.Curves, Is.EqualTo((byte)5));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 1487
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x05, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)5));
                    Assert.That(obj.Curves, Is.EqualTo((byte)6));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 252
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x02, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)2));
                    Assert.That(obj.Curves, Is.EqualTo((byte)3));
                }
            }));
        // ACME Pixel Line IP V25.pcapng, frame 80
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x02, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)2));
                    Assert.That(obj.Curves, Is.EqualTo((byte)4));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 19213
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x02, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)2));
                    Assert.That(obj.Curves, Is.EqualTo((byte)5));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 155
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x03, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));
                var obj = (RDMCurve)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentCurveId, Is.EqualTo((byte)3));
                    Assert.That(obj.Curves, Is.EqualTo((byte)6));
                }
            }));
        #endregion

        #region CURVE SET_COMMAND
        // ACME Pixel Line IP V22 Curve Bug.pcapng, frame 83
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CURVE,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 341
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CURVE,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)5));
                }
            }));
        // ACME Pixel Line IP V22 Curve Bug.pcapng, frame 85
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CURVE,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // ACME Pixel Line IP V22 Curve Bug.pcapng, frame 87
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CURVE,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        // ACME Pixel Line IP V22 Curve Bug.pcapng, frame 99
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CURVE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        #endregion

        #region CURVE_DESCRIPTION GET_COMMAND
        // Astera AX2 5.14.84 CurveDescription Bug.pcapng, frame 11
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)5));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 29
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)0));
                }
            }));
        // Astera AX2 5.14.84 CurveDescription Bug.pcapng, frame 13
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)6));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 31
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // Astera AX2 5.14.84 CurveDescription Bug.pcapng, frame 15
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)7));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 33
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // Astera AX2 5.14.84 CurveDescription Bug.pcapng, frame 17
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)8));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 35
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        #endregion

        #region CURVE_DESCRIPTION GET_COMMAND_RESPONSE
        // ACME Pixel Line IP V25.pcapng, frame 56
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01, 0x4c, 0x69, 0x6e, 0x65, 0x61, 0x72 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Linear"));
                }
            }));
        // Astera AX2 5.14.84 CurveDescription Bug.pcapng, frame 4
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01, 0x46, 0x41, 0x53, 0x54 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("FAST"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 141, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01, 0x4c, 0x69, 0x6e, 0x65, 0x61, 0x72, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Linear"));
                }
            }), normalizedPayload: new byte[] { 0x01, 0x4c, 0x69, 0x6e, 0x65, 0x61, 0x72 });
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 110
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x05, 0x48, 0x61, 0x6c, 0x6f, 0x67, 0x65, 0x6e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)5));
                    Assert.That(obj.Description, Is.EqualTo("Halogen"));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 74
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)0));
                    Assert.That(obj.Description, Is.Null.Or.Empty);
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 32
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01, 0x6c, 0x69, 0x6e, 0x65, 0x61, 0x72, 0x20, 0x63, 0x75, 0x72, 0x76, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("linear curve"));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 642
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x00, 0x53, 0x20, 0x43, 0x75, 0x72, 0x76, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)0));
                    Assert.That(obj.Description, Is.EqualTo("S Curve"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6598, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01, 0x49, 0x4e, 0x45, 0x41, 0x52, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));
                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurveId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("INEAR"));
                }
            }), normalizedPayload: new byte[] { 0x01, 0x49, 0x4e, 0x45, 0x41, 0x52 });
        #endregion

        #region DEFAULT_SLOT_VALUE GET_COMMAND_RESPONSE
        // Ayrton Diabolo S 2.3.0.pcapng, frame 152
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x80, 0x00, 0x01, 0x00, 0x00, 0x02, 0x80, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x06, 0x00, 0x00, 0x07, 0x00, 0x00, 0x08, 0x00, 0x00, 0x09, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x0d, 0x00, 0x00, 0x0e, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x10, 0x00, 0x00, 0x11, 0x00, 0x00, 0x12, 0x00, 0x00, 0x13, 0x00, 0x00, 0x14, 0x00, 0x00, 0x15, 0x00, 0x00, 0x16, 0x00, 0x00, 0x17, 0x00, 0x00, 0x18, 0x00, 0x00, 0x19, 0x00, 0x00, 0x1a, 0x00, 0x00, 0x1b, 0x00, 0x00, 0x1c, 0x00, 0x00, 0x1d, 0x00, 0x00, 0x1e, 0x00, 0x00, 0x1f, 0x00, 0x00, 0x20, 0x00, 0x00, 0x21, 0x00, 0x00, 0x22, 0x00, 0x00, 0x23, 0x00, 0x00, 0x24, 0x00, 0x00, 0x25, 0x00, 0x00, 0x26, 0x00, 0x00, 0x27, 0x00, 0x00, 0x28, 0x00, 0x00, 0x29, 0x00, 0x00, 0x2a, 0x00, 0x00, 0x2b, 0x00, 0x00, 0x2c, 0x00, 0x00, 0x2d, 0x00, 0x00, 0x2e, 0x00, 0x00, 0x2f, 0x00, 0x00, 0x30, 0x00, 0x00, 0x31, 0x00, 0x00, 0x32, 0x00, 0x00, 0x33, 0x00, 0x00, 0x34, 0x00, 0x00, 0x35, 0x00, 0x00, 0x36, 0x00, 0x00, 0x37, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 128, 0, 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
                }
            }));
        // Blackcoffe 1.pcapng, frame 9437
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x80, 0x00, 0x01, 0x00, 0x00, 0x02, 0x80, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x06, 0x00, 0x00, 0x07, 0x00, 0x00, 0x08, 0x00, 0x00, 0x09, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x0d, 0x00, 0x00, 0x0e, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x10, 0x00, 0x00, 0x11, 0x00, 0x00, 0x12, 0x00, 0x00, 0x13, 0x00, 0x00, 0x14, 0x00, 0x00, 0x15, 0x00, 0x00, 0x16, 0x00, 0x00, 0x17, 0x00, 0x00, 0x18, 0x00, 0x00, 0x19, 0x00, 0x00, 0x1a, 0x00, 0x00, 0x1b, 0x00, 0x00, 0x1c, 0x00, 0x00, 0x1d, 0x00, 0x00, 0x1e, 0x00, 0x00, 0x1f, 0x80, 0x00, 0x20, 0x00, 0x00, 0x21, 0x80, 0x00, 0x22, 0x00, 0x00, 0x23, 0x00, 0x00, 0x24, 0x00, 0x00, 0x25, 0x00, 0x00, 0x26, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 128, 0, 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 0, 128, 0, 0, 0, 0, 0 }));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 78
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x06, 0x00, 0x00, 0x07, 0x00, 0x00, 0x08, 0x00, 0x00, 0x09, 0x00, 0x00, 0x0a, 0xff, 0x00, 0x0b, 0xff, 0x00, 0x0c, 0xff, 0x00, 0x0d, 0x80, 0x00, 0x0e, 0x80, 0x00, 0x0f, 0x80, 0x00, 0x10, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 128, 128, 128, 128 }));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 221
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x06, 0x00, 0x00, 0x07, 0x00, 0x00, 0x08, 0x00, 0x00, 0x09, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 5948
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x06, 0x00, 0x00, 0x07, 0x00, 0x00, 0x08, 0x00, 0x00, 0x09, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x0d, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
                }
            }));
        // Innotrans 1 elation KL und Litekraft wash XT.pcapng, frame 126
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x06, 0x00, 0x00, 0x07, 0x00, 0x00, 0x08, 0x00, 0x00, 0x09, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x0d, 0x00, 0x00, 0x0e, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 1015
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 0, 0, 0 }));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 9109
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x03, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = (RDMDefaultSlotValue[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3 }));
                    Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
                }
            }));
        #endregion

        #region DEVICE_HOURS GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 132
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x46 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(70u));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1322
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x1c, 0xd4 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(7380u));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1343
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x0a, 0x17 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2583u));
                }
            }));
        // Astera AX2 5.14.84 (2).pcapng, frame 550
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x06, 0x94 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1684u));
                }
            }));
        // Bairut Colorado and Rivale.pcapng, frame 2053
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x02, 0xbd },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(701u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 7758
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0xdc },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(220u));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 166
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x14 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(20u));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 94
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x0b, 0x24 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2852u));
                }
            }));
        #endregion

        #region DEVICE_INFO GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 2
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x05, 0x39, 0x01, 0x02, 0x05, 0x39, 0x00, 0x14, 0x00, 0x61, 0x03, 0x04, 0x01, 0x24, 0x00, 0x00, 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)1337));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0102));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x05390014u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)97));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)3));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)4));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)292));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)9));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 274
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x00, 0x0b, 0x08, 0x01, 0xe0, 0x7d, 0x35, 0x82, 0x00, 0x00, 0x01, 0x01, 0xff, 0xff, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)11));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x08));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0801));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0xE07D3582u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)0));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)1));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)1));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)65535));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)0));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 473
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x00, 0xc5, 0x01, 0x00, 0x00, 0x00, 0x00, 0x28, 0x00, 0x1b, 0x02, 0x0a, 0x00, 0xce, 0x00, 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)197));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0100));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x00000028u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)27));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)2));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)10));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)206));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)3));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 486
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x0b, 0x04, 0x01, 0x02, 0x22, 0x20, 0x01, 0x10, 0x00, 0x29, 0x01, 0x06, 0x00, 0x2a, 0x00, 0x00, 0x13 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)2820));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0102));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x22200110u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)41));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)1));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)6));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)42));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)19));
                }
            }));
        // Astera AX2 5.14.84 (2).pcapng, frame 2
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0xa5, 0x28, 0x01, 0x01, 0x00, 0x05, 0x0e, 0x54, 0x00, 0x00, 0x18, 0x22, 0x01, 0x00, 0xc1, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)165));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)10241));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0100));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x050E5400u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)24));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)34));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)1));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)193));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)2));
                }
            }));
        // Bairut Colorado and Rivale.pcapng, frame 13
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x08, 0x33, 0x01, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0xb3, 0x04, 0x07, 0x00, 0xb4, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)2099));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0102));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x00000001u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)179));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)4));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)7));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)180));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)1));
                }
            }));
        // Blackcoffe 1.pcapng, frame 1875
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x10, 0x05, 0x01, 0x01, 0x00, 0x03, 0x04, 0x00, 0x00, 0x57, 0x03, 0x0a, 0x00, 0x01, 0x00, 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)4101));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0101));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x00030400u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)87));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)3));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)10));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)1));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)6));
                }
            }));
        // Cameo Zenit 600 SMD.pcapng, frame 25
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] { 0x01, 0x00, 0x00, 0x55, 0x01, 0x01, 0x00, 0x00, 0x00, 0x16, 0x00, 0x0f, 0x08, 0x0e, 0x00, 0x2e, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));
                var obj = (RDMDeviceInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo((byte)1));
                    Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo((byte)0));
                    Assert.That(obj.DeviceModelId, Is.EqualTo((ushort)85));
                    Assert.That(obj.ProductCategoryCoarse, Is.EqualTo((ERDM_ProductCategoryCoarse)0x01));
                    Assert.That(obj.ProductCategoryFine, Is.EqualTo((ERDM_ProductCategoryFine)0x0101));
                    Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x00000016u));
                    Assert.That(obj.Dmx512Footprint, Is.EqualTo((ushort)15));
                    Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo((byte)8));
                    Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo((byte)14));
                    Assert.That(obj.Dmx512StartAddress, Is.EqualTo((ushort)46));
                    Assert.That(obj.SubDeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.SensorCount, Is.EqualTo((byte)1));
                }
            }));
        #endregion

        #region DEVICE_LABEL GET_COMMAND_RESPONSE
        // DIHK 2025 1.pcapng, frame 189, characters after the NUL terminator
        // Known deviation of manufacturer 0x5347: SGM (e.g. Q-7 FW V. 3.28) fills the label up to 32 bytes with 0xFF after the NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x51, 0x2d, 0x37, 0x20, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: a NUL terminates the String, the characters after it are ignored and reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Trailing Characters"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Q-7 "));
                }
            }), normalizedPayload: new byte[] { 0x51, 0x2d, 0x37, 0x20 });
        // Estrel 14_01_2026_2.pcapng, frame 1875, characters after the NUL terminator
        // Known deviation of manufacturer 0x5347: SGM (e.g. Q-7 FW V. 3.28) fills the label up to 32 bytes with 0xFF after the NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x49, 0x2d, 0x35, 0x20, 0x52, 0x47, 0x42, 0x57, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: a NUL terminates the String, the characters after it are ignored and reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Trailing Characters"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("I-5 RGBW"));
                }
            }), normalizedPayload: new byte[] { 0x49, 0x2d, 0x35, 0x20, 0x52, 0x47, 0x42, 0x57 });
        // ACME CM S2 V20.pcapng, frame 102, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x43, 0x4d, 0x20, 0x53, 0x32, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("CM S2"));
                }
            }), normalizedPayload: new byte[] { 0x43, 0x4d, 0x20, 0x53, 0x32 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 325, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x58, 0x50, 0x44, 0x2d, 0x32, 0x38, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("XPD-28"));
                }
            }), normalizedPayload: new byte[] { 0x58, 0x50, 0x44, 0x2d, 0x32, 0x38 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 755, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x56, 0x20, 0x33, 0x2e, 0x36, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V 3.6"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x20, 0x33, 0x2e, 0x36 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 775, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x45, 0x55, 0x52, 0x55, 0x53, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("EURUS"));
                }
            }), normalizedPayload: new byte[] { 0x45, 0x55, 0x52, 0x55, 0x53 });
        // Astera AX2 5.14.84 (2).pcapng, frame 534, String padded with NUL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x41, 0x58, 0x32, 0x2d, 0x31, 0x30, 0x30, 0x20, 0x43, 0x61, 0x73, 0x65, 0x20, 0x37, 0x20, 0x23, 0x32, 0x36, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: the PDL has to match the String, but a NUL terminates it, the padding is reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Null-Delimiter"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("AX2-100 Case 7 #26"));
                }
            }), normalizedPayload: new byte[] { 0x41, 0x58, 0x32, 0x2d, 0x31, 0x30, 0x30, 0x20, 0x43, 0x61, 0x73, 0x65, 0x20, 0x37, 0x20, 0x23, 0x32, 0x36 });
        // Blackcoffe 1.pcapng, frame 4502
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x4a, 0x44, 0x43, 0x20, 0x42, 0x75, 0x72, 0x73, 0x74, 0x20, 0x31 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("JDC Burst 1"));
                }
            }));
        #endregion

        #region DEVICE_LABEL SET_COMMAND
        // Coemar Relite MK2 v0.94.pcapng, frame 3072
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x73, 0x73, 0x73 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("sss"));
                }
            }));
        // Elation FuzePar Z60.pcapng, frame 1715
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x46, 0x75, 0x7a, 0x65, 0x20, 0x50, 0x61, 0x72, 0x35, 0x35 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Fuze Par55"));
                }
            }));
        // Elation FuzePar Z60.pcapng, frame 1739
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x46, 0x75, 0x7a, 0x65, 0x20, 0x50, 0x61, 0x72, 0x36, 0x36 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Fuze Par66"));
                }
            }));
        #endregion

        #region DEVICE_MODEL_DESCRIPTION GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 6
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x43, 0x4d, 0x20, 0x53, 0x32 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("CM S2"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 342, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x58, 0x50, 0x44, 0x2d, 0x32, 0x38, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("XPD-28"));
                }
            }), normalizedPayload: new byte[] { 0x58, 0x50, 0x44, 0x2d, 0x32, 0x38 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 860
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x52, 0x6f, 0x62, 0x69, 0x6e, 0x20, 0x53, 0x70, 0x69, 0x69, 0x64, 0x65, 0x72 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Robin Spiider"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 877
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x45, 0x55, 0x52, 0x55, 0x53, 0x20, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("EURUS S"));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 8
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x41, 0x58, 0x35 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("AX5"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2274, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x47, 0x4c, 0x50, 0x20, 0x4a, 0x44, 0x43, 0x20, 0x42, 0x75, 0x72, 0x73, 0x74, 0x20, 0x31, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("GLP JDC Burst 1"));
                }
            }), normalizedPayload: new byte[] { 0x47, 0x4c, 0x50, 0x20, 0x4a, 0x44, 0x43, 0x20, 0x42, 0x75, 0x72, 0x73, 0x74, 0x20, 0x31 });
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 118, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x43, 0x4f, 0x4c, 0x4f, 0x52, 0x61, 0x64, 0x6f, 0x20, 0x50, 0x58, 0x4c, 0x20, 0x43, 0x75, 0x72, 0x76, 0x65, 0x20, 0x31, 0x32, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("COLORado PXL Curve 12"));
                }
            }), normalizedPayload: new byte[] { 0x43, 0x4f, 0x4c, 0x4f, 0x52, 0x61, 0x64, 0x6f, 0x20, 0x50, 0x58, 0x4c, 0x20, 0x43, 0x75, 0x72, 0x76, 0x65, 0x20, 0x31, 0x32 });
        // Cameo Zenit W600 G2 v1.7.3 Update to v1.8.4.pcapng, frame 2
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x5a, 0x45, 0x4e, 0x49, 0x54, 0x20, 0x57, 0x36, 0x30, 0x30, 0x20, 0x47, 0x32 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("ZENIT W600 G2"));
                }
            }));
        #endregion

        #region DEVICE_POWER_CYCLES GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 8284
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x37 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(55u));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 10269
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0u));
                }
            }));
        // MELH 1_7.pcapng, frame 8410
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x1a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(26u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 8294
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x28 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(40u));
                }
            }));
        // MELH 1_7.pcapng, frame 11958
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x25 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(37u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 8296
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x26 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(38u));
                }
            }));
        // MELH 1_7.pcapng, frame 14291
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x19 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(25u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 8297
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x24 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(36u));
                }
            }));
        #endregion

        #region DEVICE_UNIT_NUMBER GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 210
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_UNIT_NUMBER,
            new byte[] { 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0u));
                }
            }));
        #endregion

        #region DIMMER_INFO GET_COMMAND_RESPONSE
        // GenVI Dimmer v1.08.pcapng, frame 618
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DIMMER_INFO,
            new byte[] { 0xff, 0xff, 0x10, 0x40, 0xff, 0xff, 0x03, 0x41, 0x03, 0x10, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDimmerInfo)));
                var obj = (RDMDimmerInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.MinimumLevelLowerLimit, Is.EqualTo((ushort)65535));
                    Assert.That(obj.MinimumLevelUpperLimit, Is.EqualTo((ushort)4160));
                    Assert.That(obj.MaximumLevelLowerLimit, Is.EqualTo((ushort)65535));
                    Assert.That(obj.MaximumLevelUpperLimit, Is.EqualTo((ushort)833));
                    Assert.That(obj.NumberOfSupportedCurves, Is.EqualTo((byte)3));
                    Assert.That(obj.LevelsResolution, Is.EqualTo((byte)16));
                    Assert.That(obj.MinimumLevelSplitLevelsSupported, Is.EqualTo(false));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 644
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DIMMER_INFO,
            new byte[] { 0xff, 0xff, 0x20, 0x43, 0xff, 0xff, 0x76, 0x65, 0x03, 0x10, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDimmerInfo)));
                var obj = (RDMDimmerInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.MinimumLevelLowerLimit, Is.EqualTo((ushort)65535));
                    Assert.That(obj.MinimumLevelUpperLimit, Is.EqualTo((ushort)8259));
                    Assert.That(obj.MaximumLevelLowerLimit, Is.EqualTo((ushort)65535));
                    Assert.That(obj.MaximumLevelUpperLimit, Is.EqualTo((ushort)30309));
                    Assert.That(obj.NumberOfSupportedCurves, Is.EqualTo((byte)3));
                    Assert.That(obj.LevelsResolution, Is.EqualTo((byte)16));
                    Assert.That(obj.MinimumLevelSplitLevelsSupported, Is.EqualTo(false));
                }
            }));
        #endregion

        #region DISCOVERY_STATE GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 275
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 277
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 279
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 281
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 283
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 285
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 287
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region DISCOVERY_STATE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 276
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x08, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 286
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x08, 0x00, 0x01, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)1));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 100
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x01, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 124
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x03, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 142
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x04, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 160
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x05, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 178
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x06, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 196
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x07, 0x00, 0x00, 0x80 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
                var obj = (GetDiscoveryStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.DiscoveryState, Is.EqualTo((ERDM_DiscoveryState)0x80));
                }
            }));
        #endregion

        #region DISPLAY_INVERT GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 6750
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_INVERT,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));
                var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_DisplayInvert)0x02));
                }
            }));
        // DIHK 2025 1.pcapng, frame 316
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_INVERT,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));
                var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_DisplayInvert)0x01));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 258
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_INVERT,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));
                var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_DisplayInvert)0x00));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 2687
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_INVERT,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));
                var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_DisplayInvert)0x03));
                }
            }));
        #endregion

        #region DISPLAY_INVERT SET_COMMAND
        // DZP Esprit Creos.pcapng, frame 5428
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DISPLAY_INVERT,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));
                var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_DisplayInvert)0x02));
                }
            }));
        #endregion

        #region DISPLAY_LEVEL GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 7015
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_LEVEL,
            new byte[] { 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)255));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 132
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_LEVEL,
            new byte[] { 0xcc },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)204));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 7797
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_LEVEL,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)0));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 254
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_LEVEL,
            new byte[] { 0x71 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)113));
                }
            }));
        #endregion

        #region DMX_BLOCK_ADDRESS GET_COMMAND_RESPONSE
        // GenVI Dimmer v1.08.pcapng, frame 546
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_BLOCK_ADDRESS,
            new byte[] { 0x00, 0x0c, 0x01, 0xf5 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXBlockAddress)));
                var obj = (RDMDMXBlockAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.TotalSubDeviceFootprint, Is.EqualTo((ushort)12));
                    Assert.That(obj.BaseDMX512Address, Is.EqualTo((ushort)501));
                }
            }));
        #endregion

        #region DMX_FAIL_MODE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 158
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)0));
                    Assert.That(obj.Delay, Is.EqualTo(0.1).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8098
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0xec, 0x05, 0x40, 0x39, 0x32, 0x35 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)236));
                    Assert.That(obj.Delay, Is.EqualTo(134.4).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(1464.2).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)53));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8148
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0xe1, 0x04, 0x56, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)225));
                    Assert.That(obj.Delay, Is.EqualTo(111.0).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8178
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0x96, 0x32, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)150));
                    Assert.That(obj.Delay, Is.EqualTo(1280.0).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)0));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8352
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0x82, 0x22, 0x80, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)130));
                    Assert.That(obj.Delay, Is.EqualTo(883.2).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 10410
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0x6d, 0x70, 0xc4, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)109));
                    Assert.That(obj.Delay, Is.EqualTo(2886.8).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 12255
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0x70, 0x4c, 0x80, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)112));
                    Assert.That(obj.Delay, Is.EqualTo(1958.4).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 14991
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0xde, 0x43, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)222));
                    Assert.That(obj.Delay, Is.EqualTo(1715.2).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)0));
                }
            }));
        #endregion

        #region DMX_PERSONALITY GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 106
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x03, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)3));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)4));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1279
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x01, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)1));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)6));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1569
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x02, 0x0a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)2));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)10));
                }
            }));
        // Astera AX2 5.14.84 (2).pcapng, frame 540
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x22, 0xa5 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)34));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)165));
                }
            }));
        // Blackcoffe 1.pcapng, frame 5998
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x08, 0x0a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)8));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)10));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 1172
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x04, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)4));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)7));
                }
            }));
        // Cameo Zenit 600 SMD.pcapng, frame 4
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x08, 0x0e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)8));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)14));
                }
            }));
        // Chroma-Q Color Force II 72 CFII 72 v1.12.pcapng, frame 44
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x01, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));
                var obj = (RDMDMXPersonality)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentPersonality, Is.EqualTo((byte)1));
                    Assert.That(obj.OfPersonalities, Is.EqualTo((byte)4));
                }
            }));
        #endregion

        #region DMX_PERSONALITY SET_COMMAND
        // Cameo Zenit W600 G2 v1.7.3 Update to v1.8.4.pcapng, frame 51
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)5));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 5344
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)6));
                }
            }));
        // Elation FuzePar Z60.pcapng, frame 1599
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)9));
                }
            }));
        // JBL P10.pcapng, frame 805
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // MELH 1_7.pcapng, frame 10133
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 5458
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        // MELH 1_7.pcapng, frame 10157
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        #endregion

        #region DMX_PERSONALITY_DESCRIPTION GET_COMMAND
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 94
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 96
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 98
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 100
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 102
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)5));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 104
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)6));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 106
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)7));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 108
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)8));
                }
            }));
        #endregion

        #region DMX_PERSONALITY_DESCRIPTION GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 12
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x18, 0x32, 0x34, 0x43, 0x48 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)24));
                    Assert.That(obj.Description, Is.EqualTo("24CH"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1717
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x31, 0x44, 0x4d, 0x58, 0x20, 0x50, 0x72, 0x65, 0x73, 0x65, 0x74, 0x20, 0x31, 0x20, 0x2d, 0x20, 0x66, 0x6f, 0x6f, 0x74, 0x70, 0x72, 0x69, 0x6e, 0x74, 0x20, 0x34, 0x39 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)49));
                    Assert.That(obj.Description, Is.EqualTo("DMX Preset 1 - footprint 49"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1805
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x29, 0x53, 0x74, 0x61, 0x6e, 0x64, 0x20, 0x4d, 0x6f, 0x64, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)41));
                    Assert.That(obj.Description, Is.EqualTo("Stand Mode"));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 14
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x03, 0x31, 0x3a, 0x20, 0x52, 0x47, 0x42 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)3));
                    Assert.That(obj.Description, Is.EqualTo("1: RGB"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2326
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x13, 0x42, 0x61, 0x73, 0x69, 0x63 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)19));
                    Assert.That(obj.Description, Is.EqualTo("Basic"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 122
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x35, 0x4d, 0x4f, 0x44, 0x45, 0x20, 0x31, 0x20, 0x2d, 0x20, 0x20, 0x35, 0x33, 0x43, 0x48 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)53));
                    Assert.That(obj.Description, Is.EqualTo("MODE 1 -  53CH"));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3 Update to v1.8.4.pcapng, frame 23
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x04, 0x34, 0x43, 0x48, 0x20, 0x52, 0x47, 0x42 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)4));
                    Assert.That(obj.Description, Is.EqualTo("4CH RGB"));
                }
            }));
        // Chroma-Q Color Force II 72 CFII 72 v1.12.pcapng, frame 16, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x62, 0x52, 0x47, 0x42, 0x41, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));
                var obj = (RDMDMXPersonalityDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.PersonalityId, Is.EqualTo((byte)1));
                    Assert.That(obj.Slots, Is.EqualTo((ushort)98));
                    Assert.That(obj.Description, Is.EqualTo("RGBA"));
                }
            }), normalizedPayload: new byte[] { 0x01, 0x00, 0x62, 0x52, 0x47, 0x42, 0x41 });
        #endregion

        #region DMX_STARTUP_MODE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 162
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)0));
                    Assert.That(obj.Delay, Is.EqualTo(0.1).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8147
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0xee, 0x04, 0x56, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)238));
                    Assert.That(obj.Delay, Is.EqualTo(111.0).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8185
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0xe3, 0x29, 0x80, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)227));
                    Assert.That(obj.Delay, Is.EqualTo(1062.4).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8201
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0x98, 0x29, 0x80, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)152));
                    Assert.That(obj.Delay, Is.EqualTo(1062.4).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8385
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0x84, 0x03, 0xf1, 0x09, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)132));
                    Assert.That(obj.Delay, Is.EqualTo(100.9).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(230.4).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)0));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 10495
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0x6f, 0x1d, 0x80, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)111));
                    Assert.That(obj.Delay, Is.EqualTo(755.2).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 12550
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0x72, 0xea, 0x80, 0x00, 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)114));
                    Assert.That(obj.Delay, Is.EqualTo(6003.2).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(12.7).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 15096
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0xe0, 0x44, 0x2d, 0x32, 0x38, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));
                var obj = (RDMDMX_xxxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Scene, Is.EqualTo((ushort)224));
                    Assert.That(obj.Delay, Is.EqualTo(1745.3).Within(0.001));
                    Assert.That(obj.HoldTime, Is.EqualTo(1285.6).Within(0.001));
                    Assert.That(obj.Level, Is.EqualTo((byte)0));
                }
            }));
        #endregion

        #region DMX_START_ADDRESS GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 108
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x01, 0x24 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)292));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 291
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)65535));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 555
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0xce },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)206));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 572
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0x2a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)42));
                }
            }));
        // Astera AX2 5.14.84 (2).pcapng, frame 542
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0xc1 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)193));
                }
            }));
        // Bairut Colorado and Rivale.pcapng, frame 1981
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0xb4 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)180));
                }
            }));
        // Blackcoffe 1.pcapng, frame 6468
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)69));
                }
            }));
        // Cameo Zenit 600 SMD.pcapng, frame 7
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0x2e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)46));
                }
            }));
        #endregion

        #region DMX_START_ADDRESS SET_COMMAND
        // Cameo Zenit W600 G2 v1.7.3 Update to v1.8.4.pcapng, frame 52
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x01, 0xc3 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)451));
                }
            }));
        #endregion

        #region DNS_IPV4_NAME_SERVER GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 235
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DNS_IPV4_NAME_SERVER,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)0));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 237
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DNS_IPV4_NAME_SERVER,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        #endregion

        #region DNS_IPV4_NAME_SERVER GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 236
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DNS_IPV4_NAME_SERVER,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4NameServer)));
                var obj = (GetSetIPv4NameServer)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.NameServerIndex, Is.EqualTo((byte)0));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("0.0.0.0")));
                }
            }));
        #endregion

        #region ENDPOINT_LABEL GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 247
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 249
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 251
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 253
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 255
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 257
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 259
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region ENDPOINT_LABEL GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 248, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x01, 0x5a, 0x6f, 0x6e, 0x65, 0x20, 0x41, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Zone A"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x01, 0x5a, 0x6f, 0x6e, 0x65, 0x20, 0x41 });
        // DevCon 2026 Dump 1.pcapng, frame 250, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x03, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x31, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Port 1"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x03, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x31 });
        // DevCon 2026 Dump 1.pcapng, frame 252, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x04, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x32, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Port 2"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x04, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x32 });
        // DevCon 2026 Dump 1.pcapng, frame 254, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x05, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x33, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Port 3"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x05, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x33 });
        // DevCon 2026 Dump 1.pcapng, frame 256, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x06, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x34, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Port 4"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x06, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x34 });
        // DevCon 2026 Dump 1.pcapng, frame 258, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x07, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x35, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Port 5"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x07, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x35 });
        // DevCon 2026 Dump 1.pcapng, frame 260, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x08, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x36, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));
                var obj = (GetSetEndpointLabel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.EndpointLabel, Is.EqualTo("Port 6"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x08, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x36 });
        #endregion

        #region ENDPOINT_LIST GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 198
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LIST,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x03, 0x01, 0x00, 0x04, 0x01, 0x00, 0x05, 0x01, 0x00, 0x06, 0x01, 0x00, 0x07, 0x01, 0x00, 0x08, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointListResponse)));
                var obj = (GetEndpointListResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.Endpoints.Select(e => e.EndpointId), Is.EqualTo(new ushort[] { 1, 3, 4, 5, 6, 7, 8 }));
                    Assert.That(obj.Endpoints.Select(e => e.EndpointType), Is.EqualTo(new ERDM_EndpointType[] { (ERDM_EndpointType)1, (ERDM_EndpointType)1, (ERDM_EndpointType)1, (ERDM_EndpointType)1, (ERDM_EndpointType)1, (ERDM_EndpointType)1, (ERDM_EndpointType)1 }));
                }
            }));
        #endregion

        #region ENDPOINT_LIST_CHANGE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 202
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LIST_CHANGE,
            new byte[] { 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0u));
                }
            }));
        #endregion

        #region ENDPOINT_MODE GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 233
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 235
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 237
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 239
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 241
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 243
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 245
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region ENDPOINT_MODE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 234
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x01));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 236
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x03, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x02));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 238
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x04, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x02));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 240
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x05, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x02));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 242
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x06, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x02));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 244
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x07, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x02));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 246
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x08, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));
                var obj = (GetSetEndpointMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.EndpointMode, Is.EqualTo((ERDM_EndpointMode)0x02));
                }
            }));
        #endregion

        #region ENDPOINT_RESPONDERS GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 303
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 305
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 307
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 309
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 311
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 313
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 315
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region ENDPOINT_RESPONDERS GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 304
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] {  }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 306
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x03, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] {  }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 308
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] {  }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 310
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x05, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] {  }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 312
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x06, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] {  }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 314
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x07, 0x00, 0x00, 0x00, 0x07, 0x4c, 0x55, 0x00, 0x00, 0xd5, 0xaa },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(7u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] { new UID(0x4C55, 0x0000D5AA) }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 316
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x08, 0x00, 0x00, 0x00, 0x05, 0x57, 0x53, 0x6e, 0x92, 0x30, 0x91 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(5u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] { new UID(0x5753, 0x6E923091) }));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 200
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] { 0x00, 0x07, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
                var obj = (GetEndpointRespondersResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.ListChangedNumber, Is.EqualTo(0u));
                    Assert.That(obj.UIDs, Is.EqualTo(new UID[] {  }));
                }
            }));
        #endregion

        #region ENDPOINT_RESPONDER_LIST_CHANGE GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 317
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 319
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 321
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 323
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 325
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 327
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 329
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region ENDPOINT_RESPONDER_LIST_CHANGE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 318
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(0u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 320
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x03, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(0u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 322
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(0u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 324
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x05, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(0u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 326
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x06, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(0u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 328
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x07, 0x00, 0x00, 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(7u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 330
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x08, 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(5u));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 345
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x07, 0x00, 0x00, 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
                var obj = (GetEndpointResponderListChangeResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.ListChangeNumber, Is.EqualTo(6u));
                }
            }));
        #endregion

        #region ENDPOINT_TIMING GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 289
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 291
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 293
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 295
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 297
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 299
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 301
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region ENDPOINT_TIMING GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 290
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x01, 0x01, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingResponse)));
                var obj = (GetEndpointTimingResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.TimingId, Is.EqualTo((byte)1));
                    Assert.That(obj.Timings, Is.EqualTo((byte)3));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 106
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x01, 0x02, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingResponse)));
                var obj = (GetEndpointTimingResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.TimingId, Is.EqualTo((byte)2));
                    Assert.That(obj.Timings, Is.EqualTo((byte)3));
                }
            }));
        #endregion

        #region ENDPOINT_TIMING_DESCRIPTION GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 121
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 124
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 127
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 129
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        #endregion

        #region ENDPOINT_TIMING_DESCRIPTION GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 122
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x01, 0x46, 0x61, 0x73, 0x74, 0x20, 0x28, 0x31, 0x37, 0x36, 0x75, 0x53, 0x2f, 0x34, 0x30, 0x46, 0x50, 0x53, 0x29 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingDescriptionResponse)));
                var obj = (GetEndpointTimingDescriptionResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.TimingId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Fast (176uS/40FPS)"));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 126
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x02, 0x4d, 0x65, 0x64, 0x69, 0x75, 0x6d, 0x20, 0x28, 0x31, 0x38, 0x34, 0x75, 0x53, 0x2f, 0x33, 0x36, 0x46, 0x50, 0x53, 0x29 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingDescriptionResponse)));
                var obj = (GetEndpointTimingDescriptionResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.TimingId, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("Medium (184uS/36FPS)"));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 128
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x03, 0x53, 0x6c, 0x6f, 0x77, 0x20, 0x28, 0x32, 0x30, 0x30, 0x75, 0x53, 0x2f, 0x32, 0x35, 0x46, 0x50, 0x53, 0x29 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingDescriptionResponse)));
                var obj = (GetEndpointTimingDescriptionResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.TimingId, Is.EqualTo((byte)3));
                    Assert.That(obj.Description, Is.EqualTo("Slow (200uS/25FPS)"));
                }
            }));
        #endregion

        #region ENDPOINT_TO_UNIVERSE GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 223
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 286
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 225
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 288
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)2));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 227
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 290
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 229
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 231
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region ENDPOINT_TO_UNIVERSE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 220
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x01, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 222
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x03, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 224
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x04, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 226
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x05, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 228
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x06, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 230
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x07, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 232
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x08, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
                var obj = (GetSetEndpointToUniverse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.Universe, Is.EqualTo((ushort)1));
                }
            }));
        #endregion

        #region FACTORY_DEFAULTS GET_COMMAND_RESPONSE
        // Ayrton Diabolo S 2.3.0.pcapng, frame 186
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.FACTORY_DEFAULTS,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(true));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 227
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.FACTORY_DEFAULTS,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        #endregion

        #region IDENTIFY_DEVICE GET_COMMAND_RESPONSE
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 257
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_DEVICE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        // KLSTR Luminode Buddy Dump Wireless Solution.pcapng, frame 785
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_DEVICE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(true));
                }
            }));
        #endregion

        #region IDENTIFY_DEVICE SET_COMMAND
        // KLSTR Luminode Buddy Dump Wireless Solution.pcapng, frame 782
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_DEVICE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(true));
                }
            }));
        // KLSTR Luminode Buddy Dump Wireless Solution.pcapng, frame 791
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_DEVICE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        #endregion

        #region IDENTIFY_ENDPOINT GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 205
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 207
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 209
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 211
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 213
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 215
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 217
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region IDENTIFY_ENDPOINT GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 206
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 208
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x03, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 210
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x04, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 212
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x05, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 214
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x06, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 216
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x07, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 218
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        #endregion

        #region IDENTIFY_ENDPOINT SET_COMMAND
        // Lumisplit 1.6 FW 7.0.pcapng, frame 235
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.IdentifyState, Is.EqualTo(true));
                }
            }));
        // Lumisplit 1.6 FW 7.0.pcapng, frame 237
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
                var obj = (GetSetIdentifyEndpoint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.IdentifyState, Is.EqualTo(false));
                }
            }));
        #endregion

        #region IDENTIFY_ENDPOINT SET_COMMAND_RESPONSE
        // Lumisplit 1.6 FW 7.0.pcapng, frame 236
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        #endregion

        #region IDENTIFY_MODE GET_COMMAND_RESPONSE
        // GenVI Dimmer v1.08.pcapng, frame 822
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_MODE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_IdentifyMode)));
                var obj = (ERDM_IdentifyMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_IdentifyMode)0x00));
                }
            }));
        #endregion

        #region INTERFACE_HARDWARE_ADDRESS_TYPE GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 211
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 213
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 215
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(3u));
                }
            }));
        #endregion

        #region INTERFACE_HARDWARE_ADDRESS_TYPE GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 212
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x01, 0xe1, 0x54 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:07:00:01:e1:54")));
                }
            }));
        // Martin MAC Aura XIP v1.3.1.pcapng, frame 198
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x04, 0x42, 0xeb },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:04:42:eb")));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 263
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x06, 0xe2, 0x46 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:06:e2:46")));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 597
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x06, 0x85, 0x87 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:06:85:87")));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 665
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x06, 0x85, 0x77 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:06:85:77")));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 678
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x06, 0xe1, 0xe7 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:06:e1:e7")));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 825
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x06, 0x85, 0x75 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:06:85:75")));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 949
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x43, 0x06, 0xc9, 0x35 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetHardwareAddressResponse)));
                var obj = (GetHardwareAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.HardwareAddress, Is.EqualTo(new MACAddress("00:03:43:06:c9:35")));
                }
            }));
        #endregion

        #region INTERFACE_LABEL GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 59
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.INTERFACE_LABEL,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 61
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.INTERFACE_LABEL,
            new byte[] { 0x00, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 63
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.INTERFACE_LABEL,
            new byte[] { 0x00, 0x00, 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(3u));
                }
            }));
        #endregion

        #region INTERFACE_LABEL GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 60, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_LABEL,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x65, 0x74, 0x68, 0x30, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetInterfaceNameResponse)));
                var obj = (GetInterfaceNameResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.Label, Is.EqualTo("eth0"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x00, 0x00, 0x01, 0x65, 0x74, 0x68, 0x30 });
        // Martin MAC Aura XIP v1.3.1.pcapng, frame 104
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.INTERFACE_LABEL,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x45, 0x74, 0x68, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetInterfaceNameResponse)));
                var obj = (GetInterfaceNameResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.Label, Is.EqualTo("Eth0"));
                }
            }));
        #endregion

        #region IPV4_CURRENT_ADDRESS GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 223
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 225
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 227
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(3u));
                }
            }));
        #endregion

        #region IPV4_CURRENT_ADDRESS GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 224
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x08, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("1.1.1.1")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x02));
                }
            }));
        // Martin MAC Aura XIP v1.3.1.pcapng, frame 210
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0x1c, 0x42, 0xeb, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.28.66.235")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 275
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0xe2, 0x46, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.226.70")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 680
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0x85, 0x87, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.133.135")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 787
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0x85, 0x77, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.133.119")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 788
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0xe1, 0xe7, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.225.231")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 1008
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0x85, 0x75, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.133.117")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 1083
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0xdc, 0xc3, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetIPv4CurrentAddressResponse)));
                var obj = (GetIPv4CurrentAddressResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.220.195")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                    Assert.That(obj.DHCPStatus, Is.EqualTo((ERDM_DHCPStatusMode)0x00));
                }
            }));
        #endregion

        #region IPV4_DHCP_MODE GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 217
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_DHCP_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 219
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_DHCP_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 221
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_DHCP_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(3u));
                }
            }));
        #endregion

        #region IPV4_DHCP_MODE GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 218
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_DHCP_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPV4_xxx_Mode)));
                var obj = (GetSetIPV4_xxx_Mode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.Enabled, Is.EqualTo(false));
                }
            }));
        #endregion

        #region IPV4_STATIC_ADDRESS GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 229
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 231
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(2u));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 233
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(3u));
                }
            }));
        #endregion

        #region IPV4_STATIC_ADDRESS GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 230
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("1.1.1.1")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        // Martin MAC Aura XIP v1.3.1.pcapng, frame 216
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0x1c, 0x42, 0xeb, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.28.66.235")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        // Martin MAC One v1.3.1 Cleaned.pcapng, frame 281
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0xe2, 0x46, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.226.70")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 706
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0x85, 0x87, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.133.135")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 807
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0xe1, 0xe7, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.225.231")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 811
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0x85, 0x77, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.133.119")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        // Martin MAC One v1.3.1.pcapng, frame 1016
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0xd1, 0x85, 0x75, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIPv4StaticAddress)));
                var obj = (GetSetIPv4StaticAddress)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.InterfaceId, Is.EqualTo(1u));
                    Assert.That(obj.IPAddress, Is.EqualTo(new IPv4Address("2.209.133.117")));
                    Assert.That(obj.Netmask, Is.EqualTo((byte)8));
                }
            }));
        #endregion

        #region LAMP_HOURS GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 134
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(7u));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1427
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0xf7 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(247u));
                }
            }));
        // Bairut Colorado and Rivale.pcapng, frame 2004
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x01, 0x74 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(372u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 5478
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x04, 0x0b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1035u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 8046
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x29 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(41u));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 96
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x04, 0x4d },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(1101u));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 126
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x01, 0xbe },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(446u));
                }
            }));
        // DIHK 2025 1.pcapng, frame 300
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x1a, 0xa0 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(6816u));
                }
            }));
        #endregion

        #region LAMP_ON_MODE GET_COMMAND_RESPONSE
        // Lollapalooza 2026 1.pcapng, frame 10165
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_ON_MODE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampMode)));
                var obj = (ERDM_LampMode)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_LampMode)0x00));
                }
            }));
        #endregion

        #region LAMP_STATE GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 202
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STATE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampState)));
                var obj = (ERDM_LampState)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_LampState)0x00));
                }
            }));
        // Blackcoffe 1.pcapng, frame 6283
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STATE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampState)));
                var obj = (ERDM_LampState)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_LampState)0x01));
                }
            }));
        #endregion

        #region LAMP_STRIKES GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 5923
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x6d },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(109u));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 6955
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(0u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 5925
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x9b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(155u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 5931
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x48 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(72u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 5970
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x90 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(144u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 6004
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x68 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(104u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 6005
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x8b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(139u));
                }
            }));
        // Blackcoffe 1.pcapng, frame 6702
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x6e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(110u));
                }
            }));
        #endregion

        #region LANGUAGE GET_COMMAND_RESPONSE
        // Lollapalooza 2026 1.pcapng, frame 9302
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LANGUAGE,
            new byte[] { 0x65, 0x6e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("en"));
                }
            }));
        #endregion

        #region LANGUAGE_CAPABILITIES GET_COMMAND_RESPONSE
        // Lollapalooza 2026 1.pcapng, frame 1202
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LANGUAGE_CAPABILITIES,
            new byte[] { 0x65, 0x6e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string[])));
                var obj = (string[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new string[] { "en" }));
                }
            }));
        #endregion

        #region LIST_INTERFACES GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 58
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LIST_INTERFACES,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(InterfaceDescriptor[])));
                var obj = (InterfaceDescriptor[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(i => i.InterfaceId), Is.EqualTo(new uint[] { 1u }));
                }
            }));
        #endregion

        #region LOCK_PIN GET_COMMAND_RESPONSE
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 175
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_PIN,
            new byte[] { 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)0));
                }
            }));
        #endregion

        #region LOCK_STATE GET_COMMAND_RESPONSE
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 177
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE,
            new byte[] { 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetLockStateResponse)));
                var obj = (GetLockStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentLockStateId, Is.EqualTo((byte)0));
                    Assert.That(obj.LockStates, Is.EqualTo((byte)2));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 194
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetLockStateResponse)));
                var obj = (GetLockStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentLockStateId, Is.EqualTo((byte)0));
                    Assert.That(obj.LockStates, Is.EqualTo((byte)1));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 564
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE,
            new byte[] { 0x01, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetLockStateResponse)));
                var obj = (GetLockStateResponse)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentLockStateId, Is.EqualTo((byte)1));
                    Assert.That(obj.LockStates, Is.EqualTo((byte)3));
                }
            }));
        #endregion

        #region LOCK_STATE_DESCRIPTION GET_COMMAND
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 109
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 27
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 111
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 29
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)5));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 113
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 31
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)6));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 33
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)7));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 35
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)8));
                }
            }));
        #endregion

        #region LOCK_STATE_DESCRIPTION GET_COMMAND_RESPONSE
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 110
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x01, 0x48, 0x4f, 0x4c, 0x44 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("HOLD"));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 114
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x01, 0x46, 0x72, 0x6f, 0x6e, 0x74, 0x20, 0x70, 0x61, 0x6e, 0x65, 0x6c, 0x20, 0x6c, 0x6f, 0x63, 0x6b, 0x65, 0x64 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Front panel locked"));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 22
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x01, 0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Service"));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 112
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x02, 0x50, 0x49, 0x4e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("PIN"));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 24
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x02, 0x43, 0x6f, 0x6e, 0x66, 0x69, 0x67, 0x75, 0x72, 0x61, 0x74, 0x69, 0x6f, 0x6e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("Configuration"));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 26
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x03, 0x55, 0x73, 0x65, 0x72 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)3));
                    Assert.That(obj.Description, Is.EqualTo("User"));
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 28
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)4));
                    Assert.That(obj.Description, Is.Null.Or.Empty);
                }
            }));
        // GenVI Dimmer v1.08.pcapng, frame 30
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));
                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LockStateId, Is.EqualTo((byte)5));
                    Assert.That(obj.Description, Is.Null.Or.Empty);
                }
            }));
        #endregion

        #region MANUFACTURER_LABEL GET_COMMAND_RESPONSE
        // ACME Pixel Line IP V25.pcapng, frame 18
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x41, 0x43, 0x4d, 0x45, 0x20, 0x45, 0x46, 0x46, 0x45, 0x43, 0x54, 0x53, 0x20, 0x4c, 0x54, 0x44, 0x2e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("ACME EFFECTS LTD."));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 309, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x53, 0x57, 0x49, 0x53, 0x53, 0x4f, 0x4e, 0x20, 0x41, 0x47, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("SWISSON AG"));
                }
            }), normalizedPayload: new byte[] { 0x53, 0x57, 0x49, 0x53, 0x53, 0x4f, 0x4e, 0x20, 0x41, 0x47 });
        // Astera AX5 5.14.84.pcapng, frame 10
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x41, 0x73, 0x74, 0x65, 0x72, 0x61, 0x20, 0x4c, 0x45, 0x44 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Astera LED"));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 10
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x41, 0x59, 0x52, 0x54, 0x4f, 0x4e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("AYRTON"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2295
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x47, 0x4c, 0x50 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("GLP"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2395
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x52, 0x6f, 0x62, 0x65, 0x20, 0x4c, 0x69, 0x67, 0x68, 0x74, 0x69, 0x6e, 0x67, 0x20, 0x73, 0x2e, 0x72, 0x2e, 0x6f, 0x2e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Robe Lighting s.r.o."));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 120, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x43, 0x48, 0x41, 0x55, 0x56, 0x45, 0x54, 0x20, 0x50, 0x52, 0x4f, 0x46, 0x45, 0x53, 0x53, 0x49, 0x4f, 0x4e, 0x41, 0x4c, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("CHAUVET PROFESSIONAL"));
                }
            }), normalizedPayload: new byte[] { 0x43, 0x48, 0x41, 0x55, 0x56, 0x45, 0x54, 0x20, 0x50, 0x52, 0x4f, 0x46, 0x45, 0x53, 0x53, 0x49, 0x4f, 0x4e, 0x41, 0x4c });
        // Cameo Zenit W600 G2 v1.7.3 Update to v1.8.4.pcapng, frame 18
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x43, 0x41, 0x4d, 0x45, 0x4f },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("CAMEO"));
                }
            }));
        #endregion

        #region MAXIMUM_LEVEL GET_COMMAND_RESPONSE
        // GenVI Dimmer v1.08.pcapng, frame 798
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MAXIMUM_LEVEL,
            new byte[] { 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)65535));
                }
            }));
        #endregion

        #region MINIMUM_LEVEL GET_COMMAND_RESPONSE
        // GenVI Dimmer v1.08.pcapng, frame 793
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MINIMUM_LEVEL,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMMinimumLevel)));
                var obj = (RDMMinimumLevel)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.MinimumLevelIncrease, Is.EqualTo((ushort)0));
                    Assert.That(obj.MinimumLevelDecrease, Is.EqualTo((ushort)0));
                    Assert.That(obj.OnBelowMinimum, Is.EqualTo(false));
                }
            }));
        #endregion

        #region MODULATION_FREQUENCY GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 130
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x04, 0x0f },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));
                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)4));
                    Assert.That(obj.ModulationFrequencys, Is.EqualTo((byte)15));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 1180
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x02, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));
                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)2));
                    Assert.That(obj.ModulationFrequencys, Is.EqualTo((byte)3));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 164
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x03, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));
                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)3));
                    Assert.That(obj.ModulationFrequencys, Is.EqualTo((byte)7));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 157
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));
                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)1));
                    Assert.That(obj.ModulationFrequencys, Is.EqualTo((byte)1));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 103
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x02, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));
                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)2));
                    Assert.That(obj.ModulationFrequencys, Is.EqualTo((byte)6));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 9629
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x04, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));
                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)4));
                    Assert.That(obj.ModulationFrequencys, Is.EqualTo((byte)6));
                }
            }));
        #endregion

        #region MODULATION_FREQUENCY_DESCRIPTION GET_COMMAND
        // ACME CM S2 V20.pcapng, frame 49
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6586
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x11 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)17));
                }
            }));
        // ACME CM S2 V20.pcapng, frame 51
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6593
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x12 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)18));
                }
            }));
        // ACME CM S2 V20.pcapng, frame 53
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6600
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x13 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)19));
                }
            }));
        // ACME CM S2 V20.pcapng, frame 55
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6623
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x14 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)20));
                }
            }));
        #endregion

        #region MODULATION_FREQUENCY_DESCRIPTION GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 50
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01, 0xff, 0xff, 0xff, 0xff, 0x39, 0x30, 0x30, 0x48, 0x7a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)1));
                    Assert.That(obj.Frequency, Is.Null);
                    Assert.That(obj.Description, Is.EqualTo("900Hz"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 654, String padded with NUL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x00, 0x02, 0x58, 0x36, 0x30, 0x30, 0x48, 0x7a, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: the PDL has to match the String, but a NUL terminates it, the padding is reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Null-Delimiter"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)1));
                    Assert.That(obj.Frequency, Is.EqualTo(600u));
                    Assert.That(obj.Description, Is.EqualTo("600Hz"));
                }
            }), normalizedPayload: new byte[] { 0x01, 0x00, 0x00, 0x02, 0x58, 0x36, 0x30, 0x30, 0x48, 0x7a });
        // Cameo Zenit W600 G2 v1.8.4 Modulation_Frequency_Description.pcapng, frame 2
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x00, 0x01, 0x90, 0x34, 0x30, 0x30, 0x20, 0x48, 0x7a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)1));
                    Assert.That(obj.Frequency, Is.EqualTo(400u));
                    Assert.That(obj.Description, Is.EqualTo("400 Hz"));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 104
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x00, 0x4e, 0x20, 0x32, 0x30, 0x30, 0x30, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)1));
                    Assert.That(obj.Frequency, Is.EqualTo(20000u));
                    Assert.That(obj.Description, Is.EqualTo("20000"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6852
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01, 0x38, 0x30, 0x30, 0x48, 0x5a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)1));
                    Assert.That(obj.Frequency, Is.EqualTo(942682184u));
                    Assert.That(obj.Description, Is.EqualTo("Z"));
                }
            }));
        // ACME CM S2 V20.pcapng, frame 52
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x02, 0xff, 0xff, 0xff, 0xff, 0x31, 0x30, 0x30, 0x30, 0x48, 0x7a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)2));
                    Assert.That(obj.Frequency, Is.Null);
                    Assert.That(obj.Description, Is.EqualTo("1000Hz"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 656, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x02, 0x00, 0x00, 0x04, 0xb0, 0x31, 0x32, 0x30, 0x30, 0x48, 0x7a, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)2));
                    Assert.That(obj.Frequency, Is.EqualTo(1200u));
                    Assert.That(obj.Description, Is.EqualTo("1200Hz"));
                }
            }), normalizedPayload: new byte[] { 0x02, 0x00, 0x00, 0x04, 0xb0, 0x31, 0x32, 0x30, 0x30, 0x48, 0x7a });
        // Cameo Zenit W600 G2 v1.8.4 Modulation_Frequency_Description.pcapng, frame 4
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x02, 0x00, 0x00, 0x02, 0x8a, 0x36, 0x35, 0x30, 0x20, 0x48, 0x7a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));
                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)2));
                    Assert.That(obj.Frequency, Is.EqualTo(650u));
                    Assert.That(obj.Description, Is.EqualTo("650 Hz"));
                }
            }));
        #endregion

        #region OUTPUT_RESPONSE_TIME GET_COMMAND_RESPONSE
        // Chauvet Ovation und mehr.pcapng, frame 101
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            new byte[] { 0x01, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTime)));
                var obj = (RDMOutputResponseTime)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentResponseTimeId, Is.EqualTo((byte)1));
                    Assert.That(obj.ResponseTimes, Is.EqualTo((byte)4));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8285
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            new byte[] { 0x03, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTime)));
                var obj = (RDMOutputResponseTime)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentResponseTimeId, Is.EqualTo((byte)3));
                    Assert.That(obj.ResponseTimes, Is.EqualTo((byte)3));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 9598
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            new byte[] { 0x01, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTime)));
                var obj = (RDMOutputResponseTime)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentResponseTimeId, Is.EqualTo((byte)1));
                    Assert.That(obj.ResponseTimes, Is.EqualTo((byte)2));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 179
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            new byte[] { 0x02, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTime)));
                var obj = (RDMOutputResponseTime)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.CurrentResponseTimeId, Is.EqualTo((byte)2));
                    Assert.That(obj.ResponseTimes, Is.EqualTo((byte)4));
                }
            }));
        #endregion

        #region OUTPUT_RESPONSE_TIME_DESCRIPTION GET_COMMAND
        // Chauvet Ovation und mehr.pcapng, frame 60
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 4345
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)6));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 62
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 4348
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)7));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 64
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 4351
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)8));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 66
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)4));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 4354
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)9));
                }
            }));
        #endregion

        #region OUTPUT_RESPONSE_TIME_DESCRIPTION GET_COMMAND_RESPONSE
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 114
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x01, 0x4c, 0x65, 0x64 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Led"));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 61
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x01, 0x4f, 0x66, 0x66 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Off"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6770, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x01, 0x45, 0x44, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("ED"));
                }
            }), normalizedPayload: new byte[] { 0x01, 0x45, 0x44 });
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 116
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x02, 0x48, 0x61, 0x6c, 0x6f, 0x67, 0x65, 0x6e, 0x20, 0x31, 0x6b, 0x57 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("Halogen 1kW"));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 63
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x02, 0x44, 0x69, 0x6d, 0x6d, 0x65, 0x72, 0x20, 0x31 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("Dimmer 1"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 6791, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x02, 0x45, 0x44, 0x49, 0x55, 0x4d, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("EDIUM"));
                }
            }), normalizedPayload: new byte[] { 0x02, 0x45, 0x44, 0x49, 0x55, 0x4d });
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 118
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x03, 0x48, 0x61, 0x6c, 0x6f, 0x67, 0x65, 0x6e, 0x20, 0x32, 0x6b, 0x57 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)3));
                    Assert.That(obj.Description, Is.EqualTo("Halogen 2kW"));
                }
            }));
        // Chauvet Ovation und mehr.pcapng, frame 65
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x03, 0x44, 0x69, 0x6d, 0x6d, 0x65, 0x72, 0x20, 0x32 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));
                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.OutputResponseTimeId, Is.EqualTo((byte)3));
                    Assert.That(obj.Description, Is.EqualTo("Dimmer 2"));
                }
            }));
        #endregion

        #region PAN_INVERT GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 206
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PAN_INVERT,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 104
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PAN_INVERT,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(true));
                }
            }));
        #endregion

        #region PAN_TILT_SWAP GET_COMMAND_RESPONSE
        // Estrel 14_01_2026_2.pcapng, frame 7799
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PAN_TILT_SWAP,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        #endregion

        #region PARAMETER_DESCRIPTION GET_COMMAND
        // ACME CM S2 V20.pcapng, frame 91
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0xa0, 0x20 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0xA020));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 417
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x80, 0x82 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0x8082));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 5
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x80, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0x8001));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2204
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x86, 0x19 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0x8619));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2209
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0xa0, 0x10 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0xA010));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 13
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x80, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0x8008));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 5
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x82, 0x11 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0x8211));
                }
            }));
        // Chroma-Q Color Force II 72 CFII 72 v1.12.pcapng, frame 26
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0xff, 0xd2 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));
                var obj = (ERDM_Parameter)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Parameter)0xFFD2));
                }
            }));
        #endregion

        #region PARAMETER_DESCRIPTION GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 92
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0xa0, 0x20, 0x01, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x4f, 0x46, 0x46, 0x53, 0x45, 0x54 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0xA020));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)1));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x03));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x03));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(0u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(639u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(0u));
                    Assert.That(obj.Description, Is.EqualTo("OFFSET"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 426, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x80, 0x82, 0x02, 0x05, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf9, 0xf9, 0xff, 0x00, 0x00, 0x00, 0x00, 0x41, 0x75, 0x74, 0x6f, 0x6d, 0x61, 0x74, 0x69, 0x63, 0x20, 0x75, 0x6e, 0x69, 0x76, 0x65, 0x72, 0x73, 0x65, 0x20, 0x6c, 0x61, 0x62, 0x65, 0x6c, 0x6c, 0x69, 0x6e, 0x67, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0x8082));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)2));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x05));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x03));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(0u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(16382463u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(0u));
                    Assert.That(obj.Description, Is.EqualTo("Automatic universe labelling"));
                }
            }), normalizedPayload: new byte[] { 0x80, 0x82, 0x02, 0x05, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf9, 0xf9, 0xff, 0x00, 0x00, 0x00, 0x00, 0x41, 0x75, 0x74, 0x6f, 0x6d, 0x61, 0x74, 0x69, 0x63, 0x20, 0x75, 0x6e, 0x69, 0x76, 0x65, 0x72, 0x73, 0x65, 0x20, 0x6c, 0x61, 0x62, 0x65, 0x6c, 0x6c, 0x69, 0x6e, 0x67 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 548
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x90, 0x20, 0x01, 0x03, 0x02, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x57, 0x69, 0x72, 0x65, 0x6c, 0x65, 0x73, 0x73, 0x20, 0x55, 0x6e, 0x6c, 0x69, 0x6e, 0x6b, 0x20, 0x31, 0x2d, 0x75, 0x6e, 0x6c, 0x69, 0x6e, 0x6b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0x9020));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)1));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x03));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x02));
                    Assert.That(obj.Type, Is.EqualTo((byte)127));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(1u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(1u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(1u));
                    Assert.That(obj.Description, Is.EqualTo("Wireless Unlink 1-unlink"));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 6
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x80, 0x01, 0x04, 0x03, 0x03, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x19, 0x40, 0x00, 0x00, 0x46, 0x50, 0x52, 0x55, 0x4e, 0x54, 0x49, 0x4d, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0x8001));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)4));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x03));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x03));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x15));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(0u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(72000u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(18000u));
                    Assert.That(obj.Description, Is.EqualTo("RUNTIME"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2206
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x86, 0x19, 0x40, 0x02, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x52, 0x44, 0x4d, 0x53, 0x48, 0x45, 0x4c, 0x4c, 0x5f, 0x50, 0x52, 0x4f, 0x4d, 0x50, 0x54 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0x8619));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)64));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x02));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x02));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(0u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(0u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(0u));
                    Assert.That(obj.Description, Is.EqualTo("RDMSHELL_PROMPT"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 6, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x80, 0x01, 0x01, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x43, 0x61, 0x6c, 0x69, 0x62, 0x72, 0x61, 0x74, 0x65, 0x64, 0x20, 0x57, 0x68, 0x69, 0x74, 0x65, 0x20, 0x30, 0x2d, 0x4f, 0x4e, 0x31, 0x2d, 0x4f, 0x46, 0x46, 0x32, 0x2d, 0x43, 0x75, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0x8001));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)1));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x03));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x03));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(0u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(2u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(0u));
                    Assert.That(obj.Description, Is.EqualTo("Calibrated White 0-ON1-OFF2-Cu"));
                }
            }), normalizedPayload: new byte[] { 0x80, 0x01, 0x01, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x43, 0x61, 0x6c, 0x69, 0x62, 0x72, 0x61, 0x74, 0x65, 0x64, 0x20, 0x57, 0x68, 0x69, 0x74, 0x65, 0x20, 0x30, 0x2d, 0x4f, 0x4e, 0x31, 0x2d, 0x4f, 0x46, 0x46, 0x32, 0x2d, 0x43, 0x75 });
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 6
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0x82, 0x11, 0x01, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x57, 0x49, 0x52, 0x45, 0x4c, 0x45, 0x53, 0x53, 0x3a, 0x20, 0x31, 0x3a, 0x4f, 0x4e, 0x20, 0x30, 0x3a, 0x4f, 0x46, 0x46 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0x8211));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)1));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x03));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x03));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(0u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(1u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(1u));
                    Assert.That(obj.Description, Is.EqualTo("WIRELESS: 1:ON 0:OFF"));
                }
            }));
        // Chroma-Q Color Force II 72 CFII 72 v1.12.pcapng, frame 30
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] { 0xff, 0xd2, 0x01, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x00, 0x01, 0x47, 0x52, 0x4f, 0x55, 0x50, 0x49, 0x4e, 0x47 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));
                var obj = (RDMParameterDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.ParameterId, Is.EqualTo((ushort)0xFFD2));
                    Assert.That(obj.PDLSize, Is.EqualTo((byte)1));
                    Assert.That(obj.DataType, Is.EqualTo((ERDM_DataType)0x03));
                    Assert.That(obj.CommandClass, Is.EqualTo((ERDM_CommandClass)0x03));
                    Assert.That(obj.Type, Is.EqualTo((byte)0));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x00));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.MinValidValue, Is.EqualTo(1u));
                    Assert.That(obj.MaxValidValue, Is.EqualTo(12u));
                    Assert.That(obj.DefaultValue, Is.EqualTo(1u));
                    Assert.That(obj.Description, Is.EqualTo("GROUPING"));
                }
            }));
        #endregion

        #region PERFORM_SELFTEST GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 8836
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PERFORM_SELFTEST,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        #endregion

        #region POWER_STATE GET_COMMAND_RESPONSE
        // DIHK 2025 1.pcapng, frame 2810
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.POWER_STATE,
            new byte[] { 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_PowerState)));
                var obj = (ERDM_PowerState)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_PowerState)0xFF));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 10405
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.POWER_STATE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_PowerState)));
                var obj = (ERDM_PowerState)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_PowerState)0x00));
                }
            }));
        #endregion

        #region POWER_STATE SET_COMMAND
        // Lumisplit 1.6 FW 7.0.pcapng, frame 239
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.POWER_STATE,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_PowerState)));
                var obj = (ERDM_PowerState)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_PowerState)0x02));
                }
            }));
        #endregion

        #region PRESET_INFO GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 134
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x02, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0xff, 0xfe, 0x00, 0x01, 0xff, 0xfe, 0x00, 0x01, 0xff, 0xfe, 0x00, 0x01, 0xff, 0xfe },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMPresetInfo)));
                var obj = (RDMPresetInfo)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.LevelFieldSupported, Is.EqualTo(false));
                    Assert.That(obj.PresetSequenceSupported, Is.EqualTo(false));
                    Assert.That(obj.SplitTimesSupported, Is.EqualTo(false));
                    Assert.That(obj.DMX512FailInfiniteDelayTimeSupported, Is.EqualTo(true));
                    Assert.That(obj.DMX512FailInfiniteHoldTimeSupported, Is.EqualTo(true));
                    Assert.That(obj.StartupInfiniteHoldTimeSupported, Is.EqualTo(true));
                    Assert.That(obj.MaximumSceneNumber, Is.EqualTo((ushort)2));
                    Assert.That(obj.MinimumPresetFadeTimeSupported, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.MaximumPresetFadeTimeSupported, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.MinimumPresetWaitTimeSupported, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.MaximumPresetWaitTimeSupported, Is.EqualTo(6553.5).Within(0.001));
                    Assert.That(obj.MinimumDMX512FailDelayTimeSupported, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.MaximumDMX512FailDelayTimeSupported, Is.EqualTo(6553.4).Within(0.001));
                    Assert.That(obj.MinimumDMX512FailDelayHoldSupported, Is.EqualTo(0.1).Within(0.001));
                    Assert.That(obj.MaximumDMX512FailDelayHoldSupported, Is.EqualTo(6553.4).Within(0.001));
                    Assert.That(obj.MinimumStartupDelayTimeSupported, Is.EqualTo(0.1).Within(0.001));
                    Assert.That(obj.MaximumStartupDelayTimeSupported, Is.EqualTo(6553.4).Within(0.001));
                    Assert.That(obj.MinimumStartupDelayHoldSupported, Is.EqualTo(0.1).Within(0.001));
                    Assert.That(obj.MaximumStartupDelayHoldSupported, Is.EqualTo(6553.4).Within(0.001));
                }
            }));
        #endregion

        #region PRESET_PLAYBACK GET_COMMAND_RESPONSE
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 261
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_PLAYBACK,
            new byte[] { 0x00, 0x00, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMPresetPlayback)));
                var obj = (RDMPresetPlayback)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Mode, Is.EqualTo((ushort)0));
                    Assert.That(obj.Level, Is.EqualTo((byte)255));
                }
            }));
        #endregion

        #region PRESET_STATUS GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 335
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PRESET_STATUS,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 337
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PRESET_STATUS,
            new byte[] { 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)2));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 339
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.PRESET_STATUS,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        #endregion

        #region PRESET_STATUS GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 336
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_STATUS,
            new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetPresetStatus)));
                var obj = (GetPresetStatus)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SceneId, Is.EqualTo((ushort)1));
                    Assert.That(obj.UpFadeTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.DownFadeTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.WaitTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.Programmed, Is.EqualTo((byte)2));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 338
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_STATUS,
            new byte[] { 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetPresetStatus)));
                var obj = (GetPresetStatus)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SceneId, Is.EqualTo((ushort)2));
                    Assert.That(obj.UpFadeTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.DownFadeTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.WaitTime, Is.EqualTo(0.0).Within(0.001));
                    Assert.That(obj.Programmed, Is.EqualTo((byte)1));
                }
            }));
        #endregion

        #region PRODUCT_DETAIL_ID_LIST GET_COMMAND_RESPONSE
        // Chauvet Ovation und mehr.pcapng, frame 232
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x00, 0x04, 0x02, 0x00, 0x04, 0x0a, 0x06, 0x01, 0x06, 0x04, 0x08, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0004, (ERDM_ProductDetail)0x0200, (ERDM_ProductDetail)0x040A, (ERDM_ProductDetail)0x0601, (ERDM_ProductDetail)0x0604, (ERDM_ProductDetail)0x0802 }));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 24
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x01, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0102 }));
                }
            }));
        // Coemar ReliteKit MK2 v0.94.pcapng, frame 38
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0004 }));
                }
            }));
        // DZP Esprit Creos.pcapng, frame 173
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x06, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0600 }));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 10
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x06, 0x04, 0x06, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0604, (ERDM_ProductDetail)0x0601 }));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 593
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x03, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0301 }));
                }
            }));
        // Innotrans 1 elation KL und Litekraft wash XT.pcapng, frame 47
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x7f, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x7FFF }));
                }
            }));
        // Clay Paky Skylos 3.2.894.pcapng, frame 19
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));
                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_ProductDetail[] { (ERDM_ProductDetail)0x0000, (ERDM_ProductDetail)0x0000, (ERDM_ProductDetail)0x0000, (ERDM_ProductDetail)0x0000, (ERDM_ProductDetail)0x0000, (ERDM_ProductDetail)0x0000 }));
                }
            }));
        #endregion

        #region PROXIED_DEVICES GET_COMMAND_RESPONSE
        // DIHK 2025 2.pcapng, frame 477
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PROXIED_DEVICES,
            new byte[] { 0x4c, 0x55, 0xf0, 0xad, 0xb0, 0x60, 0x4c, 0x55, 0x00, 0x01, 0x4f, 0x0b, 0x41, 0x73, 0x99, 0x71, 0x96, 0xcc },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDevices)));
                var obj = (RDMProxiedDevices)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Devices, Is.EqualTo(new UID[] { new UID(0x4C55, 0xF0ADB060), new UID(0x4C55, 0x00014F0B), new UID(0x4173, 0x997196CC) }));
                }
            }));
        // Lumenradio Luna v1.1.1.5.pcapng, frame 41
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PROXIED_DEVICES,
            new byte[] {  },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDevices)));
                var obj = (RDMProxiedDevices)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Devices, Is.EqualTo(new UID[] {  }));
                }
            }));
        #endregion

        #region PROXIED_DEVICES_COUNT GET_COMMAND_RESPONSE
        // DIHK 2025 2.pcapng, frame 489
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PROXIED_DEVICES_COUNT,
            new byte[] { 0x00, 0x03, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDeviceCount)));
                var obj = (RDMProxiedDeviceCount)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)3));
                    Assert.That(obj.ListChange, Is.EqualTo(false));
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 5954
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PROXIED_DEVICES_COUNT,
            new byte[] { 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDeviceCount)));
                var obj = (RDMProxiedDeviceCount)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.DeviceCount, Is.EqualTo((ushort)0));
                    Assert.That(obj.ListChange, Is.EqualTo(false));
                }
            }));
        #endregion

        #region QUEUED_MESSAGE GET_COMMAND
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 348
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.QUEUED_MESSAGE,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));
                var obj = (ERDM_Status)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Status)0x02));
                }
            }));
        // KLSTR Luminode Buddy Dump Wireless Solution.pcapng, frame 728
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.QUEUED_MESSAGE,
            new byte[] { 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));
                var obj = (ERDM_Status)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Status)0x04));
                }
            }));
        #endregion

        #region RDM_TRAFFIC_ENABLE GET_COMMAND
        // DevCon 2026 Dump 1.pcapng, frame 261
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 263
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 265
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)4));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 267
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)5));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 269
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x06 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)6));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 271
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)7));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 273
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)8));
                }
            }));
        #endregion

        #region RDM_TRAFFIC_ENABLE GET_COMMAND_RESPONSE
        // DevCon 2026 Dump 1.pcapng, frame 262
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)1));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 264
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x03, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)3));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 266
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x04, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)4));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 268
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x05, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)5));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 270
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x06, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)6));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 272
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x07, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)7));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        // DevCon 2026 Dump 1.pcapng, frame 274
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
                var obj = (GetSetEndpointRDMTrafficEnable)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.EndpointId, Is.EqualTo((ushort)8));
                    Assert.That(obj.RDMTrafficEnabled, Is.EqualTo(false));
                }
            }));
        #endregion

        #region REAL_TIME_CLOCK GET_COMMAND_RESPONSE
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1627
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x06, 0x09, 0x07, 0x3a, 0x20 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)6));
                    Assert.That(obj.Day, Is.EqualTo((byte)9));
                    Assert.That(obj.Hour, Is.EqualTo((byte)7));
                    Assert.That(obj.Minute, Is.EqualTo((byte)58));
                    Assert.That(obj.Second, Is.EqualTo((byte)32));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 10331
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x12, 0x17, 0x04, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)18));
                    Assert.That(obj.Hour, Is.EqualTo((byte)23));
                    Assert.That(obj.Minute, Is.EqualTo((byte)4));
                    Assert.That(obj.Second, Is.EqualTo((byte)0));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1632
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x06, 0x09, 0x08, 0x21, 0x20 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)6));
                    Assert.That(obj.Day, Is.EqualTo((byte)9));
                    Assert.That(obj.Hour, Is.EqualTo((byte)8));
                    Assert.That(obj.Minute, Is.EqualTo((byte)33));
                    Assert.That(obj.Second, Is.EqualTo((byte)32));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 10756
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x11, 0x10, 0x13, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)17));
                    Assert.That(obj.Hour, Is.EqualTo((byte)16));
                    Assert.That(obj.Minute, Is.EqualTo((byte)19));
                    Assert.That(obj.Second, Is.EqualTo((byte)0));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1641
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x06, 0x09, 0x09, 0x06, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)6));
                    Assert.That(obj.Day, Is.EqualTo((byte)9));
                    Assert.That(obj.Hour, Is.EqualTo((byte)9));
                    Assert.That(obj.Minute, Is.EqualTo((byte)6));
                    Assert.That(obj.Second, Is.EqualTo((byte)7));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 12233
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x11, 0x10, 0x17, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)17));
                    Assert.That(obj.Hour, Is.EqualTo((byte)16));
                    Assert.That(obj.Minute, Is.EqualTo((byte)23));
                    Assert.That(obj.Second, Is.EqualTo((byte)0));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 1735
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x06, 0x09, 0x09, 0x22, 0x20 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)6));
                    Assert.That(obj.Day, Is.EqualTo((byte)9));
                    Assert.That(obj.Hour, Is.EqualTo((byte)9));
                    Assert.That(obj.Minute, Is.EqualTo((byte)34));
                    Assert.That(obj.Second, Is.EqualTo((byte)32));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 26324
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x11, 0x10, 0x1c, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)17));
                    Assert.That(obj.Hour, Is.EqualTo((byte)16));
                    Assert.That(obj.Minute, Is.EqualTo((byte)28));
                    Assert.That(obj.Second, Is.EqualTo((byte)0));
                }
            }));
        #endregion

        #region REAL_TIME_CLOCK SET_COMMAND
        // GLP X5 Bar 1000 3.0.7, Robe iEspriteLTL 1.7, Robe Spiider 4.0 Roxx Cluster B4 FC 1.6 .pcapng, frame 1206
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x0b, 0x0f, 0x16, 0x31, 0x11 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)11));
                    Assert.That(obj.Day, Is.EqualTo((byte)15));
                    Assert.That(obj.Hour, Is.EqualTo((byte)22));
                    Assert.That(obj.Minute, Is.EqualTo((byte)49));
                    Assert.That(obj.Second, Is.EqualTo((byte)17));
                }
            }));
        // Innotrans 1 elation KL und Litekraft wash XT.pcapng, frame 519
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x09, 0x0d, 0x0a, 0x20, 0x28 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)9));
                    Assert.That(obj.Day, Is.EqualTo((byte)13));
                    Assert.That(obj.Hour, Is.EqualTo((byte)10));
                    Assert.That(obj.Minute, Is.EqualTo((byte)32));
                    Assert.That(obj.Second, Is.EqualTo((byte)40));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 9282
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x12, 0x17, 0x04, 0x18 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)18));
                    Assert.That(obj.Hour, Is.EqualTo((byte)23));
                    Assert.That(obj.Minute, Is.EqualTo((byte)4));
                    Assert.That(obj.Second, Is.EqualTo((byte)24));
                }
            }));
        // GLP X5 Bar 1000 3.0.7, Robe iEspriteLTL 1.7, Robe Spiider 4.0 Roxx Cluster B4 FC 1.6 .pcapng, frame 1982
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x0b, 0x0f, 0x16, 0x31, 0x15 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)11));
                    Assert.That(obj.Day, Is.EqualTo((byte)15));
                    Assert.That(obj.Hour, Is.EqualTo((byte)22));
                    Assert.That(obj.Minute, Is.EqualTo((byte)49));
                    Assert.That(obj.Second, Is.EqualTo((byte)21));
                }
            }));
        // Innotrans 1 elation KL und Litekraft wash XT.pcapng, frame 522
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x09, 0x0d, 0x0a, 0x20, 0x29 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)9));
                    Assert.That(obj.Day, Is.EqualTo((byte)13));
                    Assert.That(obj.Hour, Is.EqualTo((byte)10));
                    Assert.That(obj.Minute, Is.EqualTo((byte)32));
                    Assert.That(obj.Second, Is.EqualTo((byte)41));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 11318
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x12, 0x17, 0x04, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)18));
                    Assert.That(obj.Hour, Is.EqualTo((byte)23));
                    Assert.That(obj.Minute, Is.EqualTo((byte)4));
                    Assert.That(obj.Second, Is.EqualTo((byte)48));
                }
            }));
        // GLP X5 Bar 1000 3.0.7, Robe iEspriteLTL 1.7, Robe Spiider 4.0 Roxx Cluster B4 FC 1.6 .pcapng, frame 2132
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x0b, 0x0f, 0x16, 0x31, 0x16 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)11));
                    Assert.That(obj.Day, Is.EqualTo((byte)15));
                    Assert.That(obj.Hour, Is.EqualTo((byte)22));
                    Assert.That(obj.Minute, Is.EqualTo((byte)49));
                    Assert.That(obj.Second, Is.EqualTo((byte)22));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 12655
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x07, 0x12, 0x17, 0x05, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));
                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Year, Is.EqualTo((ushort)2026));
                    Assert.That(obj.Month, Is.EqualTo((byte)7));
                    Assert.That(obj.Day, Is.EqualTo((byte)18));
                    Assert.That(obj.Hour, Is.EqualTo((byte)23));
                    Assert.That(obj.Minute, Is.EqualTo((byte)5));
                    Assert.That(obj.Second, Is.EqualTo((byte)1));
                }
            }));
        #endregion

        #region SELF_TEST_DESCRIPTION GET_COMMAND
        // Blackcoffe 1.pcapng, frame 3173
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)0));
                }
            }));
        // JBL P10.pcapng, frame 117
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)8));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3258
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // JBL P10.pcapng, frame 119
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)9));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3311
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)2));
                }
            }));
        // JBL P10.pcapng, frame 121
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x0a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)10));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3435
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)3));
                }
            }));
        // JBL P10.pcapng, frame 123
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x0b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)11));
                }
            }));
        #endregion

        #region SELF_TEST_DESCRIPTION GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 3301
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x01, 0x4d, 0x6f, 0x74, 0x6f, 0x72, 0x73, 0x20, 0x61, 0x6e, 0x64, 0x20, 0x4c, 0x45, 0x44, 0x73 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("Motors and LEDs"));
                }
            }));
        // Martin Atomic 3000 LED v1.4.0 .pcapng, frame 62
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x54, 0x45, 0x53, 0x54, 0x20, 0x53, 0x54, 0x4f, 0x50, 0x50, 0x45, 0x44 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)84));
                    Assert.That(obj.Description, Is.EqualTo("EST STOPPED"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3384
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x02, 0x50, 0x61, 0x6e, 0x20, 0x2f, 0x20, 0x54, 0x69, 0x6c, 0x74 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)2));
                    Assert.That(obj.Description, Is.EqualTo("Pan / Tilt"));
                }
            }));
        // Martin Atomic 3000 LED v1.4.0 .pcapng, frame 66
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x54, 0x45, 0x53, 0x54, 0x20, 0x41, 0x4c, 0x4c },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)84));
                    Assert.That(obj.Description, Is.EqualTo("EST ALL"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3554
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x03, 0x4c, 0x45, 0x44, 0x73 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)3));
                    Assert.That(obj.Description, Is.EqualTo("LEDs"));
                }
            }));
        // Martin MAC Aura XIP v1.3.1.pcapng, frame 110
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x00, 0x54, 0x45, 0x53, 0x54, 0x20, 0x53, 0x54, 0x4f, 0x50, 0x50, 0x45, 0x44 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)0));
                    Assert.That(obj.Description, Is.EqualTo("TEST STOPPED"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3775
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x04, 0x46, 0x61, 0x6e, 0x73 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)4));
                    Assert.That(obj.Description, Is.EqualTo("Fans"));
                }
            }));
        // Martin MAC Aura XIP v1.3.1.pcapng, frame 112
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x01, 0x54, 0x45, 0x53, 0x54, 0x20, 0x41, 0x4c, 0x4c },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));
                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SelfTestRequester, Is.EqualTo((byte)1));
                    Assert.That(obj.Description, Is.EqualTo("TEST ALL"));
                }
            }));
        #endregion

        #region SENSOR_DEFINITION GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 39
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)9));
                }
            }));
        // DIHK 2025 1.pcapng, frame 2543
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x12 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)18));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 180
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)0));
                }
            }));
        // MELH 1_7.pcapng, frame 7617
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x17 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)23));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 41
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x0a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)10));
                }
            }));
        // DIHK 2025 1.pcapng, frame 2545
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x13 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)19));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 182
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 43
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x0b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)11));
                }
            }));
        #endregion

        #region SENSOR_DEFINITION GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 24
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x02, 0x00, 0x01, 0x00, 0xff, 0xc9, 0x00, 0x7d, 0xff, 0xc9, 0x00, 0x69, 0x01, 0x4c, 0x45, 0x44, 0x33 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)2));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)-55));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)125));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)-55));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)105));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(true));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("LED3"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 2259
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x82, 0x00, 0x0a, 0x00, 0x64, 0x00, 0x4c, 0x45, 0x44, 0x20, 0x54, 0x65, 0x6d, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)0));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)130));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)10));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)100));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("LED Temperature"));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 16
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0xff, 0xec, 0x00, 0x50, 0x00, 0x00, 0x00, 0x50, 0x00, 0x54, 0x45, 0x4d, 0x50, 0x45, 0x52, 0x41, 0x54, 0x55, 0x52, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)-20));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)80));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)0));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)80));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("TEMPERATURE"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2519
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0xff, 0x80, 0x00, 0x7f, 0x00, 0x00, 0x00, 0x55, 0x00, 0x42, 0x65, 0x61, 0x6d, 0x20, 0x4c, 0x45, 0x44, 0x20, 0x31 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)-128));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)127));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)0));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)85));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("Beam LED 1"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2633
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0xff, 0xe0, 0x00, 0xde, 0x00, 0x00, 0x00, 0x55, 0x02, 0x48, 0x65, 0x61, 0x64, 0x20, 0x54, 0x65, 0x6d, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)-32));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)222));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)0));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)85));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(true));
                    Assert.That(obj.Description, Is.EqualTo("Head Temperature"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 137
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0xff, 0xd7, 0x00, 0x7d, 0xff, 0xd7, 0x00, 0x7d, 0x00, 0x54, 0x65, 0x6d, 0x70, 0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)-41));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)125));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)-41));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)125));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("Temperature"));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 90
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0xff, 0xd8, 0x00, 0x7d, 0x80, 0x00, 0x00, 0x50, 0x00, 0x4c, 0x45, 0x44, 0x31, 0x20, 0x54, 0x45, 0x4d, 0x50 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)-40));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)125));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)-32768));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)80));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("LED1 TEMP"));
                }
            }));
        // Chroma-Q Color Force II 72 CFII 72 v1.12.pcapng, frame 24, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x50, 0x00, 0x45, 0x6e, 0x67, 0x69, 0x6e, 0x65, 0x20, 0x54, 0x65, 0x6d, 0x70, 0x2e, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));
                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.Type, Is.EqualTo((ERDM_SensorType)0x00));
                    Assert.That(obj.Unit, Is.EqualTo((ERDM_SensorUnit)0x01));
                    Assert.That(obj.Prefix, Is.EqualTo((ERDM_UnitPrefix)0x00));
                    Assert.That(obj.RangeMinimum, Is.EqualTo((short)0));
                    Assert.That(obj.RangeMaximum, Is.EqualTo((short)100));
                    Assert.That(obj.NormalMinimum, Is.EqualTo((short)0));
                    Assert.That(obj.NormalMaximum, Is.EqualTo((short)80));
                    Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                    Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(false));
                    Assert.That(obj.Description, Is.EqualTo("Engine Temp."));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x50, 0x00, 0x45, 0x6e, 0x67, 0x69, 0x6e, 0x65, 0x20, 0x54, 0x65, 0x6d, 0x70, 0x2e });
        #endregion

        #region SENSOR_VALUE GET_COMMAND
        // Ayirton Rivale V1.2.2.pcapng, frame 179
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)9));
                }
            }));
        // DIHK 2025 1.pcapng, frame 2752
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x12 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)18));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 236
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)0));
                }
            }));
        // MELH 1_7.pcapng, frame 8395
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x17 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)23));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 181
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x0a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)10));
                }
            }));
        // DIHK 2025 1.pcapng, frame 2756
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x13 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)19));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 238
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)1));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 183
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x0b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((byte)11));
                }
            }));
        #endregion

        #region SENSOR_VALUE GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 110
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)34));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)0));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)34));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Astera AX2 5.14.84 (2).pcapng, frame 544
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x1a, 0x00, 0x00, 0x00, 0x1e, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)26));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)0));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)30));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Ayirton Rivale V1.2.2.pcapng, frame 162
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x27, 0x00, 0x0b, 0x00, 0x50, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)39));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)11));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)80));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Bairut Colorado and Rivale.pcapng, frame 2141
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x2a, 0x00, 0x2a, 0x00, 0x2a, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)42));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)42));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)42));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Blackcoffe 1.pcapng, frame 4019
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x43, 0x00, 0x00, 0x00, 0x4e, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)67));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)0));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)78));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Blackcoffe 1.pcapng, frame 6735
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x16, 0x80, 0x00, 0x7f, 0xff, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)22));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)-32768));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)32767));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3.pcapng, frame 150
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)21));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)0));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)0));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        // Chroma-Q Color Force II 72 CFII 72 v1.12.pcapng, frame 48
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] { 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));
                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SensorId, Is.EqualTo((byte)0));
                    Assert.That(obj.PresentValue, Is.EqualTo((short)20));
                    Assert.That(obj.LowestValue, Is.EqualTo((short)0));
                    Assert.That(obj.HighestValue, Is.EqualTo((short)0));
                    Assert.That(obj.RecordedValue, Is.EqualTo((short)0));
                }
            }));
        #endregion

        #region SERIAL_NUMBER GET_COMMAND_RESPONSE
        // Lollapalooza 2026 1.pcapng, frame 7935
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x34, 0x32, 0x32, 0x30, 0x30, 0x30, 0x35, 0x38 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203042200058"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8018
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x39, 0x32, 0x35, 0x30, 0x30, 0x31, 0x34, 0x33 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203092500143"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8086
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x39, 0x32, 0x35, 0x30, 0x30, 0x30, 0x36, 0x33 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203092500063"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 8155
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x34, 0x32, 0x32, 0x30, 0x30, 0x30, 0x36, 0x37 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203042200067"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 9721
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x34, 0x32, 0x34, 0x30, 0x30, 0x30, 0x33, 0x31 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203042400031"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 11734
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x34, 0x32, 0x32, 0x30, 0x30, 0x30, 0x33, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203042200030"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 14900
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x39, 0x32, 0x35, 0x30, 0x30, 0x31, 0x33, 0x35 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203092500135"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 15064
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] { 0x32, 0x30, 0x33, 0x30, 0x39, 0x32, 0x35, 0x30, 0x30, 0x31, 0x33, 0x31 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("203092500131"));
                }
            }));
        #endregion

        #region SLOT_DESCRIPTION GET_COMMAND
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 200
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)0));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 79
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x0a },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)10));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 202
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)1));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 81
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x0b },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)11));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 204
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)2));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 83
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x0c },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)12));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 206
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)3));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 85
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x0d },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)13));
                }
            }));
        #endregion

        #region SLOT_DESCRIPTION GET_COMMAND_RESPONSE
        // Ayirton Rivale V1.2.2.pcapng, frame 70
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x50, 0x61, 0x6e, 0x20, 0x43, 0x6f, 0x61, 0x72, 0x73, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("Pan Coarse"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 3183
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x50, 0x61, 0x6e },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("Pan"));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 44, String padded with NUL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x52, 0x65, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: the PDL has to match the String, but a NUL terminates it, the padding is reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Null-Delimiter"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("Red"));
                }
            }), normalizedPayload: new byte[] { 0x00, 0x00, 0x52, 0x65, 0x64 });
        // Coemar Relite MK2 v0.94.pcapng, frame 90
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x01, 0x44, 0x69, 0x6d, 0x6d, 0x65, 0x72, 0x20, 0x46, 0x69, 0x6e, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)1));
                    Assert.That(obj.Description, Is.EqualTo("Dimmer Fine"));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 201
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x52, 0x65, 0x64 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("Red"));
                }
            }));
        // GLP X4Bar20 V78.pcapng, frame 60
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x54, 0x69, 0x6c, 0x74, 0x20, 0x63, 0x6f, 0x61, 0x72, 0x73, 0x65 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("Tilt coarse"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 824
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x55, 0x4e, 0x49, 0x54, 0x20, 0x28, 0x4f, 0x6e, 0x2f, 0x4f, 0x66, 0x66, 0x29 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("UNIT (On/Off)"));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 27748
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x46, 0x6f, 0x67 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));
                var obj = (RDMSlotDescription)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.SlotId, Is.EqualTo((ushort)0));
                    Assert.That(obj.Description, Is.EqualTo("Fog"));
                }
            }));
        #endregion

        #region SLOT_INFO GET_COMMAND_RESPONSE
        // Blackcoffe 1.pcapng, frame 3023, 3043 (ACK_OVERFLOW)
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x02, 0x00, 0x03, 0x01, 0x00, 0x02, 0x00, 0x04, 0x00, 0x05, 0x03, 0x00, 0x05, 0x00, 0x05, 0x02, 0x00, 0x06, 0x00, 0x02, 0x02, 0x00, 0x07, 0x00, 0x02, 0x04, 0x00, 0x08, 0x00, 0x02, 0x03, 0x00, 0x09, 0x00, 0x02, 0x01, 0x00, 0x0a, 0x01, 0x00, 0x09, 0x00, 0x0b, 0x00, 0x05, 0x04, 0x00, 0x0c, 0x03, 0x00, 0x0f, 0x00, 0x0d, 0x02, 0x00, 0x09, 0x00, 0x0e, 0x02, 0x00, 0x1f, 0x00, 0x0f, 0x00, 0x03, 0x04, 0x00, 0x10, 0x06, 0x00, 0x0f, 0x00, 0x11, 0x04, 0x00, 0x0f, 0x00, 0x12, 0x00, 0x03, 0x01, 0x00, 0x13, 0x00, 0x03, 0x02, 0x00, 0x14, 0x07, 0x00, 0x13, 0x00, 0x15, 0x01, 0x00, 0x14, 0x00, 0x16, 0x00, 0x03, 0x03, 0x00, 0x17, 0x06, 0x00, 0x16, 0x00, 0x18, 0x00, 0x03, 0x03, 0x00, 0x19, 0x06, 0x00, 0x18, 0x00, 0x1a, 0x00, 0x04, 0x01, 0x00, 0x1b, 0x00, 0x04, 0x07, 0x00, 0x1c, 0x00, 0x05, 0x04, 0x00, 0x1d, 0x06, 0x00, 0x1c, 0x00, 0x1e, 0x00, 0x04, 0x03, 0x00, 0x1f, 0x00, 0x04, 0x05, 0x00, 0x20, 0x01, 0x00, 0x1f, 0x00, 0x21, 0x00, 0x04, 0x02, 0x00, 0x22, 0x01, 0x00, 0x21, 0x00, 0x23, 0x00, 0xff, 0xff, 0x00, 0x24, 0x00, 0x04, 0x04, 0x00, 0x25, 0x00, 0x00, 0x02, 0x00, 0x26, 0x01, 0x00, 0x25 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x03, (ERDM_SlotType)0x02, (ERDM_SlotType)0x02, (ERDM_SlotType)0x00, (ERDM_SlotType)0x06, (ERDM_SlotType)0x04, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x07, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x06, (ERDM_SlotType)0x00, (ERDM_SlotType)0x06, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x06, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0x0101, 0x0000, 0x0102, 0x0002, 0x0503, 0x0502, 0x0202, 0x0204, 0x0203, 0x0201, 0x0009, 0x0504, 0x000F, 0x0009, 0x001F, 0x0304, 0x000F, 0x000F, 0x0301, 0x0302, 0x0013, 0x0014, 0x0303, 0x0016, 0x0303, 0x0018, 0x0401, 0x0407, 0x0504, 0x001C, 0x0403, 0x0405, 0x001F, 0x0402, 0x0021, 0xFFFF, 0x0404, 0x0002, 0x0025 }));
                }
            }));
        // Clay Paky Midi-B FW 5.4.pcapng, frame 42
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0x02, 0x05, 0x00, 0x01, 0x00, 0x02, 0x06, 0x00, 0x02, 0x00, 0x02, 0x07, 0x00, 0x03, 0x00, 0x02, 0x03, 0x00, 0x04, 0x00, 0x05, 0x04, 0x00, 0x05, 0x00, 0x05, 0x04, 0x00, 0x06, 0x00, 0x04, 0x04, 0x00, 0x07, 0x00, 0x00, 0x01, 0x00, 0x08, 0x01, 0x00, 0x07, 0x00, 0x09, 0x00, 0x01, 0x01, 0x00, 0x0a, 0x01, 0x00, 0x09, 0x00, 0x0b, 0x00, 0x01, 0x02, 0x00, 0x0c, 0x01, 0x00, 0x0b, 0x00, 0x0d, 0x00, 0x04, 0x05, 0x00, 0x0e, 0x00, 0x05, 0x04, 0x00, 0x0f, 0x00, 0x05, 0x02, 0x00, 0x10, 0x00, 0x05, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0x0205, 0x0206, 0x0207, 0x0203, 0x0504, 0x0504, 0x0404, 0x0001, 0x0007, 0x0101, 0x0009, 0x0102, 0x000B, 0x0405, 0x0504, 0x0502, 0x0502 }));
                }
            }));
        // Coemar Relite MK2 v0.94.pcapng, frame 86
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x01, 0x00, 0x01, 0x00, 0x02, 0x00, 0x02, 0x08, 0x00, 0x03, 0x00, 0x04, 0x04, 0x00, 0x04, 0x00, 0x05, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x01, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0x0002, 0x0001, 0x0208, 0x0404, 0x0502 }));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 199
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0xff, 0xff, 0x00, 0x01, 0x00, 0xff, 0xff, 0x00, 0x02, 0x00, 0xff, 0xff, 0x00, 0x03, 0x00, 0xff, 0xff, 0x00, 0x04, 0x00, 0xff, 0xff, 0x00, 0x05, 0x00, 0xff, 0xff, 0x00, 0x06, 0x00, 0xff, 0xff, 0x00, 0x07, 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x04, 0x04, 0x00, 0x09, 0x00, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0x0001, 0x0404, 0xFFFF }));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 802
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0xff, 0xff, 0x00, 0x01, 0x00, 0xff, 0xff, 0x00, 0x02, 0x00, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0xFFFF, 0xFFFF, 0xFFFF }));
                }
            }));
        // Lollapalooza 2026 1.pcapng, frame 27717
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x00 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0x0001, 0x0001 }));
                }
            }));
        // MELH 1_7.pcapng, frame 373
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] { 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x02, 0x08, 0x00, 0x03, 0x00, 0x02, 0x05, 0x00, 0x04, 0x00, 0x02, 0x06, 0x00, 0x05, 0x00, 0x02, 0x07, 0x00, 0x06, 0x00, 0x02, 0x12, 0x00, 0x07, 0x00, 0x02, 0x05, 0x00, 0x08, 0x00, 0x02, 0x06, 0x00, 0x09, 0x00, 0x02, 0x07, 0x00, 0x0a, 0x00, 0x02, 0x12, 0x00, 0x0b, 0x00, 0x02, 0x05, 0x00, 0x0c, 0x00, 0x02, 0x06, 0x00, 0x0d, 0x00, 0x02, 0x07, 0x00, 0x0e, 0x00, 0x02, 0x12, 0x00, 0x0f, 0x00, 0x02, 0x05, 0x00, 0x10, 0x00, 0x02, 0x06, 0x00, 0x11, 0x00, 0x02, 0x07, 0x00, 0x12, 0x00, 0x02, 0x12, 0x00, 0x13, 0x00, 0x02, 0x05, 0x00, 0x14, 0x00, 0x02, 0x06, 0x00, 0x15, 0x00, 0x02, 0x07, 0x00, 0x16, 0x00, 0x02, 0x12, 0x00, 0x17, 0x00, 0x02, 0x05, 0x00, 0x18, 0x00, 0x02, 0x06, 0x00, 0x19, 0x00, 0x02, 0x07, 0x00, 0x1a, 0x00, 0x02, 0x12, 0x00, 0x1b, 0x00, 0x02, 0x05, 0x00, 0x1c, 0x00, 0x02, 0x06, 0x00, 0x1d, 0x00, 0x02, 0x07, 0x00, 0x1e, 0x00, 0x02, 0x12, 0x00, 0x1f, 0x00, 0x02, 0x05, 0x00, 0x20, 0x00, 0x02, 0x06, 0x00, 0x21, 0x00, 0x02, 0x07, 0x00, 0x22, 0x00, 0x02, 0x12 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                var obj = (RDMSlotInfo[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34 }));
                    Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00, (ERDM_SlotType)0x00 }));
                    Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { 0x0404, 0x0001, 0x0208, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212, 0x0205, 0x0206, 0x0207, 0x0212 }));
                }
            }));
        #endregion

        #region SOFTWARE_VERSION_LABEL GET_COMMAND_RESPONSE
        // Lollapalooza 2026 1.pcapng, frame 7260, characters after the NUL terminator
        // Known deviation of manufacturer 0x6A6A: ROXX (Cluster B4 FC FW V1.6) sends a character after the NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x31, 0x2e, 0x36, 0x00, 0x43 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                // E1.20 10.1: a NUL terminates the String, the characters after it are ignored and reported
                Assert.That(allIssues(dataTreeBranch).Select(i => i.ToString()), Has.Some.Contains("Trailing Characters"));
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V1.6"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x31, 0x2e, 0x36 });
        // ACME CM S2 V20.pcapng, frame 10, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V20-20-20-20-20-20"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30, 0x2d, 0x32, 0x30 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 362, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x30, 0x31, 0x2e, 0x30, 0x38, 0x2e, 0x32, 0x2e, 0x38, 0x34, 0x64, 0x36, 0x36, 0x33, 0x2e, 0x35, 0x31, 0x39, 0x31, 0x36, 0x32, 0x33, 0x37, 0x37, 0x36, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V01.08.2.84d663.5191623776"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x30, 0x31, 0x2e, 0x30, 0x38, 0x2e, 0x32, 0x2e, 0x38, 0x34, 0x64, 0x36, 0x36, 0x33, 0x2e, 0x35, 0x31, 0x39, 0x31, 0x36, 0x32, 0x33, 0x37, 0x37, 0x36 });
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 930
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x34, 0x2e, 0x30 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("4.0"));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 956
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x32, 0x2e, 0x32, 0x2e, 0x34, 0x20 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V2.2.4 "));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 12
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x35, 0x2e, 0x31, 0x34, 0x2e, 0x38, 0x34 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V5.14.84"));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2313
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x30, 0x2e, 0x33, 0x2e, 0x34, 0x2d, 0x72, 0x63 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("0.3.4-rc"));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 1166, String with NUL terminator
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x56, 0x31, 0x2e, 0x32, 0x35, 0x30, 0x36, 0x32, 0x34, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("V1.250624"));
                }
            }), normalizedPayload: new byte[] { 0x56, 0x31, 0x2e, 0x32, 0x35, 0x30, 0x36, 0x32, 0x34 });
        #endregion

        #region STATUS_ID_DESCRIPTION GET_COMMAND
        // Lumisplit 1.6 FW 7.0.pcapng, frame 417
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.STATUS_ID_DESCRIPTION,
            new byte[] { 0x80, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ushort)32773));
                }
            }));
        #endregion

        #region STATUS_ID_DESCRIPTION GET_COMMAND_RESPONSE
        // Lumisplit 1.6 FW 7.0.pcapng, frame 418
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.STATUS_ID_DESCRIPTION,
            new byte[] { 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x20, 0x62, 0x6f, 0x6f, 0x74, 0x65, 0x64 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo("Device booted"));
                }
            }));
        #endregion

        #region STATUS_MESSAGES GET_COMMAND
        // Astera AX5 5.14.84.pcapng, frame 538
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));
                var obj = (ERDM_Status)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Status)0x01));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 222
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));
                var obj = (ERDM_Status)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Status)0x00));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 540
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));
                var obj = (ERDM_Status)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo((ERDM_Status)0x02));
                }
            }));
        #endregion

        #region STATUS_MESSAGES GET_COMMAND_RESPONSE
        // DIHK 2025 1.pcapng, frame 2879
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x00, 0x00, 0x02, 0x00, 0x31, 0x00, 0x01, 0x00, 0xe8, 0x00, 0x00, 0x02, 0x00, 0x31, 0x00, 0x02, 0x00, 0xe7, 0x00, 0x00, 0x02, 0x00, 0x31, 0x00, 0x03, 0x00, 0xea },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMStatusMessage[])));
                var obj = (RDMStatusMessage[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => (int)s.SubDeviceId), Is.EqualTo(new int[] { 0, 0, 0 }));
                    Assert.That(obj.Select(s => (int)s.StatusType), Is.EqualTo(new int[] { 2, 2, 2 }));
                    Assert.That(obj.Select(s => (int)s.StatusMessage), Is.EqualTo(new int[] { 49, 49, 49 }));
                    Assert.That(obj.Select(s => (ushort)s.DataValue1), Is.EqualTo(new ushort[] { 1, 2, 3 }));
                    Assert.That(obj.Select(s => (ushort)s.DataValue2), Is.EqualTo(new ushort[] { 232, 231, 234 }));
                }
            }));
        // ELC SourceFourLED2 Lustr simp.pcapng, frame 223
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] {  },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMStatusMessage[])));
                var obj = (RDMStatusMessage[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.Empty);
                }
            }));
        // Estrel 14_01_2026_2.pcapng, frame 6198
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x00, 0x00, 0x02, 0x00, 0x50, 0x00, 0x01, 0x00, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMStatusMessage[])));
                var obj = (RDMStatusMessage[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => (int)s.SubDeviceId), Is.EqualTo(new int[] { 0 }));
                    Assert.That(obj.Select(s => (int)s.StatusType), Is.EqualTo(new int[] { 2 }));
                    Assert.That(obj.Select(s => (int)s.StatusMessage), Is.EqualTo(new int[] { 80 }));
                    Assert.That(obj.Select(s => (ushort)s.DataValue1), Is.EqualTo(new ushort[] { 1 }));
                    Assert.That(obj.Select(s => (ushort)s.DataValue2), Is.EqualTo(new ushort[] { 0 }));
                }
            }));
        // DIHK 2025 1.pcapng, frame 2880
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x00, 0x00, 0x02, 0x00, 0x31, 0x00, 0x01, 0x00, 0xe9, 0x00, 0x00, 0x02, 0x00, 0x31, 0x00, 0x02, 0x00, 0xe8, 0x00, 0x00, 0x02, 0x00, 0x31, 0x00, 0x03, 0x00, 0xe9 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMStatusMessage[])));
                var obj = (RDMStatusMessage[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj.Select(s => (int)s.SubDeviceId), Is.EqualTo(new int[] { 0, 0, 0 }));
                    Assert.That(obj.Select(s => (int)s.StatusType), Is.EqualTo(new int[] { 2, 2, 2 }));
                    Assert.That(obj.Select(s => (int)s.StatusMessage), Is.EqualTo(new int[] { 49, 49, 49 }));
                    Assert.That(obj.Select(s => (ushort)s.DataValue1), Is.EqualTo(new ushort[] { 1, 2, 3 }));
                    Assert.That(obj.Select(s => (ushort)s.DataValue2), Is.EqualTo(new ushort[] { 233, 232, 233 }));
                }
            }));
        #endregion

        #region SUPPORTED_PARAMETERS GET_COMMAND_RESPONSE
        // ACME CM S2 V20.pcapng, frame 4
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x30, 0x00, 0x31, 0x00, 0x32, 0x00, 0x60, 0x00, 0x81, 0x00, 0x82, 0x00, 0x80, 0x00, 0xc0, 0x00, 0x50, 0x00, 0xe0, 0x00, 0xe1, 0x00, 0xf0, 0x00, 0xe0, 0x10, 0x01, 0x10, 0x00, 0x02, 0x00, 0x02, 0x01, 0x02, 0x02, 0x00, 0x90, 0x04, 0x01, 0x04, 0x00, 0x06, 0x00, 0x06, 0x01, 0x03, 0x43, 0x03, 0x44, 0x03, 0x47, 0x03, 0x48, 0xa0, 0x21, 0xa0, 0x08, 0xa0, 0x18, 0xa0, 0x20, 0xa0, 0x04, 0xa0, 0x07, 0xa0, 0x25, 0xa0, 0x26, 0xa0, 0x10 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0030, (ERDM_Parameter)0x0031, (ERDM_Parameter)0x0032, (ERDM_Parameter)0x0060, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0080, (ERDM_Parameter)0x00C0, (ERDM_Parameter)0x0050, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x00F0, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x1001, (ERDM_Parameter)0x1000, (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x0202, (ERDM_Parameter)0x0090, (ERDM_Parameter)0x0401, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x0600, (ERDM_Parameter)0x0601, (ERDM_Parameter)0x0343, (ERDM_Parameter)0x0344, (ERDM_Parameter)0x0347, (ERDM_Parameter)0x0348, (ERDM_Parameter)0xA021, (ERDM_Parameter)0xA008, (ERDM_Parameter)0xA018, (ERDM_Parameter)0xA020, (ERDM_Parameter)0xA004, (ERDM_Parameter)0xA007, (ERDM_Parameter)0xA025, (ERDM_Parameter)0xA026, (ERDM_Parameter)0xA010 }));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 256
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x81, 0x00, 0x82, 0x00, 0x80, 0x00, 0x70, 0x00, 0xc1, 0x00, 0xc2, 0x00, 0xe0, 0x00, 0x51, 0x80, 0x82 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0070, (ERDM_Parameter)0x00C1, (ERDM_Parameter)0x00C2, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x0051, (ERDM_Parameter)0x8082 }));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 273
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0xf0, 0x00, 0x80, 0x00, 0x81, 0x06, 0x00, 0x06, 0x01, 0x00, 0x82, 0x04, 0x00, 0x05, 0x00, 0x05, 0x01, 0x00, 0xe0, 0x00, 0xe1, 0x02, 0x00, 0x02, 0x01, 0x00, 0x20, 0x00, 0x30, 0x00, 0x31, 0x10, 0x01, 0x01, 0x20, 0x01, 0x21, 0x01, 0x22, 0x06, 0x03, 0x90, 0x10, 0x90, 0x20, 0x90, 0x30, 0xa0, 0x01, 0xa0, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x00F0, (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0600, (ERDM_Parameter)0x0601, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x0500, (ERDM_Parameter)0x0501, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x0020, (ERDM_Parameter)0x0030, (ERDM_Parameter)0x0031, (ERDM_Parameter)0x1001, (ERDM_Parameter)0x0120, (ERDM_Parameter)0x0121, (ERDM_Parameter)0x0122, (ERDM_Parameter)0x0603, (ERDM_Parameter)0x9010, (ERDM_Parameter)0x9020, (ERDM_Parameter)0x9030, (ERDM_Parameter)0xA001, (ERDM_Parameter)0xA005 }));
                }
            }));
        // Airbus Swisson Nodes Ayrton Eurus S.pcapng, frame 277
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x80, 0x00, 0x81, 0x00, 0x82, 0x00, 0x90, 0x00, 0xe0, 0x00, 0xe1, 0x02, 0x00, 0x02, 0x01, 0x04, 0x00, 0x04, 0x01, 0x04, 0x03, 0x05, 0x00, 0x01, 0x20, 0x01, 0x21, 0x06, 0x00, 0x06, 0x01, 0x10, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0090, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x0401, (ERDM_Parameter)0x0403, (ERDM_Parameter)0x0500, (ERDM_Parameter)0x0120, (ERDM_Parameter)0x0121, (ERDM_Parameter)0x0600, (ERDM_Parameter)0x0601, (ERDM_Parameter)0x1001 }));
                }
            }));
        // Astera AX5 5.14.84.pcapng, frame 4
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x02, 0x00, 0x02, 0x01, 0x00, 0x82, 0x04, 0x01, 0x04, 0x03, 0x04, 0x00, 0x00, 0x81, 0x00, 0x80, 0x00, 0x30, 0x10, 0x01, 0x00, 0xe0, 0x00, 0xe1, 0x03, 0x43, 0x03, 0x44, 0x00, 0x31, 0x00, 0x50, 0x80, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0401, (ERDM_Parameter)0x0403, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0030, (ERDM_Parameter)0x1001, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x0343, (ERDM_Parameter)0x0344, (ERDM_Parameter)0x0031, (ERDM_Parameter)0x0050, (ERDM_Parameter)0x8001 }));
                }
            }));
        // Blackcoffe 1.pcapng, frame 2141
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x51, 0x00, 0x80, 0x00, 0x81, 0x00, 0x82, 0x00, 0x90, 0x00, 0xc1, 0x00, 0xc2, 0x00, 0xe0, 0x00, 0xe1, 0x02, 0x00, 0x02, 0x01, 0x02, 0x02, 0x04, 0x00, 0x04, 0x01, 0x04, 0x05, 0x10, 0x01, 0x10, 0x20, 0x10, 0x21, 0x00, 0x20, 0x00, 0xc0, 0x80, 0x60, 0x80, 0x30, 0x86, 0x1a, 0x86, 0x19 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0051, (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0090, (ERDM_Parameter)0x00C1, (ERDM_Parameter)0x00C2, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x0202, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x0401, (ERDM_Parameter)0x0405, (ERDM_Parameter)0x1001, (ERDM_Parameter)0x1020, (ERDM_Parameter)0x1021, (ERDM_Parameter)0x0020, (ERDM_Parameter)0x00C0, (ERDM_Parameter)0x8060, (ERDM_Parameter)0x8030, (ERDM_Parameter)0x861A, (ERDM_Parameter)0x8619 }));
                }
            }));
        // COLORado PXL Curve 12 V1.250624 RDM.pcapng, frame 4
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x80, 0x00, 0x81, 0x06, 0x01, 0x00, 0x82, 0x00, 0x90, 0x04, 0x00, 0x04, 0x01, 0x05, 0x00, 0x00, 0xe0, 0x00, 0xe1, 0x02, 0x00, 0x02, 0x01, 0x10, 0x01, 0x00, 0x51, 0x03, 0x43, 0x03, 0x44, 0x03, 0x47, 0x03, 0x48, 0x80, 0x01, 0x80, 0x06, 0x80, 0x05, 0x80, 0x08, 0x80, 0x09, 0x80, 0x0a, 0x80, 0x0b, 0x80, 0x07, 0x80, 0x13, 0x80, 0x14, 0x80, 0x15, 0x80, 0x16, 0x80, 0x17, 0x80, 0x18, 0x80, 0x19, 0x80, 0x1a, 0x80, 0x1b, 0x80, 0x1c, 0x80, 0x1d, 0x80, 0x1e, 0x80, 0x1f, 0x80, 0x20, 0x80, 0x21, 0x80, 0x22, 0x80, 0x23, 0x80, 0x24, 0x80, 0x25, 0x80, 0x26, 0x80, 0x27, 0x80, 0x28, 0x80, 0x29, 0x80, 0x2a, 0x80, 0x2b, 0x80, 0x2c, 0x80, 0x2d, 0x80, 0x2e, 0x80, 0x2f, 0x80, 0x30, 0x80, 0x31, 0x80, 0x32, 0x80, 0x33, 0x80, 0x34, 0x80, 0x35, 0x80, 0x36, 0x81, 0x01, 0x81, 0x02, 0x81, 0x03, 0x81, 0x04, 0x81, 0x05, 0x81, 0x06, 0x81, 0x07, 0x81, 0x08, 0x81, 0x09, 0x81, 0x0a, 0x81, 0x0b, 0x81, 0x0c },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0601, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x0090, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x0401, (ERDM_Parameter)0x0500, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x1001, (ERDM_Parameter)0x0051, (ERDM_Parameter)0x0343, (ERDM_Parameter)0x0344, (ERDM_Parameter)0x0347, (ERDM_Parameter)0x0348, (ERDM_Parameter)0x8001, (ERDM_Parameter)0x8006, (ERDM_Parameter)0x8005, (ERDM_Parameter)0x8008, (ERDM_Parameter)0x8009, (ERDM_Parameter)0x800A, (ERDM_Parameter)0x800B, (ERDM_Parameter)0x8007, (ERDM_Parameter)0x8013, (ERDM_Parameter)0x8014, (ERDM_Parameter)0x8015, (ERDM_Parameter)0x8016, (ERDM_Parameter)0x8017, (ERDM_Parameter)0x8018, (ERDM_Parameter)0x8019, (ERDM_Parameter)0x801A, (ERDM_Parameter)0x801B, (ERDM_Parameter)0x801C, (ERDM_Parameter)0x801D, (ERDM_Parameter)0x801E, (ERDM_Parameter)0x801F, (ERDM_Parameter)0x8020, (ERDM_Parameter)0x8021, (ERDM_Parameter)0x8022, (ERDM_Parameter)0x8023, (ERDM_Parameter)0x8024, (ERDM_Parameter)0x8025, (ERDM_Parameter)0x8026, (ERDM_Parameter)0x8027, (ERDM_Parameter)0x8028, (ERDM_Parameter)0x8029, (ERDM_Parameter)0x802A, (ERDM_Parameter)0x802B, (ERDM_Parameter)0x802C, (ERDM_Parameter)0x802D, (ERDM_Parameter)0x802E, (ERDM_Parameter)0x802F, (ERDM_Parameter)0x8030, (ERDM_Parameter)0x8031, (ERDM_Parameter)0x8032, (ERDM_Parameter)0x8033, (ERDM_Parameter)0x8034, (ERDM_Parameter)0x8035, (ERDM_Parameter)0x8036, (ERDM_Parameter)0x8101, (ERDM_Parameter)0x8102, (ERDM_Parameter)0x8103, (ERDM_Parameter)0x8104, (ERDM_Parameter)0x8105, (ERDM_Parameter)0x8106, (ERDM_Parameter)0x8107, (ERDM_Parameter)0x8108, (ERDM_Parameter)0x8109, (ERDM_Parameter)0x810A, (ERDM_Parameter)0x810B, (ERDM_Parameter)0x810C }));
                }
            }));
        // Cameo Zenit W600 G2 v1.7.3 Update to v1.8.4.pcapng, frame 8
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x80, 0x00, 0x81, 0x00, 0x82, 0x00, 0xe0, 0x00, 0xe1, 0x00, 0x51, 0x02, 0x00, 0x02, 0x01, 0x00, 0x90, 0x03, 0x43, 0x03, 0x44, 0x03, 0x45, 0x04, 0x00, 0x82, 0x11, 0x82, 0x12, 0x82, 0x16, 0x82, 0x17, 0x82, 0x18, 0x82, 0x19, 0x82, 0x1a, 0x82, 0x1b, 0x84, 0x41, 0x84, 0x51, 0x83, 0x63, 0x84, 0x11, 0x03, 0x47, 0x03, 0x48, 0x05, 0x00, 0x82, 0x31, 0x82, 0x32, 0x82, 0x33, 0x82, 0x34, 0x82, 0x24, 0x82, 0xa2, 0x82, 0xa3, 0x82, 0x41, 0x82, 0x81, 0x82, 0x91, 0x03, 0x46, 0x12, 0x00, 0x12, 0x04, 0x12, 0x01, 0x12, 0x02, 0x12, 0x03, 0x12, 0x05, 0x82, 0x13, 0x84, 0x21 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = (ERDM_Parameter[])dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(new ERDM_Parameter[] { (ERDM_Parameter)0x0080, (ERDM_Parameter)0x0081, (ERDM_Parameter)0x0082, (ERDM_Parameter)0x00E0, (ERDM_Parameter)0x00E1, (ERDM_Parameter)0x0051, (ERDM_Parameter)0x0200, (ERDM_Parameter)0x0201, (ERDM_Parameter)0x0090, (ERDM_Parameter)0x0343, (ERDM_Parameter)0x0344, (ERDM_Parameter)0x0345, (ERDM_Parameter)0x0400, (ERDM_Parameter)0x8211, (ERDM_Parameter)0x8212, (ERDM_Parameter)0x8216, (ERDM_Parameter)0x8217, (ERDM_Parameter)0x8218, (ERDM_Parameter)0x8219, (ERDM_Parameter)0x821A, (ERDM_Parameter)0x821B, (ERDM_Parameter)0x8441, (ERDM_Parameter)0x8451, (ERDM_Parameter)0x8363, (ERDM_Parameter)0x8411, (ERDM_Parameter)0x0347, (ERDM_Parameter)0x0348, (ERDM_Parameter)0x0500, (ERDM_Parameter)0x8231, (ERDM_Parameter)0x8232, (ERDM_Parameter)0x8233, (ERDM_Parameter)0x8234, (ERDM_Parameter)0x8224, (ERDM_Parameter)0x82A2, (ERDM_Parameter)0x82A3, (ERDM_Parameter)0x8241, (ERDM_Parameter)0x8281, (ERDM_Parameter)0x8291, (ERDM_Parameter)0x0346, (ERDM_Parameter)0x1200, (ERDM_Parameter)0x1204, (ERDM_Parameter)0x1201, (ERDM_Parameter)0x1202, (ERDM_Parameter)0x1203, (ERDM_Parameter)0x1205, (ERDM_Parameter)0x8213, (ERDM_Parameter)0x8421 }));
                }
            }));
        #endregion

        #region TILT_INVERT GET_COMMAND_RESPONSE
        // GLP X4Bar20 V78.pcapng, frame 260
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.TILT_INVERT,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(allIssues(dataTreeBranch), Is.Empty);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject!;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(obj, Is.EqualTo(false));
                }
            }));
        #endregion
    }
}
