﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.MODULATION_FREQUENCY, Command.ECommandDublicate.GetResponse)]
    public class RDMModulationFrequency : AbstractRDMPayloadObjectOneOf
    {
        [DataTreeObjectConstructor]
        public RDMModulationFrequency(
            [DataTreeObjectParameter("setting")] byte modulationFrequencyId = 1,
            [DataTreeObjectParameter("setting_count")] byte modulationFrequencys = 0)
        {
            this.ModulationFrequencyId = modulationFrequencyId;
            this.ModulationFrequencys = modulationFrequencys;
        }

        [DataTreeObjectProperty("setting", 0)]
        public byte ModulationFrequencyId { get; private set; }

        [DataTreeObjectDependecieProperty("setting", ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION, Command.ECommandDublicate.GetRequest)]
        [DataTreeObjectProperty("setting_count", 1)]
        public byte ModulationFrequencys { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => ModulationFrequencyId;

        public override object Count => ModulationFrequencys;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"RDMModulationFrequency: {ModulationFrequencyId} of {ModulationFrequencys}";
        }
        public static RDMModulationFrequency FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.MODULATION_FREQUENCY, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMModulationFrequency FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new RDMModulationFrequency(
                modulationFrequencyId: Tools.DataToByte(ref data),
                modulationFrequencys: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ModulationFrequencyId));
            data.AddRange(Tools.ValueToData(this.ModulationFrequencys));
            return data.ToArray();
        }
    }
}
