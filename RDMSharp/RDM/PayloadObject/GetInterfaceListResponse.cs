using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.LIST_INTERFACES, Command.ECommandDublicate.GetResponse, true, "interfaces")]
    public class GetInterfaceListResponse : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetInterfaceListResponse([DataTreeObjectParameter("interfaces")] params InterfaceDescriptor[] interfaces)
        {
            this.Interfaces = interfaces;
        }

        [DataTreeObjectDependecieProperty("id", nameof(InterfaceDescriptor.InterfaceId), ERDM_Parameter.INTERFACE_LABEL, Command.ECommandDublicate.GetRequest)]
        [DataTreeObjectProperty("interfaces", 0)]
        public InterfaceDescriptor[] Interfaces { get; private set; }
        public const int PDL_MIN = 0;
        public const int PDL_MAX = 0xE6;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("GetInterfaceListResponse");
            b.AppendLine($"Interfaces:");
            if (Interfaces is not null)
                foreach (InterfaceDescriptor _interface in Interfaces)
                    b.AppendLine(_interface.ToString());

            return b.ToString();
        }
        public static GetInterfaceListResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.LIST_INTERFACES, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetInterfaceListResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            List<InterfaceDescriptor> _interfaces = new List<InterfaceDescriptor>();
            int pdl = 6;
            while (data.Length >= pdl)
            {
                var bytes = data.Take(pdl).ToArray();
                _interfaces.Add(InterfaceDescriptor.FromPayloadData(bytes));
                data = data.Skip(pdl).ToArray();
            }

            var i = new GetInterfaceListResponse(_interfaces.ToArray());

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            foreach (InterfaceDescriptor _interface in Interfaces)
                data.AddRange(_interface.ToPayloadData());
            return data.ToArray();
        }
    }
    public class InterfaceDescriptor : AbstractRDMPayloadObject
    {
        public InterfaceDescriptor(uint interfaceId = 0, ushort hardwareType = 0)
        {
            this.InterfaceId = interfaceId;
            this.HardwareType = hardwareType;
        }

        public uint InterfaceId { get; private set; }
        public ushort HardwareType { get; private set; }
        public const int PDL = 6;

        public override string ToString()
        {
            return $"Id: {InterfaceId} HardwareType: {HardwareType}";
        }
        public static InterfaceDescriptor FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new InterfaceDescriptor(
                interfaceId: Tools.DataToUInt(ref data),
                hardwareType: Tools.DataToUShort(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.HardwareType));
            return data.ToArray();
        }
    }
}