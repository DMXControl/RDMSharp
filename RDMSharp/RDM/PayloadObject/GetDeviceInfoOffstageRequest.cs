using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DEVICE_INFO_OFFSTAGE, Command.ECommandDublicate.GetRequest)]
    public class GetDeviceInfoOffstageRequest : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetDeviceInfoOffstageRequest(
            [DataTreeObjectParameter("root_personality")] byte rootPersonality = 1,
            [DataTreeObjectParameter("subdevice")] ushort subDeviceRequested = 0,
            [DataTreeObjectParameter("subdevice_personality")] byte subDevicePersonalityRequested = 0)
        {
            RootPersonality = rootPersonality;
            SubDeviceRequested = subDeviceRequested;
            SubDevicePersonalityRequested = subDevicePersonalityRequested;
        }

        [DataTreeObjectProperty("root_personality", 0)]
        public byte RootPersonality { get; private set; }
        [DataTreeObjectProperty("subdevice", 1)]
        public ushort SubDeviceRequested { get; private set; }
        [DataTreeObjectProperty("subdevice_personality", 2)]
        public byte SubDevicePersonalityRequested { get; private set; }
        public const int PDL = 4;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"RootPersonality: {RootPersonality}");
            b.AppendLine($"SubDeviceRequested: {SubDeviceRequested}");
            b.AppendLine($"SubDevicePersonalityRequested:     {SubDevicePersonalityRequested}");

            return b.ToString();
        }
        public static GetDeviceInfoOffstageRequest FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND, ERDM_Parameter.DEVICE_INFO_OFFSTAGE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetDeviceInfoOffstageRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetDeviceInfoOffstageRequest(
                rootPersonality: Tools.DataToByte(ref data),
                subDeviceRequested: Tools.DataToUShort(ref data),
                subDevicePersonalityRequested: Tools.DataToByte(ref data)
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.RootPersonality));
            data.AddRange(Tools.ValueToData(this.SubDeviceRequested));
            data.AddRange(Tools.ValueToData(this.SubDevicePersonalityRequested));
            return data.ToArray();
        }
    }
}