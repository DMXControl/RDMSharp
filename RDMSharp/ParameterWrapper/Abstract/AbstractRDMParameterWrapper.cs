using System;

namespace RDMSharp.ParameterWrapper
{
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