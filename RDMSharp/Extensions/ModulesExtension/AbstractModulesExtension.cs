using Microsoft.Extensions.Logging;
using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Generic;

namespace RDMSharp.Extensions.ModulesExtension;

public abstract class AbstractModulesExtension : IModulesExtension
{
    protected ILogger Logger;
    public abstract string Key { get; }
    public abstract EManufacturer Manufacturer { get; }

    public AbstractModulesExtension()
    {
        Logger = Logging.CreateLogger(this.Key);
    }

    public abstract bool TryGetModules(ERDM_Parameter[] parameters, out IReadOnlyCollection<Type> modules);

    public virtual bool TryCreateModuleInstance(Type moduleType, IRDMRemoteDevice remoteDevice, out IModule moduleInstance)
    {
        try
        {
            moduleInstance = Activator.CreateInstance(moduleType, remoteDevice) as IModule
                ?? throw new InvalidOperationException($"Could not create instance of module type '{moduleType.FullName}'");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating module instance of type '{ModuleType}'", moduleType.FullName);
            moduleInstance = null;
            return false;
        }
    }
}