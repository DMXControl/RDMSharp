using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMProxiedDeviceCount : AbstractRDMPayloadObject
    {
        public RDMProxiedDeviceCount(
            ushort deviceCount = 0,
            bool listChange = false)
        {
            this.DeviceCount = deviceCount;
            this.ListChange = listChange;
        }

        public ushort DeviceCount { get; private set; }
        public bool ListChange { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMProxiedDeviceCount");
            b.AppendLine($"DeviceCount:      {DeviceCount}");
            b.AppendLine($"ListChange:         {ListChange}");

            return b.ToString();
        }

        public static RDMProxiedDeviceCount FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.PROXIED_DEVICES_COUNT) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMProxiedDeviceCount FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMProxiedDeviceCount(
                deviceCount: Tools.DataToUShort(ref data),
                listChange: Tools.DataToBool(ref data)
                );

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.DeviceCount));
            data.AddRange(Tools.ValueToData(this.ListChange));
            return data.ToArray();
        }
    }
}