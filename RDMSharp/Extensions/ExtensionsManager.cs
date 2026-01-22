using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

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

}
