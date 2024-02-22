using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class GetEndpointListResponse : AbstractRDMPayloadObject
    {
        public GetEndpointListResponse(uint listChangedNumber = 0, params EndpointDescriptor[] endpoints)
        {
            this.ListChangedNumber = listChangedNumber;
            this.Endpoints = endpoints;
        }
        /// <summary>
        /// The Endpoint List Change Number is a monotonically increasing number used by controllers to
        /// track that the list of Endpoints has changed, or that an Endpoint Type has changed.This Change
        /// Number shall be incremented by one each time the set of Endpoints change. The Change
        /// Number is an unsigned 32-bit field which shall roll over from 0xFFFFFFFF to 0. Upon start-up
        /// (due to power-on reset, start of software, etc) this field shall be initialized to 0.
        /// </summary>
        public uint ListChangedNumber { get; private set; }
        public EndpointDescriptor[] Endpoints { get; private set; }
        public const int PDL_MIN = 0x07;
        public const int PDL_MAX = 0xE5;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("GetEndpointListResponse");
            b.AppendLine($"ListChangedNumber: {ListChangedNumber.ToString("X")}");
            b.AppendLine($"Endpoints:");
            foreach (EndpointDescriptor _interface in Endpoints)
                b.AppendLine(_interface.ToString());

            return b.ToString();
        }
        public static GetEndpointListResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_LIST) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointListResponse FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            uint listChangedNumber = Tools.DataToUInt(ref data);

            List<EndpointDescriptor> _endpoints = new List<EndpointDescriptor>();
            int pdl = EndpointDescriptor.PDL;
            while (data.Length >= pdl)
            {
                var bytes = data.Take(pdl).ToArray();
                _endpoints.Add(EndpointDescriptor.FromPayloadData(bytes));
                data = data.Skip(pdl).ToArray();
            }

            var i = new GetEndpointListResponse(listChangedNumber, _endpoints.ToArray());

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ListChangedNumber));

            foreach (EndpointDescriptor _interface in Endpoints)
                data.AddRange(_interface.ToPayloadData());

            return data.ToArray();
        }
    }
    public class EndpointDescriptor : AbstractRDMPayloadObject
    {
        public EndpointDescriptor(ushort endpointId = 0, ERDM_EndpointType endpointType = 0)
        {
            this.EndpointId = endpointId;
            this.EndpointType = endpointType;
        }

        public ushort EndpointId { get; private set; }
        public ERDM_EndpointType EndpointType { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            return $"Id: {EndpointId} EndpointType: {EndpointType}";
        }
        public static EndpointDescriptor FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new EndpointDescriptor(
                endpointId: Tools.DataToUShort(ref data),
                endpointType: Tools.DataToEnum<ERDM_EndpointType>(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.EndpointType));
            return data.ToArray();
        }
    }
}