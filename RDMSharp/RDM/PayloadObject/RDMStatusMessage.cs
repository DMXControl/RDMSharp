using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.STATUS_MESSAGES, Command.ECommandDublicate.GetResponse, true, "slots")]
    public class RDMStatusMessage : AbstractRDMPayloadObject, IEquatable<RDMStatusMessage>
    {
        public RDMStatusMessage(
            ushort subDeviceId = 0,
            ERDM_Status statusType = (ERDM_Status)0,
            ERDM_StatusMessage statusMessage = (ERDM_StatusMessage)0,
            short dataValue1 = 0,
            short dataValue2 = 0)
        {
            this.SubDeviceId = subDeviceId;
            this.EStatusType = statusType;
            this.EStatusMessage = statusMessage;
            this.DataValue1 = dataValue1;
            this.DataValue2 = dataValue2;
        }

        [DataTreeObjectConstructor]
        public RDMStatusMessage(
            [DataTreeObjectParameter("subdevice_id")] ushort subDeviceId,
            [DataTreeObjectParameter("status_type")] byte statusType,
            [DataTreeObjectParameter("status_message_id")] ushort statusMessage,
            [DataTreeObjectParameter("data_value_1")] short dataValue1,
            [DataTreeObjectParameter("data_value_2")] short dataValue2)
            : this(subDeviceId, (ERDM_Status)statusType, (ERDM_StatusMessage)statusMessage, dataValue1, dataValue2)
        {
        }

        [DataTreeObjectProperty("subdevice_id", 0)]
        public ushort SubDeviceId { get; private set; }
        public ERDM_Status EStatusType
        {
            get
            {
                return (ERDM_Status)StatusType;
            }
            private set
            {
                StatusType = (byte)value;
            }
        }
        [DataTreeObjectProperty("status_type", 1)]
        public byte StatusType { get; private set; }
        public ERDM_StatusMessage EStatusMessage
        {
            get
            {
                return (ERDM_StatusMessage)StatusMessage;
            }
            private set
            {
                StatusMessage = (ushort)value;
            }
        }
        [DataTreeObjectProperty("status_message_id", 2)]
        public ushort StatusMessage { get; private set; }
        [DataTreeObjectProperty("data_value_1", 3)]
        public short DataValue1 { get; private set; }
        [DataTreeObjectProperty("data_value_2", 4)]
        public short DataValue2 { get; private set; }
        public const int PDL = 9;
        public string FormatedString => EStatusMessage.GetStatusMessage(DataValue1, DataValue2);

        internal void Clear()
        {
            EStatusType |= ERDM_Status.CLEARED;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMStatusMessage");
            b.AppendLine($"SubDeviceId:   {SubDeviceId}");
            b.AppendLine($"StatusType: {EStatusType}");
            b.AppendLine($"StatusMessage:   {EStatusMessage}");
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
            data.AddRange(Tools.ValueToData(this.EStatusType));
            data.AddRange(Tools.ValueToData(this.EStatusMessage));
            data.AddRange(Tools.ValueToData(this.DataValue1));
            data.AddRange(Tools.ValueToData(this.DataValue2));
            return data.ToArray();
        }

        public bool Equals(RDMStatusMessage other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.SubDeviceId == other.SubDeviceId &&
                   this.EStatusType == other.EStatusType &&
                   this.EStatusMessage == other.EStatusMessage &&
                   this.DataValue1 == other.DataValue1 &&
                   this.DataValue2 == other.DataValue2;
        }
    }
}