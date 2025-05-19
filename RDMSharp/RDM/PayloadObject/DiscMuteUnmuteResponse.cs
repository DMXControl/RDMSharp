using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class DiscMuteUnmuteResponse : AbstractRDMPayloadObject
    {
        public DiscMuteUnmuteResponse(
            bool managedProxyFlag = false,
            bool subDeviceFlag = false,
            bool bootLoaderFlag = false,
            bool proxiedDeviceFlag = false,
            UID? bindingUID = null)
        {
            ManagedProxyFlag = managedProxyFlag;
            SubDeviceFlag = subDeviceFlag;
            BootLoaderFlag = bootLoaderFlag;
            ProxiedDeviceFlag = proxiedDeviceFlag;
            BindingUID = bindingUID;
        }

        /// <summary>
        /// The Managed Proxy Flag (Bit 0) shall be set to 1 when the responder is a Proxy device. See Section 8 for information on Proxy Devices.
        /// </summary>
        public bool ManagedProxyFlag { get; private set; }
        /// <summary>
        /// The Sub-Device Flag (Bit 1) shall be set to 1 when the responder supports Sub-Devices. See Section 9 for information on Sub-Devices.
        /// </summary>
        public bool SubDeviceFlag { get; private set; }
        /// <summary>
        /// The Boot-Loader Flag (Bit 2) shall only be set to 1 when the device is incapable of normal operation until receiving a firmware upload. It is expected that when in this Boot-Loader mode the device will be capable of very limited RDM communication. The process of uploading firmware is beyond the scope of this document.
        /// </summary>
        public bool BootLoaderFlag { get; private set; }
        /// <summary>
        /// The Proxied Device Flag (Bit 3) shall only be set to 1 when a Proxy is responding to Discovery on behalf of another device.This flag indicates that the response has come from a Proxy, rather than the actual device.
        /// </summary>
        public bool ProxiedDeviceFlag { get; private set; }

        public UID? BindingUID { get; private set; } = null;

        public const int PDL = 2;
        public const int PDLWithBindUID = 8;

        public override string ToString()
        {
            if (BindingUID.HasValue)
                return $"DiscMuteUnmuteResponse:{Environment.NewLine}ManagedProxyFlag: {ManagedProxyFlag}{Environment.NewLine}SubDeviceFlag: {SubDeviceFlag}{Environment.NewLine}BootLoaderFlag: {BootLoaderFlag}{Environment.NewLine}ProxiedDeviceFlag: {ProxiedDeviceFlag}{Environment.NewLine}BindingUID: {BindingUID}";
            return $"DiscMuteUnmuteResponse:{Environment.NewLine}ManagedProxyFlag: {ManagedProxyFlag}{Environment.NewLine}SubDeviceFlag: {SubDeviceFlag}{Environment.NewLine}BootLoaderFlag: {BootLoaderFlag}{Environment.NewLine}ProxiedDeviceFlag: {ProxiedDeviceFlag}";
        }
        public static DiscMuteUnmuteResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.DISCOVERY_COMMAND_RESPONSE, new ERDM_Parameter[] { ERDM_Parameter.DISC_MUTE, ERDM_Parameter.DISC_UN_MUTE }, PDL, PDLWithBindUID);

            return FromPayloadData(msg.ParameterData);
        }
        public static DiscMuteUnmuteResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL, PDLWithBindUID);

            bool[] boolArray = Tools.DataToBoolArray(ref data, 16);
            UID? uid = null;
            if (data.Length == PDLWithBindUID - PDL)
                uid = Tools.DataToRDMUID(ref data);
            var i = new DiscMuteUnmuteResponse(
            managedProxyFlag: boolArray[0],
            subDeviceFlag: boolArray[1],
            bootLoaderFlag: boolArray[2],
            proxiedDeviceFlag: boolArray[3],
            bindingUID: uid);

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ManagedProxyFlag, this.SubDeviceFlag, this.BootLoaderFlag, this.ProxiedDeviceFlag));
            while (data.Count < 2) { data.Add(new byte()); }
            if (BindingUID.HasValue)
                data.AddRange(Tools.ValueToData(this.BindingUID));
            return data.ToArray();
        }
    }
}
