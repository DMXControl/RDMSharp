using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// MANUFACTURER_URL (E1.37-5 4.2): GET only, the URL is a string without length limit,
/// a URL longer than one packet (231 bytes) is sent with ACK_OVERFLOW.
/// </summary>
public class TestManufacturerUrlModule
{
    private ManufacturerUrlModuleMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.MANUFACTURER_URL;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);
    private const string URL = "https://esta.org";

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        Assert.That(defines, Is.Not.Null);
        generated = new ManufacturerUrlModuleMockDevice(DEVCIE_UID, URL);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestDisplayName()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<ManufacturerUrlModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Manufacturer URL"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetMANUFACTURER_URL()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<ManufacturerUrlModule>().Single();
        Assert.That(module.ManufacturerUrl, Is.EqualTo(URL));

        RDMMessage? response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(URL.Length));
            Assert.That(response.Value, Is.EqualTo(URL));
        }
    }

    [Test, DependsOnTest(nameof(TestGetMANUFACTURER_URL))]
    public void TestGetLongMANUFACTURER_URL()
    {
        // E1.37-5: "If the URL is longer than what can fit in a single packet, then an ACK_OVERFLOW shall be used."
        string longUrl = "https://example.com/" + new string('a', 300);
        using var device = new ManufacturerUrlModuleMockDevice(new UID(DEVCIE_UID.ManufacturerID, DEVCIE_UID.DeviceID + 1), longUrl);
        var request = createRequest(ERDM_Command.GET_COMMAND);
        request.DestUID = device.UID;

        List<byte> data = new List<byte>();
        RDMMessage? response = device.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK_OVERFLOW));
        Assert.That(response.ParameterData, Has.Length.EqualTo(231));
        data.AddRange(response.ParameterData);

        response = device.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        data.AddRange(response.ParameterData);

        Assert.That(System.Text.Encoding.UTF8.GetString(data.ToArray()), Is.EqualTo(longUrl));
    }

    [Test, DependsOnTest(nameof(TestGetMANUFACTURER_URL))]
    public void TestSetMANUFACTURER_URL()
    {
        Assert.That(generated, Is.Not.Null);
        var request = createRequest(ERDM_Command.SET_COMMAND);
        request.ParameterData = Tools.ValueToData("https://other.example");

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
            Assert.That(generated.Modules.OfType<ManufacturerUrlModule>().Single().ManufacturerUrl, Is.EqualTo(URL));
        }
    }

    [Test, DependsOnTest(nameof(TestGetMANUFACTURER_URL))]
    public async Task TestRemoteDevice()
    {
        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<ManufacturerUrlModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.ManufacturerUrl, Is.EqualTo(URL)));
        Assert.That(module.ManufacturerUrl, Is.EqualTo(URL));
    }

    [Test, DependsOnTest(nameof(TestGetLongMANUFACTURER_URL))]
    public async Task TestRemoteDeviceLongUrl()
    {
        // the remote device has to put the ACK_OVERFLOW packets together again.
        // The URL is the same for all devices of a model (blueprint parameter), so this device needs its own model (manufacturer).
        string longUrl = "https://example.com/" + new string('b', 300);
        UID uid = new UID((ushort)(DEVCIE_UID.ManufacturerID + 0x4000), DEVCIE_UID.DeviceID);
        using var device = new ManufacturerUrlModuleMockDevice(uid, longUrl);
        using MockDevice mockDevice = new MockDevice(uid);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<ManufacturerUrlModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.ManufacturerUrl, Is.EqualTo(longUrl)));
        Assert.That(module.ManufacturerUrl, Is.EqualTo(longUrl));
    }

    private static RDMMessage createRequest(ERDM_Command command)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = MODULE_PARAMETER,
            SubDevice = SubDevice.Root,
        };
    }

    class ManufacturerUrlModuleMockDevice : MockGeneratedDevice1
    {
        public ManufacturerUrlModuleMockDevice(UID uid, string url) : base(uid, new IModule[] { new ManufacturerUrlModule(url) })
        {
        }
    }
}
