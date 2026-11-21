using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.BINDING_CONTROL_FIELDS, Command.ECommandDublicate.GetResponse)]
public class GetBindingAndControlFieldsResponse : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public GetBindingAndControlFieldsResponse(
        [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
        [DataTreeObjectParameter("uid")] UID uid = default,
        [DataTreeObjectParameter("control")] ushort controlField = default,
        [DataTreeObjectParameter("binding_uid")] UID bindingUID = default)
    {
        this.EndpointId = endpointId;
        this.UID = uid;
        this.ControlField = controlField;
        this.BindingUID = bindingUID;
    }
    [DataTreeObjectProperty("endpoint_id", 0)]
    public ushort EndpointId { get; private set; }
    [DataTreeObjectProperty("uid", 1)]
    public UID UID { get; private set; }
    [DataTreeObjectProperty("control", 2)]
    public ushort ControlField { get; private set; }
    [DataTreeObjectProperty("binding_uid", 3)]
    public UID BindingUID { get; private set; }
    public const int PDL = 0x10;

    public override string ToString()
    {
        return $"Endpoint: {EndpointId} - UID: {UID} ControlField: {ControlField} BindingUID: {BindingUID}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.UID));
        data.AddRange(Tools.ValueToData(this.ControlField));
        data.AddRange(Tools.ValueToData(this.BindingUID));
        return data.ToArray();
    }
}