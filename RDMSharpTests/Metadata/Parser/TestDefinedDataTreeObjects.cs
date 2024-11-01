using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata.Parser;

public class TestDefinedDataTreeObjects
{
    [Test]
    public async Task Test_DeviceInfo()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x05, 0x06, 0x01, 0x02, 0x00,
            0x01, 0x01, 0x00, 0x01, 0x01, 0x03, 0x00, 0x01,
            0x00, 0x04, 0x00
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.DEVICE_INFO);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.GetResponse, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));

            var obj = dataTreeBranch.ParsedObject as RDMDeviceInfo;
            Assert.That(obj.RdmProtocolVersionMajor, Is.EqualTo(1));
            Assert.That(obj.RdmProtocolVersionMinor, Is.EqualTo(0));
            Assert.That(obj.DeviceModelId, Is.EqualTo(0x0005));
            Assert.That(obj.ProductCategoryCoarse, Is.EqualTo(ERDM_ProductCategoryCoarse.POWER));
            Assert.That(obj.ProductCategoryFine, Is.EqualTo(ERDM_ProductCategoryFine.POWER_CONTROL));
            Assert.That(obj.SoftwareVersionId, Is.EqualTo(0x02000101));
            Assert.That(obj.Dmx512Footprint, Is.EqualTo(1));
            Assert.That(obj.Dmx512CurrentPersonality, Is.EqualTo(1));
            Assert.That(obj.Dmx512NumberOfPersonalities, Is.EqualTo(3));
            Assert.That(obj.Dmx512StartAddress, Is.EqualTo(1));
            Assert.That(obj.SubDeviceCount, Is.EqualTo(4));
            Assert.That(obj.SensorCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Test_Personality()
    {
        byte[] data = {
            0x01, 0x03
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.DMX_PERSONALITY);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.GetResponse, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));

            var obj = dataTreeBranch.ParsedObject as RDMDMXPersonality;
            Assert.That(obj.CurrentPersonality, Is.EqualTo(1));
            Assert.That(obj.OfPersonalities, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Test_Personality_Description()
    {
        byte[] data = {
            0x01, 0x00, 0x01, 0x53, 0x45, 0x51, 0x55, 0x45,
            0x4e, 0x43, 0x45
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.GetResponse, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));

            var obj = dataTreeBranch.ParsedObject as RDMDMXPersonalityDescription;
            Assert.That(obj.PersonalityId, Is.EqualTo(1));
            Assert.That(obj.Slots, Is.EqualTo(1));
            Assert.That(obj.Description, Is.EqualTo("SEQUENCE"));
        });
    }

    [Test]
    public async Task Test_Slot_Description()
    {
        byte[] data = {
            0x00, 0x00, 0x53, 0x41, 0x46, 0x45, 0x54, 0x59
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.SLOT_DESCRIPTION);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.GetResponse, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));

            var obj = dataTreeBranch.ParsedObject as RDMSlotDescription;
            Assert.That(obj.SlotId, Is.EqualTo(0));
            Assert.That(obj.Description, Is.EqualTo("SAFETY"));
        });
    }

    [Test]
    public async Task Test_Display_Invert()
    {
        byte[] data = {
            0x01
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.DISPLAY_INVERT);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.SetRequest, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));

            var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject;
            Assert.That(obj, Is.EqualTo(ERDM_DisplayInvert.ON));
        });
    }

    [Test]
    public async Task Test_Status_Messages()
    {
        byte[] data = {
            0x02
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.STATUS_MESSAGES);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.GetRequest, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));

            var obj = (ERDM_Status)dataTreeBranch.ParsedObject;
            Assert.That(obj, Is.EqualTo(ERDM_Status.ADVISORY));
        });
    }
}