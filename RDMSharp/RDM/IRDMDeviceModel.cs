using System;
using System.Collections.Generic;
using System.ComponentModel;
using static RDMSharp.AbstractRDMCache;

namespace RDMSharp;

public interface IRDMDeviceModel : IDisposable
{
    IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get; }
    IReadOnlyCollection<ERDM_Parameter> SupportedBlueprintParameters { get; }
    IReadOnlyCollection<ERDM_Parameter> SupportedNonBlueprintParameters { get; }
    IReadOnlyCollection<ERDM_Parameter> KnownNotSupportedParameters { get; }
    IReadOnlyCollection<RDMSensorDefinition> GetSensorDefinitions();
    IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues { get; }

    event EventHandler Initialized;
    event PropertyChangedEventHandler PropertyChanged;
    event EventHandler<ParameterValueAddedEventArgs> ParameterValueAdded;

    bool IsDisposing { get; }
    bool IsDisposed { get; }
    bool IsModelOf(UID uid, SubDevice subDevice, RDMDeviceInfo other);
}
