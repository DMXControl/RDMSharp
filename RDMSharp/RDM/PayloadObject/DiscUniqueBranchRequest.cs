using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class DiscUniqueBranchRequest : AbstractRDMPayloadObject
    {
        public DiscUniqueBranchRequest(in UID startUid, in UID endUid)
        {
            StartUid = startUid;
            EndUid = endUid;
        }

        public UID StartUid { get; private set; }
        public UID EndUid { get; private set; }

        public const int PDL = 12;

        public override string ToString()
        {
            return $"DiscUniqueBranchRequest:{Environment.NewLine}StartUid: {StartUid}{Environment.NewLine}EndUid: {EndUid}";
        }
        public static DiscUniqueBranchRequest FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.DISCOVERY_COMMAND, ERDM_Parameter.DISC_UNIQUE_BRANCH, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static DiscUniqueBranchRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new DiscUniqueBranchRequest(Tools.DataToRDMUID(ref data), Tools.DataToRDMUID(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();

            data.AddRange(Tools.ValueToData(this.StartUid));
            data.AddRange(Tools.ValueToData(this.EndUid));
            return data.ToArray();
        }
    }
}
