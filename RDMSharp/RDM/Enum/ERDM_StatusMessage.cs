using System;

namespace RDMSharp
{
    public enum ERDM_StatusMessage : ushort
    {
        [StatusMessageAttribute("{0} failed calibration.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        CAL_FAIL = 0x0001,

        [StatusMessageAttribute("{0} sensor not found.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        SENS_NOT_FOUND = 0x0002,

        [StatusMessageAttribute("{0} sensor always on.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        SENS_ALWAYS_ON = 0x0003,
        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions

        [StatusMessageAttribute("{0} feedback error.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        FEEDBACK_ERROR = 0x0004,

        [StatusMessageAttribute("{0} index circuit error.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        INDEX_ERROR = 0x0005,
        #endregion

        [StatusMessageAttribute("Lamp doused.")]
        LAMP_DOUSED = 0x0011,
        [StatusMessageAttribute("Lamp failed to strike.")]
        LAMP_STRIKE = 0x0012,
        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [StatusMessageAttribute("Lamp access open.")]
        LAMP_ACCESS_OPEN = 0x0013,
        [StatusMessageAttribute("Lamp on without command.")]
        LAMP_ALWAYS_ON = 0x0014,
        #endregion

        [StatusMessageAttribute("Sensor {0} over temp at {1} °C.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        OVERTEMP = 0x0021,
        [StatusMessageAttribute("Sensor {0} under temp at {1} °C.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        UNDERTEMP = 0x0022,
        [StatusMessageAttribute("Sensor {0} out of range.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        SENS_OUT_RANGE = 0x0023,
        
        [StatusMessageAttribute("Phase {0} over voltage at {1} V.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        OVERVOLTAGE_PHASE = 0x0031,
        [StatusMessageAttribute("Phase {0} under voltage at {1} V.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        UNDERVOLTAGE_PHASE = 0x0032,
        [StatusMessageAttribute("Phase {0} over current at {1} A.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        OVERCURRENT = 0x0033,
        [StatusMessageAttribute("Phase {0} under current at {1} A.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        UNDERCURRENT = 0x0034,
        [StatusMessageAttribute("Phase {0} is at {1} degrees.", StatusMessageAttribute.EDataValueFormat.DecimalNumber, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        PHASE = 0x0035,
        [StatusMessageAttribute("Phase {0} Error.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        PHASE_ERROR = 0x0036,
        [StatusMessageAttribute("{0} A.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        AMPS = 0x0037,
        [StatusMessageAttribute("{0} V.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        VOLTS = 0x0038,

        [StatusMessageAttribute("No Dimmer.")]
        DIMSLOT_OCCUPIED = 0x0041,
        [StatusMessageAttribute("Tripped Breaker.")]
        BREAKER_TRIP = 0x0042,
        [StatusMessageAttribute("{0} W.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        WATTS = 0x0043,
        [StatusMessageAttribute("Dimmer Failure.")]
        DIM_FAILURE = 0x0044,
        [StatusMessageAttribute("Panic Mode.")]
        DIM_PANIC = 0x0045,
        # region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [StatusMessageAttribute("Lamp or cable failure.")]
        LOAD_FAILURE = 0x0046,
        #endregion

        [StatusMessageAttribute("{0} ready.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        READY = 0x0050,
        [StatusMessageAttribute("{0} not ready.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        NOT_READY = 0x0051,
        [StatusMessageAttribute("{0} low fluid.", StatusMessageAttribute.EDataValueFormat.SlotLabelCode)]
        LOW_FLUID = 0x0052,

        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [StatusMessageAttribute("EEPROM error.")]
        EEPROM_ERROR = 0x0060,
        [StatusMessageAttribute("RAM error.")]
        RAM_ERROR = 0x0061,
        [StatusMessageAttribute("FPGA programming error.")]
        FPGA_ERROR = 0x0062,
        #endregion

        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [StatusMessageAttribute("Proxy Drop: PID {0} at Transaction Number {1}.", StatusMessageAttribute.EDataValueFormat.ParameterID, StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        PROXY_BROADCAST_DROPPED = 0x0070,
        [StatusMessageAttribute("DMX ASC {0} received OK.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        ASC_RXOK = 0x0071,
        [StatusMessageAttribute("DMX ASC {0} received dropped.", StatusMessageAttribute.EDataValueFormat.DecimalNumber)]
        ASC_DROPPED = 0x0072,
        #endregion

        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [StatusMessageAttribute("DMX NSC never received.")]
        DMXNSCNONE = 0x0080,
        [StatusMessageAttribute("DMX NSC received, now dropped.")]
        DMXNSCLOSS = 0x0081,
        [StatusMessageAttribute("DMX NSC timing, or packet error.")]
        DMXNSCERROR = 0x0082,
        [StatusMessageAttribute("DMX NSC received OK.")]
        DMXNSC_OK = 0x0083,
        #endregion
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StatusMessageAttribute : Attribute
    {
        public enum EDataValueFormat
        {
            NotUsed,
            DecimalNumber,
            SlotLabelCode,
            ParameterID
        }
        public readonly string Message;
        public readonly EDataValueFormat DataValue1 = EDataValueFormat.NotUsed;
        public readonly EDataValueFormat DataValue2 = EDataValueFormat.NotUsed;
        public StatusMessageAttribute(string message)
        {
            Message = message;
        }
        public StatusMessageAttribute(string message, EDataValueFormat dataValue1) : this(message)
        {
            DataValue1 = dataValue1;
        }
        public StatusMessageAttribute(string message, EDataValueFormat dataValue1, EDataValueFormat dataValue2) : this(message, dataValue1)
        {
            DataValue2 = dataValue2;
        }

        public string GetFormatedString(short? dataValue1 = null, short? dataValue2 = null)
        {
            string formatedDataValue1 = null;
            string formatedDataValue2 = null;
            if (DataValue1 != EDataValueFormat.NotUsed && dataValue1 == null)
                throw new ArgumentNullException(nameof(dataValue1));
            if (DataValue2 != EDataValueFormat.NotUsed && dataValue2 == null)
                throw new ArgumentNullException(nameof(dataValue2));

            if (DataValue1 != EDataValueFormat.NotUsed)
                formatedDataValue1 = getFormatedValue(DataValue1, dataValue1.Value);
            if (DataValue2 != EDataValueFormat.NotUsed)
                formatedDataValue2 = getFormatedValue(DataValue2, dataValue2.Value);


            if (DataValue2 != EDataValueFormat.NotUsed)
                return string.Format(this.Message, formatedDataValue1, formatedDataValue2);
            if (DataValue1 != EDataValueFormat.NotUsed)
                return string.Format(this.Message, formatedDataValue1);

            return Message;
        }

        //Local Functions

        private static string getFormatedValue(EDataValueFormat format, short s)
        {
            switch (format)
            {
                case EDataValueFormat.SlotLabelCode:
                    return SlotLabelCode(s);
                case EDataValueFormat.ParameterID:
                    return ParameterID(s);
                case EDataValueFormat.DecimalNumber:
                default:
                    return DecimalNumber(s).ToString();

            }
        }

        private static short DecimalNumber(short d) => d;
        private static string SlotLabelCode(short s)
        {
            ERDM_SlotCategory sc = (ERDM_SlotCategory)(ushort)s;
            return Tools.GetEnumDescription(sc);
        }
        private static string ParameterID(short s)
        {
            ERDM_Parameter pid = (ERDM_Parameter)(ushort)DecimalNumber(s);
            return $"0x{(ushort)pid:X4}({pid})";
        }
    }
}
