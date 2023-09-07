using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetIPV4_xxx_Mode : AbstractRDMPayloadObject
    {
        public GetSetIPV4_xxx_Mode(
            uint interfaceId = 0,
            bool enabled = false)
        {
            this.InterfaceId = interfaceId;
            this.Enabled = enabled;
        }

        public uint InterfaceId { get; private set; }
        public bool Enabled { get; private set; }
        public const int PDL = 5;

        public override string ToString()
        {
            return $"GetSetDHCPMode: {InterfaceId} - {Enabled}";
        }

        public static GetSetIPV4_xxx_Mode FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.IPV4_DHCP_MODE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPV4_xxx_Mode FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetIPV4_xxx_Mode(
                interfaceId: Tools.DataToUInt(ref data),
                enabled: Tools.DataToBool(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.Enabled));
            return data.ToArray();
        }
    }
}
