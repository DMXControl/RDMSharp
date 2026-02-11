using RDMSharp.Metadata;
using RDMSharp.PayloadObject;

namespace RDMSharpTests.Metadata.Parser;

public class PayloadToParsedObjectTestSubject
{
    public static readonly object[] TestSubjects = getTestSubjects();

    private static object[] getTestSubjects()
    {
        List<PayloadToParsedObjectTestSubject> instances = new List<PayloadToParsedObjectTestSubject>();

        MetadataFactory.AwaitInitialize().GetAwaiter().GetResult();
        var payloadList = getPayloadList();
        foreach (ERDM_Parameter parameter in Enum.GetValues(typeof(ERDM_Parameter)))
        {
            try
            {
                switch (parameter)
                {
                    case ERDM_Parameter.NONE:
                    case ERDM_Parameter.DISC_UNIQUE_BRANCH:
                    case ERDM_Parameter.DISC_MUTE:
                    case ERDM_Parameter.DISC_UN_MUTE:
                        continue;
                }
                MetadataJSONObjectDefine define = MetadataFactory.GetDefine(new ParameterBag(parameter));
                foreach (PayloadToParseBagData payloadToParseBagData in payloadList.Where(ppbd => ppbd.Parameter == parameter))
                    instances.Add(new PayloadToParsedObjectTestSubject(define, payloadToParseBagData));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing parameter {parameter}: {ex.Message}");
            }
        }
        return instances.ToArray();
    }

