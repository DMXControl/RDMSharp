using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Generic;

namespace RDMSharp.Extensions;

public interface IModulesExtension
{
    string Key { get; }
    EManufacturer Manufacturer { get; }

    bool TryGetModules(ERDM_Parameter[] parameters, out IReadOnlyCollection<Type> modules);
    bool TryCreateModuleInstance(Type moduleType, IRDMRemoteDevice remoteDevice, out IModule moduleInstance);
}