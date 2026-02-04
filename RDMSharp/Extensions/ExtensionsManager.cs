using Microsoft.Extensions.Logging;
using RDMSharp.RDM.Device;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.Extensions;

internal class ExtensionsManager
{
    private readonly ILogger Logger;
    private static ExtensionsManager _instance;
    public static ExtensionsManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ExtensionsManager();
            return _instance;
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

    public bool TryGetBytesParser(string formatIdentifyer, out IBytesParser? bytesParser)
    {
        if (string.IsNullOrWhiteSpace(formatIdentifyer))
        {
            bytesParser = null;
            return false;
        }

        if (bytesParsers.IsEmpty)
            LoadBytesParsersFromAssemblies();

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
        try
        {
            if (supportedParametersExtensions.IsEmpty)
                LoadSupportedParametersExtension();

            List<ISupportedParametersExtension> result = new();

            foreach (var spe in supportedParametersExtensions.Values)
            {
                if (spe.Manufacturer == EManufacturer.ESTA)
                    result.Add(spe);
                if (spe.Manufacturer == manufacturer)
                    result.Add(spe);
            }
            supportedParametersExtensionsResult = result;
            return true;
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error while getting SupportedParametersExtensions");
            supportedParametersExtensionsResult = Array.Empty<ISupportedParametersExtension>();
            return false;
        }
    }
    public bool TryGetSupportedParameterMetadata(EManufacturer manufacturer, ERDM_Parameter parameter, out SupportedParameterMetadata? supportedParameterMetadata)
    {
        try
        {
            if (supportedParametersExtensions.IsEmpty)
                LoadSupportedParametersExtension();

            bool isBlueprintModelParameter = false;
            bool isBlueprintModelPersonalityParameter = false;
            bool isManufacturerInternalParameter = false;
            string name = null;
            int updateTimeMilliseconds = -1;
            foreach (var spe in supportedParametersExtensions.Values)
            {
                if (spe.Manufacturer != EManufacturer.ESTA && spe.Manufacturer != manufacturer)
                    continue;

                isBlueprintModelParameter |= spe.BlueprintModelParameters.Contains(parameter);
                isBlueprintModelPersonalityParameter |= spe.BlueprintModelPersonalityParameters.Contains(parameter);
                isManufacturerInternalParameter |= spe.ManufacturerInternalParameters.Contains(parameter);

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
        try
        {
            if (modulesExtensions.IsEmpty)
                LoadModulesExtension();

            List<IModulesExtension> result = new();

            foreach (var spe in modulesExtensions.Values)
            {
                if (spe.Manufacturer == EManufacturer.ESTA)
                    result.Add(spe);
                if (spe.Manufacturer == manufacturer)
                    result.Add(spe);
            }
            modulesExtensionsResult = result;
            return true;
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error while getting ModulesExtensions");
            modulesExtensionsResult = Array.Empty<IModulesExtension>();
            return false;
        }
    }
}