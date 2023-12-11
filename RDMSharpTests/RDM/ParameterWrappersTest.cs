using RDMSharp;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RDMSharp.ParameterWrapper;

namespace RDMSharpTest.RDM
{
    public class ParameterWrappersTest
    {
        private static ERDM_Parameter[] notUsableParameters = new ERDM_Parameter[]
           {
               ERDM_Parameter.NONE,
               ERDM_Parameter.DISC_MUTE,
               ERDM_Parameter.DISC_UNIQUE_BRANCH,
               ERDM_Parameter.DISC_UN_MUTE
           };
        private static ERDM_Parameter[] e1_20Parameters = new ERDM_Parameter[]
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
        private static ERDM_Parameter[] e1_37_1Parameters = new ERDM_Parameter[]
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
        private static ERDM_Parameter[] e1_37_2Parameters = new ERDM_Parameter[]
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
        private static ERDM_Parameter[] e1_37_7Parameters = new ERDM_Parameter[]
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
        private static ERDM_Parameter[] e1_33Parameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.COMPONENT_SCOPE,
            ERDM_Parameter.SEARCH_DOMAIN,
            ERDM_Parameter.TCP_COMMS_STATUS,
            ERDM_Parameter.BROKER_STATUS
        };
        private static ERDM_Parameter[] sgmParameters = new ERDM_Parameter[]
        {
            ERDM_Parameter.SERIAL_NUMBER,
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
            var parameterLeft = parameters.Except(e1_20Parameters).Except(e1_37_1Parameters).Except(e1_37_2Parameters).Except(e1_37_7Parameters).Except(e1_33Parameters).Except(sgmParameters).ToList();

            Assert.That(parameterLeft.Count, Is.EqualTo(notUsableParameters.Length));
            Assert.That(notUsableParameters.Except(parameterLeft).ToList().Count, Is.EqualTo(0));
        }
        [Test]
        public void CheckE1_20WrappersDefined()
        {
            var notDefinedParameters = e1_20Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters.Length, Is.EqualTo(0), $"The not defined Parameters:{Environment.NewLine}{ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_1WrappersDefined()
        {
            var notDefinedParameters = e1_37_1Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters.Length, Is.EqualTo(0), $"The not defined Parameters:{Environment.NewLine}{ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_2WrappersDefined()
        {
            var notDefinedParameters = e1_37_2Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters.Length, Is.EqualTo(0), $"The not defined Parameters:{Environment.NewLine}{ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_37_7WrappersDefined()
        {
            var notDefinedParameters = e1_37_7Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters.Length, Is.EqualTo(0), $"The not defined Parameters:{Environment.NewLine}{ParametersToString(notDefinedParameters)}");
        }
        [Test]
        public void CheckE1_33WrappersDefined()
        {
            var notDefinedParameters = e1_33Parameters.Except(parameterWrappers.Select(pw => pw.Parameter)).ToArray();
            Assert.That(notDefinedParameters.Length, Is.EqualTo(0), $"The not defined Parameters:{Environment.NewLine}{ParametersToString(notDefinedParameters)}");
        }

        [Test]
        public void AssemblyListenerTest()
        {
            foreach (IRDMParameterWrapper pW in parameterWrappers)
            {
                Assert.That(pW.Name.EndsWith(" "), Is.False, $"{pW.Name} Name is not Vaild");
                Assert.That(pW.Name.StartsWith(" "), Is.False, $"{pW.Name} Name is not Vaild");
                Assert.That(pW.Description.EndsWith(" "), Is.False, $"{pW.Name} Description is not Vaild");
                Assert.That(pW.Description.StartsWith(" "), Is.False, $"{pW.Name} Description is not Vaild");
                Assert.That(pW.Description.EndsWith("."), Is.True, $"{pW.Name} Description should end with a \".\"");
            }

            foreach (ERDM_Parameter parameter in e1_20Parameters)
                Assert.That(parameterWrappers.Count(pw => pw.Parameter == parameter), Is.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

            foreach (ERDM_Parameter parameter in e1_37_1Parameters)
                Assert.That(parameterWrappers.Count(pw => pw.Parameter == parameter), Is.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

            foreach (ERDM_Parameter parameter in e1_37_2Parameters)
                Assert.That(parameterWrappers.Count(pw => pw.Parameter == parameter), Is.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

            foreach (ERDM_Parameter parameter in e1_37_7Parameters)
                Assert.That(parameterWrappers.Count(pw => pw.Parameter == parameter), Is.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

            foreach (ERDM_Parameter parameter in e1_33Parameters)
                Assert.That(parameterWrappers.Count(pw => pw.Parameter == parameter), Is.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");
            
            foreach (ERDM_Parameter parameter in sgmParameters)
                Assert.That(parameterWrappers.Count(pw => pw.Parameter == parameter), Is.EqualTo(1), $"There are more then one ParameterWrapper for the Parameter: {parameter}");

            Assert.That(e1_20Parameters.Length + e1_37_1Parameters.Length + e1_37_2Parameters.Length + e1_37_7Parameters.Length + e1_33Parameters.Length + sgmParameters.Length, Is.EqualTo(parameterWrappers.Count));
        }

        private string ParametersToString(params ERDM_Parameter[] parameters)
        {
            StringBuilder b = new StringBuilder();
            foreach (ERDM_Parameter p in parameters)
                b.AppendLine(p.ToString());

            return b.ToString();
        }
    }
}