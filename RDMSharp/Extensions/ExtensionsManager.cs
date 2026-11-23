using Microsoft.Extensions.Logging;
using RDMSharp.RDM.Device;
using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.Extensions;

internal class ExtensionsManager
{
    private readonly ILogger Logger;
    // Lazy: the first access can come from several threads at once (e.g. parallel device initialization)
    private static readonly Lazy<ExtensionsManager> _instance = new(() => new ExtensionsManager(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
    public static ExtensionsManager Instance => _instance.Value;

    private readonly object loadLock = new();
    private bool bytesParsersLoaded;
    private bool supportedParametersExtensionsLoaded;
    private bool modulesExtensionsLoaded;

    /// <summary>
    /// Loads the extensions once, if none were registered before. Other threads wait until the loading is complete,
    /// so they never see a partly filled dictionary.
    /// </summary>
    private void ensureLoaded(ref bool loaded, Func<bool> isEmpty, Action load)
    {
        if (System.Threading.Volatile.Read(ref loaded))
            return;
        lock (loadLock)
        {
            if (loaded)
                return;
            if (isEmpty())
                load();
            System.Threading.Volatile.Write(ref loaded, true);
        }
    }

    private readonly ConcurrentDictionary<string, IBytesParser> bytesParsers = new();
    private readonly ConcurrentDictionary<string, ISupportedParametersExtension> supportedParametersExtensions = new();
    private readonly ConcurrentDictionary<string, IModulesExtension> modulesExtensions = new();

    private ExtensionsManager()
    {
        Logger = Logging.CreateLogger<ExtensionsManager>();
    }

    private void LoadBytesParsersFromAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (typeof(IBytesParser).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        if (Activator.CreateInstance(type) is IBytesParser parser)
                            RegisterBytesParser(parser);
            }
            catch
            {
                // Ignore assemblies that can't be loaded
            }
        }
    }

    public void RegisterBytesParser(IBytesParser bytesParser)
    {
        if (!bytesParsers.ContainsKey(bytesParser.FormatIdentifyer))
            if (bytesParsers.TryAdd(bytesParser.FormatIdentifyer, bytesParser))
                Logger?.LogInformation($"Registered BytesParser for format '{bytesParser.FormatIdentifyer}'");
    }

    public bool TryGetBytesParser(string formatIdentifyer, out IBytesParser bytesParser)
    {
        if (string.IsNullOrWhiteSpace(formatIdentifyer))
        {
            bytesParser = null;
            return false;
        }

        ensureLoaded(ref bytesParsersLoaded, () => bytesParsers.IsEmpty, LoadBytesParsersFromAssemblies);

        return bytesParsers.TryGetValue(formatIdentifyer, out bytesParser);
    }

