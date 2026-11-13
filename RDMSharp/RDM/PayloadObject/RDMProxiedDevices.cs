using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.PROXIED_DEVICES, Command.ECommandDublicate.GetResponse, true)]
public class RDMProxiedDevices : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public RDMProxiedDevices([DataTreeObjectParameter(ERDM_Parameter.PROXIED_DEVICES, "device_uids", true)] params UID[] devices)
    {
        this.Devices = devices;
    }

    [DataTreeObjectProperty("device_uids/device_uid", 0)]
    public UID[] Devices { get; private set; }
    public const int PDL_MIN = 0;
    public const int PDL_MAX = 0xE4;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMProxiedDevices");
        b.AppendLine($"Devices:");
        if (Devices is not null)
            foreach (UID device in Devices)
                b.AppendLine(device.ToString());

        return b.ToString();
    }
    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.Devices));
        return data.ToArray();
    }
}