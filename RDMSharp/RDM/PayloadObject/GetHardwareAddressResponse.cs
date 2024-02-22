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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetHardwareAddressResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

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