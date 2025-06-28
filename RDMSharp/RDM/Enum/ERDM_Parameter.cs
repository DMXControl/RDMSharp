using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    /// <summary>
    /// https://www.rdmprotocol.org/rdm/developers/developer-resources/
    /// </summary>
    [DataTreeEnum(ERDM_Parameter.SUPPORTED_PARAMETERS, Command.ECommandDublicate.GetResponse, "pid", true, "pids")]
    [DataTreeEnum(ERDM_Parameter.PARAMETER_DESCRIPTION, Command.ECommandDublicate.GetRequest, "pid")]
    [DataTreeEnum(ERDM_Parameter.METADATA_JSON, Command.ECommandDublicate.GetRequest, "pid")]
    [DataTreeEnum(ERDM_Parameter.METADATA_PARAMETER_VERSION, Command.ECommandDublicate.GetRequest, "pid")]
    public enum ERDM_Parameter : ushort
    {
        NONE = 0x0000,
        DISC_UNIQUE_BRANCH = 0x0001,
        DISC_MUTE = 0x0002,
        DISC_UN_MUTE = 0x0003,
        [ParameterGroup("Proxie")]
        PROXIED_DEVICES = 0x0010,
        [ParameterGroup("Proxie")]
        PROXIED_DEVICES_COUNT = 0x0011,
        COMMS_STATUS = 0x0015,

        [ParameterGroup("Status")]
        QUEUED_MESSAGE = 0x0020,
        [ParameterGroup("Status")]
        STATUS_MESSAGES = 0x0030,
        [ParameterGroup("Status")]
        STATUS_ID_DESCRIPTION = 0x0031,
        [ParameterGroup("Status")]
        CLEAR_STATUS_ID = 0x0032,
        [ParameterGroup("Status")]
        SUB_DEVICE_STATUS_REPORT_THRESHOLD = 0x0033,

        [ParameterGroup("Info")]
        SUPPORTED_PARAMETERS = 0x0050,
        [ParameterGroup("Info")]
        PARAMETER_DESCRIPTION = 0x0051,

        [ParameterGroup("Info")]
        DEVICE_INFO = 0x0060,
        [ParameterGroup("Info")]
        PRODUCT_DETAIL_ID_LIST = 0x0070,
        [ParameterGroup("Info")]
        DEVICE_MODEL_DESCRIPTION = 0x0080,
        [ParameterGroup("Info")]
        MANUFACTURER_LABEL = 0x0081,
        [ParameterGroup("Info")]
        DEVICE_LABEL = 0x0082,
        [ParameterGroup("Control")]
        FACTORY_DEFAULTS = 0x0090,
        [ParameterGroup("Control")]
        LANGUAGE_CAPABILITIES = 0x00A0,
        [ParameterGroup("Control")]
        LANGUAGE = 0x00B0,
        [ParameterGroup("Firmware")]
        SOFTWARE_VERSION_LABEL = 0x00C0,
        [ParameterGroup("Firmware")]
        BOOT_SOFTWARE_VERSION_ID = 0x00C1,
        [ParameterGroup("Firmware")]
        BOOT_SOFTWARE_VERSION_LABEL = 0x00C2,

        [ParameterGroup("DMX")]
        DMX_PERSONALITY = 0x00E0,
        [ParameterGroup("DMX")]
        DMX_PERSONALITY_DESCRIPTION = 0x00E1,
        [ParameterGroup("DMX")]
        DMX_START_ADDRESS = 0x00F0,
        [ParameterGroup("Slots")]
        SLOT_INFO = 0x0120,
        [ParameterGroup("Slots")]
        SLOT_DESCRIPTION = 0x0121,
        [ParameterGroup("Slots")]
        DEFAULT_SLOT_VALUE = 0x0122,

        [ParameterGroup("Sensors")]
        SENSOR_DEFINITION = 0x0200,
        [ParameterGroup("Sensors")]
        SENSOR_VALUE = 0x0201,
        [ParameterGroup("Sensors")]
        RECORD_SENSORS = 0x0202,

        [ParameterGroup("Info")]
        DEVICE_HOURS = 0x0400,
        [ParameterGroup("Lamp")]
        LAMP_HOURS = 0x0401,
        [ParameterGroup("Lamp")]
        LAMP_STRIKES = 0x0402,
        [ParameterGroup("Lamp")]
        LAMP_STATE = 0x0403,
        [ParameterGroup("Lamp")]
        LAMP_ON_MODE = 0x0404,
        [ParameterGroup("Info")]
        DEVICE_POWER_CYCLES = 0x0405,

        [ParameterGroup("Display")]
        DISPLAY_INVERT = 0x0500,
        [ParameterGroup("Display")]
        DISPLAY_LEVEL = 0x0501,

        [ParameterGroup("Pan/Tilt")]
        PAN_INVERT = 0x0600,
        [ParameterGroup("Pan/Tilt")]
        TILT_INVERT = 0x0601,
        [ParameterGroup("Pan/Tilt")]
        PAN_TILT_SWAP = 0x0602,
        REAL_TIME_CLOCK = 0x0603,

        [ParameterGroup("Control")]
        IDENTIFY_DEVICE = 0x1000,
        [ParameterGroup("Control")]
        RESET_DEVICE = 0x1001,
        [ParameterGroup("Control")]
        POWER_STATE = 0x1010,
        [ParameterGroup("Control")]
        PERFORM_SELFTEST = 0x1020,
        [ParameterGroup("Control")]
        SELF_TEST_DESCRIPTION = 0x1021,
        [ParameterGroup("Preset")]
        CAPTURE_PRESET = 0x1030,
        [ParameterGroup("Preset")]
        PRESET_PLAYBACK = 0x1031,



        //E1.37-1 - 2012
        [ParameterGroup("DMX")]
        DMX_BLOCK_ADDRESS = 0x0140,
        [ParameterGroup("DMX")]
        DMX_FAIL_MODE = 0x0141,
        [ParameterGroup("DMX")]
        DMX_STARTUP_MODE = 0x0142,

        [ParameterGroup("Dimming")]
        DIMMER_INFO = 0x0340,
        [ParameterGroup("Dimming")]
        MINIMUM_LEVEL = 0x0341,
        [ParameterGroup("Dimming")]
        MAXIMUM_LEVEL = 0x0342,
        [ParameterGroup("Dimming")]
        CURVE = 0x0343,
        [ParameterGroup("Dimming")]
        CURVE_DESCRIPTION = 0x0344,
        [ParameterGroup("Dimming")]
        OUTPUT_RESPONSE_TIME = 0x0345,
        [ParameterGroup("Dimming")]
        OUTPUT_RESPONSE_TIME_DESCRIPTION = 0x0346,
        [ParameterGroup("Dimming")]
        MODULATION_FREQUENCY = 0x0347,
        [ParameterGroup("Dimming")]
        MODULATION_FREQUENCY_DESCRIPTION = 0x0348,

        BURN_IN = 0x0440,

        [ParameterGroup("Lock")]
        LOCK_PIN = 0x0640,
        [ParameterGroup("Lock")]
        LOCK_STATE = 0x0641,
        [ParameterGroup("Lock")]
        LOCK_STATE_DESCRIPTION = 0x0642,

        [ParameterGroup("Control")]
        IDENTIFY_MODE = 0x1040,
        [ParameterGroup("Preset")]
        PRESET_INFO = 0x1041,
        [ParameterGroup("Preset")]
        PRESET_STATUS = 0x1042,
        [ParameterGroup("Preset")]
        PRESET_MERGEMODE = 0x1043,
        [ParameterGroup("Control")]
        POWER_ON_SELF_TEST = 0x1044,

        //E1.37-2 - 2015
        [ParameterGroup("Network")]
        LIST_INTERFACES = 0x0700,
        [ParameterGroup("Network")]
        INTERFACE_LABEL = 0x0701,
        [ParameterGroup("Network")]
        INTERFACE_HARDWARE_ADDRESS_TYPE = 0x0702,
        [ParameterGroup("Network")]
        IPV4_DHCP_MODE = 0x0703,
        [ParameterGroup("Network")]
        IPV4_ZEROCONF_MODE = 0x0704,
        [ParameterGroup("Network")]
        IPV4_CURRENT_ADDRESS = 0x0705,
        [ParameterGroup("Network")]
        IPV4_STATIC_ADDRESS = 0x0706,
        [ParameterGroup("Network")]
        INTERFACE_RENEW_DHCP = 0x0707,
        [ParameterGroup("Network")]
        INTERFACE_RELEASE_DHCP = 0x0708,
        [ParameterGroup("Network")]
        INTERFACE_APPLY_CONFIGURATION = 0x0709,
        [ParameterGroup("Network")]
        IPV4_DEFAULT_ROUTE = 0x070A,
        [ParameterGroup("Network")]
        DNS_IPV4_NAME_SERVER = 0x070B,
        [ParameterGroup("Network")]
        DNS_HOSTNAME = 0x070C,
        [ParameterGroup("Network")]
        DNS_DOMAIN_NAME = 0x070D,

        //E1.33 - 2019 RDM-Net
        [ParameterGroup("E1.33")]
        COMPONENT_SCOPE = 0x0800,
        [ParameterGroup("E1.33")]
        SEARCH_DOMAIN = 0x0801,
        [ParameterGroup("E1.33")]
        TCP_COMMS_STATUS = 0x0802,
        [ParameterGroup("E1.33")]
        BROKER_STATUS = 0x0803,

        //E1.37-7 - 2019
        [ParameterGroup("E1.37-7")]
        ENDPOINT_LIST = 0x0900,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_LIST_CHANGE = 0x0901,
        [ParameterGroup("E1.37-7")]
        IDENTIFY_ENDPOINT = 0x0902,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_TO_UNIVERSE = 0x0903,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_MODE = 0x0904,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_LABEL = 0x0905,
        [ParameterGroup("E1.37-7")]
        RDM_TRAFFIC_ENABLE = 0x0906,
        [ParameterGroup("E1.37-7")]
        DISCOVERY_STATE = 0x0907,
        [ParameterGroup("E1.37-7")]
        BACKGROUND_DISCOVERY = 0x0908,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_TIMING = 0x0909,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_TIMING_DESCRIPTION = 0x090A,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_RESPONDERS = 0x090B,
        [ParameterGroup("E1.37-7")]
        ENDPOINT_RESPONDER_LIST_CHANGE = 0x090C,
        [ParameterGroup("E1.37-7")]
        BINDING_CONTROL_FIELDS = 0x090D,
        [ParameterGroup("E1.37-7")]
        BACKGROUND_QUEUED_STATUS_POLICY = 0x090E,
        [ParameterGroup("E1.37-7")]
        BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION = 0x090F,

        //E1.37-5 - 2024
        [ParameterGroup("URL")]
        MANUFACTURER_URL = 0x00D0,
        [ParameterGroup("URL")]
        PRODUCT_URL = 0x00D1,
        [ParameterGroup("URL")]
        FIRMWARE_URL = 0x00D2,
        [ParameterGroup("Info")]
        SERIAL_NUMBER = 0x00D3,
        [ParameterGroup("Info")]
        DEVICE_INFO_OFFSTAGE = 0x00D4,
        [ParameterGroup("Control")]
        TEST_DATA = 0x0016,
        [ParameterGroup("Control")]
        COMMS_STATUS_NSC = 0x0017,
        [ParameterGroup("Control")]
        IDENTIFY_TIMEOUT = 0x1050,
        [ParameterGroup("Control")]
        POWER_OFF_READY = 0x1051,
        [ParameterGroup("Control")]
        SHIPPING_LOCK = 0x0650,
        [ParameterGroup("Tag")]
        LIST_TAGS = 0x0651,
        [ParameterGroup("Tag")]
        ADD_TAG = 0x0652,
        [ParameterGroup("Tag")]
        REMOVE_TAG = 0x0653,
        [ParameterGroup("Tag")]
        CHECK_TAG = 0x0654,
        [ParameterGroup("Tag")]
        CLEAR_TAGS = 0x0655,
        [ParameterGroup("Info")]
        DEVICE_UNIT_NUMBER = 0x0656,
        [ParameterGroup("DMX")]
        DMX_PERSONALITY_ID = 0x00E2,
        [ParameterGroup("Sensor")]
        SENSOR_TYPE_CUSTOM = 0x0210,
        [ParameterGroup("Sensor")]
        SENSOR_UNIT_CUSTOM = 0x0211,
        [ParameterGroup("Metadata")]
        METADATA_PARAMETER_VERSION = 0x0052,
        [ParameterGroup("Metadata")]
        METADATA_JSON = 0x0053,
        [ParameterGroup("Metadata")]
        METADATA_JSON_URL = 0x0054,

        ////SGM specific IDs
        //SERIAL_NUMBER_SGM = 0x8060,
        //REFRESH_RATE = 0x8620,
        ////=0x8621,
        ////=0x8622,
        //DIMMING_CURVE = 0x8623,
        ////=0x8624,
        //FAN_MODE = 0x8625,
        //CRMX_LOG_OFF = 0x8626,
        ////=0x8627,
        //DIM_MODE = 0x8628,
        //INVERT_PIXEL_ORDER = 0x8629,
        //NORTH_CALIBRATION = 0x8630,
        //BATTERY_EXTENSION = 0x8631,
        //SMPS_CALIBRATION = 0x8632,
        //WIRELESS_DMX = 0x8633,
        ////=3635,
        ////ACTIVE_ERROR = 0x8636,
        //CRMX_BRIDGE_MODE = 0x8637
    }
}
