﻿using System.Collections.Generic;

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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.BROKER_STATUS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBrokerStatusResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetBrokerStatusResponse(
                setAllowed: Tools.DataToBool(ref data),
                brokerStatus: Tools.DataToEnum<ERDM_BrokerStatus>(ref data));

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