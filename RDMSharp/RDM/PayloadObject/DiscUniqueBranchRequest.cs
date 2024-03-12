using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class DiscUniqueBranchRequest : AbstractRDMPayloadObject
    {
        public DiscUniqueBranchRequest(in RDMUID startUid, in RDMUID endUid)
        {
            StartUid = startUid;
            EndUid = endUid;
        }

        public RDMUID StartUid { get; private set; }
        public RDMUID EndUid { get; private set; }

        public const int PDL = 12;

        public override string ToString()
        {
            return $"DiscUniqueBranchRequest:{Environment.NewLine}StartUid: {StartUid}{Environment.NewLine}EndUid: {EndUid}";
        }
        public static DiscUniqueBranchRequest FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (msg.Command != ERDM_Command.DISCOVERY_COMMAND) throw new Exception($"Command is not a {ERDM_Command.DISCOVERY_COMMAND}");
            if (msg.Parameter != ERDM_Parameter.DISC_UNIQUE_BRANCH) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static DiscUniqueBranchRequest FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
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
