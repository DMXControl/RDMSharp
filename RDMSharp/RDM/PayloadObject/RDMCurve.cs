using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class RDMCurve : AbstractRDMPayloadObjectOneOf
    {
        public RDMCurve(
            byte currentCurveId = 1,
            byte curves = 0)
        {
            this.CurrentCurveId = currentCurveId;
            this.Curves = curves;
        }

        public byte CurrentCurveId { get; private set; }
        public byte Curves { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => CurrentCurveId;

        public override object Count => Curves;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.CURVE_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"RDMCurve: {CurrentCurveId} of {Curves}";
        }
        public static RDMCurve FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.CURVE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMCurve FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new RDMCurve(
                currentCurveId: Tools.DataToByte(ref data),
                curves: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.CurrentCurveId));
            data.AddRange(Tools.ValueToData(this.Curves));
            return data.ToArray();
        }
    }
}
