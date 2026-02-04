using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp;

[DataTreeObject(ERDM_Parameter.LIST_INTERFACES, Command.ECommandDublicate.GetResponse, true, "interfaces")]
public class InterfaceDescriptor : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public InterfaceDescriptor(
        [DataTreeObjectParameter("id")] uint interfaceId = 0,
        [DataTreeObjectParameter("hardware_type")] ushort hardwareType = 0)
    {
        this.InterfaceId = interfaceId;
        this._HardwareType = hardwareType;
    }
    public InterfaceDescriptor(uint interfaceId, EARP_HardwareTypes hardwareType)
    {
        this.InterfaceId = interfaceId;
        this._HardwareType = (ushort)hardwareType;
    }

    [DataTreeObjectProperty("id", 0)]
    public uint InterfaceId { get; private set; }
    [DataTreeObjectProperty("hardware_type", 1)]
    public ushort _HardwareType { get; private set; }
    public EARP_HardwareTypes HardwareType { get => (EARP_HardwareTypes)this._HardwareType; }
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