﻿using System;
using System.ComponentModel;

namespace RDMSharp.ParameterWrapper.Generic
{
    public abstract class AbstractGenericParameterWrapper<TGet, TSet> : AbstractRDMParameterWrapper<Empty, TGet, TSet, Empty>, IRDMGetParameterWrapperWithEmptyGetRequest
    {
        private readonly RDMParameterDescription parameterDescription;
        public AbstractGenericParameterWrapper(in RDMParameterDescription parameterDescription) : base((ERDM_Parameter)parameterDescription.ParameterId)
        {
            this.parameterDescription = parameterDescription;
        }
        public sealed override string Name => this.parameterDescription.Description;
        public sealed override string Description => null;
        public Type GetResponseType => typeof(TGet);

        public sealed override ERDM_CommandClass CommandClass => this.parameterDescription.CommandClass;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override byte[] getRequestValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override Empty getRequestParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override Empty setResponseParameterDataToValue(byte[] parameterData)
        {
            throw new NotSupportedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override byte[] setResponseValueToParameterData(Empty value)
        {
            throw new NotSupportedException();
        }

        public RDMMessage BuildGetRequestMessage()
        {
            return this.buildGetRequestMessage();
        }
    }
}