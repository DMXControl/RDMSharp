using System;

namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequest : IRDMParameterWrapper
    {
        Type GetRequestType { get; }
        RDMMessage BuildGetRequestMessage(object value);
        byte[] GetRequestObjectToParameterData(object value);
        object GetRequestParameterDataToObject(byte[] parameterData);
    }
}