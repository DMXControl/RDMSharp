using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractParameterIDModule : IParameterIDModule, IDisposable
{
    protected static ILogger Logger = Logging.CreateLogger<AbstractParameterIDModule>();

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly string _name;
    public string Name { get => _name; }
    public abstract string DisplayName { get; }

    private readonly IReadOnlyCollection<ERDM_Parameter> _supportedParameters;

    public IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get => _supportedParameters; }

    protected AbstractGeneratedRDMDevice ParentGeneratedDevice { get; private set; }
    protected AbstractRemoteRDMDevice ParentRemoteDevice { get; private set; }
    protected AbstractRDMDevice ParentDevice
    {
        get
        {
            if (ParentRemoteDevice is not null)
                return ParentRemoteDevice;

            return ParentGeneratedDevice;
        }
    }

    /// <summary>
    /// The device supports <paramref name="parameter"/>: a module can cover parameters the device does not all support
    /// (e.g. CAPTURE_PRESET without PRESET_PLAYBACK). A generated device supports every parameter of its modules.
    /// </summary>
    public bool IsSupportedByDevice(ERDM_Parameter parameter)
    {
        if (ParentRemoteDevice is AbstractRemoteRDMDevice remote)
            return remote.DeviceModel?.GetSupportedParameters()?.Any(spm => spm.Parameter == parameter) == true;
        return _supportedParameters.Contains(parameter);
    }

    protected AbstractParameterIDModule(string name, params ERDM_Parameter[] supportedParameters)
    {
        this._name = name;
        this._supportedParameters = supportedParameters;
    }
    protected AbstractParameterIDModule(AbstractRemoteRDMDevice remoteDevice, string name, params ERDM_Parameter[] supportedParameters) : this(name, supportedParameters)
    {
        ParentRemoteDevice = remoteDevice;
        if (remoteDevice is AbstractRemoteRDMDevice device)
            SetRemoteParentDevice(device);
    }
    public RDMMessage HandleRequest(RDMMessage message)
    {
        if (!this.IsHandlingParameter(message.Parameter, message.Command))
            return null;

        ERDM_NackReason? nackReason;
        try
        {
            return handleRequest(message);
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex, "Error handling request for parameter {Parameter} with command {Command}", message.Parameter, message.Command);
            nackReason = ERDM_NackReason.HARDWARE_FAULT;
        }
        return new RDMMessage(nackReason ?? ERDM_NackReason.UNKNOWN_PID)
        {
            DestUID = message.SourceUID,
            SourceUID = message.DestUID,
            // the response to the command of the request (a SET is not answered as a GET)
            Command = message.Command | ERDM_Command.RESPONSE,
            Parameter = message.Parameter
        };
    }
    protected virtual RDMMessage handleRequest(RDMMessage message)
    {
        throw new System.NotImplementedException("This method should be overridden in derived classes to handle specific requests.");
    }

    public virtual bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        return false;
    }

    internal void SetGeneratedParentDevice(AbstractGeneratedRDMDevice device)
    {
        if (ParentGeneratedDevice is not null)
            return;
        ParentGeneratedDevice = device;
        device.ParameterValueAdded += Device_ParameterValueAdded;
        device.ParameterValueChanged += Device_ParameterValueChanged;
        OnParentGeneratedDeviceChanged(ParentGeneratedDevice);
    }
    private void SetRemoteParentDevice(AbstractRemoteRDMDevice device)
    {
        device.ParameterValueAdded += Device_ParameterValueAdded;
        device.ParameterValueChanged += Device_ParameterValueChanged;
        OnRemoteParentDeviceChanged(device);
    }

    protected virtual void OnRemoteParentDeviceChanged(AbstractRemoteRDMDevice device)
    {

    }

    /// <summary>
    /// Hands the values the device already has to <see cref="ParameterChanged"/>, once the module is completely constructed.
    /// A module is created after the device answered most of its parameters: without this it would only learn them when they change.
    /// </summary>
    internal void ApplyCachedValues()
    {
        if (ParentRemoteDevice is null || IsDisposed)
            return;
        foreach (var parameter in SupportedParameters)
        {
            if (!ParentRemoteDevice.ParameterValues.TryGetValue(parameter, out object value))
                continue;
            try
            {
                if (value is ConcurrentDictionary<object, object> indexed)
                    foreach (var pair in indexed)
                        ParameterChanged(parameter, pair.Value, pair.Key);
                else
                    ParameterChanged(parameter, value, null);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error applying the cached value of {parameter} to module {Name}");
            }
        }
    }

    private void Device_ParameterValueAdded(object sender, AbstractRDMCache.ParameterValueAddedEventArgs e)
    {
        if (!this.SupportedParameters.Contains(e.Parameter))
            return;

        ParameterChanged(e.Parameter, e.Value, e.Index);
    }
    private void Device_ParameterValueChanged(object sender, AbstractRDMCache.ParameterValueChangedEventArgs e)
    {
        ParameterChanged(e.Parameter, e.NewValue, e.Index);
    }
    protected virtual void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {

    }

    protected virtual void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {

    }

    /// <summary>
    /// Runs asynchronous work started from a synchronous override (e.g. setting the initial value) in the background.
    /// An exception is logged: from an <c>async void</c> method it would terminate the host application.
    /// </summary>
    protected void RunSafe(System.Func<System.Threading.Tasks.Task> work, [CallerMemberName] string caller = null)
    {
        _ = runSafe(work, caller);
    }
    private async System.Threading.Tasks.Task runSafe(System.Func<System.Threading.Tasks.Task> work, string caller)
    {
        try
        {
            await work();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, $"Error in {GetType().Name}.{caller}");
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool IsDisposed { get; private set; }
    /// <summary>
    /// Called by the device when it is disposed or replaces its modules: the module stops listening to the device
    /// and releases what it subscribed to (e.g. timers), so it does not keep working for a device that is gone.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        if (ParentRemoteDevice is not null)
        {
            ParentRemoteDevice.ParameterValueAdded -= Device_ParameterValueAdded;
            ParentRemoteDevice.ParameterValueChanged -= Device_ParameterValueChanged;
        }
        if (ParentGeneratedDevice is not null)
        {
            ParentGeneratedDevice.ParameterValueAdded -= Device_ParameterValueAdded;
            ParentGeneratedDevice.ParameterValueChanged -= Device_ParameterValueChanged;
        }
        try
        {
            OnDispose();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, $"Error disposing module {Name}");
        }
        PropertyChanged = null;
        GC.SuppressFinalize(this);
    }
    /// <summary>Release what the module subscribed to.</summary>
    protected virtual void OnDispose()
    {
    }
}