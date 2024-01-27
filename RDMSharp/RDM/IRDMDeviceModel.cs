using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public interface IRDMDeviceModel : IDisposable
    {
        IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get; }
        IReadOnlyCollection<ERDM_Parameter> SupportedBlueprintParameters { get; }
        IReadOnlyCollection<ERDM_Parameter> SupportedNonBlueprintParameters { get; }
        IReadOnlyCollection<ERDM_Parameter> KnownNotSupportedParameters { get; }
        RDMSensorDefinition[] GetSensorDefinitions();
        IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues { get; }

        IRDMParameterWrapper GetRDMParameterWrapperByID(ushort parameter);

        bool IsDisposing { get; }
        bool IsDisposed { get; }
    }
}
