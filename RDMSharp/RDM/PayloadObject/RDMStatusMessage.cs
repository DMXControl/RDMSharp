using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.STATUS_MESSAGES, Command.ECommandDublicte.GetResponse, true, "slots")]
    public class RDMStatusMessage : AbstractRDMPayloadObject
    {
        public RDMStatusMessage(
            ushort subDeviceId = 0,
            ERDM_Status statusType = (ERDM_Status)0,
            ERDM_StatusMessage statusMessage = (ERDM_StatusMessage)0,
            short dataValue1 = 0,
            short dataValue2 = 0)
        {
            this.SubDeviceId = subDeviceId;
            this.StatusType = statusType;
            this.StatusMessage = statusMessage;
            this.DataValue1 = dataValue1;
            this.DataValue2 = dataValue2;
        }

        [DataTreeObjectConstructor]
        public RDMStatusMessage(
            [DataTreeObjectParameter("slots/subdevice_id")] ushort subDeviceId,
            [DataTreeObjectParameter("slots/status_type")] byte statusType,
            [DataTreeObjectParameter("slots/status_message_id")] byte statusMessage,
            [DataTreeObjectParameter("slots/data_value_1")] short dataValue1,
            [DataTreeObjectParameter("slots/data_value_2")] short dataValue2)
            : this(subDeviceId, (ERDM_Status)statusType, (ERDM_StatusMessage)statusMessage, dataValue1, dataValue2)
        {
        }

        public ushort SubDeviceId { get; private set; }

        public ERDM_Status StatusType { get; private set; }
        public ERDM_StatusMessage StatusMessage { get; private set; }
        public short DataValue1 { get; private set; }
        public short DataValue2 { get; private set; }
        public const int PDL = 9;
        public string FormatedString => StatusMessage.GetStatusMessage(DataValue1, DataValue2);

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMStatusMessage");
            b.AppendLine($"SubDeviceId:   {SubDeviceId}");
            b.AppendLine($"StatusType: {StatusType}");
            b.AppendLine($"StatusMessage:   {StatusMessage}");
            b.AppendLine($"DataValue1:   {DataValue1}");
            b.AppendLine($"DataValue2:   {DataValue2}");
            b.AppendLine($"FormatedString:   {FormatedString}");

            return b.ToString();
        }
        public static RDMStatusMessage FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.STATUS_MESSAGES, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMStatusMessage FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMStatusMessage(
                subDeviceId: Tools.DataToUShort(ref data),
                statusType: Tools.DataToEnum<ERDM_Status>(ref data),
                statusMessage: Tools.DataToEnum<ERDM_StatusMessage>(ref data),
                dataValue1: Tools.DataToShort(ref data),
                dataValue2: Tools.DataToShort(ref data)
                );
            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SubDeviceId));
            data.AddRange(Tools.ValueToData(this.StatusType));
            data.AddRange(Tools.ValueToData(this.StatusMessage));
            data.AddRange(Tools.ValueToData(this.DataValue1));
            data.AddRange(Tools.ValueToData(this.DataValue2));
            return data.ToArray();
        }
    }
}