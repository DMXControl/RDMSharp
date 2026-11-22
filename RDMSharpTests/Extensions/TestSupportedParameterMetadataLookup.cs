using RDMSharp.Extensions;
using RDMSharp.Metadata;

namespace RDMSharpTests.Extensions;

/// <summary>
/// <see cref="ExtensionsManager.TryGetSupportedParameterMetadata"/> for a PID in the ESTA range that has no define
/// (e.g. a draft or reserved PID a device lists in SUPPORTED_PARAMETERS): the metadata is created, named after the PID.
/// </summary>
public class TestSupportedParameterMetadataLookup
{
    // 0x7FD4: ESTA range, no define (seen on a real device)
    private const ERDM_Parameter UNKNOWN_ESTA_PID = (ERDM_Parameter)0x7FD4;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [Test]
    public void TestUnknownEstaPidGetsMetadata()
    {
        bool found = ExtensionsManager.Instance.TryGetSupportedParameterMetadata(EManufacturer.ESTA, UNKNOWN_ESTA_PID, out var metadata);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(found, Is.True);
            Assert.That(metadata, Is.Not.Null);
            Assert.That(metadata!.Parameter, Is.EqualTo(UNKNOWN_ESTA_PID));
            Assert.That(metadata.Name, Is.EqualTo(UNKNOWN_ESTA_PID.ToString()));
        }
    }
}
