using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMProxiedDevices : AbstractRDMPayloadObject
    {
        public RDMProxiedDevices(params RDMUID[] devices)
        {
            this.Devices = devices;
        }

        public RDMUID[] Devices { get; private set; }
        public const int PDL_MIN = 0;
        public const int PDL_MAX = 0xE4;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMProxiedDevices");
            b.AppendLine($"Devices:");
            foreach (RDMUID device in Devices)
                b.AppendLine(device.ToString());

            return b.ToString();
        }
        public static RDMProxiedDevices FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.PROXIED_DEVICES, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMProxiedDevices FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            List<RDMUID> uids = new List<RDMUID>();
            while (data.Length >= 6)
                uids.Add(Tools.DataToRDMUID(ref data));

            var i = new RDMProxiedDevices(uids.ToArray());

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.Devices));
            return data.ToArray();
        }
    }
}