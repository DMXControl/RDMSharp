using RDMSharp.Metadata;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>
/// A module that is built from the JSON define of a parameter, not written for it by hand.
/// It covers simple parameters (one boolean, integer or string) that no specific module handles,
/// so an application can show them, and mark them as generic.
/// </summary>
public interface IGenericModule : IParameterIDModule
{
    /// <summary>The name to show: displayName of the define, else its name.</summary>
    string DisplayName { get; }
    /// <summary>The parameter the module is built for.</summary>
    ERDM_Parameter Parameter { get; }
    /// <summary>The define the module is built from.</summary>
    MetadataJSONObjectDefine Define { get; }
    /// <summary>What the device supports for the parameter (GET and/or SET), from the define.</summary>
    ERDM_CommandClass SupportedCommandClass { get; }
}
