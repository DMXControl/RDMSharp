using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
    public abstract class AbstractRDMGetParameterWrapper<GetRequest, GetResponse> : AbstractRDMParameterWrapper<GetRequest, GetResponse, Empty, Empty>, IRDMGetParameterWrapper<GetRequest, GetResponse>
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET;

        public Type GetRequestType => typeof(GetRequest);

        public Type GetResponseType => typeof(GetResponse);

        public abstract ERDM_Parameter[] DescriptiveParameters { get; }

        protected AbstractRDMGetParameterWrapper(in ERDM_Parameter parameter) : base(parameter)
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

        public abstract IRequestRange GetRequestRange(object value);
        IRequestRange IRDMGetParameterWrapperRequest.GetRequestRange(object value)
        {
            return this.GetRequestRange(value);
        }

        IRequestRange<GetRequest> IRDMGetParameterWrapperRequest<GetRequest>.GetRequestRange(object value)
        {
            return (IRequestRange<GetRequest>)this.GetRequestRange(value);
        }
        #endregion
    }
}
