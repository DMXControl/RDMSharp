using RDMSharp.Metadata;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>
/// A SET only parameter without request data, built from its define: an action (e.g. "Recalibrate Fixture", "Wireless Link").
/// It has no value, <see cref="PerformAsync"/> sends the SET.
/// Unlike the other generic modules it is also created for manufacturer internal parameters (e.g. SGM "Internal Test Pixel"),
/// an application shows those apart (<see cref="IsManufacturerInternal"/>).
/// </summary>
public sealed class GenericActionModule : AbstractParameterIDModule, IGenericModule
{
    public override string DisplayName { get; }
    public ERDM_Parameter Parameter { get; }
    public MetadataJSONObjectDefine Define { get; }
    public ERDM_CommandClass SupportedCommandClass => ERDM_CommandClass.SET;
    /// <summary>A parameter the manufacturer marks as internal (service, test, ...).</summary>
    public bool IsManufacturerInternal { get; }

    internal GenericActionModule(AbstractRemoteRDMDevice remoteDevice, MetadataJSONObjectDefine define, ERDM_Parameter parameter, bool isManufacturerInternal)
        : base(remoteDevice, GenericModuleFactory.ModuleName(define, parameter), parameter)
    {
        Define = define;
        Parameter = parameter;
        DisplayName = GenericModuleFactory.DisplayNameOf(define);
        IsManufacturerInternal = isManufacturerInternal;
    }

    /// <summary>Sends the SET (without data).</summary>
    public Task<bool> PerformAsync() => ParentRemoteDevice is null ? Task.FromResult(false) : ParentRemoteDevice.SetParameter(Parameter);

    /// <summary>The define is a SET without request data and without GET: an action.</summary>
    internal static bool IsAction(MetadataJSONObjectDefine define)
        => define is not null && !define.GetRequest.HasValue && define.SetRequest.HasValue && define.SetRequest.Value.GetIsEmpty();
}
