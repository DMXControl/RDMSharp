using System;

namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperRequest
    {
        Type SetRequestType { get; }
        RDMMessage BuildSetRequestMessage(object value);
        object SetRequestParameterDataToObject(byte[] parameterData);
        byte[] SetRequestObjectToParameterData(object value);
    }
}