using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    //[DataTreeObject(ERDM_Parameter.PROXIED_DEVICES, Command.ECommandDublicte.GetResponse, path: "device_uids")] Dont use this because its an Arrya and this is handled faster
    public class RDMProxiedDevices : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMProxiedDevices([DataTreeObjectParameter("device_uid", true)] params UID[] devices)
        {
            this.Devices = devices;
        }

        public UID[] Devices { get; private set; }
        public const int PDL_MIN = 0;
        public const int PDL_MAX = 0xE4;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMProxiedDevices");
            b.AppendLine($"Devices:");
            foreach (UID device in Devices)
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

            List<UID> uids = new List<UID>();
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