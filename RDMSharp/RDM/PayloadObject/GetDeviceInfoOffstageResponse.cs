using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DEVICE_INFO_OFFSTAGE, Command.ECommandDublicate.GetResponse)]
    public class GetDeviceInfoOffstageResponse : AbstractRDMPayloadObject
    {
        public GetDeviceInfoOffstageResponse(
            byte rootPersonality = 1,
            ushort subDeviceRequested = 0,
            byte subDevicePersonalityRequested = 0,
            RDMDeviceInfo deviceInfo = null)
        {
            RootPersonality = rootPersonality;
            SubDeviceRequested = subDeviceRequested;
            SubDevicePersonalityRequested = subDevicePersonalityRequested;
            DeviceInfo = deviceInfo;
        }

        [DataTreeObjectConstructor]
        public GetDeviceInfoOffstageResponse(
            [DataTreeObjectParameter("root_personality")] byte rootPersonality,
            [DataTreeObjectParameter("subdevice")] ushort subDeviceRequested,
            [DataTreeObjectParameter("subdevice_personality")] byte subDevicePersonalityRequested,
            [DataTreeObjectParameter("protocol_major")] byte rdmProtocolVersionMajor,
            [DataTreeObjectParameter("protocol_minor")] byte rdmProtocolVersionMinor,
            [DataTreeObjectParameter("device_model_id")] ushort deviceModelId,
            [DataTreeObjectParameter("product_category")] ushort productCategory,
            [DataTreeObjectParameter("software_version_id")] uint softwareVersionId,
            [DataTreeObjectParameter("dmx_footprint")] ushort dmx512Footprint,
            [DataTreeObjectParameter("current_personality")] byte dmx512CurrentPersonality,
            [DataTreeObjectParameter("personality_count")] byte dmx512NumberOfPersonalities,
            [DataTreeObjectParameter("dmx_start_address")] ushort dmx512StartAddress,
            [DataTreeObjectParameter("sub_device_count")] ushort subDeviceCount,
            [DataTreeObjectParameter("sensor_count")] byte sensorCount)
            : this(rootPersonality,
                  subDeviceRequested,
                  subDevicePersonalityRequested,
                  new RDMDeviceInfo(
                      rdmProtocolVersionMajor,
                        rdmProtocolVersionMinor,
                        deviceModelId,
                        productCategory,
                        softwareVersionId,
                        dmx512Footprint,
                        dmx512CurrentPersonality,
                        dmx512NumberOfPersonalities,
                        dmx512StartAddress,
                        subDeviceCount,
                        sensorCount))
        {
        }

        [DataTreeObjectProperty("root_personality", 0)]
        public byte RootPersonality { get; private set; }
        [DataTreeObjectProperty("subdevice", 1)]
        public ushort SubDeviceRequested { get; private set; }
        [DataTreeObjectProperty("subdevice_personality", 2)]
        public byte SubDevicePersonalityRequested { get; private set; }

        [DataTreeObjectProperty("protocol_major", 3)]
        public byte RdmProtocolVersionMajor { get => DeviceInfo.RdmProtocolVersionMajor; }
        [DataTreeObjectProperty("protocol_minor", 4)]
        public byte RdmProtocolVersionMinor { get => DeviceInfo.RdmProtocolVersionMinor; }
        [DataTreeObjectProperty("device_model_id", 5)]
        public ushort DeviceModelId { get => DeviceInfo.DeviceModelId; }
        [DataTreeObjectProperty("product_category", 6)]
        public ushort ProductCategory { get => DeviceInfo.ProductCategory; }
        [DataTreeObjectProperty("software_version_id", 7)]
        public uint SoftwareVersionId { get => DeviceInfo.SoftwareVersionId; }
        [DataTreeObjectProperty("dmx_footprint", 8)]
        public ushort? Dmx512Footprint { get => DeviceInfo.Dmx512Footprint; }
        [DataTreeObjectProperty("current_personality", 9)]
        public byte? Dmx512CurrentPersonality { get => DeviceInfo.Dmx512CurrentPersonality; }
        [DataTreeObjectProperty("personality_count", 10)]
        public byte Dmx512NumberOfPersonalities { get => DeviceInfo.Dmx512NumberOfPersonalities; }
        [DataTreeObjectProperty("dmx_start_address", 11)]
        public ushort? Dmx512StartAddress { get => DeviceInfo.Dmx512StartAddress; }
        [DataTreeObjectProperty("sub_device_count", 12)]
        public ushort SubDeviceCount { get => DeviceInfo.SubDeviceCount; }
        [DataTreeObjectProperty("sensor_count", 13)]
        public byte SensorCount { get => DeviceInfo.SensorCount; }
        public RDMDeviceInfo DeviceInfo { get; private set; }
        public const int PDL = 4 + RDMDeviceInfo.PDL;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"RootPersonality: {RootPersonality}");
            b.AppendLine($"SubDeviceRequested: {SubDeviceRequested}");
            b.AppendLine($"SubDevicePersonalityRequested: {SubDevicePersonalityRequested}");
            b.AppendLine($"DeviceInfo: {DeviceInfo}");

            return b.ToString();
        }
        public static GetDeviceInfoOffstageResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO_OFFSTAGE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetDeviceInfoOffstageResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetDeviceInfoOffstageResponse(
                rootPersonality: Tools.DataToByte(ref data),
                subDeviceRequested: Tools.DataToUShort(ref data),
                subDevicePersonalityRequested: Tools.DataToByte(ref data),
                deviceInfo: RDMDeviceInfo.FromPayloadData(data)
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.RootPersonality));
            data.AddRange(Tools.ValueToData(this.SubDeviceRequested));
            data.AddRange(Tools.ValueToData(this.SubDevicePersonalityRequested));
            data.AddRange(Tools.ValueToData(this.DeviceInfo));
            return data.ToArray();
        }
    }
}