using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class GetInterfaceListResponse : AbstractRDMPayloadObject
    {
        public GetInterfaceListResponse(params InterfaceDescriptor[] interfaces)
        {
            this.Interfaces = interfaces;
        }

        public InterfaceDescriptor[] Interfaces { get; private set; }
        public const int PDL_MIN = 0;
        public const int PDL_MAX = 0xE6;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("GetInterfaceListResponse");
            b.AppendLine($"Interfaces:");
            foreach (InterfaceDescriptor _interface in Interfaces)
                b.AppendLine(_interface.ToString());

            return b.ToString();
        }
        public static GetInterfaceListResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.LIST_INTERFACES) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static GetInterfaceListResponse FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            List<InterfaceDescriptor> _interfaces = new List<InterfaceDescriptor>();
            int pdl = 6;
            while (data.Length >= pdl)
            {
                var bytes = data.Take(pdl).ToArray();
                _interfaces.Add(InterfaceDescriptor.FromPayloadData(bytes));
                data = data.Skip(pdl).ToArray();
            }

            var i = new GetInterfaceListResponse(_interfaces.ToArray());

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            foreach (InterfaceDescriptor _interface in Interfaces)
                data.AddRange(_interface.ToPayloadData());
            return data.ToArray();
        }
    }
    public class InterfaceDescriptor : AbstractRDMPayloadObject
    {
        public InterfaceDescriptor(uint interfaceId = 0, ushort hardwareType = 0)
        {
            this.InterfaceId = interfaceId;
            this.HardwareType = hardwareType;
        }

        public uint InterfaceId { get; private set; }
        public ushort HardwareType { get; private set; }
        public const int PDL = 6;

        public override string ToString()
        {
            return $"Id: {InterfaceId} HardwareType: {HardwareType}";
        }
        public static InterfaceDescriptor FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new InterfaceDescriptor(
                interfaceId: Tools.DataToUInt(ref data),
                hardwareType: Tools.DataToUShort(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.HardwareType));
            return data.ToArray();
        }
    }
}