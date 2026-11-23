using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>A parameter with one string field, built from its define.</summary>
public sealed class GenericStringModule : AbstractSimpleStringModule, IGenericModule
{
    public MetadataJSONObjectDefine Define { get; }
    public override string DisplayName { get; }
    public uint MinLength { get; }
    public uint MaxLength { get; }

    public new string Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    internal GenericStringModule(AbstractRemoteRDMDevice remoteDevice, MetadataJSONObjectDefine define, ERDM_Parameter parameter, ERDM_CommandClass commandClass, StringType field)
        : base(remoteDevice, GenericModuleFactory.ModuleName(define, parameter), parameter, commandClass)
    {
        Define = define;
        DisplayName = GenericModuleFactory.DisplayNameOf(define);
        MinLength = field?.MinLength ?? 0;
        MaxLength = field?.MaxLength ?? 32;
    }

    public Task<bool> SetValueAsync(string value) => SetValue(value);
}
