using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
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
}
