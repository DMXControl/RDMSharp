using System;

namespace RDMSharp.ParameterWrapper
{
    public interface IRDMParameterWrapper : IEquatable<IRDMParameterWrapper>
    {
        string Name { get; }
        string Description { get; }
        ERDM_Parameter Parameter { get; }
        ERDM_CommandClass CommandClass { get; }

        bool SupportSubDevices { get; }
    }
    #region GET
    public interface IRDMGetParameterWrapper<TRequest, TResponse> : IRDMGetParameterWrapperRequest<TRequest>, IRDMGetParameterWrapperResponse<TResponse>
    {
        ERDM_SupportedSubDevice SupportedGetSubDevices { get; }
    }
    #region Request
    public interface IRDMGetParameterWrapperRequest : IRDMParameterWrapper
    {
        Type GetRequestType { get; }
        RDMMessage BuildGetRequestMessage(object value);
        byte[] GetRequestObjectToParameterData(object value);
        object GetRequestParameterDataToObject(byte[] parameterData);
    }
    public interface IRDMGetParameterWrapperRequest<TRequest> : IRDMGetParameterWrapperRequest
    {
        ERDM_Parameter[] DescriptiveParameters { get; }
        RequestRange<TRequest> GetRequestRange(object value);
        RDMMessage BuildGetRequestMessage(TRequest value);
        byte[] GetRequestValueToParameterData(TRequest value);
        TRequest GetRequestParameterDataToValue(byte[] parameterData);
    }
    public interface IRDMGetParameterWrapperWithEmptyGetRequest : IRDMParameterWrapper
    {
        RDMMessage BuildGetRequestMessage();
    }
    #endregion
    #region Response
    public interface IRDMGetParameterWrapperResponse : IRDMParameterWrapper
    {
        Type GetResponseType { get; }
        RDMMessage BuildGetResponseMessage(object value);
        object GetResponseParameterDataToObject(byte[] parameterData);
        byte[] GetResponseObjectToParameterData(object value);
    }
    public interface IRDMGetParameterWrapperResponse<TResponse> : IRDMGetParameterWrapperResponse
    {
        RDMMessage BuildGetResponseMessage(TResponse value);
        TResponse GetResponseParameterDataToValue(byte[] parameterData);
        byte[] GetResponseValueToParameterData(TResponse value);
    }
    public interface IRDMGetParameterWrapperWithEmptyGetResponse : IRDMParameterWrapper
    {
        RDMMessage BuildGetResponseMessage();
    }
    #endregion
    #endregion




    public interface IRDMSetParameterWrapper<TRequest, TResponse> : IRDMSetParameterWrapperRequest<TRequest>, IRDMSetParameterWrapperResponse<TResponse>
    {
        ERDM_SupportedSubDevice SupportedSetSubDevices { get; }
    }
    #region Request
    public interface IRDMSetParameterWrapperRequest
    {
        Type SetRequestType { get; }
        RDMMessage BuildSetRequestMessage(object value);
        object SetRequestParameterDataToObject(byte[] parameterData);
        byte[] SetRequestObjectToParameterData(object value);
    }
    public interface IRDMSetParameterWrapperRequest<TRequest> : IRDMSetParameterWrapperRequestContravariance<TRequest>
    {
        TRequest SetRequestParameterDataToValue(byte[] parameterData);
    }
    public interface IRDMSetParameterWrapperWithEmptySetRequest : IRDMParameterWrapper
    {
        RDMMessage BuildSetRequestMessage();
    }
    #region Response
    public interface IRDMSetParameterWrapperResponse : IRDMParameterWrapper
    {
        Type SetResponseType { get; }
        RDMMessage BuildSetResponseMessage(object value);
        object SetResponseParameterDataToObject(byte[] parameterData);
        byte[] SetResponseObjectToParameterData(object value);
    }
    public interface IRDMSetParameterWrapperResponse<TResponse> : IRDMSetParameterWrapperSetResponseContravariance<TResponse>
    {
        TResponse SetResponseParameterDataToValue(byte[] parameterData);
    }
    public interface IRDMSetParameterWrapperWithEmptySetResponse : IRDMParameterWrapper
    {
        RDMMessage BuildSetResponseMessage();
    }
    #endregion
    #endregion
    /// <summary>
    /// A Parameter with this Interface is used as Blueprint.
    /// This means that the parameter contains information that is identical for every device of the same model.
    /// </summary>
    public interface IRDMBlueprintParameterWrapper : IRDMParameterWrapper
    {
    }
    public interface IRDMBlueprintDescriptionListParameterWrapper : IRDMBlueprintParameterWrapper, IRDMDescriptionParameterWrapper
    {
    }
    public interface IRDMDescriptionParameterWrapper : IRDMParameterWrapper, IRDMGetParameterWrapperRequest, IRDMGetParameterWrapperResponse
    {
        ERDM_Parameter ValueParameterID { get; }
    }


    public interface IRDMManufacturerParameterWrapper : IRDMParameterWrapper
    {
        EManufacturer Manufacturer { get; }
    }
    public interface IRDMDeviceModelIdParameterWrapper : IRDMManufacturerParameterWrapper
    {
        ushort[] DeviceModelIds { get; }
    }
    public interface IRDMSetParameterWrapperRequestContravariance<in TRequest> : IRDMSetParameterWrapperRequest
    {
        RDMMessage BuildSetRequestMessage(TRequest value);
        byte[] SetRequestValueToParameterData(TRequest value);
    }

    public interface IRDMSetParameterWrapperSetResponseContravariance<in TResponse>: IRDMSetParameterWrapperResponse
    {
        RDMMessage BuildSetResponseMessage(TResponse value);
        byte[] SetResponseValueToParameterData(TResponse value);
    }
}