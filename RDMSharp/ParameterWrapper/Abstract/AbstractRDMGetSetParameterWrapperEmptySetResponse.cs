using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
    public abstract class AbstractRDMGetSetParameterWrapperEmptySetResponse<GetRequest, GetResponse, SetRequest> : AbstractRDMParameterWrapper<GetRequest, GetResponse, SetRequest, Empty>, IRDMGetParameterWrapper<GetRequest, GetResponse>, IRDMSetParameterWrapperRequest<SetRequest>, IRDMSetParameterWrapperWithEmptySetResponse
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET | ERDM_CommandClass.SET;

        public Type GetRequestType => typeof(GetRequest);

        public Type GetResponseType => typeof(GetResponse);

        public Type SetRequestType => typeof(SetRequest);

        public abstract ERDM_Parameter[] DescriptiveParameters { get; }

        protected AbstractRDMGetSetParameterWrapperEmptySetResponse(in ERDM_Parameter parameter) : base(parameter)
        {
        }

        #region GET
        public byte[] GetRequestValueToParameterData(GetRequest value)
        {
            return this.getRequestValueToParameterData(value);
        }
        public GetRequest GetRequestParameterDataToValue(byte[] parameterData)
        {
            return this.getRequestParameterDataToValue(parameterData);
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
        public RDMMessage BuildSetRequestMessage(object setRequestValue)
        {
            return this.BuildSetRequestMessage((SetRequest)setRequestValue);
        }
        public RDMMessage BuildSetRequestMessage(SetRequest setRequestValue)
        {
            return this.buildSetRequestMessage(setRequestValue);
        }

        public RDMMessage BuildSetResponseMessage()
        {
            return this.buildSetResponseMessage();
        }

        public RDMMessage BuildGetRequestMessage(object getRequestValue)
        {
            return this.BuildGetRequestMessage((GetRequest)getRequestValue);
        }
        public RDMMessage BuildGetRequestMessage(GetRequest getRequestValue)
        {
            return this.buildGetRequestMessage(getRequestValue);
        }

        public RDMMessage BuildGetResponseMessage(object getResponseValue)
        {
            return this.BuildGetResponseMessage((GetResponse)getResponseValue);
        }
        public RDMMessage BuildGetResponseMessage(GetResponse getResponseValue)
        {
            return this.buildGetResponseMessage(getResponseValue);
        }

        public abstract RequestRange<GetRequest> GetRequestRange(object value);
        #endregion
    }
}
