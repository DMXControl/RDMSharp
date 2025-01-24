using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DEVICE_INFO, Command.ECommandDublicte.GetResponse)]
    public class RDMDeviceInfo : AbstractRDMPayloadObject
    {
        public RDMDeviceInfo(
            byte rdmProtocolVersionMajor = 1,
            byte rdmProtocolVersionMinor = 0,
            ushort deviceModelId = 0,
            ERDM_ProductCategoryCoarse productCategoryCoarse = ERDM_ProductCategoryCoarse.NOT_DECLARED,
            ERDM_ProductCategoryFine productCategoryFine = ERDM_ProductCategoryFine.NOT_DECLARED,
            uint softwareVersionId = 0,
            ushort dmx512Footprint = 0,
            byte dmx512CurrentPersonality = 0,
            byte dmx512NumberOfPersonalities = 0,
            ushort dmx512StartAddress = 0,
            ushort subDeviceCount = 0,
            byte sensorCount = 0)
        {
            RdmProtocolVersionMajor = rdmProtocolVersionMajor;
            RdmProtocolVersionMinor = rdmProtocolVersionMinor;
            DeviceModelId = deviceModelId;
            ProductCategoryCoarse = productCategoryCoarse;
            ProductCategoryFine = productCategoryFine;
            SoftwareVersionId = softwareVersionId;
            Dmx512Footprint = dmx512Footprint;
            Dmx512CurrentPersonality = dmx512CurrentPersonality;
            Dmx512NumberOfPersonalities = dmx512NumberOfPersonalities;
            Dmx512StartAddress = dmx512StartAddress;
            SubDeviceCount = subDeviceCount;
            SensorCount = sensorCount;
        }
        [DataTreeObjectConstructor]
        public RDMDeviceInfo(
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
            [DataTreeObjectParameter("sensor_count")] byte sensorCount):
            this(rdmProtocolVersionMajor,
                rdmProtocolVersionMinor,
                deviceModelId,
                (ERDM_ProductCategoryCoarse)(byte)(productCategory >> 8),
                (ERDM_ProductCategoryFine)productCategory,
                softwareVersionId,
                dmx512Footprint,
                dmx512CurrentPersonality,
                dmx512NumberOfPersonalities,
                dmx512StartAddress,
                subDeviceCount,
                sensorCount)
        {
        }

        [DataTreeObjectProperty("protocol_major", 0)]
        public byte RdmProtocolVersionMajor { get; private set; }

        [DataTreeObjectProperty("protocol_minor", 1)]
        public byte RdmProtocolVersionMinor { get; private set; }

        [DataTreeObjectProperty("device_model_id", 2)]
        public ushort DeviceModelId { get; private set; }
        public ERDM_ProductCategoryCoarse ProductCategoryCoarse { get; private set; }
        public ERDM_ProductCategoryFine ProductCategoryFine { get; private set; }

        [DataTreeObjectProperty("product_category", 3)]
        public ushort ProductCategory => (ushort)ProductCategoryFine;

        [DataTreeObjectProperty("software_version_id", 4)]
        public uint SoftwareVersionId { get; private set; }

        [DataTreeObjectDependecieProperty("slot", ERDM_Parameter.SLOT_DESCRIPTION, Command.ECommandDublicte.GetRequest)]
        [DataTreeObjectProperty("dmx_footprint", 5)]
        public ushort? Dmx512Footprint { get; private set; }

        [DataTreeObjectProperty("current_personality", 6)]
        public byte? Dmx512CurrentPersonality { get; private set; }

        [DataTreeObjectDependecieProperty("personality", ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, Command.ECommandDublicte.GetRequest)]
        [DataTreeObjectDependecieProperty("personality", ERDM_Parameter.DMX_PERSONALITY_ID, Command.ECommandDublicte.GetRequest)]
        [DataTreeObjectProperty("personality_count", 7)]
        public byte Dmx512NumberOfPersonalities { get; private set; }

        [DataTreeObjectProperty("dmx_start_address", 8)]
        public ushort? Dmx512StartAddress { get; private set; }

        [DataTreeObjectProperty("sub_device_count", 9)]
        public ushort SubDeviceCount { get; private set; }

        [DataTreeObjectDependecieProperty("sensor", ERDM_Parameter.SENSOR_DEFINITION, Command.ECommandDublicte.GetRequest)]
        [DataTreeObjectDependecieProperty("sensor", ERDM_Parameter.SENSOR_VALUE, Command.ECommandDublicte.GetRequest)]
        [DataTreeObjectProperty("sensor_count", 10)]
        public byte SensorCount { get; private set; }

        public const int PDL = 19;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"RDM protocol version: {RdmProtocolVersionMajor}.{RdmProtocolVersionMinor}");
            b.AppendLine($"Device model ID:      {DeviceModelId}");
            b.AppendLine($"Product category:     {ProductCategoryCoarse} / {ProductCategoryFine}");
            b.AppendLine($"Software version ID:  0x{SoftwareVersionId.ToString("X")}");
            b.AppendLine($"DMX512 start address: {Dmx512StartAddress}");
            b.AppendLine($"DMX512 Footprint:     {Dmx512Footprint}");
            b.AppendLine($"DMX512 Personality:   {Dmx512CurrentPersonality} / {Dmx512NumberOfPersonalities}");
            b.AppendLine($"Number of subdevices: {SubDeviceCount}");
            b.AppendLine($"Number of sensors:    {SensorCount}");

            return b.ToString();
        }
        public static RDMDeviceInfo FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDeviceInfo FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMDeviceInfo(
                rdmProtocolVersionMajor: Tools.DataToByte(ref data),
                rdmProtocolVersionMinor: Tools.DataToByte(ref data),
                deviceModelId: Tools.DataToUShort(ref data),
                productCategoryCoarse: (ERDM_ProductCategoryCoarse)data[0], //Because we need this byte in productCategoryFine too
                productCategoryFine: Tools.DataToEnum<ERDM_ProductCategoryFine>(ref data),
                softwareVersionId: Tools.DataToUInt(ref data),
                dmx512Footprint: Tools.DataToUShort(ref data),
                dmx512CurrentPersonality: Tools.DataToByte(ref data),
                dmx512NumberOfPersonalities: Tools.DataToByte(ref data),
                dmx512StartAddress: Tools.DataToUShort(ref data),
                subDeviceCount: Tools.DataToUShort(ref data),
                sensorCount: Tools.DataToByte(ref data)
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.RdmProtocolVersionMajor));
            data.AddRange(Tools.ValueToData(this.RdmProtocolVersionMinor));
            data.AddRange(Tools.ValueToData(this.DeviceModelId));
            //data.AddRange(Tools.ValueToData(this.ProductCategoryCoarse));//Because we this byte is in productCategoryFine too
            data.AddRange(Tools.ValueToData(this.ProductCategoryFine));
            data.AddRange(Tools.ValueToData(this.SoftwareVersionId));
            data.AddRange(Tools.ValueToData(this.Dmx512Footprint));
            data.AddRange(Tools.ValueToData(this.Dmx512CurrentPersonality));
            data.AddRange(Tools.ValueToData(this.Dmx512NumberOfPersonalities));
            data.AddRange(Tools.ValueToData(this.Dmx512StartAddress));
            data.AddRange(Tools.ValueToData(this.SubDeviceCount));
            data.AddRange(Tools.ValueToData(this.SensorCount));
            return data.ToArray();
        }
    }
}