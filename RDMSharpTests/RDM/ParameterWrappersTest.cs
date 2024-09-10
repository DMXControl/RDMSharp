using RDMSharp.ParameterWrapper;
using RDMSharp.ParameterWrapper.Generic;
using RDMSharp.ParameterWrapper.SGM;
using System.Collections.ObjectModel;

namespace RDMSharpTests.RDM
{
    public class ParameterWrappersTest
    {
        private static readonly ERDM_Parameter[] notUsableParameters = new ERDM_Parameter[]
           {
               ERDM_Parameter.NONE,
               ERDM_Parameter.DISC_MUTE,
               ERDM_Parameter.DISC_UNIQUE_BRANCH,
               ERDM_Parameter.DISC_UN_MUTE
           };
        private static readonly ERDM_Parameter[] e1_20Parameters = new ERDM_Parameter[]
     {
            ERDM_Parameter.PROXIED_DEVICES,
            ERDM_Parameter.PROXIED_DEVICES_COUNT,
            ERDM_Parameter.COMMS_STATUS,
            ERDM_Parameter.QUEUED_MESSAGE,
            ERDM_Parameter.STATUS_MESSAGES,
            ERDM_Parameter.STATUS_ID_DESCRIPTION,
            ERDM_Parameter.CLEAR_STATUS_ID,
            ERDM_Parameter.SUB_DEVICE_STATUS_REPORT_THRESHOLD,
            ERDM_Parameter.SUPPORTED_PARAMETERS,
            ERDM_Parameter.PARAMETER_DESCRIPTION,
            ERDM_Parameter.DEVICE_INFO,
            ERDM_Parameter.PRODUCT_DETAIL_ID_LIST,
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
            ERDM_Parameter.MANUFACTURER_LABEL,
            ERDM_Parameter.DEVICE_LABEL,
            ERDM_Parameter.FACTORY_DEFAULTS,
            ERDM_Parameter.LANGUAGE_CAPABILITIES,
            ERDM_Parameter.LANGUAGE,
            ERDM_Parameter.SOFTWARE_VERSION_LABEL,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
            ERDM_Parameter.DMX_PERSONALITY,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            ERDM_Parameter.DMX_START_ADDRESS,
            ERDM_Parameter.SLOT_INFO,
            ERDM_Parameter.SLOT_DESCRIPTION,
            ERDM_Parameter.DEFAULT_SLOT_VALUE,
            ERDM_Parameter.SENSOR_DEFINITION,
            ERDM_Parameter.SENSOR_VALUE,
            ERDM_Parameter.RECORD_SENSORS,
            ERDM_Parameter.DEVICE_HOURS,
            ERDM_Parameter.LAMP_HOURS,
            ERDM_Parameter.LAMP_STRIKES,
            ERDM_Parameter.LAMP_STATE,
            ERDM_Parameter.LAMP_ON_MODE,
            ERDM_Parameter.DEVICE_POWER_CYCLES,
            ERDM_Parameter.DISPLAY_INVERT,
            ERDM_Parameter.DISPLAY_LEVEL,
            ERDM_Parameter.PAN_INVERT,
            ERDM_Parameter.TILT_INVERT,
            ERDM_Parameter.PAN_TILT_SWAP,
            ERDM_Parameter.REAL_TIME_CLOCK,
            ERDM_Parameter.IDENTIFY_DEVICE,
            ERDM_Parameter.RESET_DEVICE,
            ERDM_Parameter.POWER_STATE,
            ERDM_Parameter.PERFORM_SELFTEST,
            ERDM_Parameter.SELF_TEST_DESCRIPTION,
            ERDM_Parameter.CAPTURE_PRESET,
            ERDM_Parameter.PRESET_PLAYBACK
     };
        private static readonly ERDM_Parameter[] e1_37_1Parameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.DMX_BLOCK_ADDRESS,
            ERDM_Parameter.DMX_FAIL_MODE,
            ERDM_Parameter.DMX_STARTUP_MODE,
            ERDM_Parameter.DIMMER_INFO,
            ERDM_Parameter.MINIMUM_LEVEL,
            ERDM_Parameter.MAXIMUM_LEVEL,
            ERDM_Parameter.CURVE,
            ERDM_Parameter.CURVE_DESCRIPTION,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME,
            ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
            ERDM_Parameter.MODULATION_FREQUENCY,
            ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
            ERDM_Parameter.BURN_IN,
            ERDM_Parameter.LOCK_PIN,
            ERDM_Parameter.LOCK_STATE,
            ERDM_Parameter.LOCK_STATE_DESCRIPTION,
            ERDM_Parameter.IDENTIFY_MODE,
            ERDM_Parameter.PRESET_INFO,
            ERDM_Parameter.PRESET_STATUS,
            ERDM_Parameter.PRESET_MERGEMODE,
            ERDM_Parameter.POWER_ON_SELF_TEST
        };
        private static readonly ERDM_Parameter[] e1_37_2Parameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.LIST_INTERFACES,
            ERDM_Parameter.INTERFACE_LABEL,
            ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
            ERDM_Parameter.IPV4_DHCP_MODE,
            ERDM_Parameter.IPV4_ZEROCONF_MODE,
            ERDM_Parameter.IPV4_CURRENT_ADDRESS,
            ERDM_Parameter.IPV4_STATIC_ADDRESS,
            ERDM_Parameter.INTERFACE_RENEW_DHCP,
            ERDM_Parameter.INTERFACE_RELEASE_DHCP,
            ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION,
            ERDM_Parameter.IPV4_DEFAULT_ROUTE,
            ERDM_Parameter.DNS_IPV4_NAME_SERVER,
            ERDM_Parameter.DNS_HOSTNAME,
            ERDM_Parameter.DNS_DOMAIN_NAME
        };
        private static readonly ERDM_Parameter[] e1_37_5Parameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.MANUFACTURER_URL,
            ERDM_Parameter.PRODUCT_URL,
            ERDM_Parameter.FIRMWARE_URL,
            ERDM_Parameter.SERIAL_NUMBER,
            ERDM_Parameter.DEVICE_INFO_OFFSTAGE,
            ERDM_Parameter.TEST_DATA,
            ERDM_Parameter.COMMS_STATUS_NSC,
            ERDM_Parameter.IDENTIFY_TIMEOUT,
            ERDM_Parameter.POWER_OFF_READY,
            ERDM_Parameter.SHIPPING_LOCK,
            ERDM_Parameter.LIST_TAGS,
            ERDM_Parameter.ADD_TAG,
            ERDM_Parameter.REMOVE_TAG,
            ERDM_Parameter.CHECK_TAG,
            ERDM_Parameter.CLEAR_TAGS,
            ERDM_Parameter.DEVICE_UNIT_NUMBER,
            ERDM_Parameter.DMX_PERSONALITY_ID,
            ERDM_Parameter.SENSOR_TYPE_CUSTOM,
            ERDM_Parameter.SENSOR_UNIT_CUSTOM,
            ERDM_Parameter.METADATA_PARAMETER_VERSION,
            ERDM_Parameter.METADATA_JSON,
            ERDM_Parameter.METADATA_JSON_URL
        };
        private static readonly ERDM_Parameter[] e1_37_7Parameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.ENDPOINT_LIST,
            ERDM_Parameter.ENDPOINT_LIST_CHANGE,
            ERDM_Parameter.IDENTIFY_ENDPOINT,
            ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            ERDM_Parameter.ENDPOINT_MODE,
            ERDM_Parameter.ENDPOINT_LABEL,
            ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            ERDM_Parameter.DISCOVERY_STATE,
            ERDM_Parameter.BACKGROUND_DISCOVERY,
            ERDM_Parameter.ENDPOINT_TIMING,
            ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            ERDM_Parameter.ENDPOINT_RESPONDERS,
            ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            ERDM_Parameter.BINDING_CONTROL_FIELDS,
            ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
            ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION
        };
        private static readonly ERDM_Parameter[] e1_33Parameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.COMPONENT_SCOPE,
            ERDM_Parameter.SEARCH_DOMAIN,
            ERDM_Parameter.TCP_COMMS_STATUS,
            ERDM_Parameter.BROKER_STATUS
        };
        private static readonly ERDM_Parameter[] sgmParameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.SERIAL_NUMBER_SGM,
            ERDM_Parameter.REFRESH_RATE,
            ERDM_Parameter.DIMMING_CURVE,
            ERDM_Parameter.FAN_MODE,
            ERDM_Parameter.CRMX_LOG_OFF,
            ERDM_Parameter.DIM_MODE,
            ERDM_Parameter.INVERT_PIXEL_ORDER,
            ERDM_Parameter.NORTH_CALIBRATION,
            ERDM_Parameter.BATTERY_EXTENSION,
            ERDM_Parameter.SMPS_CALIBRATION,
            ERDM_Parameter.WIRELESS_DMX,
            ERDM_Parameter.CRMX_BRIDGE_MODE
        };

        private RDMParameterWrapperCatalogueManager manager;
        private ERDM_Parameter[] parameters;
        private ReadOnlyCollection<IRDMParameterWrapper> parameterWrappers;
        [SetUp]
        public void Setup()
        {
            manager = RDMParameterWrapperCatalogueManager.GetInstance();
            parameters = (ERDM_Parameter[])Enum.GetValues(typeof(ERDM_Parameter));
            parameterWrappers = manager.ParameterWrappers;
        }

        [Test]
        public void CheckAllParametersArDefined()
        {
            var parameterLeft = parameters.Except(e1_20Parameters).Except(e1_37_1Parameters).Except(e1_37_2Parameters).Except(e1_37_5Parameters).Except(e1_37_7Parameters).Except(e1_33Parameters).Except(sgmParameters).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(parameterLeft, Has.Count.EqualTo(notUsableParameters.Length));
                Assert.That(notUsableParameters.Except(parameterLeft).ToList(), Is.Empty);
            });
        }
        [Test]
        public void CheckE1_20WrappersDefined()
        {
            var notDefinedParameters = e1_20Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters, Is.Empty, $"The not defined Parameters:{Environment.NewLine}{ParameterWrappersTest.ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_1WrappersDefined()
        {
            var notDefinedParameters = e1_37_1Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters, Is.Empty, $"The not defined Parameters:{Environment.NewLine}{ParameterWrappersTest.ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_2WrappersDefined()
        {
            var notDefinedParameters = e1_37_2Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters, Is.Empty, $"The not defined Parameters:{Environment.NewLine}{ParameterWrappersTest.ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_5WrappersDefined()
        {
            var notDefinedParameters = e1_37_5Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters, Is.Empty, $"The not defined Parameters:{Environment.NewLine}{ParameterWrappersTest.ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_7WrappersDefined()
        {
            var notDefinedParameters = e1_37_7Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters, Is.Empty, $"The not defined Parameters:{Environment.NewLine}{ParameterWrappersTest.ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_33WrappersDefined()
        {
            var notDefinedParameters = e1_33Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters, Is.Empty, $"The not defined Parameters:{Environment.NewLine}{ParameterWrappersTest.ParametersToString(notDefinedParameters)}");
        }

        [Test]
        public void AssemblyListenerTest()
        {
            foreach (IRDMParameterWrapper pW in parameterWrappers)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(pW.Name, Does.Not.EndWith(" "), $"{pW.Name} Name is not Vaild");
                    Assert.That(pW.Name, Does.Not.StartWith(" "), $"{pW.Name} Name is not Vaild");
                });
                Assert.Multiple(() =>
                {
                    Assert.That(pW.Description, Does.Not.EndWith(" "), $"{pW.Name} Description is not Vaild");
                    Assert.That(pW.Description, Does.Not.StartWith(" "), $"{pW.Name} Description is not Vaild");
                    Assert.That(pW.Description, Does.EndWith("."), $"{pW.Name} Description should end with a \".\"");
                });
                var toString = pW.ToString();

                Assert.Multiple(() =>
                {
                    Assert.That(toString, Is.Not.Null);
                    Assert.That(toString, Does.Not.StartWith("{"));
                });
            }
            Assert.Multiple(() =>
            {
                foreach (ERDM_Parameter parameter in e1_20Parameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

                foreach (ERDM_Parameter parameter in e1_37_1Parameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

                foreach (ERDM_Parameter parameter in e1_37_2Parameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");
               
                foreach (ERDM_Parameter parameter in e1_37_5Parameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

                foreach (ERDM_Parameter parameter in e1_37_7Parameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

                foreach (ERDM_Parameter parameter in e1_33Parameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

                foreach (ERDM_Parameter parameter in sgmParameters)
                    Assert.That(parameterWrappers.Where(pw => pw.Parameter == parameter).ToList(), Has.Count.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");
            });
            Assert.That(e1_20Parameters.Length + e1_37_1Parameters.Length + e1_37_2Parameters.Length + e1_37_7Parameters.Length + e1_33Parameters.Length + sgmParameters.Length, Is.EqualTo(parameterWrappers.Count));
        }
        [Test]
        public void GenericParameterWrapperTestFwBw()
        {
            List<IRDMParameterWrapper> wrappers = new();
            wrappers.Add(new ASCIIParameterWrapper(new RDMParameterDescription(0x1000, 20, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "ASCII")));
            wrappers.Add(new SignedByteParameterWrapper(new RDMParameterDescription(0x1000, 1, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "sbyte")));
            wrappers.Add(new UnsignedByteParameterWrapper(new RDMParameterDescription(0x1000, 1, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "byte")));
            wrappers.Add(new SignedWordParameterWrapper(new RDMParameterDescription(0x1000, 2, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "short")));
            wrappers.Add(new UnsignedWordParameterWrapper(new RDMParameterDescription(0x1000, 2, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "ushort")));
            wrappers.Add(new SignedDWordParameterWrapper(new RDMParameterDescription(0x1000, 4, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "int")));
            wrappers.Add(new UnsignedDWordParameterWrapper(new RDMParameterDescription(0x1000, 4, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "uint")));
            wrappers.Add(new NotDefinedParameterWrapper(new RDMParameterDescription(0x1000, 20, ERDM_DataType.ASCII, ERDM_CommandClass.GET | ERDM_CommandClass.SET, description: "NotDefined")));
            TestParameterWrapperForwardBackwardSerialization(wrappers);
        }
        [Test]
        public void ParameterWrapperE1_20TestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(e1_20Parameters.Except([ERDM_Parameter.LANGUAGE]).Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        [Test]
        public void ParameterWrapperE1_33TestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(e1_33Parameters.Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        [Test]
        public void ParameterWrapperE1_37_1TestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(e1_37_1Parameters.Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        [Test]
        public void ParameterWrapperE1_37_2TestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(e1_37_2Parameters.Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        [Test]
        public void ParameterWrapperE1_37_5TestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(e1_37_5Parameters.Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        [Test]
        public void ParameterWrapperE1_37_7TestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(e1_37_7Parameters.Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        [Test]
        public void ParameterWrapperSGMTestFwBw()
        {
            TestParameterWrapperForwardBackwardSerialization(sgmParameters.Select(p => manager.GetRDMParameterWrapperByID(p)));
        }
        private static void TestParameterWrapperForwardBackwardSerialization(IEnumerable<IRDMParameterWrapper> wrappers)
        {
            object value = getValue(null!);
            foreach (var wrapper in wrappers)
            {
                byte tested = 0;

                if (wrapper is IRDMGetParameterWrapperRequest getRequest)
                {
                    tested++;
                    value = getValue(getRequest.GetRequestType);
                    value.GetHashCode();//For Coverage;

                    Assert.That(value, Is.Not.Null);
                    var data = getRequest.GetRequestObjectToParameterData(value);
                    var res = getRequest.GetRequestParameterDataToObject(data);
                    Assert.That(res, Is.EqualTo(value));

                    RDMMessage buildGetRequestMessage = getRequest.BuildGetRequestMessage(value);
                    Assert.That(buildGetRequestMessage, Is.Not.Null);
                }
                if (wrapper is IRDMGetParameterWrapperResponse getResponse)
                {
                    tested++;
                    value = getValue(getResponse.GetResponseType);
                    value.GetHashCode();//For Coverage;

                    Assert.That(value, Is.Not.Null);
                    var data = getResponse.GetResponseObjectToParameterData(value);
                    var res = getResponse.GetResponseParameterDataToObject(data);
                    Assert.That(res, Is.EqualTo(value));

                    RDMMessage buildGetResponseMessage = getResponse.BuildGetResponseMessage(value);
                    Assert.That(buildGetResponseMessage, Is.Not.Null);
                }
                if (wrapper is IRDMSetParameterWrapperRequest setRequest)
                {
                    tested++;
                    value = getValue(setRequest.SetRequestType);
                    value.GetHashCode();//For Coverage;

                    Assert.That(value, Is.Not.Null);
                    var data = setRequest.SetRequestObjectToParameterData(value);
                    var res = setRequest.SetRequestParameterDataToObject(data);
                    Assert.That(res, Is.EqualTo(value));

                    RDMMessage buildSetRequestMessage = setRequest.BuildSetRequestMessage(value);
                    Assert.That(buildSetRequestMessage, Is.Not.Null);
                }
                if (wrapper is IRDMSetParameterWrapperResponse setResponse)
                {
                    tested++;
                    value = getValue(setResponse.SetResponseType);
                    value.GetHashCode();//For Coverage;

                    Assert.That(value, Is.Not.Null);
                    var data = setResponse.SetResponseObjectToParameterData(value);
                    var res = setResponse.SetResponseParameterDataToObject(data);
                    Assert.That(res, Is.EqualTo(value));


                    RDMMessage buildSetResponseMessage = setResponse.BuildSetResponseMessage(value);
                    Assert.That(buildSetResponseMessage, Is.Not.Null);
                }
                if (wrapper is IRDMGetParameterWrapperWithEmptyGetRequest iGetParameterWrapperEmptyRequest)
                {
                    tested++;
                    RDMMessage buildGetRequestMessage = iGetParameterWrapperEmptyRequest.BuildGetRequestMessage();
                    Assert.That(buildGetRequestMessage, Is.Not.Null);
                }
                if (wrapper is IRDMGetParameterWrapperWithEmptyGetResponse iGetParameterWrapperEmptyResponse)
                {
                    tested++;
                    RDMMessage buildGetResponseMessage = iGetParameterWrapperEmptyResponse.BuildGetResponseMessage();
                    Assert.That(buildGetResponseMessage, Is.Not.Null);
                }
                if (wrapper is IRDMSetParameterWrapperWithEmptySetRequest iSetParameterWrapperEmptyRequest)
                {
                    tested++;
                    RDMMessage buildSetRequestMessage = iSetParameterWrapperEmptyRequest.BuildSetRequestMessage();
                    Assert.That(buildSetRequestMessage, Is.Not.Null);
                }
                if (wrapper is IRDMSetParameterWrapperWithEmptySetResponse iSetParameterWrapperEmptyResponse)
                {
                    tested++;
                    RDMMessage buildSetResponseMessage = iSetParameterWrapperEmptyResponse.BuildSetResponseMessage();
                    Assert.That(buildSetResponseMessage, Is.Not.Null);
                }
                if (wrapper is AbstractRDMParameterWrapper<Empty, Empty, Empty, Empty> abstractRDMParameterWrapperEmpty4)
                {
                    tested++;
                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.GetRequestObjectToParameterData(null); });
                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.GetRequestParameterDataToObject(null); });

                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.GetResponseObjectToParameterData(null); });
                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.GetResponseParameterDataToObject(null); });

                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.SetRequestObjectToParameterData(null); });
                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.SetRequestParameterDataToObject(null); });

                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.SetResponseObjectToParameterData(null); });
                    Assert.Throws(typeof(NotSupportedException), () => { abstractRDMParameterWrapperEmpty4.SetResponseParameterDataToObject(null); });
                }
                Assert.That(tested, Is.AtLeast(2));
            }

            static object getValue(Type type)
            {
                if (type == typeof(string))
                    return "Test String";
                if (type == typeof(bool))
                    return true;
                if (type == typeof(sbyte))
                    return (sbyte)55;
                if (type == typeof(byte))
                    return (byte)99;
                if (type == typeof(short))
                    return (short)-0x1234;
                if (type == typeof(ushort))
                    return (ushort)1523;
                if (type == typeof(ushort?))
                    return (ushort?)1523;
                if (type == typeof(int))
                    return (int)-0x123334;
                if (type == typeof(uint))
                    return (uint)154523;
                if (type == typeof(byte[]))
                    return new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
                if (type == typeof(RDMRealTimeClock))
                    return new RDMRealTimeClock(DateTime.Now);
                if (type == typeof(RDMDMXPersonality))
                    return new RDMDMXPersonality(3, 4);
                if (type == typeof(RDMCurve))
                    return new RDMCurve(3, 4);
                if (type == typeof(RDMPresetPlayback))
                    return new RDMPresetPlayback(3, 200);
                if (type == typeof(RDMSensorValue))
                    return new RDMSensorValue(3, 200);
                if (type == typeof(RDMPresetStatus))
                    return new RDMPresetStatus(3, 200);
                if (type == typeof(RDMCommunicationStatus))
                    return new RDMCommunicationStatus(3, 200);
                if (type == typeof(RDMDeviceInfo))
                    return new RDMDeviceInfo(3, 200);
                if (type == typeof(RDMDMXPersonalityDescription))
                    return new RDMDMXPersonalityDescription(3, 200);
                if (type == typeof(RDMParameterDescription))
                    return new RDMParameterDescription(3, 200);
                if (type == typeof(RDMProxiedDeviceCount))
                    return new RDMProxiedDeviceCount(3);
                if (type == typeof(RDMProxiedDevices))
                    return new RDMProxiedDevices([new UID((ushort)EManufacturer.Swisson_AG, 1342143), new UID((ushort)EManufacturer.DMXControlProjects_eV, 334412), new UID((ushort)EManufacturer.Martin_Professional_AS, 3123)]);
                if (type == typeof(RDMSelfTestDescription))
                    return new RDMSelfTestDescription(3);
                if (type == typeof(RDMSensorDefinition))
                    return new RDMSensorDefinition(3);
                if (type == typeof(RDMSlotDescription))
                    return new RDMSlotDescription(3);
                if (type == typeof(GetBrokerStatusResponse))
                    return new GetBrokerStatusResponse(true, ERDM_BrokerStatus.ACTIVE);
                if (type == typeof(GetSetComponentScope))
                    return new GetSetComponentScope(3, scopeString: "eqadeqew", new IPv4Address(123, 45, 22, 4));
                if (type == typeof(TCPCommsEntry))
                    return new TCPCommsEntry("eqadeqew");
                if (type == typeof(RDMCurveDescription))
                    return new RDMCurveDescription(3);
                if (type == typeof(RDMDimmerInfo))
                    return new RDMDimmerInfo(3, 200);
                if (type == typeof(RDMDMXBlockAddress))
                    return new RDMDMXBlockAddress(3, 200);
                if (type == typeof(RDMDMX_xxxx_Mode))
                    return new RDMDMX_xxxx_Mode(3, 200);
                if (type == typeof(SetLockPinRequest))
                    return new SetLockPinRequest(3, 200);
                if (type == typeof(RDMLockStateDescription))
                    return new RDMLockStateDescription(3);
                if (type == typeof(GetLockStateResponse))
                    return new GetLockStateResponse(3, 200);
                if (type == typeof(RDMMinimumLevel))
                    return new RDMMinimumLevel(3, 200);
                if (type == typeof(RDMModulationFrequencyDescription))
                    return new RDMModulationFrequencyDescription(3, 200);
                if (type == typeof(RDMModulationFrequency))
                    return new RDMModulationFrequency(3, 200);
                if (type == typeof(RDMOutputResponseTimeDescription))
                    return new RDMOutputResponseTimeDescription(3);
                if (type == typeof(RDMOutputResponseTime))
                    return new RDMOutputResponseTime(3, 200);
                if (type == typeof(SetLockStateRequest))
                    return new SetLockStateRequest(12314, 23);
                if (type == typeof(RDMPresetInfo))
                    return new RDMPresetInfo(true, true, true, true, true, true, 12354, 21567, 7432, 23467, 7632, 24567, 7532, 23456, ushort.MaxValue, 23456, 6543, ushort.MaxValue, 5432);
                if (type == typeof(GetInterfaceListResponse))
                    return new GetInterfaceListResponse(new InterfaceDescriptor(1, 444), new InterfaceDescriptor(4, 32));
                if (type == typeof(GetInterfaceNameResponse))
                    return new GetInterfaceNameResponse(2, "Test");
                if (type == typeof(GetHardwareAddressResponse))
                    return new GetHardwareAddressResponse(9, new MACAddress(12, 34, 56, 88, 55, 32));
                if (type == typeof(GetSetIPV4_xxx_Mode))
                    return new GetSetIPV4_xxx_Mode(2, true);
                if (type == typeof(GetIPv4CurrentAddressResponse))
                    return new GetIPv4CurrentAddressResponse(4, new IPv4Address(123, 45, 22, 4), 12, ERDM_DHCPStatusMode.ACTIVE);
                if (type == typeof(GetSetIPv4StaticAddress))
                    return new GetSetIPv4StaticAddress(3, new IPv4Address(123, 45, 22, 4), 20);
                if (type == typeof(GetSetIPv4DefaultRoute))
                    return new GetSetIPv4DefaultRoute(5, new IPv4Address(153, 49, 122, 234));
                if (type == typeof(GetSetIPv4NameServer))
                    return new GetSetIPv4NameServer(2, new IPv4Address(123, 45, 22, 4));
                if (type == typeof(GetEndpointListResponse))
                    return new GetEndpointListResponse(1, new EndpointDescriptor(4, ERDM_EndpointType.PHYSICAL), new EndpointDescriptor(7, ERDM_EndpointType.VIRTUAL));
                if (type == typeof(GetSetIdentifyEndpoint))
                    return new GetSetIdentifyEndpoint(3, true);
                if (type == typeof(GetSetEndpointToUniverse))
                    return new GetSetEndpointToUniverse(1, 55);
                if (type == typeof(GetSetEndpointMode))
                    return new GetSetEndpointMode(1, ERDM_EndpointMode.OUTPUT);
                if (type == typeof(GetSetEndpointLabel))
                    return new GetSetEndpointLabel(3, "Test Endpoint Label");
                if (type == typeof(GetSetEndpointRDMTrafficEnable))
                    return new GetSetEndpointRDMTrafficEnable(56, true);
                if (type == typeof(GetDiscoveryStateResponse))
                    return new GetDiscoveryStateResponse(1, 545, ERDM_DiscoveryState.NOT_ACTIVE);
                if (type == typeof(GetSetEndpointBackgroundDiscovery))
                    return new GetSetEndpointBackgroundDiscovery(1, true);
                if (type == typeof(GetEndpointTimingResponse))
                    return new GetEndpointTimingResponse(1, 44, 55);
                if (type == typeof(GetEndpointTimingDescriptionResponse))
                    return new GetEndpointTimingDescriptionResponse(1, "Test Timing");
                if (type == typeof(GetEndpointRespondersResponse))
                    return new GetEndpointRespondersResponse(1, [new UID((ushort)EManufacturer.Swisson_AG, 1342143), new UID((ushort)EManufacturer.DMXControlProjects_eV, 334412), new UID((ushort)EManufacturer.Martin_Professional_AS, 3123)]);
                if (type == typeof(GetEndpointResponderListChangeResponse))
                    return new GetEndpointResponderListChangeResponse(1, 55);
                if (type == typeof(GetBindingAndControlFieldsResponse))
                    return new GetBindingAndControlFieldsResponse(1, new UID((ushort)EManufacturer.Swisson_AG, 1342143), 22, new UID((ushort)EManufacturer.Martin_Professional_AS, 3123));
                if (type == typeof(GetBackgroundQueuedStatusPolicyResponse))
                    return new GetBackgroundQueuedStatusPolicyResponse(21, 55);
                if (type == typeof(GetBackgroundQueuedStatusPolicyDescriptionResponse))
                    return new GetBackgroundQueuedStatusPolicyDescriptionResponse(21, "Test QueuedStatusPolicyDescription");
                if (type == typeof(SetDiscoveryStateRequest))
                    return new SetDiscoveryStateRequest(22, ERDM_DiscoveryState.INCOMPLETE);
                if (type == typeof(SetEndpointTimingRequest))
                    return new SetEndpointTimingRequest(22, 34);
                if (type == typeof(GetBindingAndControlFieldsRequest))
                    return new GetBindingAndControlFieldsRequest(22, new UID((ushort)EManufacturer.Martin_Professional_AS, 3123));
                if (type == typeof(ERDM_DisplayInvert))
                    return ERDM_DisplayInvert.AUTO;
                if (type == typeof(ERDM_LampMode))
                    return ERDM_LampMode.ON_MODE_AFTER_CAL;
                if (type == typeof(ERDM_LampState))
                    return ERDM_LampState.STANDBY;
                if (type == typeof(ERDM_IdentifyMode))
                    return ERDM_IdentifyMode.LOUD;
                if (type == typeof(ERDM_PowerState))
                    return ERDM_PowerState.STANDBY;
                if (type == typeof(ERDM_ResetType))
                    return ERDM_ResetType.Warm;
                if (type == typeof(ERDM_Status))
                    return ERDM_Status.GET_LAST_MESSAGE;
                if (type == typeof(ERDM_MergeMode))
                    return ERDM_MergeMode.DMX_ONLY;
                if (type == typeof(ERDM_BrokerStatus))
                    return ERDM_BrokerStatus.ACTIVE;

                if (type == typeof(ERDM_Parameter))
                    return ERDM_Parameter.LAMP_HOURS;
                if (type == typeof(ERDM_Parameter[]))
                    return e1_20Parameters;
                if (type == typeof(RDMStatusMessage[]))
                    return new RDMStatusMessage[] { new RDMStatusMessage(1, ERDM_Status.ERROR, ERDM_StatusMessage.BREAKER_TRIP), new RDMStatusMessage(1, ERDM_Status.WARNING, ERDM_StatusMessage.WATTS) };
                if (type == typeof(RDMSlotInfo[]))
                    return new RDMSlotInfo[] { new RDMSlotInfo(1, ERDM_SlotType.PRIMARY, ERDM_SlotCategory.PAN), new RDMSlotInfo(2, ERDM_SlotType.SEC_FINE, ERDM_SlotCategory.PAN), new RDMSlotInfo(3, ERDM_SlotType.PRIMARY, ERDM_SlotCategory.TILT), new RDMSlotInfo(4, ERDM_SlotType.SEC_FINE, ERDM_SlotCategory.TILT) };
                if (type == typeof(ERDM_ProductDetail[]))
                    return new ERDM_ProductDetail[] { ERDM_ProductDetail.ANALOG_DEMULTIPLEX, ERDM_ProductDetail.BUBBLE, ERDM_ProductDetail.CONFETTI, ERDM_ProductDetail.CO2 };
                if (type == typeof(string[]))
                    return new string[] { "de", "en", "es" };
                if (type == typeof(RDMDefaultSlotValue[]))
                    return new RDMDefaultSlotValue[] { new RDMDefaultSlotValue(1, 128), new RDMDefaultSlotValue(2, 12), new RDMDefaultSlotValue(3, 44), };
                #region SGM
                if (type == typeof(RefreshRate))
                    return new RefreshRate((byte)34);
                if (type == typeof(EDimmingCurve))
                    return EDimmingCurve.GAMMA_CORRECTED;
                if (type == typeof(EFanMode))
                    return EFanMode.HIGH;
                if (type == typeof(EDimMode))
                    return EDimMode.MAX_POWER;
                if (type == typeof(EInvertPixelOrder))
                    return EInvertPixelOrder.INVERT;
                if (type == typeof(EBatteryExtension))
                    return EBatteryExtension._20H;
                #endregion

                return null!;
            }
        }

        private static string ParametersToString(params ERDM_Parameter[] parameters)
        {
            return String.Join(";", parameters);
        }
    }
}