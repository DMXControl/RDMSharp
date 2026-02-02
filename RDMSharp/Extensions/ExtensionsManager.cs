using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
}