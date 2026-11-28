using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using System.Reflection;
using static RDMSharp.Metadata.JSON.Command;

namespace RDMSharpTests.Metadata;

/// <summary>
/// Looking up defines and parsed-object types: what <see cref="MetadataFactory"/> answers for parameters nobody described.
/// Invented data only (manufacturer 0x7FF0 is the ESTA prototyping range).
/// </summary>
public class TestMetadataFactoryLookups
{
    private const ushort PROTOTYPE_MANUFACTURER = 0x7FF0;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [Test]
    public void TestUnknownStandardParameterThrows()
    {
        // below 0x8000 every parameter has to be defined
        Assert.Throws<DefineNotFoundException>(() => MetadataFactory.GetDefine(new ParameterBag((ERDM_Parameter)0x7F00)));
    }

    [Test]
    public void TestParameterAboveTheManufacturerSpecificRangeThrows()
    {
        // 0xFFE0 - 0xFFFF are reserved, a manufacturer can not describe them
        Assert.Throws<DefineNotFoundException>(() => MetadataFactory.GetDefine(new ParameterBag((ERDM_Parameter)0xFFE0, PROTOTYPE_MANUFACTURER)));
    }

    [Test]
    public void TestUndescribedManufacturerParameterIsNull()
    {
        Assert.That(MetadataFactory.GetDefine(new ParameterBag((ERDM_Parameter)0x8F30, PROTOTYPE_MANUFACTURER)), Is.Null, "nobody described it");
    }

    [Test]
    public void TestManufacturerParameterNeedsAManufacturer()
    {
        // a PID of 0x8000 - 0xFFFF only has a meaning together with the manufacturer
        Assert.Throws<ArgumentNullException>(() => new ParameterBag((ERDM_Parameter)0x8F31));
    }

    [Test]
    public void TestDefinedDataTreeObjectTypeForACommand()
    {
        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.DEVICE_INFO));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(MetadataFactory.GetDefinedDataTreeObjectType(define, ERDM_Command.GET_COMMAND_RESPONSE), Is.EqualTo(typeof(RDMDeviceInfo)));
            Assert.That(MetadataFactory.GetDefinedDataTreeObjectType(EManufacturer.ESTA, ERDM_Parameter.DEVICE_INFO, ERDM_Command.GET_COMMAND_RESPONSE), Is.EqualTo(typeof(RDMDeviceInfo)));
            Assert.That(MetadataFactory.GetDefinedDataTreeObjectType(define, ECommandDublicate.GetResponse), Is.EqualTo(typeof(RDMDeviceInfo)));
            Assert.That(MetadataFactory.GetDefinedDataTreeObjectType(EManufacturer.ESTA, ERDM_Parameter.DEVICE_INFO, ECommandDublicate.GetRequest), Is.Null, "a GET has no parsed object");
        }
    }

    [Test]
    public void TestAddingAResourceProviderTwiceIsHarmless()
    {
        var assembly = Assembly.GetExecutingAssembly();
        MetadataFactory.AddResourceProvider(assembly);
        MetadataFactory.AddResourceProvider(assembly); // already added

        // the defines are loaded once, the provider does not change what is there
        using (Assert.EnterMultipleScope())
        {
            Assert.That(MetadataFactory.IsInitialized, Is.True);
            Assert.That(MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.DEVICE_INFO)), Is.Not.Null);
        }
    }
}
