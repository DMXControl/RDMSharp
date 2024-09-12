using System;

namespace RDMSharp.ParameterWrapper
{
    public abstract class AbstractRDMGetSetParameterWrapperRanged<GetRequest, GetResponse, SetRequest, SetResponse> : AbstractRDMGetSetParameterWrapper<GetRequest, GetResponse, SetRequest, SetResponse>, IRDMGetParameterWrapperRequestRanged, IRDMGetParameterWrapperRequestRanged<GetRequest>
    {
        public abstract ERDM_Parameter[] DescriptiveParameters { get; }
        protected AbstractRDMGetSetParameterWrapperRanged(in ERDM_Parameter parameter) : base(parameter)
        {
        }
        public abstract IRequestRange GetRequestRange(object value);
        IRequestRange IRDMGetParameterWrapperRequestRanged.GetRequestRange(object value)
        {
            return this.GetRequestRange(value);
        }

        IRequestRange<GetRequest> IRDMGetParameterWrapperRequestRanged<GetRequest>.GetRequestRange(object value)
        {
            return (IRequestRange<GetRequest>)this.GetRequestRange(value);
        }
    }
        public abstract class AbstractRDMGetSetParameterWrapper<GetRequest, GetResponse, SetRequest, SetResponse> : AbstractRDMParameterWrapper<GetRequest, GetResponse, SetRequest, SetResponse>, IRDMGetParameterWrapper<GetRequest, GetResponse>, IRDMSetParameterWrapper<SetRequest, SetResponse>
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET | ERDM_CommandClass.SET;

        public Type GetRequestType => typeof(GetRequest);

        public Type GetResponseType => typeof(GetResponse);

        public Type SetRequestType => typeof(SetRequest);

        public Type SetResponseType => typeof(SetResponse);

        protected AbstractRDMGetSetParameterWrapper(in ERDM_Parameter parameter) : base(parameter)
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
        public SetResponse SetResponseParameterDataToValue(byte[] parameterData)
        {
            return this.setResponseParameterDataToValue(parameterData);
        }
        public byte[] SetResponseValueToParameterData(SetResponse value)
        {
            return this.setResponseValueToParameterData(value);
        }
        #endregion

        #region Build RDMMessage
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

        public RDMMessage BuildSetRequestMessage(object setRequestValue)
        {
            return this.BuildSetRequestMessage((SetRequest)setRequestValue);
        }
        public RDMMessage BuildSetRequestMessage(SetRequest setRequestValue)
        {
            return this.buildSetRequestMessage(setRequestValue);
        }

        public RDMMessage BuildSetResponseMessage(object setResponseValue)
        {
            return this.BuildSetResponseMessage((SetResponse)setResponseValue);
        }

        public RDMMessage BuildSetResponseMessage(SetResponse setResponseValue)
        {
            return this.buildSetResponseMessage(setResponseValue);
        }
        #endregion
    }
}
