using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
    public abstract class AbstractRDMGetSetParameterWrapper<GetRequest, GetResponse, SetRequest, SetResponse> : AbstractRDMParameterWrapper<GetRequest, GetResponse, SetRequest, SetResponse>, IRDMGetParameterWrapper<GetRequest, GetResponse>, IRDMSetParameterWrapper<SetRequest, SetResponse>
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET | ERDM_CommandClass.SET;

        public Type GetRequestType => typeof(GetRequest);

        public Type GetResponseType => typeof(GetResponse);

        public Type SetRequestType => typeof(SetRequest);

        public Type SetResponseType => typeof(SetResponse);

        public abstract ERDM_Parameter[] DescriptiveParameters { get; }

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

        public abstract RequestRange<GetRequest> GetRequestRange(object value);
    }
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
    public abstract class AbstractRDMGetSetParameterWrapperEmptyGetRequestSetRequestSetResponse<GetResponse> : AbstractRDMParameterWrapper<Empty, GetResponse, Empty, Empty>, IRDMGetParameterWrapperResponse<GetResponse>, IRDMGetParameterWrapperWithEmptyGetRequest, IRDMSetParameterWrapperWithEmptySetRequest, IRDMSetParameterWrapperWithEmptySetResponse
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET | ERDM_CommandClass.SET;

        public Type GetResponseType => typeof(GetResponse);

        protected AbstractRDMGetSetParameterWrapperEmptyGetRequestSetRequestSetResponse(in ERDM_Parameter parameter) : base(parameter)
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
        protected override sealed Empty setRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
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

        public RDMMessage BuildSetRequestMessage()
        {
            return this.buildSetRequestMessage();
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

        public abstract RequestRange<GetRequest> GetRequestRange(object value);
        #endregion
    }
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
    public abstract class AbstractRDMGetParameterWrapperEmptyRequestResponse : AbstractRDMParameterWrapper<Empty, Empty, Empty, Empty>, IRDMGetParameterWrapperWithEmptyGetRequest, IRDMSetParameterWrapperWithEmptySetResponse
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET;

        protected AbstractRDMGetParameterWrapperEmptyRequestResponse(in ERDM_Parameter parameter) : base(parameter)
        {
        }

        #region GET
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        protected override sealed Empty getRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getResponseParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
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

        public RDMMessage BuildGetRequestMessage()
        {
            return this.buildGetRequestMessage();
        }

        public RDMMessage BuildSetResponseMessage()
        {
            return this.buildGetResponseMessage();
        }
        #endregion
    }
    public abstract class AbstractRDMSetParameterWrapper<SetRequest, SetResponse> : AbstractRDMParameterWrapper<Empty, Empty, SetRequest, SetResponse>, IRDMSetParameterWrapper<SetRequest, SetResponse>
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.SET;

        public Type SetRequestType => typeof(SetRequest);

        public Type SetResponseType => typeof(SetResponse);

        protected AbstractRDMSetParameterWrapper(in ERDM_Parameter parameter) : base(parameter)
        {
        }

        #region GET
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getResponseParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
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
    public abstract class AbstractRDMSetParameterWrapperEmptyResponse<SetRequest> : AbstractRDMParameterWrapper<Empty, Empty, SetRequest, Empty>, IRDMSetParameterWrapperRequest<SetRequest>, IRDMSetParameterWrapperWithEmptySetResponse
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.SET;

        public Type SetRequestType => typeof(SetRequest);

        protected AbstractRDMSetParameterWrapperEmptyResponse(in ERDM_Parameter parameter) : base(parameter)
        {
        }

        #region GET
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getResponseParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
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
        #endregion
    }
    public abstract class AbstractRDMSetParameterWrapperEmptyRequestResponse : AbstractRDMParameterWrapper<Empty, Empty, Empty, Empty>, IRDMSetParameterWrapperWithEmptySetRequest, IRDMSetParameterWrapperWithEmptySetResponse
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.SET;
        protected AbstractRDMSetParameterWrapperEmptyRequestResponse(in ERDM_Parameter parameter) : base(parameter)
        {
        }

        #region GET
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] getResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty getResponseParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region SET
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed Empty setRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed byte[] setRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
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
        public RDMMessage BuildSetRequestMessage()
        {
            return this.buildSetRequestMessage();
        }

        public RDMMessage BuildSetResponseMessage()
        {
            return this.buildSetResponseMessage();
        }
        #endregion
    }
    public abstract class AbstractRDMParameterWrapper<GetRequest, GetResponse, SetRequest, SetResponse> : IRDMParameterWrapper
    {
        public ERDM_Parameter Parameter { get; }
        public abstract ERDM_CommandClass CommandClass { get; }

        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual ERDM_SupportedSubDevice SupportedGetSubDevices => this is IRDMGetParameterWrapperRequest ? ERDM_SupportedSubDevice.ALL_EXCEPT_BROADCAST : ERDM_SupportedSubDevice.NONE;
        public virtual ERDM_SupportedSubDevice SupportedSetSubDevices => this is IRDMSetParameterWrapperRequest ? ERDM_SupportedSubDevice.ALL : ERDM_SupportedSubDevice.NONE;

        public bool SupportSubDevices
        {
            get
            {
                if (this.SupportedGetSubDevices.HasFlag(ERDM_SupportedSubDevice.RANGE_0X0001_0x0200) || this.SupportedGetSubDevices.HasFlag(ERDM_SupportedSubDevice.BROADCAST))
                    return true;
                else if (this.SupportedSetSubDevices.HasFlag(ERDM_SupportedSubDevice.RANGE_0X0001_0x0200) || this.SupportedSetSubDevices.HasFlag(ERDM_SupportedSubDevice.BROADCAST))
                    return true;

                return false;
            }
        }

        internal AbstractRDMParameterWrapper(in ERDM_Parameter parameter)
        {
            this.Parameter = parameter;
        }

        #region GET
        public byte[] GetRequestObjectToParameterData(object value)
        {
            return this.getRequestValueToParameterData((GetRequest)value);
        }
        public object GetRequestParameterDataToObject(byte[] parameterData)
        {
            return this.getRequestParameterDataToValue(parameterData);
        }
        public object GetResponseParameterDataToObject(byte[] parameterData)
        {
            return (object)getResponseParameterDataToValue(parameterData);
        }
        public byte[] GetResponseObjectToParameterData(object value)
        {
            return getResponseValueToParameterData((GetResponse)value);
        }

        protected abstract byte[] getRequestValueToParameterData(GetRequest value);
        protected abstract GetRequest getRequestParameterDataToValue(byte[] parameterData);
        protected abstract GetResponse getResponseParameterDataToValue(byte[] parameterData);
        protected abstract byte[] getResponseValueToParameterData(GetResponse value);
        private protected RDMMessage buildGetRequestMessage()
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.GET))
                throw new NotSupportedException($"This parameter is not allowed to GET");

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                Parameter = this.Parameter,
            };

            return message;
        }
        private protected RDMMessage buildGetRequestMessage(in GetRequest getRequestValue)
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.GET))
                throw new NotSupportedException($"This parameter is not allowed to GET");

            var payload = this.getRequestValueToParameterData(getRequestValue);

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                Parameter = this.Parameter,
                ParameterData = payload
            };

            return message;
        }
        private protected RDMMessage buildGetResponseMessage()
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.GET))
                throw new NotSupportedException($"This parameter is not allowed to GET");

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = this.Parameter,
            };

            return message;
        }
        private protected RDMMessage buildGetResponseMessage(in GetResponse getResponseValue)
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.GET))
                throw new NotSupportedException($"This parameter is not allowed to GET");

            var payload = this.getResponseValueToParameterData(getResponseValue);

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = this.Parameter,
                ParameterData = payload
            };

            return message;
        }
        #endregion

        #region SET
        public byte[] SetRequestObjectToParameterData(object value)
        {
            return this.setRequestValueToParameterData((SetRequest)value);
        }
        public object SetRequestParameterDataToObject(byte[] parameterData)
        {
            return this.setRequestParameterDataToValue(parameterData);
        }
        public object SetResponseParameterDataToObject(byte[] parameterData)
        {
            return (object)setResponseParameterDataToValue(parameterData);
        }
        public byte[] SetResponseObjectToParameterData(object value)
        {
            return setResponseValueToParameterData((SetResponse)value);
        }
        protected abstract byte[] setRequestValueToParameterData(SetRequest value);
        protected abstract SetRequest setRequestParameterDataToValue(byte[] parameterData);
        protected abstract SetResponse setResponseParameterDataToValue(byte[] parameterData);
        protected abstract byte[] setResponseValueToParameterData(SetResponse value);

        #endregion

        #region Build RDMMessage
        protected RDMMessage buildSetRequestMessage()
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.SET))
                throw new NotSupportedException($"This parameter is not allowed to SET");

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.SET_COMMAND,
                Parameter = this.Parameter,
            };

            return message;
        }
        protected RDMMessage buildSetRequestMessage(in SetRequest setRequestValue)
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.SET))
                throw new NotSupportedException($"This parameter is not allowed to SET");

            var payload = this.setRequestValueToParameterData(setRequestValue);

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.SET_COMMAND,
                Parameter = this.Parameter,
                ParameterData = payload
            };

            return message;
        }
        protected RDMMessage buildSetResponseMessage()
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.SET))
                throw new NotSupportedException($"This parameter is not allowed to SET");

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                Parameter = this.Parameter,
            };

            return message;
        }
        protected RDMMessage buildSetResponseMessage(in SetResponse setResponseValue)
        {
            if (!this.CommandClass.HasFlag(ERDM_CommandClass.SET))
                throw new NotSupportedException($"This parameter is not allowed to SET");

            var payload = this.setResponseValueToParameterData(setResponseValue);

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                Parameter = this.Parameter,
                ParameterData = payload
            };

            return message;
        }
        #endregion

        public bool Equals(IRDMParameterWrapper other)
        {
            if (this.Parameter != other.Parameter)
                return false;

            if (this.CommandClass != other.CommandClass)
                return false;

            if (this is IRDMManufacturerParameterWrapper thisM &&
                other is IRDMManufacturerParameterWrapper otherM &&
                thisM.Manufacturer != otherM.Manufacturer)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            if (!this.Name.Equals(other.Name))
                return false;

            if (!this.Description.Equals(other.Description))
                return false;

            return true;
        }
        public override bool Equals(object obj)
        {
            return obj is IRDMParameterWrapper pw && this.Equals(pw);
        }

        public override int GetHashCode()
        {
            return this.Parameter.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
