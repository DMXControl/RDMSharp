using System;

namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperResponse : IRDMParameterWrapper
    {
        Type GetResponseType { get; }
        RDMMessage BuildGetResponseMessage(object value);
        object GetResponseParameterDataToObject(byte[] parameterData);
        byte[] GetResponseObjectToParameterData(object value);
    }
}