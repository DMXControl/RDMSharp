using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class ListTagsParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string[]>
    {
        public ListTagsParameterWrapper() : base(ERDM_Parameter.LIST_TAGS)
        {
        }
        public override string Name => "List Tags";
        public override string Description => "Responder tags allow Controllers to associate textual metadata with RDM Responders\r\nManufacturers who wish to display tag data are reminded that it may contain non-displayable characters and that these must be handled appropriately. Users must be able to easily remove any tags they desire, including tags containing non-displayable characters.\r\nBecause it is possible that a Responder may have Responder tags that were set by a Controller using Unicode, it is encouraged that all Controllers implementing any of the Responder tag messages (LIST_TAGS, ADD_TAG, REMOVE_TAG, CHECK_TAG, CLEAR_TAG) should support Unicode. A Responder shall return the tags as they were originally set, regardless of encoding and regardless of what the current Controller supports.";

        protected override string[] getResponseParameterDataToValue(byte[] parameterData)
        {
            var rawString = Tools.DataToString(ref parameterData);
            return rawString.Split((char)0).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        protected override byte[] getResponseValueToParameterData(string[] value)
        {
            List<byte> data= new List<byte>();
            foreach (string s in value)
            {
                data.AddRange(Tools.ValueToData(s));
                data.Add(0);
            }
            return data.ToArray();
        }
    }
}