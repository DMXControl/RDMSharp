using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestCurveModule
{
    private CurveModuleMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(871, 5215198);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public async Task Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new CurveModuleMockDevice(DEVCIE_UID);
        while (!generated.IsInitialized)
            await Task.Delay(100);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }
    [Test, Order(10)]
    public void TestGetCURVE_DESCRIPTION()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var curveModule = generated.Modules.OfType<CurveModule>().Single();
        Assert.That(curveModule, Is.Not.Null);
        Assert.That(curveModule.CurrentId, Is.EqualTo(1));
        Assert.That(curveModule.Count, Is.EqualTo(4));

        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CURVE_DESCRIPTION,
            SubDevice = SubDevice.Root,
            ParameterData = new byte[] { 0x00 } // Requesting invalid 0 
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CURVE_DESCRIPTION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        for (byte b = 0; b < curveModule.Count; b++)
        {
            byte id = (byte)(b + 1);
            request.ParameterData = new byte[] { id };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CURVE_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            var expected = curveModule._generatedCurves.FirstOrDefault(gen => gen.CurveId == id);
            Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
            Assert.That(response.Value, Is.EqualTo(expected));
            Assert.That(((RDMCurveDescription)response.Value).Index, Is.EqualTo(expected.Index));
            Assert.That(((RDMCurveDescription)response.Value).Description, Is.EqualTo(expected.Description));
        }
        #endregion
    }
    [Test, Order(11)]
    public async Task TestGetCURVE()
    {
        await Task.Delay(500);
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.CURVE), Is.True);
        await Task.Delay(1000);
        var curveModule = generated.Modules.OfType<CurveModule>().FirstOrDefault();
        Assert.That(curveModule, Is.Not.Null);
        Assert.That(curveModule.CurrentId, Is.Not.Null);
        Assert.That(curveModule.CurrentId.Value, Is.EqualTo(1));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CURVE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CURVE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(2));
        Assert.That(response.Value, Is.TypeOf(typeof(RDMCurve)));
        RDMCurve curve = (RDMCurve)response.Value;
        Assert.That(curve.CurrentCurveId, Is.EqualTo(1));
        Assert.That(curve.Curves, Is.EqualTo(4));

        #endregion

    }
    [Test, Retry(3), Order(101)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<CurveModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.CurrentId, Is.EqualTo(1));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<CurveModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentId, Is.EqualTo(1));
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        await module.SetCurve(2);
        await semaphoreSlim.WaitAsync();
        await Task.Delay(1000);

        Assert.That(generatedModule.CurrentId, Is.EqualTo(2));
        Assert.That(module.CurrentId, Is.EqualTo(2));
    }

    class CurveModuleMockDevice : MockGeneratedDevice1
    {
        public CurveModuleMockDevice(UID uid) : base(uid, new IModule[] { new CurveModule(1,
            new RDMCurveDescription(1, "Linear"),
            new RDMCurveDescription(2, "Logarithmic"),
            new RDMCurveDescription(3, "Exponential"),
            new RDMCurveDescription(4, "S-Curve")) })
        {
        }
    }
}