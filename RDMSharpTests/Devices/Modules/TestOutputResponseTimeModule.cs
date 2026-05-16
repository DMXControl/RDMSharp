using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestOutputResponseTimeModule
{
    private OutputResponseTimeModuleMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(875, 5215199);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public async Task Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new OutputResponseTimeModuleMockDevice(DEVCIE_UID);
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
    public void TestGetOUTPUT_RESPONSE_TIME_DESCRIPTION()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var outputResponseTimeModule = generated.Modules.OfType<OutputResponseTimeModule>().Single();
        Assert.That(outputResponseTimeModule, Is.Not.Null);
        Assert.That(outputResponseTimeModule.CurrentId, Is.EqualTo(1));
        Assert.That(outputResponseTimeModule.Count, Is.EqualTo(4));

        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            SubDevice = SubDevice.Root,
            ParameterData = new byte[] { 0x00 } // Requesting invalid 0 
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        for (byte b = 0; b < outputResponseTimeModule.Count; b++)
        {
            byte id = (byte)(b + 1);
            request.ParameterData = new byte[] { id };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            var expected = outputResponseTimeModule._generatedOutputResponseTimes.FirstOrDefault(gen => gen.OutputResponseTimeId == id);
            Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
            Assert.That(response.Value, Is.EqualTo(expected));
            Assert.That(((RDMOutputResponseTimeDescription)response.Value).Index, Is.EqualTo(expected.Index));
            Assert.That(((RDMOutputResponseTimeDescription)response.Value).Description, Is.EqualTo(expected.Description));
        }
        #endregion
    }
    [Test, Order(11)]
    public async Task TestGetOUTPUT_RESPONSE_TIME()
    {
        await Task.Delay(500);
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.OUTPUT_RESPONSE_TIME), Is.True);
        await Task.Delay(1000);
        var outputResponseTimeModule = generated.Modules.OfType<OutputResponseTimeModule>().FirstOrDefault();
        Assert.That(outputResponseTimeModule, Is.Not.Null);
        Assert.That(outputResponseTimeModule.CurrentId, Is.Not.Null);
        Assert.That(outputResponseTimeModule.CurrentId.Value, Is.EqualTo(1));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.OUTPUT_RESPONSE_TIME));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(2));
        Assert.That(response.Value, Is.TypeOf(typeof(RDMOutputResponseTime)));
        RDMOutputResponseTime outputResponseTime = (RDMOutputResponseTime)response.Value;
        Assert.That(outputResponseTime.CurrentResponseTimeId, Is.EqualTo(1));
        Assert.That(outputResponseTime.ResponseTimes, Is.EqualTo(4));

        #endregion

    }
    [Test, Retry(3), Order(101)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<OutputResponseTimeModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.CurrentId, Is.EqualTo(1));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<OutputResponseTimeModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentId, Is.EqualTo(1));
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        await module.SetOutputResponseTime(2);
        await semaphoreSlim.WaitAsync();
        await Task.Delay(1000);

        Assert.That(generatedModule.CurrentId, Is.EqualTo(2));
        Assert.That(module.CurrentId, Is.EqualTo(2));
    }

    class OutputResponseTimeModuleMockDevice : MockGeneratedDevice1
    {
        public OutputResponseTimeModuleMockDevice(UID uid) : base(uid, new IModule[] { new OutputResponseTimeModule(1,
            new RDMOutputResponseTimeDescription(1, "Very Slow"),
            new RDMOutputResponseTimeDescription(2, "Slow"),
            new RDMOutputResponseTimeDescription(3, "Normal"),
            new RDMOutputResponseTimeDescription(4, "Fast")) })
        {
        }
    }
}