    private void LoadSupportedParametersExtension()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (typeof(ISupportedParametersExtension).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        if (Activator.CreateInstance(type) is ISupportedParametersExtension supportedParametersExtension)
                            RegisterSupportedParametersExtension(supportedParametersExtension);
            }
            catch
            {
                // Ignore assemblies that can't be loaded
            }
        }
    }

    public void RegisterSupportedParametersExtension(ISupportedParametersExtension supportedParametersExtension)
    {
        if (!supportedParametersExtensions.ContainsKey(supportedParametersExtension.Key))
            if (supportedParametersExtensions.TryAdd(supportedParametersExtension.Key, supportedParametersExtension))
                Logger?.LogInformation($"Registered SupportedParametersExtension with key '{supportedParametersExtension.Key}'");
    }

    public bool TryGetSupportedParametersExtensions(EManufacturer manufacturer, out IReadOnlyCollection<ISupportedParametersExtension> supportedParametersExtensionsResult)
    {
        ensureLoaded(ref supportedParametersExtensionsLoaded, () => supportedParametersExtensions.IsEmpty, LoadSupportedParametersExtension);

        supportedParametersExtensionsResult = extensionsFor(manufacturer).ToList();
        return true;
    }

    /// <summary>
    /// The extensions for a manufacturer: the ESTA extensions first, so their result counts first (the order of the dictionary is undefined).
    /// </summary>
    private IEnumerable<ISupportedParametersExtension> extensionsFor(EManufacturer manufacturer)
        => supportedParametersExtensions.Values
            .Where(spe => spe.Manufacturer == EManufacturer.ESTA || spe.Manufacturer == manufacturer)
            .OrderBy(spe => spe.Manufacturer == EManufacturer.ESTA ? 0 : 1)
            .ThenBy(spe => spe.Key, StringComparer.Ordinal);

    /// <summary>E1.20: 0x8000 - 0xFFDF are the manufacturer specific PIDs.</summary>
    private static bool isManufacturerSpecific(ERDM_Parameter parameter)
        => (ushort)parameter >= 0x8000 && (ushort)parameter <= 0xFFDF;
    public bool TryGetSupportedParameterMetadata(EManufacturer manufacturer, ERDM_Parameter parameter, out SupportedParameterMetadata supportedParameterMetadata)
    {
        try
        {
            ensureLoaded(ref supportedParametersExtensionsLoaded, () => supportedParametersExtensions.IsEmpty, LoadSupportedParametersExtension);

            bool isBlueprintModelParameter = false;
            bool isBlueprintModelPersonalityParameter = false;
            bool isManufacturerInternalParameter = false;
            string name = null;
            int updateTimeMilliseconds = -1;
            foreach (var spe in extensionsFor(manufacturer))
            {
                isBlueprintModelParameter |= spe.BlueprintModelParameters.Contains(parameter);
                isBlueprintModelPersonalityParameter |= spe.BlueprintModelPersonalityParameters.Contains(parameter);
                isManufacturerInternalParameter |= spe.ManufacturerInternalParameters.Contains(parameter);

                // A manufacturer extension describes its own (manufacturer specific) parameters only. Its defaults are meant for those,
                // for an ESTA parameter (e.g. SENSOR_VALUE) they would replace the update time of the standard (seen: 10 min instead of 20 s).
                if (spe.Manufacturer != EManufacturer.ESTA && !isManufacturerSpecific(parameter))
                    continue;

                if (name == null && spe.TryGetParameterName(parameter, out var paramName))
                    name = paramName;
                if (updateTimeMilliseconds < 0 && spe.TryGetParameterUpdateTimeMilliseconds(parameter, out var paramUpdateTime))
                    updateTimeMilliseconds = paramUpdateTime;
            }

            if (isBlueprintModelParameter)
                supportedParameterMetadata = SupportedParameterMetadata.CreateBlueprintModel(parameter, isManufacturerInternalParameter);
            else if (isBlueprintModelPersonalityParameter)
                supportedParameterMetadata = SupportedParameterMetadata.CreateBlueprintModelPersonality(parameter, isManufacturerInternalParameter);
            else
                supportedParameterMetadata = SupportedParameterMetadata.Create(parameter, isManufacturerInternalParameter);


            supportedParameterMetadata.SetName(name);
            supportedParameterMetadata.SetParameterUpdateTime(updateTimeMilliseconds);

            return true;
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error while getting SupportedParametersExtensions");
            supportedParameterMetadata = null;
            return false;
        }
    }

    private void LoadModulesExtension()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (typeof(IModulesExtension).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        if (Activator.CreateInstance(type) is IModulesExtension modulesExtension)
                            RegisterModulesExtension(modulesExtension);
            }
            catch
            {
                // Ignore assemblies that can't be loaded
            }
        }
    }
    public void RegisterModulesExtension(IModulesExtension modulesExtension)
    {
        if (!modulesExtensions.ContainsKey(modulesExtension.Key))
            if (modulesExtensions.TryAdd(modulesExtension.Key, modulesExtension))
                Logger?.LogInformation($"Registered ModulesExtension with key '{modulesExtension.Key}'");
    }
    public bool TryGetModulesExtensions(EManufacturer manufacturer, out IReadOnlyCollection<IModulesExtension> modulesExtensionsResult)
    {
        ensureLoaded(ref modulesExtensionsLoaded, () => modulesExtensions.IsEmpty, LoadModulesExtension);

        List<IModulesExtension> result = new();

        foreach (var spe in modulesExtensions.Values)
            if (spe.Manufacturer == EManufacturer.ESTA || spe.Manufacturer == manufacturer)
                result.Add(spe);

        modulesExtensionsResult = result;
        return true;
    }

    public bool TryGetMatchingModules(AbstractRemoteRDMDevice device, out IReadOnlyCollection<IModule> modulesResult)
        => TryGetMatchingModules(device, null, out modulesResult);

    /// <summary>
    /// Creates the modules of the extensions that match the supported parameters of <paramref name="device"/>,
    /// except the module types in <paramref name="existingTypes"/> (a module can do something when it is created, e.g. send a SET).
    /// </summary>
    public bool TryGetMatchingModules(AbstractRemoteRDMDevice device, IReadOnlyCollection<Type> existingTypes, out IReadOnlyCollection<IModule> modulesResult)
    {
        List<IModule> result = new();
        try
        {
            if (this.TryGetModulesExtensions((EManufacturer)device.UID.ManufacturerID, out IReadOnlyCollection<IModulesExtension> modulesExtensionsResult))
                foreach (var modulesExtension in modulesExtensionsResult)
                    if (modulesExtension.TryGetModules(device.DeviceModel.GetSupportedParameters().Select(spm => spm.Parameter).ToArray(), out IReadOnlyCollection<Type> modulTypes))
                        foreach (var modulType in modulTypes)
                            if (existingTypes?.Contains(modulType) != true && modulesExtension.TryCreateModuleInstance(modulType, device, out IModule moduleInstance))
                                result.Add(moduleInstance);
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }
        modulesResult = result.AsReadOnly();
        return result.Count != 0;
    }
}