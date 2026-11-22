using RDMSharp.Extensions;
using RDMSharp.Metadata;
using RDMSharp.RDM.Device;

namespace RDMSharpTests.Extensions;

/// <summary>
/// Name and update time of a supported parameter: for an ESTA parameter the ESTA values count, a manufacturer extension
/// only describes manufacturer specific parameters (0x8000 - 0xFFDF).
/// Seen live: the default of a manufacturer extension (10 min) replaced the 20 s of SENSOR_VALUE, depending on the order of a dictionary.
/// The manufacturer 0x7FFE is in the ESTA prototyping range and used by no other test.
/// </summary>
public class TestSupportedParameterMetadataPriority
{
    private const EManufacturer MANUFACTURER = (EManufacturer)0x7FFE;
    private const ERDM_Parameter MANUFACTURER_PID = (ERDM_Parameter)0x9123;
    private const int MANUFACTURER_DEFAULT = 612345;

    /// <summary>Like the extensions of the console: the default answers for every parameter. Found by the ExtensionsManager in the loaded assemblies.</summary>
    public sealed class GreedyManufacturerExtension : ISupportedParametersExtension
    {
        public string Key => "TestGreedyManufacturerExtension";
        public EManufacturer Manufacturer => MANUFACTURER;
        public ERDM_Parameter[] BlueprintModelParameters => Array.Empty<ERDM_Parameter>();
        public ERDM_Parameter[] BlueprintModelPersonalityParameters => Array.Empty<ERDM_Parameter>();
        public ERDM_Parameter[] ManufacturerInternalParameters => Array.Empty<ERDM_Parameter>();
        public Task RegisterAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler) => Task.CompletedTask;

        public bool TryGetParameterName(ERDM_Parameter parameter, out string name)
        {
            name = "Greedy";
            return true;
        }
        public bool TryGetParameterUpdateTimeMilliseconds(ERDM_Parameter parameter, out int milliseconds)
        {
            milliseconds = MANUFACTURER_DEFAULT;
            return true;
        }
    }

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
        ExtensionsManager.Instance.RegisterSupportedParametersExtension(new GreedyManufacturerExtension());
    }

    private static SupportedParameterMetadata metadata(ERDM_Parameter parameter)
    {
        Assert.That(ExtensionsManager.Instance.TryGetSupportedParameterMetadata(MANUFACTURER, parameter, out var metadata), Is.True);
        return metadata!;
    }

    [Test]
    public void TestEstaParameterKeepsTheEstaUpdateTime()
    {
        var sensorValue = metadata(ERDM_Parameter.SENSOR_VALUE);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sensorValue.ParameterUpdateTimeMilliseconds, Is.EqualTo(20000), "SENSOR_VALUE: ParameterUpdateTime(20000) of the standard");
            Assert.That(sensorValue.Name, Is.Not.EqualTo("Greedy"));
        }
    }

    [Test]
    public void TestEstaParameterWithoutUpdateTimeIsNotTakenFromTheManufacturer()
    {
        // IDENTIFY_DEVICE has no ParameterUpdateTime: the general update time applies (-1), not the manufacturer default
        Assert.That(metadata(ERDM_Parameter.IDENTIFY_DEVICE).ParameterUpdateTimeMilliseconds, Is.EqualTo(-1));
    }

    [Test]
    public void TestManufacturerParameterUsesTheExtension()
    {
        var parameter = metadata(MANUFACTURER_PID);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parameter.ParameterUpdateTimeMilliseconds, Is.EqualTo(MANUFACTURER_DEFAULT));
            Assert.That(parameter.Name, Is.EqualTo("Greedy"));
        }
    }

    [Test]
    public void TestEstaExtensionsComeFirst()
    {
        Assert.That(ExtensionsManager.Instance.TryGetSupportedParametersExtensions(MANUFACTURER, out var extensions), Is.True);
        var manufacturers = extensions.Select(e => e.Manufacturer).ToList();
        int firstManufacturer = manufacturers.FindIndex(m => m != EManufacturer.ESTA);
        Assert.That(firstManufacturer, Is.GreaterThan(0));
        Assert.That(manufacturers.Skip(firstManufacturer), Has.None.EqualTo(EManufacturer.ESTA));
    }
}
