using RDMSharp.Metadata;
using System.Data;
using System.Reflection;
using System.Text;

namespace RDMSharpTests.Metadata.Parser;

public class TestDefinedDataTreeObjects
{
    [Test]
    public void Test_AllDefinedDataTreeObjectsForValidility()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var type in MetadataFactory.DefinedDataTreeObjects)
        {
            if (type.IsEnum)
                continue;

            StringBuilder stringBuilder2 = new StringBuilder();

            var constructors = type.GetConstructors().Where(c => c.GetCustomAttributes<DataTreeObjectConstructorAttribute>().Count() != 0);
            if (constructors.Count() == 0)
                stringBuilder2.AppendLine($"{type} not defines a {nameof(DataTreeObjectConstructorAttribute)}");

            foreach (var constructor in constructors)
            {
                StringBuilder stringBuilder3 = new StringBuilder();
                var parameters = constructor.GetParameters();
                foreach (var para in parameters.Where(p => !p.GetCustomAttributes<DataTreeObjectParameterAttribute>().Any(a=>a is DataTreeObjectParameterAttribute)))
                    stringBuilder3.AppendLine($"\t{para.Name}");
                if (stringBuilder3.Length > 0)
                {
                    stringBuilder2.AppendLine($"{type} Constructor not defines {nameof(DataTreeObjectParameterAttribute)} for the Parameters:");
                    stringBuilder2.AppendLine(stringBuilder3.ToString().TrimEnd());
                }
            }
            if (stringBuilder2.Length > 0)
                stringBuilder.AppendLine(stringBuilder2.ToString().Trim());
        }
        if (stringBuilder.Length > 0)
            Assert.Fail(stringBuilder.ToString().Trim());
    }

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

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));        
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

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_PERSONALITY);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
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

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
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

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SLOT_DESCRIPTION);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
    }
    [Test]
    public async Task Test_Slot_Info()
    {
        byte[] data = {
            0,0,0,0,1,
            0,1,0,4,4,
            0,2,0,2,5,
            0,3,0,2,6,
            0,4,0,2,7
        };

        var parameterBag = new ParameterBag(ERDM_Parameter.SLOT_INFO);
        var define = MetadataFactory.GetDefine(parameterBag);

        var dataTreeBranch = MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicte.GetResponse, data);

        Assert.Multiple(() =>
        {
            Assert.That(dataTreeBranch.IsUnset, Is.False);
            Assert.That(dataTreeBranch.IsEmpty, Is.False);
            Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
            Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));

            var obj = dataTreeBranch.ParsedObject as RDMSlotInfo[];
            Assert.That(obj[0].SlotOffset, Is.EqualTo(0));
            Assert.That(obj[0].SlotType, Is.EqualTo(ERDM_SlotType.PRIMARY));
            Assert.That(obj[0].SlotLabelId, Is.EqualTo(ERDM_SlotCategory.INTENSITY));

            Assert.That(obj[1].SlotOffset, Is.EqualTo(1));
            Assert.That(obj[1].SlotType, Is.EqualTo(ERDM_SlotType.PRIMARY));
            Assert.That(obj[1].SlotLabelId, Is.EqualTo(ERDM_SlotCategory.STROBE));

            Assert.That(obj[2].SlotOffset, Is.EqualTo(2));
            Assert.That(obj[2].SlotType, Is.EqualTo(ERDM_SlotType.PRIMARY));
            Assert.That(obj[2].SlotLabelId, Is.EqualTo(ERDM_SlotCategory.COLOR_ADD_RED));

            Assert.That(obj[3].SlotOffset, Is.EqualTo(3));
            Assert.That(obj[3].SlotType, Is.EqualTo(ERDM_SlotType.PRIMARY));
            Assert.That(obj[3].SlotLabelId, Is.EqualTo(ERDM_SlotCategory.COLOR_ADD_GREEN));

            Assert.That(obj[4].SlotOffset, Is.EqualTo(4));
            Assert.That(obj[4].SlotType, Is.EqualTo(ERDM_SlotType.PRIMARY));
            Assert.That(obj[4].SlotLabelId, Is.EqualTo(ERDM_SlotCategory.COLOR_ADD_BLUE));
        });

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SLOT_INFO);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
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

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.SET_COMMAND, ERDM_Parameter.DISPLAY_INVERT);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
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

        var reversed = DataTreeBranch.FromObject(dataTreeBranch.ParsedObject, null, ERDM_Command.GET_COMMAND, ERDM_Parameter.STATUS_MESSAGES);
        Assert.That(reversed, Is.EqualTo(dataTreeBranch));
    }
}