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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.PROXIED_DEVICES_COUNT, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMProxiedDeviceCount FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMProxiedDeviceCount(
                deviceCount: Tools.DataToUShort(ref data),
                listChange: Tools.DataToBool(ref data)
                );

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