using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class GetEndpointRespondersResponse : AbstractRDMPayloadObject
    {
        public GetEndpointRespondersResponse(uint listChangedNumber = 0, params RDMUID[] uids)
        {
            this.ListChangedNumber = listChangedNumber;
            this.UIDs = uids;
        }
        /// <summary>
        /// The Endpoint List Change Number is a monotonically increasing number used by controllers to
        /// track that the list of Endpoints has changed, or that an Endpoint Type has changed.This Change
        /// Number shall be incremented by one each time the set of Endpoints change. The Change
        /// Number is an unsigned 32-bit field which shall roll over from 0xFFFFFFFF to 0. Upon start-up
        /// (due to power-on reset, start of software, etc) this field shall be initialized to 0.
        /// </summary>
        public uint ListChangedNumber { get; private set; }
        public RDMUID[] UIDs { get; private set; }
        public const int PDL_MIN = 0x07;
        public const int PDL_MAX = 0xE5;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("GetEndpointRespondersResponse");
            b.AppendLine($"ListChangedNumber: {ListChangedNumber.ToString("X")}");
            b.AppendLine($"UIDs:");
            foreach (RDMUID uid in UIDs)
                b.AppendLine(uid.ToString());

            return b.ToString();
        }
        public static GetEndpointRespondersResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_RESPONDERS, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointRespondersResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            uint listChangedNumber = Tools.DataToUInt(ref data);

            List<RDMUID> uids = new List<RDMUID>();
            while (data.Length >= 6)
                uids.Add(Tools.DataToRDMUID(ref data));

            var i = new GetEndpointRespondersResponse(listChangedNumber, uids.ToArray());

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ListChangedNumber));

            data.AddRange(Tools.ValueToData(this.UIDs));

            return data.ToArray();
        }
    }
}