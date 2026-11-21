using RDMSharp.Metadata;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.Devices;

/// <summary>
/// What the application can set on a generated device with <see cref="AbstractGeneratedRDMDevice.TrySetParameter"/> and what it can not:
/// <list type="bullet">
/// <item>a parameter the device does not support,</item>
/// <item>parameters that have their own property (DMX_START_ADDRESS, DEVICE_LABEL),</item>
/// <item>parameters the protocol does not allow to set (DEVICE_INFO, SENSOR_VALUE, ...), and parameters without a SET,</item>
/// <item>a value that does not fit the define.</item>
/// </list>
/// </summary>
public class TestGeneratedDeviceSetParameter
{
    private static readonly UID CONTROLLER_UID = new UID(0x1fff, 0x00000005);
    private static readonly UID DEVICE_UID = new UID(0x0a5a, 0x00000F04);
    private MockGeneratedDevice1? generated;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }
    [SetUp]
    public void Setup()
    {
        generated = new MockGeneratedDevice1(DEVICE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestParameterTheDeviceDoesNotSupport()
    {
        Assert.That(generated!.Parameters, Does.Not.Contain(ERDM_Parameter.LAMP_HOURS), "precondition");
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<NotSupportedException>(() => generated.TrySetParameter(ERDM_Parameter.LAMP_HOURS, 5u));
            Assert.That(generated.TrySetParameter(ERDM_Parameter.LAMP_HOURS, 5u, throwException: false), Is.False);
        }
    }

    [TestCase(ERDM_Parameter.DMX_START_ADDRESS, Description = "has its own property")]
    [TestCase(ERDM_Parameter.DEVICE_LABEL, Description = "has its own property")]
    [TestCase(ERDM_Parameter.DEVICE_INFO, Description = "the protocol does not allow to set it")]
    [TestCase(ERDM_Parameter.SUPPORTED_PARAMETERS, Description = "the protocol does not allow to set it")]
    [TestCase(ERDM_Parameter.SENSOR_VALUE, Description = "the protocol does not allow to set it")]
    [TestCase(ERDM_Parameter.SENSOR_DEFINITION, Description = "the protocol does not allow to set it")]
    [TestCase(ERDM_Parameter.SLOT_INFO, Description = "the protocol does not allow to set it")]
    public void TestParameterThatCanNotBeSetFromOutside(ERDM_Parameter parameter)
    {
        Assert.That(generated!.Parameters, Does.Contain(parameter), "precondition");
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<NotSupportedException>(() => generated.TrySetParameter(parameter, new object()));
            Assert.That(generated.TrySetParameter(parameter, new object(), throwException: false), Is.False);
        }
    }

    [Test]
    public void TestParameterWithoutASet()
    {
        // SOFTWARE_VERSION_LABEL can only be read by a controller
        Assert.That(generated!.Parameters, Does.Contain(ERDM_Parameter.SOFTWARE_VERSION_LABEL), "precondition");
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<NotSupportedException>(() => generated.TrySetParameter(ERDM_Parameter.SOFTWARE_VERSION_LABEL, "1.0"));
            Assert.That(generated.TrySetParameter(ERDM_Parameter.SOFTWARE_VERSION_LABEL, "1.0", throwException: false), Is.False);
        }
    }

    [Test]
    public void TestValidValueIsSet()
    {
        Assert.That(generated!.TrySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, true), Is.True);
        Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.EqualTo(true));

        var response = generated.ProcessRequestMessage_Internal(new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            Parameter = ERDM_Parameter.IDENTIFY_DEVICE,
            DestUID = DEVICE_UID,
            SourceUID = CONTROLLER_UID,
            SubDevice = SubDevice.Root
        });
        Assert.That(response!.Value, Is.EqualTo(true), "a controller reads what the application set");
    }

    [Test]
    public void TestValueThatDoesNotFitTheDefineIsNotSet()
    {
        var before = generated!.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE];

        // a text is no boolean: no exception, the value stays
        Assert.That(generated.TrySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, "yes"), Is.False);
        Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.EqualTo(before));
    }

    [Test]
    public void TestSetParameterWithoutAValueRemovesIt()
    {
        Assert.That(generated!.GetAllParameterValues(), Does.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE), "precondition");

        Assert.That(generated.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE), Is.True);
        Assert.That(generated.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.IDENTIFY_DEVICE));
    }
}
