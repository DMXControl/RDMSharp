﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.SENSOR_UNIT_CUSTOM, Command.ECommandDublicte.GetResponse)]
    public class RDMSensorUnitCustomDefine : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        [DataTreeObjectConstructor]
        public RDMSensorUnitCustomDefine(
            [DataTreeObjectParameter("sensor_unit")] byte id,
            [DataTreeObjectParameter("label")] string label)
        {
            this.Id = id;
            this.Label = label;
        }

        public byte Id { get; private set; }
        public string Label { get; private set; }

        public object MinIndex => (byte)0x80;
        public object Index => Id;

        public const int PDL_MIN = 0x01;
        public const int PDL_MAX = 0x21;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMSensorUnitCustomDefine");
            b.AppendLine($"Id:    0x{Id:X2}");
            b.AppendLine($"Label: {Label}");

            return b.ToString();
        }

        public static RDMSensorUnitCustomDefine FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SENSOR_UNIT_CUSTOM, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSensorUnitCustomDefine FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var id = Tools.DataToByte(ref data);
            var label = Tools.DataToString(ref data, 32);

            var i = new RDMSensorUnitCustomDefine(
                id: id,
                label: label
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.Id));
            data.AddRange(Tools.ValueToData(this.Label));
            return data.ToArray();
        }
    }
}
