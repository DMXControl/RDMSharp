using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>A parameter with one boolean field, built from its define.</summary>
public sealed class GenericBooleanModule : AbstractSimpleBooleanParameterIDModule, IGenericModule
{
    public MetadataJSONObjectDefine Define { get; }
    public override string DisplayName { get; }
    /// <summary>The texts of the define for false and true, <c>null</c> if it has none.</summary>
    public string FalseLabel { get; }
    public string TrueLabel { get; }

    public new bool? Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    internal GenericBooleanModule(AbstractRemoteRDMDevice remoteDevice, MetadataJSONObjectDefine define, ERDM_Parameter parameter, ERDM_CommandClass commandClass, BooleanType field)
        : base(remoteDevice, GenericModuleFactory.ModuleName(define, parameter), parameter, commandClass)
    {
        Define = define;
        DisplayName = GenericModuleFactory.DisplayNameOf(define);
        FalseLabel = field?.Labels?.FirstOrDefault(l => !l.Value)?.Name;
        TrueLabel = field?.Labels?.FirstOrDefault(l => l.Value)?.Name;
    }

    public Task<bool> SetValueAsync(bool value) => SetValue(value);
}
