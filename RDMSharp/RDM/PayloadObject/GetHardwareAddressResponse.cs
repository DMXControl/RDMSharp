using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetHardwareAddressResponse : AbstractRDMPayloadObject
    {
        public GetHardwareAddressResponse(
            uint interfaceId = 0,
            MACAddress hardwareAddress = default)
        {
            this.InterfaceId = interfaceId;
            this.HardwareAddress = hardwareAddress;
        }

        public uint InterfaceId { get; private set; }
        public MACAddress HardwareAddress { get; private set; }
        public const int PDL = 0x0A;

        public override string ToString()
        {
            return $"GetHardwareAddressResponse: {InterfaceId} - {HardwareAddress}";
        }

        public static GetHardwareAddressResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetHardwareAddressResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetHardwareAddressResponse(
                interfaceId: Tools.DataToUInt(ref data),
                hardwareAddress: new MACAddress(data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange((byte[])this.HardwareAddress);
            return data.ToArray();
        }
    }
}