using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.COMMS_STATUS_NSC, Command.ECommandDublicate.GetResponse)]
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
        [DataTreeObjectParameter("supported/additive_checksum")] bool additiveChecksumSupported,
        [DataTreeObjectParameter("supported/packet_count")] bool packetCountSupported,
        [DataTreeObjectParameter("supported/most_recent_slot_count")] bool mostRecentSlotCountSupported,
        [DataTreeObjectParameter("supported/min_slot_count")] bool minSlotCountSupported,
        [DataTreeObjectParameter("supported/max_slot_count")] bool maxSlotCountSupported,
        [DataTreeObjectParameter("supported/error_count")] bool errorCountSupported,
        [DataTreeObjectParameter("additive_checksum")] uint additiveChecksumOfMostRecentPacket,
        [DataTreeObjectParameter("packet_count")] uint packetCount,
        [DataTreeObjectParameter("most_recent_slot_count")] ushort mostRecentSlotCount,
        [DataTreeObjectParameter("min_slot_count")] ushort minimumSlotCount,
        [DataTreeObjectParameter("max_slot_count")] ushort maximumSlotCount,
        [DataTreeObjectParameter("error_count")] uint numberOfPacketsWithAnError) : this(
            additiveChecksumSupported ? additiveChecksumOfMostRecentPacket : null,
            packetCountSupported ? packetCount : null,
            mostRecentSlotCountSupported ? mostRecentSlotCount : null,
            minSlotCountSupported ? minimumSlotCount : null,
            maxSlotCountSupported ? maximumSlotCount : null,
            errorCountSupported ? numberOfPacketsWithAnError : null
            )
    {
    }

    [DataTreeObjectProperty("supported/additive_checksum", 0)]
    public bool AdditiveChecksumSupported
    {
        get => AdditiveChecksumOfMostRecentPacket is not null;
    }
    [DataTreeObjectProperty("supported/packet_count", 1)]
    public bool PacketCountSupported
    {
        get => PacketCount is not null;
    }
    [DataTreeObjectProperty("supported/most_recent_slot_count", 2)]
    public bool MostRecentSlotCountSupported
    {
        get => MostRecentSlotCount is not null;
    }
    [DataTreeObjectProperty("supported/min_slot_count", 3)]
    public bool MinimumSlotCountSupported
    {
        get => MinimumSlotCount is not null;
    }
    [DataTreeObjectProperty("supported/max_slot_count", 4)]
    public bool MaximumSlotCountSupported
    {
        get => MaximumSlotCount is not null;
    }
    [DataTreeObjectProperty("supported/error_count", 5)]
    public bool ErrorCountSupported
    {
        get => NumberOfPacketsWithAnError is not null;
    }


    [DataTreeObjectProperty("additive_checksum", 1)]
    public uint? AdditiveChecksumOfMostRecentPacket { get; private set; }
    [DataTreeObjectProperty("packet_count", 2)]
    public uint? PacketCount { get; private set; }
    [DataTreeObjectProperty("most_recent_slot_count", 3)]
    public ushort? MostRecentSlotCount { get; private set; }
    [DataTreeObjectProperty("min_slot_count", 4)]
    public ushort? MinimumSlotCount { get; private set; }
    [DataTreeObjectProperty("max_slot_count", 5)]
    public ushort? MaximumSlotCount { get; private set; }
    [DataTreeObjectProperty("error_count", 6)]
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