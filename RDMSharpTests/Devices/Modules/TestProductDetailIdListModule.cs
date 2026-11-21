using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// PRODUCT_DETAIL_ID_LIST (E1.20 10.5.2): GET only, a packed list of 16-bit Product Detail IDs (Table A-6).
/// The response has a PDL of at least 2, so the list has at least one entry.
/// </summary>
public class TestProductDetailIdListModule
{
    private ProductDetailIdListMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.PRODUCT_DETAIL_ID_LIST;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);
    private static readonly ERDM_ProductDetail[] PRODUCT_DETAILS = { ERDM_ProductDetail.LED, ERDM_ProductDetail.FLASHTUBE };

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
        generated = new ProductDetailIdListMockDevice(DEVCIE_UID, PRODUCT_DETAILS);
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
        var module = generated.Modules.OfType<ProductDetailIdListModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Product Detail ID List"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetPRODUCT_DETAIL_ID_LIST()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<ProductDetailIdListModule>().Single();
        Assert.That(module.ProductDetails, Is.EqualTo(PRODUCT_DETAILS));

        RDMMessage? response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, DEVCIE_UID));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.EqualTo(PRODUCT_DETAILS));
        }
    }

    [Test, DependsOnTest(nameof(TestGetPRODUCT_DETAIL_ID_LIST))]
    public void TestNoProductDetails()
    {
        // at least one entry is required, without details the device declares NONE (PRODUCT_DETAIL_NOT_DECLARED)
        UID uid = new UID((ushort)(DEVCIE_UID.ManufacturerID + 0x4000), DEVCIE_UID.DeviceID);
        using var device = new ProductDetailIdListMockDevice(uid, Array.Empty<ERDM_ProductDetail>());
        var module = device.Modules.OfType<ProductDetailIdListModule>().Single();
        Assert.That(module.ProductDetails, Is.EqualTo(new[] { ERDM_ProductDetail.NONE }));

        RDMMessage? response = device.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND, uid));
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.Value, Is.EqualTo(new[] { ERDM_ProductDetail.NONE }));
    }

    [Test, DependsOnTest(nameof(TestGetPRODUCT_DETAIL_ID_LIST))]
    public void TestSetPRODUCT_DETAIL_ID_LIST()
    {
        Assert.That(generated, Is.Not.Null);
        var request = createRequest(ERDM_Command.SET_COMMAND, DEVCIE_UID);
        // PRODUCT_DETAIL_ID_LIST is GET only, there is no SET payload the parser could build
        request.ParameterData = new byte[] { 0x00, 0x04 };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
        }
    }

    [Test, DependsOnTest(nameof(TestGetPRODUCT_DETAIL_ID_LIST))]
    public async Task TestRemoteDevice()
    {
        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<ProductDetailIdListModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.ProductDetails, Is.EqualTo(PRODUCT_DETAILS)));
        Assert.That(module.ProductDetails, Is.EqualTo(PRODUCT_DETAILS));
    }

    private static RDMMessage createRequest(ERDM_Command command, UID destination)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = destination,
            SourceUID = CONTROLLER_UID,
            Parameter = MODULE_PARAMETER,
            SubDevice = SubDevice.Root,
        };
    }

    class ProductDetailIdListMockDevice : MockGeneratedDevice1
    {
        public ProductDetailIdListMockDevice(UID uid, ERDM_ProductDetail[] productDetails) : base(uid, new IModule[] { new ProductDetailIdListModule(productDetails) })
        {
        }
    }
}
