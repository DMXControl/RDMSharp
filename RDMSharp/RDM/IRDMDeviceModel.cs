using RDMSharp.RDM.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static RDMSharp.AbstractRDMCache;

namespace RDMSharp;

public interface IRDMDeviceModel : IDisposable
{
    IReadOnlyCollection<SupportedParameterMetadata> GetSupportedParameters();
    IReadOnlyCollection<SupportedParameterMetadata> GetSupportedBlueprintModelParameters();
    IReadOnlyCollection<SupportedParameterMetadata> GetSupportedBlueprintModelPersonalityParameters();
    IReadOnlyCollection<SupportedParameterMetadata> GetSupportedNonBlueprintParameters();
    IReadOnlyCollection<SupportedParameterMetadata> GetKnownNotSupportedParameters();
    IReadOnlyCollection<RDMSensorDefinition> GetSensorDefinitions();
    IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues { get; }

    event EventHandler Initialized;
    event PropertyChangedEventHandler PropertyChanged;
    event EventHandler<ParameterValueAddedEventArgs> ParameterValueAdded;

    RDMDeviceInfo DeviceInfo { get; }

    bool IsDisposing { get; }
    bool IsDisposed { get; }
    bool IsModelOf(UID uid, SubDevice subDevice, RDMDeviceInfo other);
}