    private static IEnumerable<PayloadToParseBagData> getPayloadList()
    {
        List<PayloadToParseBagData> list = new List<PayloadToParseBagData>();

        #region PROXIED_DEVICES
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND_RESPONSE,
           ERDM_Parameter.PROXIED_DEVICES,
           new byte[] { 0x53, 0x47, 0x94, 0x71, 0xaf, 0x2f },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDevices)));

               var obj = dataTreeBranch.ParsedObject as RDMProxiedDevices;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Devices, Is.Not.Null);
               Assert.That(obj.Devices.Length, Is.EqualTo(1));
               Assert.That(obj.Devices[0], Is.EqualTo(new UID(0x5347, 0x9471af2f)));
           }));

        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND_RESPONSE,
           ERDM_Parameter.PROXIED_DEVICES,
           new byte[] {
               0x53, 0x47, 0x94, 0x71, 0xaf, 0x2f,
               0x53, 0x47, 0x94, 0x03, 0x02, 0x01
           },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDevices)));

               var obj = dataTreeBranch.ParsedObject as RDMProxiedDevices;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Devices, Is.Not.Null);
               Assert.That(obj.Devices.Length, Is.EqualTo(2));
               Assert.That(obj.Devices[0], Is.EqualTo(new UID(0x5347, 0x9471af2f)));
               Assert.That(obj.Devices[1], Is.EqualTo(new UID(0x5347, 0x94030201)));
           }));
        #endregion

        #region PROXIED_DEVICES_COUNT
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND_RESPONSE,
           ERDM_Parameter.PROXIED_DEVICES_COUNT,
           new byte[] { 0x00, 0x05, 0x01 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMProxiedDeviceCount)));
               var obj = dataTreeBranch.ParsedObject as RDMProxiedDeviceCount;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.DeviceCount, Is.EqualTo(5));
               Assert.That(obj.ListChange, Is.EqualTo(true));
           }));
        #endregion

        #region COMMS_STATUS
        #endregion

        #region QUEUED_MESSAGE
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND,
           ERDM_Parameter.QUEUED_MESSAGE,
              new byte[] { 0x01 },
              new Action<DataTreeBranch>((dataTreeBranch) =>
                {
                    Assert.That(dataTreeBranch.IsUnset, Is.False);
                    Assert.That(dataTreeBranch.IsEmpty, Is.False);
                    Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                    Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));

                    var obj = (ERDM_Status)dataTreeBranch.ParsedObject;
                    Assert.That(obj, Is.EqualTo(ERDM_Status.GET_LAST_MESSAGE));
                }));
        #endregion

        #region STATUS_MESSAGES
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.STATUS_MESSAGES,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Status)));

                var obj = (ERDM_Status)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_Status.ADVISORY));
            }));

        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND_RESPONSE,
           ERDM_Parameter.STATUS_MESSAGES,
           new RDMStatusMessage(0, ERDM_Status.ERROR, ERDM_StatusMessage.UNDERCURRENT, 2, 20).ToPayloadData(),
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMStatusMessage[])));
               var obj = dataTreeBranch.ParsedObject as RDMStatusMessage[];
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Length, Is.EqualTo(1));
               Assert.That(obj[0].SubDeviceId, Is.EqualTo(0));
               Assert.That(obj[0].EStatusType, Is.EqualTo(ERDM_Status.ERROR));
               Assert.That(obj[0].EStatusMessage, Is.EqualTo(ERDM_StatusMessage.UNDERCURRENT));
               Assert.That(obj[0].DataValue1, Is.EqualTo(2));
               Assert.That(obj[0].DataValue2, Is.EqualTo(20));
           }));

        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND_RESPONSE,
           ERDM_Parameter.STATUS_MESSAGES,
           new byte[0],
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMStatusMessage[])));
           }));
        #endregion

        #region STATUS_ID_DESCRIPTION
        #endregion

        #region CLEAR_STATUS_ID
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CLEAR_STATUS_ID,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND_RESPONSE,
            ERDM_Parameter.CLEAR_STATUS_ID,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        #endregion

        #region SUB_DEVICE_STATUS_REPORT_THRESHOLD
        #endregion

        #region QUEDUED_MESSAGE_SENSOR_SUBSCRIBE
        #endregion

        #region SUPPORTED_PARAMETERS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            new byte[] { 0x00, 0x60, 0x00, 0xE0 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter[])));
                var obj = dataTreeBranch.ParsedObject as ERDM_Parameter[];
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Length, Is.EqualTo(2));
                Assert.That(obj[0], Is.EqualTo(ERDM_Parameter.DEVICE_INFO));
                Assert.That(obj[1], Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY));
            }));
        #endregion

        #region PARAMETER_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            new byte[] {
                0xa0, 0x51, 0x02, 0x05, 0x01, 0x7f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff,
                0x00, 0x00, 0x00, 0x00, 0x54, 0x45, 0x20, 0x4c,
                0x45, 0x44, 0x73, 0x20, 0x68, 0x6f, 0x75, 0x72,
                0x73
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMParameterDescription)));

                var obj = dataTreeBranch.ParsedObject as RDMParameterDescription;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj.ParameterId, Is.EqualTo(0xa051));
                Assert.That(obj.PDLSize, Is.EqualTo(0x02));
                Assert.That(obj.DataType, Is.EqualTo(ERDM_DataType.UINT16));
                Assert.That(obj.CommandClass, Is.EqualTo(ERDM_CommandClass.GET));
                Assert.That(obj.Type, Is.EqualTo(127));
                Assert.That(obj.Unit, Is.EqualTo(ERDM_SensorUnit.NONE));
                Assert.That(obj.Prefix, Is.EqualTo(ERDM_UnitPrefix.NONE));
                Assert.That(obj.MinValidValue, Is.EqualTo(0));
                Assert.That(obj.MaxValidValue, Is.EqualTo(255));
                Assert.That(obj.DefaultValue, Is.EqualTo(0));
                Assert.That(obj.Description, Is.EqualTo("TE LEDs hours"));
            }));
        #endregion

        #region SUPPORTED_PARAMETERS_ENHANCED
        #endregion

        #region CONTROLLER_FLAG_SUPPORT
        #endregion

        #region NACK_DESCRIPTION
        #endregion

        #region PACKED_PID_SUB
        #endregion

        #region PACKED_PID_INDEX
        #endregion

        #region ENUM_LABLE
        #endregion

        #region DEVICE_INFO
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO,
            new byte[] {
                0x01, 0x00, 0x00, 0x05, 0x06, 0x01, 0x02, 0x00,
                0x01, 0x01, 0x00, 0x01, 0x01, 0x03, 0x00, 0x01,
                0x00, 0x04, 0x00
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDeviceInfo)));

                var obj = dataTreeBranch.ParsedObject as RDMDeviceInfo;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.RdmProtocolVersionMajor, Is.EqualTo(1));
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
            }));
        #endregion

        #region PRODUCT_DETAIL_ID_LIST
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            new byte[] { 0x00, 0x01, 0x00, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ProductDetail[])));

                var obj = (ERDM_ProductDetail[])dataTreeBranch.ParsedObject;
                Assert.That(obj.Length, Is.EqualTo(2));
                Assert.That(obj[0], Is.EqualTo(ERDM_ProductDetail.ARC));
                Assert.That(obj[1], Is.EqualTo(ERDM_ProductDetail.METAL_HALIDE));
            }));
        #endregion

        #region DEVICE_MODEL_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            new byte[] { 0x53, 0x45, 0x51, 0x55, 0x45, 0x4e, 0x43, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("SEQUENCE"));
            }));
        #endregion

        #region MANUFACTURER_LABEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_LABEL,
            new byte[] { 0x53, 0x45, 0x51, 0x55, 0x45, 0x4e, 0x43, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("SEQUENCE"));
            }));
        #endregion

        #region DEVICE_LABEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x53, 0x45, 0x51, 0x55, 0x45, 0x4e, 0x43, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("SEQUENCE"));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_LABEL,
            new byte[] { 0x53, 0x45, 0x51, 0x55, 0x45, 0x4e, 0x43, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("SEQUENCE"));
            }));
        #endregion

        #region FACTORY_DEFAULTS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.FACTORY_DEFAULTS,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));

                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(false));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.FACTORY_DEFAULTS,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        #endregion

        #region LANGUAGE_CAPABILITIES
        #endregion

        #region LANGUAGE
        #endregion

        #region SOFTWARE_VERSION_LABEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            new byte[] { 0x53, 0x45, 0x51, 0x55, 0x45, 0x4e, 0x43, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("SEQUENCE"));
            }));
        #endregion

        #region BOOT_SOFTWARE_VERSION_ID
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            new byte[] { 0x01, 0x02, 0x03, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));

                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(0x01020304));
            }));
        #endregion

        #region BOOT_SOFTWARE_VERSION_LABEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            new byte[] { 0x53, 0x45, 0x51, 0x55, 0x45, 0x4e, 0x43, 0x45 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("SEQUENCE"));
            }));
        #endregion

        #region DMX_PERSONALITY
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY,
            new byte[] { 0x01, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonality)));

                var obj = dataTreeBranch.ParsedObject as RDMDMXPersonality;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.CurrentPersonality, Is.EqualTo(1));
                Assert.That(obj.OfPersonalities, Is.EqualTo(3));
            }));
        #endregion

        #region DMX_PERSONALITY_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            new byte[] {
                0x01, 0x00, 0x01, 0x53, 0x45, 0x51, 0x55, 0x45,
                0x4e, 0x43, 0x45
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMXPersonalityDescription)));

                var obj = dataTreeBranch.ParsedObject as RDMDMXPersonalityDescription;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.PersonalityId, Is.EqualTo(1));
                Assert.That(obj.Slots, Is.EqualTo(1));
                Assert.That(obj.Description, Is.EqualTo("SEQUENCE"));
            }));
        #endregion

        #region DMX_START_ADDRESS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DMX_START_ADDRESS,
            new byte[] { 0x00, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(3));
            }));
        #endregion

        #region SLOT_INFO
        yield return new PayloadToParseBagData(
        ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_INFO,
            new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x01,
                0x00, 0x01, 0x00, 0x04, 0x04,
                0x00, 0x02, 0x00, 0x02, 0x05,
                0x00, 0x03, 0x00, 0x02, 0x06,
                0x00, 0x04, 0x00, 0x02, 0x07
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));

                var obj = dataTreeBranch.ParsedObject as RDMSlotInfo[];
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj![0].SlotOffset, Is.EqualTo(0));
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
            }));
        #endregion

        #region SLOT_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SLOT_DESCRIPTION,
            new byte[] { 0x00, 0x00, 0x53, 0x41, 0x46, 0x45, 0x54, 0x59 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSlotDescription)));

                var obj = dataTreeBranch.ParsedObject as RDMSlotDescription;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.SlotId, Is.EqualTo(0));
                Assert.That(obj.Description, Is.EqualTo("SAFETY"));
            }));
        #endregion

        #region DEFAULT_SLOT_VALUE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            new byte[] {
                0x00, 0x00, 0x80, 0x00, 0x01, 0x00, 0x00, 0x02,
                0x80, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00,
                0x05, 0x00, 0x00, 0x06, 0x0a, 0x00, 0x07, 0x80,
                0x00, 0x08, 0x00, 0x00, 0x09, 0x00, 0x00, 0x0a,
                0x00, 0x00, 0x0b, 0x00, 0x00, 0x0c, 0x00, 0x00,
                0x0d, 0x00, 0x00, 0x0e, 0x00, 0x00, 0x0f, 0x00,
                0x00, 0x10, 0x00, 0x00, 0x11, 0x80, 0x00, 0x12,
                0x00, 0x00, 0x13, 0x00, 0x00, 0x14, 0x00, 0x00,
                0x15, 0x00, 0x00, 0x16, 0x00, 0x00, 0x17, 0x80,
                0x00, 0x18, 0x00, 0x00, 0x19, 0x00, 0x00, 0x1a,
                0x80, 0x00, 0x1b, 0x00, 0x00, 0x1c, 0x00, 0x00,
                0x1d, 0x80, 0x00, 0x1e, 0x00, 0x00, 0x1f, 0x00,
                0x00, 0x20, 0x80, 0x00, 0x21, 0x00, 0x00, 0x22,
                0x80, 0x00, 0x23, 0x00, 0x00, 0x24, 0x00, 0x00,
                0x25, 0x00, 0x00, 0x26, 0x80, 0x00, 0x27, 0x00,
                0x00, 0x28, 0x80, 0x00, 0x29, 0x00, 0x00, 0x2a,
                0x80, 0x00, 0x2b, 0x00, 0x00, 0x2c, 0x80, 0x00,
                0x2d, 0x00, 0x00, 0x2e, 0x80, 0x00, 0x2f, 0x00,
                0x00, 0x30, 0x80, 0x00, 0x31, 0x00, 0x00, 0x32,
                0x80, 0x00, 0x33, 0x20, 0x00, 0x34, 0x00, 0x00,
                0x35, 0x00
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDefaultSlotValue[])));
                var obj = dataTreeBranch.ParsedObject as RDMDefaultSlotValue[];
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Length, Is.EqualTo(54));
                Assert.That(obj[0].SlotOffset, Is.EqualTo(0));
                Assert.That(obj[0].DefaultSlotValue, Is.EqualTo(128));
                Assert.That(obj[1].SlotOffset, Is.EqualTo(1));
                Assert.That(obj[1].DefaultSlotValue, Is.EqualTo(0));
                Assert.That(obj[2].SlotOffset, Is.EqualTo(2));
                Assert.That(obj[2].DefaultSlotValue, Is.EqualTo(128));
                Assert.That(obj[3].SlotOffset, Is.EqualTo(3));
                Assert.That(obj[3].DefaultSlotValue, Is.EqualTo(0));
                Assert.That(obj[4].SlotOffset, Is.EqualTo(4));
                Assert.That(obj[4].DefaultSlotValue, Is.EqualTo(0));
            }));
        #endregion

        #region SENDOR_DEFINITION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_DEFINITION,
            new byte[] {
                0x01, 0x00, 0x01, 0x00, 0xff, 0xe0, 0x00, 0xde,
                0x00, 0x00, 0x00, 0x64, 0x02, 0x44, 0x72, 0x69,
                0x76, 0x65, 0x72, 0x20, 0x54, 0x65, 0x6d, 0x70,
                0x65, 0x72, 0x61, 0x74, 0x75, 0x72, 0x65
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorDefinition)));

                var obj = (RDMSensorDefinition)dataTreeBranch.ParsedObject;
                Assert.That(obj!.SensorId, Is.EqualTo(1));
                Assert.That(obj.Index, Is.EqualTo(1));
                Assert.That(obj.MinIndex, Is.EqualTo(0));
                Assert.That(obj.Type, Is.EqualTo(ERDM_SensorType.TEMPERATURE));
                Assert.That(obj.Unit, Is.EqualTo(ERDM_SensorUnit.CENTIGRADE));
                Assert.That(obj.Prefix, Is.EqualTo(ERDM_UnitPrefix.NONE));
                Assert.That(obj.RangeMinimum, Is.EqualTo(-32));
                Assert.That(obj.RangeMaximum, Is.EqualTo(222));
                Assert.That(obj.NormalMinimum, Is.EqualTo(0));
                Assert.That(obj.NormalMaximum, Is.EqualTo(100));
                Assert.That(obj.RecordedValueSupported, Is.EqualTo(false));
                Assert.That(obj.LowestHighestValueSupported, Is.EqualTo(true));
                Assert.That(obj.Description, Is.EqualTo("Driver Temperature"));
            }));
        #endregion

        #region SENSOR_VALUE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_VALUE,
            new byte[] {
                0x00, 0x00, 0x1f, 0x00, 0x00, 0x00, 0x39, 0x00,
                0x00
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorValue)));

                var obj = (RDMSensorValue)dataTreeBranch.ParsedObject;
                Assert.That(obj!.SensorId, Is.EqualTo(0));
                Assert.That(obj.Index, Is.EqualTo(0));
                Assert.That(obj.MinIndex, Is.EqualTo(0));
                Assert.That(obj.PresentValue, Is.EqualTo(31));
                Assert.That(obj.LowestValue, Is.EqualTo(0));
                Assert.That(obj.HighestValue, Is.EqualTo(57));
                Assert.That(obj.RecordedValue, Is.EqualTo(0));
            }));
        #endregion

        #region RECORD_SENSORS
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.RECORD_SENSORS,
            new byte[] { 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));
        #endregion

        #region DEVICE_HOURS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));
        #endregion

        #region LAMP_HOURS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.LAMP_HOURS,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));
        #endregion

        #region LAMP_STRIKES
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.LAMP_STRIKES,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));
        #endregion

        #region LAMP_STATE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_STATE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampState)));
                var obj = (ERDM_LampState)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_LampState.ON));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.LAMP_STATE,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampState)));
                var obj = (ERDM_LampState)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_LampState.STRIKE));
            }));
        #endregion

        #region LAMP_ON_MODE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LAMP_ON_MODE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampMode)));

                var obj = (ERDM_LampMode)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_LampMode.ON_MODE_DMX));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.LAMP_ON_MODE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_LampMode)));
                var obj = (ERDM_LampMode)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_LampMode.ON_MODE_OFF));
            }));
        #endregion

        #region DEVICE_POWER_CYCLES
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            new byte[] { 0x00, 0x00, 0x00, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));
                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(5));
            }));
        #endregion

        #region DISPLAY_INVERT
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_INVERT,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_DisplayInvert)));

                var obj = (ERDM_DisplayInvert)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_DisplayInvert.ON));
            }));
        #endregion

        #region DISPLAY_LEVEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISPLAY_LEVEL,
            new byte[] { 0x7f },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(0x7f));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DISPLAY_LEVEL,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(0x00));
            }));
        #endregion

        #region PAN_INVERT
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PAN_INVERT,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(true));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.PAN_INVERT,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(false));
            }));
        #endregion

        #region TILT_INVERT
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.TILT_INVERT,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(true));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.TILT_INVERT,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(false));
            }));
        #endregion

        #region PAN_TILT_SWAP
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PAN_TILT_SWAP,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(true));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.PAN_TILT_SWAP,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));
                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(false));
            }));
        #endregion

        #region REAL_TIME_CLOCK
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x01, 0x0e, 0x0f, 0x2f, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));

                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject;
                Assert.That(obj!.Year, Is.EqualTo(2026));
                Assert.That(obj.Month, Is.EqualTo(1));
                Assert.That(obj.Day, Is.EqualTo(14));
                Assert.That(obj.Hour, Is.EqualTo(15));
                Assert.That(obj.Minute, Is.EqualTo(47));
                Assert.That(obj.Second, Is.EqualTo(7));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[] { 0x07, 0xea, 0x01, 0x0e, 0x0f, 0x2f, 0x07 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMRealTimeClock)));

                var obj = (RDMRealTimeClock)dataTreeBranch.ParsedObject;
                Assert.That(obj!.Year, Is.EqualTo(2026));
                Assert.That(obj.Month, Is.EqualTo(1));
                Assert.That(obj.Day, Is.EqualTo(14));
                Assert.That(obj.Hour, Is.EqualTo(15));
                Assert.That(obj.Minute, Is.EqualTo(47));
                Assert.That(obj.Second, Is.EqualTo(7));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND_RESPONSE,
            ERDM_Parameter.REAL_TIME_CLOCK,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        #endregion

        #region IDENTIFY_DEVICE
        #endregion

        #region RESET_DEVICE
        #endregion

        #region POWER_STATE
        #endregion

        #region PERFORM_SELFTEST
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PERFORM_SELFTEST,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));

                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.True);
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.PERFORM_SELFTEST,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(3));
            }));
        #endregion

        #region SELF_TEST_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(3));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            new byte[] { 0x03, 0x53, 0x45, 0x4c, 0x46 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSelfTestDescription)));

                var obj = (RDMSelfTestDescription)dataTreeBranch.ParsedObject;
                Assert.That(obj.SelfTestRequester, Is.EqualTo(3));
                Assert.That(obj.Description, Is.EqualTo("SELF"));
            }));
        #endregion

        #region SELFTEST_ENHANCED
        #endregion

        #region CAPTURE_PRESET
        #endregion

        #region PRESET_PLAYBACK
        #endregion

        #region DMX_BLOCK_ADDRESS
        #endregion

        #region DMX_FAIL_MODE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_FAIL_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));

                var obj = dataTreeBranch.ParsedObject as RDMDMX_xxxx_Mode;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Scene, Is.EqualTo(0));
                Assert.That(obj.Delay, Is.EqualTo(0.1).Within(1e-9).Percent);
                Assert.That(obj.HoldTime, Is.EqualTo(6553.5).Within(1e-9).Percent);
                Assert.That(obj.Level, Is.EqualTo(byte.MaxValue));
            }));
        #endregion

        #region DMX_STARTUP_MODE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            new byte[] { 0x00, 0x00, 0x00, 0x01, 0xff, 0xff, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDMX_xxxx_Mode)));

                var obj = dataTreeBranch.ParsedObject as RDMDMX_xxxx_Mode;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Scene, Is.EqualTo(0));
                Assert.That(obj.Delay, Is.EqualTo(0.1).Within(1e-9).Percent);
                Assert.That(obj.HoldTime, Is.EqualTo(6553.5).Within(1e-9).Percent);
                Assert.That(obj.Level, Is.EqualTo(byte.MaxValue));
            }));
        #endregion

        #region DIMMER_INFO
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DIMMER_INFO,
            new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x03, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMDimmerInfo)));
                var obj = (RDMDimmerInfo)dataTreeBranch.ParsedObject;
                Assert.That(obj.MinimumLevelLowerLimit, Is.EqualTo(0xffff));
                Assert.That(obj.MinimumLevelUpperLimit, Is.EqualTo(0xffff));
                Assert.That(obj.MaximumLevelLowerLimit, Is.EqualTo(0xffff));
                Assert.That(obj.MaximumLevelUpperLimit, Is.EqualTo(0xffff));
                Assert.That(obj.NumberOfSupportedCurves, Is.EqualTo(3));
                Assert.That(obj.LevelsResolution, Is.EqualTo(1));
                Assert.That(obj.MinimumLevelSplitLevelsSupported, Is.EqualTo(true));
            }));
        #endregion

        #region MINIMUM_LEVEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MINIMUM_LEVEL,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MINIMUM_LEVEL,
            new byte[] { 0x00, 0x01, 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMMinimumLevel)));

                var obj = (RDMMinimumLevel)dataTreeBranch.ParsedObject;
                Assert.That(obj.MinimumLevelIncrease, Is.EqualTo(1));
                Assert.That(obj.MinimumLevelDecrease, Is.EqualTo(1));
                Assert.That(obj.OnBelowMinimum, Is.EqualTo(true));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.MINIMUM_LEVEL,
            new byte[] { 0x00, 0x02, 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMMinimumLevel)));

                var obj = (RDMMinimumLevel)dataTreeBranch.ParsedObject;
                Assert.That(obj.MinimumLevelIncrease, Is.EqualTo(2));
                Assert.That(obj.MinimumLevelDecrease, Is.EqualTo(1));
                Assert.That(obj.OnBelowMinimum, Is.EqualTo(false));
            }));
        #endregion

        #region MAXIMUM_LEVEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MAXIMUM_LEVEL,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MAXIMUM_LEVEL,
            new byte[] { 0x00, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(255));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.MAXIMUM_LEVEL,
            new byte[] { 0x00, 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));
                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(255));
            }));
        #endregion

        #region CURVE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE,
            new byte[] { 0x01, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurve)));

                var obj = (RDMCurve)dataTreeBranch.ParsedObject;
                Assert.That(obj.CurrentCurveId, Is.EqualTo(1));
                Assert.That(obj.Index, Is.EqualTo(1));
                Assert.That(obj.Curves, Is.EqualTo(4));
                Assert.That(obj.Count, Is.EqualTo(4));
                Assert.That(obj.MinIndex, Is.EqualTo(1));
                Assert.That(obj.IndexType, Is.EqualTo(typeof(byte)));
                Assert.That(obj.DescriptorParameter, Is.EqualTo(ERDM_Parameter.CURVE_DESCRIPTION));
            }));
        #endregion

        #region CURVE_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            new byte[] { 0x01, 0x4C, 0x69, 0x6E, 0x65, 0x61, 0x72 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMCurveDescription)));

                var obj = (RDMCurveDescription)dataTreeBranch.ParsedObject;
                Assert.That(obj.CurveId, Is.EqualTo(1));
                Assert.That(obj.Description, Is.EqualTo("Linear"));
            }));
        #endregion

        #region OUTPUT_RESPONSE_TIME
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            new byte[] { 0x01, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTime)));

                var obj = (RDMOutputResponseTime)dataTreeBranch.ParsedObject;
                Assert.That(obj!.CurrentResponseTimeId, Is.EqualTo(1));
                Assert.That(obj.Index, Is.EqualTo(1));
                Assert.That(obj.ResponseTimes, Is.EqualTo(5));
                Assert.That(obj.Count, Is.EqualTo(5));
                Assert.That(obj.MinIndex, Is.EqualTo(1));
                Assert.That(obj.IndexType, Is.EqualTo(typeof(byte)));
                Assert.That(obj.DescriptorParameter, Is.EqualTo(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION));
            }));
        #endregion

        #region OUTPUT_RESPONSE_TIME_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            new byte[] { 0x01, 0x46, 0x61, 0x73, 0x74 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMOutputResponseTimeDescription)));

                var obj = (RDMOutputResponseTimeDescription)dataTreeBranch.ParsedObject;
                Assert.That(obj.OutputResponseTimeId, Is.EqualTo(1));
                Assert.That(obj.Description, Is.EqualTo("Fast"));
            }));
        #endregion

        #region MODULATION_FREQUENCY
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY,
            new byte[] { 0x01, 0x05 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequency)));

                var obj = (RDMModulationFrequency)dataTreeBranch.ParsedObject;
                Assert.That(obj.ModulationFrequencyId, Is.EqualTo(1));
                Assert.That(obj.Index, Is.EqualTo(1));
                Assert.That(obj.ModulationFrequencys, Is.EqualTo(5));
                Assert.That(obj.Count, Is.EqualTo(5));
                Assert.That(obj.MinIndex, Is.EqualTo(1));
                Assert.That(obj.IndexType, Is.EqualTo(typeof(byte)));
                Assert.That(obj.DescriptorParameter, Is.EqualTo(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION));
            }));
        #endregion

        #region MODULATION_FREQUENCY_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            new byte[] { 0x01, 0x00, 0x00, 0x00, 0xFF, 0x46, 0x61, 0x73, 0x74 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));

                var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject;
                Assert.That(obj.ModulationFrequencyId, Is.EqualTo(1));
                Assert.That(obj.Frequency, Is.EqualTo(255));
                Assert.That(obj.Description, Is.EqualTo("Fast"));
            }));

        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND_RESPONSE,
           ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
           new byte[] { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x46, 0x61, 0x73, 0x74 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMModulationFrequencyDescription)));

               var obj = (RDMModulationFrequencyDescription)dataTreeBranch.ParsedObject;
               Assert.That(obj.ModulationFrequencyId, Is.EqualTo(1));
               Assert.That(obj.Frequency, Is.Null);
               Assert.That(obj.Description, Is.EqualTo("Fast"));
           }));
        #endregion

        #region BURN_IN
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BURN_IN,
            new byte[] { 0x09 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(9));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.BURN_IN,
            new byte[] { 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));
                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(255));
            }));
        #endregion

        #region LOCK_PIN
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_PIN,
            new byte[] { 0x01, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(258));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.LOCK_PIN,
            new byte[] { 0x01, 0x02, 0x03, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(SetLockPinRequest)));

                var obj = (SetLockPinRequest)dataTreeBranch.ParsedObject;
                Assert.That(obj.NewPinCode, Is.EqualTo(0258));
                Assert.That(obj.CurrentPinCode, Is.EqualTo(0772));
            }));
        #endregion

        #region LOCK_STATE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE,
            new byte[] { 0x01, 0x08 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetLockStateResponse)));

                var obj = (GetLockStateResponse)dataTreeBranch.ParsedObject;
                Assert.That(obj.CurrentLockStateId, Is.EqualTo(1));
                Assert.That(obj.Index, Is.EqualTo(1));
                Assert.That(obj.LockStates, Is.EqualTo(8));
                Assert.That(obj.Count, Is.EqualTo(8));
                Assert.That(obj.MinIndex, Is.EqualTo(0));
                Assert.That(obj.IndexType, Is.EqualTo(typeof(byte)));
                Assert.That(obj.DescriptorParameter, Is.EqualTo(ERDM_Parameter.LOCK_STATE_DESCRIPTION));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.LOCK_STATE,
            new byte[] { 0x01, 0x02, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(SetLockStateRequest)));

                var obj = (SetLockStateRequest)dataTreeBranch.ParsedObject;
                Assert.That(obj.LockStateId, Is.EqualTo(3));
                Assert.That(obj.PinCode, Is.EqualTo(0258));
            }));
        #endregion

        #region LOCK_STATE_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            new byte[] { 0x01, 0x4C, 0x6F, 0x63, 0x6B, 0x65, 0x64 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMLockStateDescription)));

                var obj = (RDMLockStateDescription)dataTreeBranch.ParsedObject;
                Assert.That(obj.LockStateId, Is.EqualTo(1));
                Assert.That(obj.Description, Is.EqualTo("Locked"));
            }));
        #endregion

        #region IDENTIFY_MODE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_MODE,
            new byte[] { 0xff },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_IdentifyMode)));

                var obj = (ERDM_IdentifyMode)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_IdentifyMode.LOUD));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_MODE,
            new byte[] { 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_IdentifyMode)));

                var obj = (ERDM_IdentifyMode)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_IdentifyMode.QUIET));
            }));
        #endregion

        #region PRESET_INFO
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_INFO,
            new byte[] {
                0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x02,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
                0x00, 0x00, 0xff, 0xfe, 0x00, 0x01, 0xff, 0xfe,
                0x00, 0x01, 0xff, 0xfe, 0x00, 0x01, 0xff, 0xfe
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMPresetInfo)));

                var obj = dataTreeBranch.ParsedObject as RDMPresetInfo;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.LevelFieldSupported, Is.EqualTo(true));
                Assert.That(obj!.PresetSequenceSupported, Is.EqualTo(true));
                Assert.That(obj!.SplitTimesSupported, Is.EqualTo(true));
                Assert.That(obj!.DMX512FailInfiniteDelayTimeSupported, Is.EqualTo(true));
                Assert.That(obj!.DMX512FailInfiniteHoldTimeSupported, Is.EqualTo(true));
                Assert.That(obj!.StartupInfiniteHoldTimeSupported, Is.EqualTo(true));
                Assert.That(obj!.MaximumSceneNumber, Is.EqualTo(2));
                Assert.That(obj!.MinimumPresetFadeTimeSupported, Is.EqualTo(6553.5).Within(1e-9).Percent);
                Assert.That(obj!.MaximumPresetFadeTimeSupported, Is.EqualTo(6553.5).Within(1e-9).Percent);
                Assert.That(obj!.MinimumPresetWaitTimeSupported, Is.EqualTo(6553.5).Within(1e-9).Percent);
                Assert.That(obj!.MaximumPresetWaitTimeSupported, Is.EqualTo(6553.5).Within(1e-9).Percent);
                Assert.That(obj!.MinimumDMX512FailDelayTimeSupported, Is.EqualTo(0).Within(1e-9).Percent);
                Assert.That(obj!.MaximumDMX512FailDelayTimeSupported, Is.EqualTo(6553.4).Within(1e-9).Percent);
                Assert.That(obj!.MinimumDMX512FailDelayHoldSupported, Is.EqualTo(0.1).Within(1e-9).Percent);
                Assert.That(obj!.MaximumDMX512FailDelayHoldSupported, Is.EqualTo(6553.4).Within(1e-9).Percent);
                Assert.That(obj!.MinimumStartupDelayTimeSupported, Is.EqualTo(0.1).Within(1e-9).Percent);
                Assert.That(obj!.MaximumStartupDelayTimeSupported, Is.EqualTo(6553.4).Within(1e-9).Percent);
                Assert.That(obj!.MinimumStartupDelayHoldSupported, Is.EqualTo(0.1).Within(1e-9).Percent);
                Assert.That(obj!.MaximumStartupDelayHoldSupported, Is.EqualTo(6553.4).Within(1e-9).Percent);
            }));
        #endregion

        #region PRESET_STATUS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_STATUS,
            new byte[] {
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x02
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMPresetStatus)));

                var obj = dataTreeBranch.ParsedObject as RDMPresetStatus;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.SceneId, Is.EqualTo(1));
                Assert.That(obj!.UpFadeTime, Is.EqualTo(0));
                Assert.That(obj!.DownFadeTime, Is.EqualTo(0));
                Assert.That(obj!.WaitTime, Is.EqualTo(0));
                Assert.That(obj!.EProgrammed, Is.EqualTo(ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY));
            }));
        #endregion

        #region PRESET_MERGEMODE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_MERGEMODE,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_MergeMode)));
                var obj = (ERDM_MergeMode)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_MergeMode.HTP));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.PRESET_MERGEMODE,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_MergeMode)));
                var obj = (ERDM_MergeMode)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_MergeMode.LTP));
            }));
        #endregion

        #region POWER_ON_SELF_TEST
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.POWER_ON_SELF_TEST,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.POWER_ON_SELF_TEST,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = (byte)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));
        #endregion

        #region LIST_INTERFACES
        #endregion

        #region INTERFACE_LABEL
        #endregion

        #region INTERFACE_HARDWARE_ADDRESS_TYPE
        #endregion

        #region IPV4_DHCP_MODE
        #endregion

        #region IPV4_ZEROCONF_MODE
        #endregion

        #region IPV4_CURRENT_ADDRESS
        #endregion

        #region IPV4_STATIC_ADDRESS
        #endregion

        #region INTERFACE_RENEW_DHCP
        #endregion

        #region INTERFACE_RELEASE_DHCP
        #endregion

        #region INTERFACE_APPLY_CONFIGURATION
        #endregion

        #region IPV4_DEFAULT_ROUTE
        #endregion

        #region DNS_IPV4_NAME_SERVER
        #endregion

        #region DNS_HOSTNAME
        #endregion

        #region DNS_DOMAIN_NAME
        #endregion

        #region COMPONENT_SCOPE
        #endregion

        #region SEARCH_DOMAIN
        #endregion

        #region TCP_COMMS_STATUS
        #endregion

        #region BROKER_STATUS
        #endregion

        #region ENDPOINT_LIST
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LIST,
            new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00,
                0x03, 0x01, 0x00, 0x04, 0x01, 0x00, 0x05, 0x01,
                0x00, 0x06, 0x01, 0x00, 0x07, 0x01, 0x00, 0x08,
                0x01
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointListResponse)));

                var obj = dataTreeBranch.ParsedObject as GetEndpointListResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Endpoints.Length, Is.EqualTo(7));
                Assert.That(obj.Endpoints, Is.Not.Null);
            }));
        #endregion

        #region ENDPOINT_LIST_CHANGE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LIST_CHANGE,
            new byte[] { 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));

                var obj = dataTreeBranch.ParsedObject as uint?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));
        #endregion

        #region IDENTIFY_ENDPOINT
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));

                var obj = dataTreeBranch.ParsedObject as GetSetIdentifyEndpoint;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.IdentifyState, Is.EqualTo(false));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));

                var obj = dataTreeBranch.ParsedObject as GetSetIdentifyEndpoint;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.IdentifyState, Is.EqualTo(true));
            }));
        #endregion

        #region ENDPOINT_TO_UNIVERSE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x01, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointToUniverse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.Universe, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            new byte[] { 0x00, 0x01, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointToUniverse)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointToUniverse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.Universe, Is.EqualTo(1));
            }));
        #endregion

        #region ENDPOINT_MODE
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND,
           ERDM_Parameter.ENDPOINT_MODE,
           new byte[] { 0x00, 0x01 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

               var obj = dataTreeBranch.ParsedObject as ushort?;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Value, Is.EqualTo(1));
           }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointMode;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.EndpointMode, Is.EqualTo(ERDM_EndpointMode.INPUT));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.ENDPOINT_MODE,
            new byte[] { 0x00, 0x01, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointMode)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointMode;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.EndpointMode, Is.EqualTo(ERDM_EndpointMode.OUTPUT));
            }));
        #endregion

        #region ENDPOINT_LABEL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x08, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x36 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointLabel;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(8));
                Assert.That(obj!.EndpointLabel, Is.EqualTo("Port 6"));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x08, 0x50, 0x6f, 0x72, 0x74, 0x20, 0x36 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointLabel)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointLabel;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(8));
                Assert.That(obj!.EndpointLabel, Is.EqualTo("Port 6"));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_LABEL,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));
        #endregion

        #region RDM_TRAFFIC_ENABLE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointRDMTrafficEnable;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.RDMTrafficEnabled, Is.EqualTo(false));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointRDMTrafficEnable;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.RDMTrafficEnabled, Is.EqualTo(true));
            }));
        #endregion

        #region DISCOVERY_STATE
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND,
           ERDM_Parameter.DISCOVERY_STATE,
           new byte[] { 0x00, 0x01 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

               var obj = dataTreeBranch.ParsedObject as ushort?;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Value, Is.EqualTo(1));
           }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x01, 0x00, 0x02, 0x04 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDiscoveryStateResponse)));

                var obj = dataTreeBranch.ParsedObject as GetDiscoveryStateResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.DeviceCount, Is.EqualTo(2));
                Assert.That(obj!.DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.NOT_ACTIVE));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DISCOVERY_STATE,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(SetDiscoveryStateRequest)));

                var obj = dataTreeBranch.ParsedObject as SetDiscoveryStateRequest;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.INCREMENTAL));
            }));

        yield return new PayloadToParseBagData(
           ERDM_Command.SET_COMMAND_RESPONSE,
           ERDM_Parameter.DISCOVERY_STATE,
           new byte[] { 0x00, 0x01 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

               var obj = dataTreeBranch.ParsedObject as ushort?;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Value, Is.EqualTo(1));
           }));
        #endregion

        #region BACKGROUND_DISCOVERY
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND,
           ERDM_Parameter.BACKGROUND_DISCOVERY,
           new byte[] { 0x00, 0x01 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

               var obj = dataTreeBranch.ParsedObject as ushort?;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Value, Is.EqualTo(1));
           }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BACKGROUND_DISCOVERY,
            new byte[] { 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointBackgroundDiscovery)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointBackgroundDiscovery;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.BackgroundDiscovery, Is.EqualTo(false));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.BACKGROUND_DISCOVERY,
            new byte[] { 0x00, 0x01, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetSetEndpointBackgroundDiscovery)));

                var obj = dataTreeBranch.ParsedObject as GetSetEndpointBackgroundDiscovery;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.BackgroundDiscovery, Is.EqualTo(true));
            }));

        yield return new PayloadToParseBagData(
           ERDM_Command.SET_COMMAND_RESPONSE,
           ERDM_Parameter.BACKGROUND_DISCOVERY,
           new byte[] { 0x00, 0x01 },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.False);
               Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
               Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

               var obj = dataTreeBranch.ParsedObject as ushort?;
               Assert.That(obj, Is.Not.Null);
               Assert.That(obj!.Value, Is.EqualTo(1));
           }));
        #endregion

        #region ENDPOINT_TIMING
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x01, 0x02, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingResponse)));

                var obj = dataTreeBranch.ParsedObject as GetEndpointTimingResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.TimingId, Is.EqualTo(2));
                Assert.That(obj!.Count, Is.EqualTo(3));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING,
            new byte[] { 0x00, 0x01, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(SetEndpointTimingRequest)));

                var obj = dataTreeBranch.ParsedObject as SetEndpointTimingRequest;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.TimingId, Is.EqualTo(2));
            }));
        #endregion

        #region ENDPOINT_TIMING_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = dataTreeBranch.ParsedObject as byte?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(3));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            new byte[] {
                0x03, 0x53, 0x6c, 0x6f, 0x77, 0x20, 0x28, 0x32,
                0x30, 0x30, 0x75, 0x53, 0x2f, 0x32, 0x35, 0x46,
                0x50, 0x53, 0x29
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointTimingDescriptionResponse)));

                var obj = dataTreeBranch.ParsedObject as GetEndpointTimingDescriptionResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.TimingId, Is.EqualTo(3));
                Assert.That(obj.Description, Is.EqualTo("Slow (200uS/25FPS)"));
            }));
        #endregion

        #region ENDPOINT_RESPONDERS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] {
                0x00, 0x03, 0x00, 0x00, 0x00, 0x09, 0x53, 0x47,
                0x94, 0x71, 0xaf, 0x2f
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));

                var obj = dataTreeBranch.ParsedObject as GetEndpointRespondersResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(3));
                Assert.That(obj.ListChangedNumber, Is.EqualTo(9));
                Assert.That(obj.UIDs, Is.Not.Null);
                Assert.That(obj.UIDs.Length, Is.EqualTo(1));
                Assert.That(obj.UIDs[0], Is.EqualTo(new UID(0x5347, 0x9471af2f)));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            new byte[] {
                0x00, 0x03, 0x00, 0x00, 0x00, 0x09, 0x53, 0x47,
                0x94, 0x71, 0xaf, 0x2f, 0x53, 0x47, 0x94, 0x03,
                0x02, 0x01
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointRespondersResponse)));

                var obj = dataTreeBranch.ParsedObject as GetEndpointRespondersResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(3));
                Assert.That(obj.ListChangedNumber, Is.EqualTo(9));
                Assert.That(obj.UIDs, Is.Not.Null);
                Assert.That(obj.UIDs.Length, Is.EqualTo(2));
                Assert.That(obj.UIDs[0], Is.EqualTo(new UID(0x5347, 0x9471af2f)));
                Assert.That(obj.UIDs[1], Is.EqualTo(new UID(0x5347, 0x94030201)));
            }));
        #endregion

        #region ENDPOINT_RESPONDER_LIST_CHANGE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = dataTreeBranch.ParsedObject as ushort?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            new byte[] { 0x00, 0x01, 0x02, 0x03, 0x02, 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));

                var obj = dataTreeBranch.ParsedObject as GetEndpointResponderListChangeResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.ListChangeNumber, Is.EqualTo(33751555));
            }));
        #endregion

        #region BINDING_CONTROL_FIELDS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.BINDING_CONTROL_FIELDS,
            new byte[] { 0x00, 0x01, 0x53, 0x47, 0x94, 0x71, 0xaf, 0x2f },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetBindingAndControlFieldsRequest)));

                var obj = dataTreeBranch.ParsedObject as GetBindingAndControlFieldsRequest;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.UID, Is.EqualTo(new UID(0x5347, 0x9471af2f)));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BINDING_CONTROL_FIELDS,
            new byte[] {
                0x00, 0x01, 0x53, 0x47, 0x94, 0x71, 0xaf, 0x2f,
                0x00, 0x00, 0x52, 0x46, 0x93, 0x72, 0xad, 0x21,
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetBindingAndControlFieldsResponse)));

                var obj = dataTreeBranch.ParsedObject as GetBindingAndControlFieldsResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.EndpointId, Is.EqualTo(1));
                Assert.That(obj!.UID, Is.EqualTo(new UID(0x5347, 0x9471af2f)));
                Assert.That(obj!.ControlField, Is.EqualTo(0));
                Assert.That(obj!.BindingUID, Is.EqualTo(new UID(0x5246, 0x9372ad21)));
            }));
        #endregion

        #region BACKGROUND_QUEUED_STATUS_POLICY
        yield return new PayloadToParseBagData(
           ERDM_Command.GET_COMMAND,
           ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
           new byte[] { },
           new Action<DataTreeBranch>((dataTreeBranch) =>
           {
               Assert.That(dataTreeBranch.IsUnset, Is.False);
               Assert.That(dataTreeBranch.IsEmpty, Is.True);
               Assert.That(dataTreeBranch.ParsedObject, Is.Null);
           }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
            new byte[] { 0x01, 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetBackgroundQueuedStatusPolicyResponse)));

                var obj = dataTreeBranch.ParsedObject as GetBackgroundQueuedStatusPolicyResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.PolicyId, Is.EqualTo(1));
                Assert.That(obj!.Count, Is.EqualTo(2));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = dataTreeBranch.ParsedObject as byte?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(2));
            }));
        #endregion

        #region BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION,
            new byte[] { 0x03 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = dataTreeBranch.ParsedObject as byte?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(3));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION,
            new byte[] {
                0x03, 0x53, 0x6c, 0x6f, 0x77, 0x20, 0x28, 0x32,
                0x30, 0x30, 0x75, 0x53, 0x2f, 0x32, 0x35, 0x46,
                0x50, 0x53, 0x29
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetBackgroundQueuedStatusPolicyDescriptionResponse)));

                var obj = dataTreeBranch.ParsedObject as GetBackgroundQueuedStatusPolicyDescriptionResponse;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.PolicyId, Is.EqualTo(3));
                Assert.That(obj.Description, Is.EqualTo("Slow (200uS/25FPS)"));
            }));
        #endregion

        #region MANUFACTURER_URL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.MANUFACTURER_URL,
            new byte[] { 0x68, 0x74, 0x74, 0x70, 0x73, 0x3a, 0x2f, 0x2f, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x2e, 0x63, 0x6f, 0x6d },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = dataTreeBranch.ParsedObject as string;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!, Is.EqualTo("https://example.com"));
            }));
        #endregion

        #region PRODUCT_URL
        yield return new PayloadToParseBagData(
             ERDM_Command.GET_COMMAND_RESPONSE,
             ERDM_Parameter.PRODUCT_URL,
             new byte[] { 0x68, 0x74, 0x74, 0x70, 0x73, 0x3a, 0x2f, 0x2f, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x2e, 0x63, 0x6f, 0x6d },
             new Action<DataTreeBranch>((dataTreeBranch) =>
             {
                 Assert.That(dataTreeBranch.IsUnset, Is.False);
                 Assert.That(dataTreeBranch.IsEmpty, Is.False);
                 Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                 Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                 var obj = dataTreeBranch.ParsedObject as string;
                 Assert.That(obj, Is.Not.Null);
                 Assert.That(obj!, Is.EqualTo("https://example.com"));
             }));
        #endregion

        #region FIRMWARE_URL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.FIRMWARE_URL,
            new byte[] { 0x68, 0x74, 0x74, 0x70, 0x73, 0x3a, 0x2f, 0x2f, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x2e, 0x63, 0x6f, 0x6d },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = dataTreeBranch.ParsedObject as string;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!, Is.EqualTo("https://example.com"));
            }));
        #endregion

        #region SERIAL_NUMBER
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SERIAL_NUMBER,
            new byte[] {
                0x31, 0x41, 0x46, 0x33, 0x38, 0x36, 0x37, 0x38,
                0x30, 0x30, 0x33, 0x30
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = dataTreeBranch.ParsedObject as string;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!, Is.EqualTo("1AF386780030"));
            }));

        #endregion

        #region DEVICE_INFO_OFFSTAGE
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DEVICE_INFO_OFFSTAGE,
            new byte[] { 0x04, 0x00, 0x08, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDeviceInfoOffstageRequest)));
                var obj = dataTreeBranch.ParsedObject as GetDeviceInfoOffstageRequest;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.RootPersonality, Is.EqualTo(4));
                Assert.That(obj!.SubDeviceRequested, Is.EqualTo(8));
                Assert.That(obj!.SubDevicePersonalityRequested, Is.EqualTo(0));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_INFO_OFFSTAGE,
            new byte[] {
                0x04, 0x00, 0x08, 0x00,
                0x01, 0x00, 0x00, 0x05, 0x06, 0x01, 0x02, 0x00,
                0x01, 0x01, 0x00, 0x01, 0x01, 0x03, 0x00, 0x01,
                0x00, 0x04, 0x00
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetDeviceInfoOffstageResponse)));

                var obj = (GetDeviceInfoOffstageResponse)dataTreeBranch.ParsedObject;
                Assert.That(obj.RootPersonality, Is.EqualTo(4));
                Assert.That(obj.SubDeviceRequested, Is.EqualTo(8));
                Assert.That(obj.SubDevicePersonalityRequested, Is.EqualTo(0));

                var deviceInfo = obj.DeviceInfo;
                Assert.That(deviceInfo, Is.Not.Null);
                Assert.That(deviceInfo.RdmProtocolVersionMajor, Is.EqualTo(1));
                Assert.That(deviceInfo.RdmProtocolVersionMinor, Is.EqualTo(0));
                Assert.That(deviceInfo.DeviceModelId, Is.EqualTo(0x0005));
                Assert.That(deviceInfo.ProductCategoryCoarse, Is.EqualTo(ERDM_ProductCategoryCoarse.POWER));
                Assert.That(deviceInfo.ProductCategoryFine, Is.EqualTo(ERDM_ProductCategoryFine.POWER_CONTROL));
                Assert.That(deviceInfo.SoftwareVersionId, Is.EqualTo(0x02000101));
                Assert.That(deviceInfo.Dmx512Footprint, Is.EqualTo(1));
                Assert.That(deviceInfo.Dmx512CurrentPersonality, Is.EqualTo(1));
                Assert.That(deviceInfo.Dmx512NumberOfPersonalities, Is.EqualTo(3));
                Assert.That(deviceInfo.Dmx512StartAddress, Is.EqualTo(1));
                Assert.That(deviceInfo.SubDeviceCount, Is.EqualTo(4));
                Assert.That(deviceInfo.SensorCount, Is.EqualTo(0));
            }));
        #endregion

        #region TEST_DATA        
        #endregion

        #region COMMS_STATUS_NSC
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.COMMS_STATUS_NSC,
            new byte[] { 0b00111111, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(GetCommunicationStatusNullStartCodeResponse)));

                var obj = (GetCommunicationStatusNullStartCodeResponse)dataTreeBranch.ParsedObject;
                Assert.That(obj.AdditiveChecksumOfMostRecentPacket, Is.EqualTo(1));
                Assert.That(obj.PacketCount, Is.EqualTo(1));
                Assert.That(obj.MostRecentSlotCount, Is.EqualTo(1));
                Assert.That(obj.MinimumSlotCount, Is.EqualTo(1));
                Assert.That(obj.MaximumSlotCount, Is.EqualTo(1));
                Assert.That(obj.NumberOfPacketsWithAnError, Is.EqualTo(1));
            }));
        #endregion

        #region IDENTIFY_TIMEOUT
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.IDENTIFY_TIMEOUT,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.IDENTIFY_TIMEOUT,
            new byte[] { 0x00, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ushort)));

                var obj = (ushort)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(1));
            }));
        #endregion

        #region POWER_OFF_READY
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.POWER_OFF_READY,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));

                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.True);
            }));
        #endregion

        #region SHIPPING_LOCK
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SHIPPING_LOCK,
            new byte[] { 0x02 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ShippingLockState)));

                var obj = (ERDM_ShippingLockState)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_ShippingLockState.PARTIALLY_LOCKED));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.SHIPPING_LOCK,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_ShippingLockState)));

                var obj = (ERDM_ShippingLockState)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(ERDM_ShippingLockState.LOCKED));
            }));
        #endregion

        #region LIST_TAGS
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.LIST_TAGS,
            new byte[] {
                0x42, 0x41, 0x43, 0x4B, 0x54, 0x52, 0x55, 0x53, 0x53,
                0x00,
                0x4D, 0x49, 0x44, 0x54, 0x52, 0x55, 0x53, 0x53,
                0x00,
                0x46, 0x52, 0x4F, 0x4E, 0x54, 0x54, 0x52, 0x55, 0x53, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string[])));

                var obj = (string[])dataTreeBranch.ParsedObject;
                Assert.That(obj.Length, Is.EqualTo(3));
                Assert.That(obj[0], Is.EqualTo("BACKTRUSS"));
                Assert.That(obj[1], Is.EqualTo("MIDTRUSS"));
                Assert.That(obj[2], Is.EqualTo("FRONTTRUSS"));
            }));
        #endregion

        #region ADD_TAG
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.ADD_TAG,
            new byte[] { 0x42, 0x41, 0x43, 0x4B, 0x54, 0x52, 0x55, 0x53, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("BACKTRUSS"));

            }));
        #endregion

        #region REMOVE_TAG
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.REMOVE_TAG,
            new byte[] { 0x42, 0x41, 0x43, 0x4B, 0x54, 0x52, 0x55, 0x53, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));

                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("BACKTRUSS"));

            }));
        #endregion

        #region CHECK_TAG
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.CHECK_TAG,
            new byte[] { 0x42, 0x41, 0x43, 0x4B, 0x54, 0x52, 0x55, 0x53, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = (string)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo("BACKTRUSS"));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.CHECK_TAG,
            new byte[] { 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(bool)));

                var obj = (bool)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.True);
            }));
        #endregion

        #region CLEAR_TAGS
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.CLEAR_TAGS,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND_RESPONSE,
            ERDM_Parameter.CLEAR_TAGS,
            new byte[0],
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.True);
                Assert.That(dataTreeBranch.ParsedObject, Is.Null);
            }));
        #endregion

        #region DEVICE_UNIT_NUMBER
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DEVICE_UNIT_NUMBER,
            new byte[] { 0x00, 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));

                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(256));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.SET_COMMAND,
            ERDM_Parameter.DEVICE_UNIT_NUMBER,
            new byte[] { 0x00, 0x00, 0x01, 0x00 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(uint)));

                var obj = (uint)dataTreeBranch.ParsedObject;
                Assert.That(obj, Is.EqualTo(256));
            }));
        #endregion

        #region DMX_PERSONALITY_ID
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.DMX_PERSONALITY_ID,
            new byte[] { 0x10 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = dataTreeBranch.ParsedObject as byte?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!, Is.EqualTo(0x10));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.DMX_PERSONALITY_ID,
            new byte[] { 0x10, 0x53, 0x4D, 0x61, 0x72, 0x00, 0x00, 0x00, 0x0F },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMPersonalityId)));

                var obj = dataTreeBranch.ParsedObject as RDMPersonalityId;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.PersonalityId, Is.EqualTo(0x10));
                Assert.That(obj!.Index, Is.EqualTo(0x10));
                Assert.That(obj!.MajorPersonalityId, Is.EqualTo(1397580146));
                Assert.That(obj!.MinorPersonalityId, Is.EqualTo(15));
                Assert.That(obj!.MinIndex, Is.EqualTo(1));
            }));
        #endregion

        #region SENSOR_TYPE_CUSTOM
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.SENSOR_TYPE_CUSTOM,
            new byte[] { 0xee, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(byte)));

                var obj = dataTreeBranch.ParsedObject as byte?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(0xee));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_TYPE_CUSTOM,
            new byte[] { 0xee, 0x45, 0x75, 0x72, 0x6F },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorTypeCustomDefine)));

                var obj = dataTreeBranch.ParsedObject as RDMSensorTypeCustomDefine;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Id, Is.EqualTo(0xee));
                Assert.That(obj!.Index, Is.EqualTo(0xee));
                Assert.That(obj!.MinIndex, Is.EqualTo(0x80));
                Assert.That(obj!.Label, Is.EqualTo("Euro"));
            }));
        #endregion

        #region SENSOR_UNIT_CUSTOM
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.SENSOR_UNIT_CUSTOM,
            new byte[] { 0xee, 0x45, 0x75, 0x72, 0x6F },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMSensorUnitCustomDefine)));

                var obj = dataTreeBranch.ParsedObject as RDMSensorUnitCustomDefine;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Id, Is.EqualTo(0xee));
                Assert.That(obj!.Index, Is.EqualTo(0xee));
                Assert.That(obj!.MinIndex, Is.EqualTo(0x80));
                Assert.That(obj!.Label, Is.EqualTo("Euro"));
            }));
        #endregion

        #region METADATA_PARAMETER_VERSION
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND,
            ERDM_Parameter.METADATA_PARAMETER_VERSION,
            new byte[] { 0x06, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));

                var obj = dataTreeBranch.ParsedObject as ERDM_Parameter?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
            }));
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.METADATA_PARAMETER_VERSION,
            new byte[] { 0x06, 0x53, 0x02, 0x01 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMMetadataParameterVersion)));

                var obj = dataTreeBranch.ParsedObject as RDMMetadataParameterVersion;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.ParameterId, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
                Assert.That(obj.Version, Is.EqualTo(513));
            }));
        #endregion

        #region METADATA_JSON
        yield return new PayloadToParseBagData(
        ERDM_Command.GET_COMMAND,
            ERDM_Parameter.METADATA_JSON,
            new byte[] { 0x06, 0x53 },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(ERDM_Parameter)));

                var obj = dataTreeBranch.ParsedObject as ERDM_Parameter?;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.Value, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
            }));

        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.METADATA_JSON,
            new byte[] {
                0x81, 0x0d,
                0x7B, 0x22, 0x6E, 0x61, 0x6D, 0x65, 0x22, 0x3A, 0x22, 0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x6C,
                0x6F, 0x6E, 0x65, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x72, 0x6F, 0x6C, 0x20, 0x4D, 0x6F, 0x64,
                0x65, 0x22, 0x2C, 0x22, 0x6D, 0x61, 0x6E, 0x75, 0x66, 0x61, 0x63, 0x74, 0x75, 0x72, 0x65,
                0x72, 0x5F, 0x69, 0x64, 0x22, 0x3A, 0x31, 0x39, 0x37, 0x39, 0x32, 0x2C, 0x22, 0x70, 0x69,
                0x64, 0x22, 0x3A, 0x33, 0x33, 0x30, 0x33, 0x37, 0x2C, 0x22, 0x76, 0x65, 0x72, 0x73, 0x69,
                0x6F, 0x6E, 0x22, 0x3A, 0x31, 0x2C, 0x22, 0x6E, 0x6F, 0x74, 0x65, 0x73, 0x22, 0x3A, 0x22,
                0x54, 0x6F, 0x64, 0x6F, 0x3A, 0x20, 0x41, 0x64, 0x64, 0x20, 0x6E, 0x6F, 0x74, 0x65, 0x73,
                0x20, 0x66, 0x6F, 0x72, 0x20, 0x4D, 0x61, 0x72, 0x74, 0x69, 0x6E, 0x20, 0x53, 0x74, 0x61,
                0x6E, 0x64, 0x61, 0x6C, 0x6F, 0x6E, 0x65, 0x20, 0x43, 0x6F, 0x6E, 0x74, 0x72, 0x6F, 0x6C,
                0x20, 0x4D, 0x6F, 0x64, 0x65, 0x20, 0x28, 0x30, 0x78, 0x38, 0x31, 0x30, 0x44, 0x29, 0x22,
                0x2C, 0x22, 0x67, 0x65, 0x74, 0x5F, 0x72, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x5F, 0x73,
                0x75, 0x62, 0x64, 0x65, 0x76, 0x69, 0x63, 0x65, 0x5F, 0x72, 0x61, 0x6E, 0x67, 0x65, 0x22,
                0x3A, 0x5B, 0x22, 0x72, 0x6F, 0x6F, 0x74, 0x22, 0x2C, 0x22, 0x73, 0x75, 0x62, 0x64, 0x65,
                0x76, 0x69, 0x63, 0x65, 0x73, 0x22, 0x5D, 0x2C, 0x22, 0x67, 0x65, 0x74, 0x5F, 0x72, 0x65,
                0x71, 0x75, 0x65, 0x73, 0x74, 0x22, 0x3A, 0x5B, 0x5D, 0x2C, 0x22, 0x67, 0x65, 0x74, 0x5F,
                0x72, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x73, 0x65, 0x22, 0x3A, 0x5B, 0x7B, 0x22, 0x6E, 0x61,
                0x6D, 0x65, 0x22, 0x3A, 0x22, 0x73, 0x74, 0x61, 0x74, 0x65, 0x22, 0x2C, 0x22, 0x74, 0x79,
                0x70, 0x65, 0x22, 0x3A, 0x22, 0x75, 0x69, 0x6E, 0x74, 0x38, 0x22, 0x7D, 0x5D, 0x2C, 0x22,
                0x73, 0x65, 0x74, 0x5F, 0x72, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x5F, 0x73, 0x75, 0x62,
                0x64, 0x65, 0x76, 0x69, 0x63, 0x65, 0x5F, 0x72, 0x61, 0x6E, 0x67, 0x65, 0x22, 0x3A, 0x5B,
                0x22, 0x72, 0x6F, 0x6F, 0x74, 0x22, 0x2C, 0x22, 0x73, 0x75, 0x62, 0x64, 0x65, 0x76, 0x69,
                0x63, 0x65, 0x73, 0x22, 0x2C, 0x22, 0x62, 0x72, 0x6F, 0x61, 0x64, 0x63, 0x61, 0x73, 0x74,
                0x22, 0x5D, 0x2C, 0x22, 0x73, 0x65, 0x74, 0x5F, 0x72, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74,
                0x22, 0x3A, 0x22, 0x67, 0x65, 0x74, 0x5F, 0x72, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x73, 0x65,
                0x22, 0x2C, 0x22, 0x73, 0x65, 0x74, 0x5F, 0x72, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x73, 0x65,
                0x22, 0x3A, 0x5B, 0x5D, 0x7D
            },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(RDMMetadataJson)));

                var obj = dataTreeBranch.ParsedObject as RDMMetadataJson;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!.ParameterId, Is.EqualTo((ERDM_Parameter)0x810D));
                Assert.That(obj!.Index, Is.EqualTo((ERDM_Parameter)0x810D));
                Assert.That(obj!.JSON, Is.EqualTo("{\"name\":\"Standalone Control Mode\",\"manufacturer_id\":19792,\"pid\":33037,\"version\":1,\"notes\":\"Todo: Add notes for Martin Standalone Control Mode (0x810D)\",\"get_request_subdevice_range\":[\"root\",\"subdevices\"],\"get_request\":[],\"get_response\":[{\"name\":\"state\",\"type\":\"uint8\"}],\"set_request_subdevice_range\":[\"root\",\"subdevices\",\"broadcast\"],\"set_request\":\"get_response\",\"set_response\":[]}"));
            }));
        #endregion

        #region METADATA_JSON_URL
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.METADATA_JSON_URL,
            new byte[] { 0x68, 0x74, 0x74, 0x70, 0x73, 0x3a, 0x2f, 0x2f, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x2e, 0x63, 0x6f, 0x6d },
            new Action<DataTreeBranch>((dataTreeBranch) =>
            {
                Assert.That(dataTreeBranch.IsUnset, Is.False);
                Assert.That(dataTreeBranch.IsEmpty, Is.False);
                Assert.That(dataTreeBranch.ParsedObject, Is.Not.Null);
                Assert.That(dataTreeBranch.ParsedObject, Is.TypeOf(typeof(string)));
                var obj = dataTreeBranch.ParsedObject as string;
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj!, Is.EqualTo("https://example.com"));
            }));
        #endregion
    }

    public PayloadToParseBagData PayloadToParseBag { get; private set; }
    public MetadataJSONObjectDefine Define { get; private set; }
    public override string ToString() => PayloadToParseBag.ToString();

    public PayloadToParsedObjectTestSubject(MetadataJSONObjectDefine define, PayloadToParseBagData payloadToParseBagData)
    {
        this.Define = define;
        this.PayloadToParseBag = payloadToParseBagData;
    }

    public readonly struct PayloadToParseBagData
    {
        public readonly ERDM_Command Command;
        public readonly RDMSharp.Metadata.JSON.Command.ECommandDublicate CommandDublicate;
        public readonly ERDM_Parameter Parameter;
        public readonly byte[] Payload;
        public readonly Action<DataTreeBranch> TestPayload;

        internal PayloadToParseBagData(in ERDM_Command command, in ERDM_Parameter parameter, in byte[] payload, Action<DataTreeBranch> testPayload)
        {
            this.Command = command;
            switch (command)
            {
                case ERDM_Command.GET_COMMAND:
                    this.CommandDublicate = RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetRequest;
                    break;
                case ERDM_Command.GET_COMMAND_RESPONSE:
                    this.CommandDublicate = RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse;
                    break;
                case ERDM_Command.SET_COMMAND:
                    this.CommandDublicate = RDMSharp.Metadata.JSON.Command.ECommandDublicate.SetRequest;
                    break;
                case ERDM_Command.SET_COMMAND_RESPONSE:
                    this.CommandDublicate = RDMSharp.Metadata.JSON.Command.ECommandDublicate.SetResponse;
                    break;
            }
            this.Parameter = parameter;
            this.Payload = payload;
            this.TestPayload = testPayload;
        }

        public override string ToString()
        {
            return $"{Parameter} ({CommandDublicate}), Payload: {BitConverter.ToString(Payload)}";
        }
    }
}