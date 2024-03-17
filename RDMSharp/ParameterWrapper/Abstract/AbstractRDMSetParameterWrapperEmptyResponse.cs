using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
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
}
