using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.COMMS_STATUS_NSC, Command.ECommandDublicte.GetResponse)]
    public class GetCommunicationStatusNullStartCodeResponse : AbstractRDMPayloadObject
    {
        public GetCommunicationStatusNullStartCodeResponse(
            uint? additiveChecksumOfMostRecentPacket = null,
            uint? packetCount = null,
            ushort? mostRecentSlotCount = null,
            ushort? minimumSlotCount = null,
            ushort? maximumSlotCount = null,
            uint? numberOfPacketsWithAnError = null)
        {
            AdditiveChecksumOfMostRecentPacket = additiveChecksumOfMostRecentPacket;
            PacketCount = packetCount;
            MostRecentSlotCount = mostRecentSlotCount;
            MinimumSlotCount = minimumSlotCount;
            MaximumSlotCount = maximumSlotCount;
            NumberOfPacketsWithAnError = numberOfPacketsWithAnError;
        }
        [DataTreeObjectConstructor]
        public GetCommunicationStatusNullStartCodeResponse(
            [DataTreeObjectParameter("supported")] bool[] supported,
            [DataTreeObjectParameter("additive_checksum")] uint additiveChecksumOfMostRecentPacket,
            [DataTreeObjectParameter("packet_count")] uint packetCount,
            [DataTreeObjectParameter("most_recent_slot_count")] ushort mostRecentSlotCount,
            [DataTreeObjectParameter("min_slot_count")] ushort minimumSlotCount,
            [DataTreeObjectParameter("max_slot_count")] ushort maximumSlotCount,
            [DataTreeObjectParameter("error_count")] uint numberOfPacketsWithAnError)
        {
            if (supported[0])
                AdditiveChecksumOfMostRecentPacket = additiveChecksumOfMostRecentPacket;

            if (supported[1])
                PacketCount = packetCount;

            if (supported[2])
                MostRecentSlotCount = mostRecentSlotCount;

            if (supported[3])
                MinimumSlotCount = minimumSlotCount;

            if (supported[4])
                MaximumSlotCount = maximumSlotCount;

            if (supported[5])
                NumberOfPacketsWithAnError = numberOfPacketsWithAnError;
        }

        public uint? AdditiveChecksumOfMostRecentPacket { get; private set; }
        public uint? PacketCount { get; private set; }
        public ushort? MostRecentSlotCount { get; private set; }
        public ushort? MinimumSlotCount { get; private set; }
        public ushort? MaximumSlotCount { get; private set; }
        public uint? NumberOfPacketsWithAnError { get; private set; }
        public const int PDL = 0x13;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            if (AdditiveChecksumOfMostRecentPacket.HasValue)
                b.AppendLine($"AdditiveChecksumOfMostRecentPacket: {AdditiveChecksumOfMostRecentPacket}");
            if (PacketCount.HasValue)
                b.AppendLine($"PacketCount: {PacketCount}");
            if (MostRecentSlotCount.HasValue)
                b.AppendLine($"MostRecentSlotCount: {MostRecentSlotCount}");
            if (MinimumSlotCount.HasValue)
                b.AppendLine($"MinimumSlotCount: {MinimumSlotCount}");
            if (MaximumSlotCount.HasValue)
                b.AppendLine($"MaximumSlotCount: {MaximumSlotCount}");
            if (NumberOfPacketsWithAnError.HasValue)
                b.AppendLine($"NumberOfPacketsWithAnError: {NumberOfPacketsWithAnError}");
            return b.ToString();
        }
        public static GetCommunicationStatusNullStartCodeResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.COMMS_STATUS_NSC, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetCommunicationStatusNullStartCodeResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var flags = Tools.DataToBoolArray(ref data, 8);
            var additiveChecksumOfMostRecentPacket = Tools.DataToUInt(ref data);
            var packetCount = Tools.DataToUInt(ref data);
            var mostRecentSlotCount = Tools.DataToUShort(ref data);
            var minimumSlotCount = Tools.DataToUShort(ref data);
            var maximumSlotCount = Tools.DataToUShort(ref data);
            var numberOfPacketsWithAnError = Tools.DataToUInt(ref data);
            var i = new GetCommunicationStatusNullStartCodeResponse(
                additiveChecksumOfMostRecentPacket: flags[0] ? additiveChecksumOfMostRecentPacket : null,
                packetCount: flags[1] ? packetCount : null,
                mostRecentSlotCount: flags[2] ? mostRecentSlotCount : null,
                minimumSlotCount: flags[3] ? minimumSlotCount : null,
                maximumSlotCount: flags[4] ? maximumSlotCount : null,
                numberOfPacketsWithAnError: flags[5] ? numberOfPacketsWithAnError : null
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(
                this.AdditiveChecksumOfMostRecentPacket.HasValue,
                this.PacketCount.HasValue,
                this.MostRecentSlotCount.HasValue,
                this.MinimumSlotCount.HasValue,
                this.MaximumSlotCount.HasValue,
                this.NumberOfPacketsWithAnError.HasValue,
                false,
                false));
            data.AddRange(Tools.ValueToData(this.AdditiveChecksumOfMostRecentPacket));
            data.AddRange(Tools.ValueToData(this.PacketCount));
            data.AddRange(Tools.ValueToData(this.MostRecentSlotCount));
            data.AddRange(Tools.ValueToData(this.MinimumSlotCount));
            data.AddRange(Tools.ValueToData(this.MaximumSlotCount));
            data.AddRange(Tools.ValueToData(this.NumberOfPacketsWithAnError));
            return data.ToArray();
        }
    }
}