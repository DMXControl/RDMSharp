using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetBrokerStatusResponse : AbstractRDMPayloadObject
    {
        public GetBrokerStatusResponse(
            bool setAllowed = default,
            ERDM_BrokerStatus brokerStatus = default)
        {
            this.SetAllowed = setAllowed;
            this.BrokerStatus = brokerStatus;
        }

        public bool SetAllowed { get; private set; }
        public ERDM_BrokerStatus BrokerStatus { get; private set; }
        public const int PDL = 0x02;

        public override string ToString()
        {
            return $"SetAllowed: {SetAllowed} - BrokerStatus: {BrokerStatus}";
        }

        public static GetBrokerStatusResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.BROKER_STATUS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBrokerStatusResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetBrokerStatusResponse(
                setAllowed: Tools.DataToBool(ref data),
                brokerStatus: Tools.DataToEnum<ERDM_BrokerStatus>(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SetAllowed));
            data.AddRange(Tools.ValueToData(this.BrokerStatus));
            return data.ToArray();
        }
    }
}