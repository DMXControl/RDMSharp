using System;

namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperResponse : IRDMParameterWrapper
    {
        Type SetResponseType { get; }
        RDMMessage BuildSetResponseMessage(object value);
        object SetResponseParameterDataToObject(byte[] parameterData);
        byte[] SetResponseObjectToParameterData(object value);
    }
}