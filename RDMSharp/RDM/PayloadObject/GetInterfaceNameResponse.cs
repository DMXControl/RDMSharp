using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetInterfaceNameResponse : AbstractRDMPayloadObject
    {
        public GetInterfaceNameResponse(
            uint interfaceId = 0,
            string label = "")
        {
            this.InterfaceId = interfaceId;

            if (string.IsNullOrWhiteSpace(label))
                return;

            if (label.Length > 32)
                label = label.Substring(0, 32);

            this.Label = label;
        }

        public uint InterfaceId { get; private set; }
        public string Label { get; private set; }
        public const int PDL_MIN = 1;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"GetInterfaceNameResponse: {InterfaceId} - {Label}";
        }

        public static GetInterfaceNameResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.INTERFACE_LABEL) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static GetInterfaceNameResponse FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var i = new GetInterfaceNameResponse(
                interfaceId: Tools.DataToUInt(ref data),
                label: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.Label, 32));
            return data.ToArray();
        }
    }
}
