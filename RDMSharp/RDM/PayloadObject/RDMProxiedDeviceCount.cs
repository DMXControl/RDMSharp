using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.PROXIED_DEVICES_COUNT, Command.ECommandDublicate.GetResponse)]
    public class RDMProxiedDeviceCount : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMProxiedDeviceCount(
            [DataTreeObjectParameter("device_count")] ushort deviceCount = 0,
            [DataTreeObjectParameter("list_change")] bool listChange = false)
        {
            this.DeviceCount = deviceCount;
            this.ListChange = listChange;
        }

        [DataTreeObjectProperty("device_count", 0)]
        public ushort DeviceCount { get; private set; }
        [DataTreeObjectProperty("list_change", 1)]
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