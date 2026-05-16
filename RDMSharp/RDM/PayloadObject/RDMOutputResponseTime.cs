using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.OUTPUT_RESPONSE_TIME, Command.ECommandDublicate.GetResponse)]
public class RDMOutputResponseTime : AbstractRDMPayloadObjectOneOf
{
    [DataTreeObjectConstructor]
    public RDMOutputResponseTime(
        [DataTreeObjectParameter("setting")] byte currentResponseTimeId = 1,
        [DataTreeObjectParameter("setting_count")] byte responseTimes = 0)
    {
        this.CurrentResponseTimeId = currentResponseTimeId;
        this.ResponseTimes = responseTimes;
    }

    [DataTreeObjectProperty("setting", 0)]
    public byte CurrentResponseTimeId { get; private set; }
    [DataTreeObjectDependecieProperty("setting", ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION, Command.ECommandDublicate.GetRequest)]
    [DataTreeObjectProperty("setting_count", 1)]
    public byte ResponseTimes { get; private set; }

    public override Type IndexType => typeof(byte);
    public override object MinIndex => (byte)1;

    public override object Index => CurrentResponseTimeId;

    public override object Count => ResponseTimes;

    public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION;

    public const int PDL = 2;

    public override string ToString()
    {
        return $"RDMOutputResponseTime: {CurrentResponseTimeId} of {ResponseTimes}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.CurrentResponseTimeId));
        data.AddRange(Tools.ValueToData(this.ResponseTimes));
        return data.ToArray();
    }
}
