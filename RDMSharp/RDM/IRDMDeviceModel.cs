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
        IReadOnlyCollection<RDMSensorDefinition> GetSensorDefinitions();
        IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues { get; }

        bool IsDisposing { get; }
        bool IsDisposed { get; }
        bool IsModelOf(UID uid, RDMDeviceInfo other);
    }
}
