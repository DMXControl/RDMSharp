using RDMSharp.Metadata;
using System.Reflection;

namespace RDMSharpTests.Metadata.Parser;

public class PayloadToParsedObjectTestSubject
{
    public static readonly object[] TestSubjects = getTestSubjects();
    internal static string[] GetResources()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceNames();
    }
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
                if (define == null)
                    continue;
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

        #region PROXIED_DEVICE_COUNT
        #endregion

        #region COMMS_STATUS
        #endregion

        #region QUEUED_MESSAGE
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
        #endregion

        #region SUB_DEVICE_STATUS_REPORT_THRESHOLD
        #endregion

        #region QUEDUED_MESSAGE_SENSOR_SUBSCRIBE
        #endregion

        #region SUPPORTED_PARAMETERS
        #endregion

        #region PARAMETER_DESCRIPTION
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
        #endregion

        #region DEVICE_MODEL_DESCRIPTION
        #endregion

        #region MANUFACTURER_LABEL
        #endregion

        #region DEVICE_LABEL
        #endregion

        #region FACTORY_DEFAULTS
        #endregion

        #region LANGUAGE_CAPABILITIES
        #endregion

        #region LANGUAGE
        #endregion

        #region SOFTWARE_VERSION_LABEL
        #endregion

        #region BOOT_SOFTWARE_VERSION_ID
        #endregion

        #region BOOT_SOFTWARE_VERSION_LABEL
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

        #region DEFALUT_SLOT_VALUE
        #endregion

        #region SENDOR_DEFINITION
        #endregion

        #region SENSOR_VALUE
        #endregion

        #region RECORD_SENSOR
        #endregion

        #region DEVICE_HOURS
        #endregion

        #region LAMP_HOURS
        #endregion

        #region LAMP_STRIKES
        #endregion

        #region LAMP_STATE
        #endregion

        #region LAMP_ON_MODE
        #endregion

        #region DEVICE_POWER_CYCLE
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
        #endregion

        #region PAN_INVERT
        #endregion

        #region TILT_INVERT
        #endregion

        #region PAN_TILT_SWAP
        #endregion

        #region REAL_TIME_CLOCK
        #endregion

        #region IDENTIFY_DEVICE
        #endregion

        #region RESET_DEVICE
        #endregion

        #region POWER_STATE
        #endregion

        #region PERFORM_SELFTEST
        #endregion

        #region SELF_TEST_DESCRIPTION
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



        #region PRESET_INFO
        yield return new PayloadToParseBagData(
            ERDM_Command.GET_COMMAND_RESPONSE,
            ERDM_Parameter.PRESET_INFO,
            new byte[] {
                0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x02,
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
                Assert.That(obj!.LevelFieldSupported, Is.EqualTo(false));
                Assert.That(obj!.PresetSequenceSupported, Is.EqualTo(false));
                Assert.That(obj!.SplitTimesSupported, Is.EqualTo(false));
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
        #endregion

        #region POWER_ON_SELF_TEST
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
        #endregion

        #region TEST_DATA
        #endregion

        #region COMMS_STATUS_NSC
        #endregion

        #region IDENTIFY_TIMEOUT
        #endregion

        #region POWER_OFF_READY
        #endregion

        #region SHIPPING_LOCK
        #endregion

        #region LIST_TAGS
        #endregion

        #region ADD_TAG
        #endregion

        #region REMOVE_TAG
        #endregion

        #region CHECK_TAG
        #endregion

        #region CLEAR_TAGS
        #endregion

        #region DEVICE_UNIT_NUMBER
        #endregion

        #region DMX_PERSONALITY_ID
        #endregion

        #region SENSOR_TYPE_CUSTOM
        #endregion

        #region SENSOR_UNIT_CUSTOM
        #endregion

        #region METADATA_PARAMETER_VERSION
        #endregion

        #region METADATA_JSON
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