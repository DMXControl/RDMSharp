using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
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
}
