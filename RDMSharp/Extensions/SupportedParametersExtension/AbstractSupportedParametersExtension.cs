using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RDMSharp.Extensions.SupportedParametersExtension;

public abstract class AbstractSupportedParametersExtension : ISupportedParametersExtension
{
    protected ILogger Logger;
    public abstract string Key { get; }
    public abstract EManufacturer Manufacturer { get; }
    public abstract ERDM_Parameter[] BlueprintModelParameters { get; }
    public abstract ERDM_Parameter[] BlueprintModelPersonalityParameters { get; }

    private ConcurrentDictionary<IRDMDeviceModel, Func<ERDM_Parameter[], Task>> _handlers = new();

    protected AbstractSupportedParametersExtension()
    {
        Logger = Logging.CreateLogger(Key);
    }

    protected void registerHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler)
    {
        _handlers.TryAdd(deviceModel, handler);
    }
    protected void unregisterHandler(IRDMDeviceModel deviceModel)
    {
        _handlers.TryRemove(deviceModel, out _);
    }
    protected Func<ERDM_Parameter[], Task> getHandler(IRDMDeviceModel deviceModel)
    {
        _handlers.TryGetValue(deviceModel, out var handler);
        return handler;
    }

    public async Task RegisterAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler)
    {
        registerHandler(deviceModel, handler);
        await registerAddSupportedParametersHandler(deviceModel, handler);
    }
    protected abstract Task registerAddSupportedParametersHandler(IRDMDeviceModel deviceModel, Func<ERDM_Parameter[], Task> handler);
}