using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMDeviceInfo : AbstractRDMPayloadObject
    {
        public RDMDeviceInfo(
            byte rdmProtocolVersionMajor = 0,
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

        public byte RdmProtocolVersionMajor { get; private set; }
        public byte RdmProtocolVersionMinor { get; private set; }
        public ushort DeviceModelId { get; private set; }
        public ERDM_ProductCategoryCoarse ProductCategoryCoarse { get; private set; }
        public ERDM_ProductCategoryFine ProductCategoryFine { get; private set; }
        public uint SoftwareVersionId { get; private set; }
        public ushort? Dmx512Footprint { get; private set; }
        public byte? Dmx512CurrentPersonality { get; private set; }
        public byte Dmx512NumberOfPersonalities { get; private set; }
        public ushort? Dmx512StartAddress { get; private set; }
        public ushort SubDeviceCount { get; private set; }
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DEVICE_INFO) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDeviceInfo FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

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
            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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

        public RDMDeviceInfo ToTemplate()
        {
            return new RDMDeviceInfo()
            {
                RdmProtocolVersionMajor = this.RdmProtocolVersionMajor,
                RdmProtocolVersionMinor = this.RdmProtocolVersionMinor,
                DeviceModelId = this.DeviceModelId,
                ProductCategoryCoarse = this.ProductCategoryCoarse,
                ProductCategoryFine = this.ProductCategoryFine,
                SoftwareVersionId = this.SoftwareVersionId,
                Dmx512NumberOfPersonalities = this.Dmx512NumberOfPersonalities,
                SubDeviceCount = this.SubDeviceCount,
                SensorCount = this.SensorCount,
                Dmx512CurrentPersonality = null,
                Dmx512Footprint = null,
                Dmx512StartAddress = null
            };
        }
    }
}