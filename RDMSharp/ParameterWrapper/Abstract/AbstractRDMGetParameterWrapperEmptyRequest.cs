using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
    public abstract class AbstractRDMGetParameterWrapperEmptyRequest<GetResponse> : AbstractRDMParameterWrapper<Empty, GetResponse, Empty, Empty>, IRDMGetParameterWrapperResponse<GetResponse>, IRDMGetParameterWrapperWithEmptyGetRequest
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET;

        public Type GetResponseType => typeof(GetResponse);

        protected AbstractRDMGetParameterWrapperEmptyRequest(in ERDM_Parameter parameter) : base(parameter)
        {
        }

        #region GET
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        public GetResponse GetResponseParameterDataToValue(byte[] parameterData)
        {
            return this.getResponseParameterDataToValue(parameterData);
        }
        public byte[] GetResponseValueToParameterData(GetResponse value)
        {
            return this.getResponseValueToParameterData(value);
        }
        #endregion

        #region SET

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] setRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty setResponseParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty setRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] setResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Build RDMMessage
        public RDMMessage BuildGetResponseMessage(object getResponseValue)
        {
            return this.BuildGetResponseMessage((GetResponse)getResponseValue);
        }
        public RDMMessage BuildGetResponseMessage(GetResponse getResponseValue)
        {
            return this.buildGetResponseMessage(getResponseValue);
        }

        public RDMMessage BuildGetRequestMessage()
        {
            return this.buildGetRequestMessage();
        }
        #endregion
    }
}
