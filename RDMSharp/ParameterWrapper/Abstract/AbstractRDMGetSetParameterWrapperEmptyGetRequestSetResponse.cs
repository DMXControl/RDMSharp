using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
    public abstract class AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<GetResponse, SetRequest> : AbstractRDMParameterWrapper<Empty, GetResponse, SetRequest, Empty>, IRDMGetParameterWrapperResponse<GetResponse>, IRDMSetParameterWrapperRequest<SetRequest>, IRDMGetParameterWrapperWithEmptyGetRequest, IRDMSetParameterWrapperWithEmptySetResponse
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET | ERDM_CommandClass.SET;

        public Type GetResponseType => typeof(GetResponse);

        public Type SetRequestType => typeof(SetRequest);

        protected AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse(in ERDM_Parameter parameter) : base(parameter)
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
        public byte[] SetRequestValueToParameterData(SetRequest value)
        {
            return this.setRequestValueToParameterData(value);
        }
        public SetRequest SetRequestParameterDataToValue(byte[] parameterData)
        {
            return this.setRequestParameterDataToValue(parameterData);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] setResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty setResponseParameterDataToValue(byte[] parameterData)
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

        public RDMMessage BuildSetRequestMessage(object setRequestValue)
        {
            return this.BuildSetRequestMessage((SetRequest)setRequestValue);
        }
        public RDMMessage BuildSetRequestMessage(SetRequest setRequestValue)
        {
            return this.buildSetRequestMessage(setRequestValue);
        }

        public RDMMessage BuildGetRequestMessage()
        {
            return this.buildGetRequestMessage();
        }
        public RDMMessage BuildSetResponseMessage()
        {
            return this.buildSetResponseMessage();
        }
        #endregion
    }
}
