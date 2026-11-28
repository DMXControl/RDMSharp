using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// IDENTIFY_TIMEOUT (E1.37-5 4.1): time in seconds after which IDENTIFY_DEVICE stops by itself.
/// <list type="bullet">
/// <item>0x0000 disables the timeout, Identify stays on until it is switched off (default, backwards compatible).</item>
/// <item>GET returns the configured timeout, not the remaining time.</item>
/// <item>After the timeout Identify is switched off, which is queued as IDENTIFY_DEVICE off.</item>
/// <item>A change during a running Identify takes effect immediately: a reduced timeout that is already over ends Identify,
/// an increased timeout extends it.</item>
/// </list>
/// On a generated device the module watches the <see cref="IdentifyDeviceModule"/> of the same device.
/// </summary>
public sealed class IdentifyTimeoutModule : AbstractSimpleNumericParameterIDModule<ushort>
{
    private const string _moduleName = "IdentifyTimeout";
    private const string _moduleDisplayName = "Identify Timeout";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.IDENTIFY_TIMEOUT;
    public const ushort Disabled = 0;

    public sealed override string DisplayName => _moduleDisplayName;

    private readonly object timerLock = new object();
    private IdentifyDeviceModule identifyModule;
    private Stopwatch identifyRunning;
    private CancellationTokenSource timeoutCancellation;

    /// <summary>
    /// The configured timeout in seconds, 0 disables the timeout.
    /// </summary>
    public ushort? TimeoutSeconds
    {
        get
        {
            return base.Value;
        }
        set
        {
            base.Value = value;
        }
    }

    public IdentifyTimeoutModule(ushort initialTimeoutSeconds = Disabled) : base(
        initialTimeoutSeconds,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public IdentifyTimeoutModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        base.OnParentGeneratedDeviceChanged(device);
        identifyModule = device.Modules.OfType<IdentifyDeviceModule>().FirstOrDefault();
        if (identifyModule is null)
            return;
        identifyModule.PropertyChanged += IdentifyModule_PropertyChanged;
        if (identifyModule.Identify == true)
            identifyStarted();
    }

    private void IdentifyModule_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IdentifyDeviceModule.Identify))
            return;

        lock (timerLock)
        {
            bool identify = identifyModule.Identify == true;
            if (identify && identifyRunning is null)
                identifyStarted();
            else if (!identify && identifyRunning is not null)
                identifyStopped();
        }
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        base.ParameterChanged(parameter, newValue, index);
        if (parameter == ModuleParameter && ParentGeneratedDevice is not null)
            lock (timerLock)
                if (identifyRunning is not null)
                    scheduleTimeout(); // the new timeout applies to the running Identify
    }

    private void identifyStarted()
    {
        lock (timerLock)
        {
            identifyRunning = Stopwatch.StartNew();
            scheduleTimeout();
        }
    }
    private void identifyStopped()
    {
        lock (timerLock)
        {
            identifyRunning = null;
            cancelTimeout();
        }
    }
    private void cancelTimeout()
    {
        timeoutCancellation?.Cancel();
        timeoutCancellation?.Dispose();
        timeoutCancellation = null;
    }

    /// <summary>
    /// (Re)schedules the end of the running Identify based on the time Identify is already running.
    /// </summary>
    private void scheduleTimeout()
    {
        cancelTimeout();
        ushort seconds = TimeoutSeconds ?? Disabled;
        if (seconds == Disabled || identifyRunning is null)
            return;

        TimeSpan remaining = TimeSpan.FromSeconds(seconds) - identifyRunning.Elapsed;
        if (remaining <= TimeSpan.Zero)
        {
            endIdentify();
            return;
        }

        var cancellation = new CancellationTokenSource();
        timeoutCancellation = cancellation;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(remaining, cancellation.Token);
                lock (timerLock)
                {
                    if (cancellation.IsCancellationRequested)
                        return;
                    endIdentify();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger?.LogError(e);
            }
        });
    }

    private void endIdentify()
    {
        var device = ParentGeneratedDevice;
        if (device is null || device.IsDisposing || device.IsDisposed || identifyModule is null)
            return;
        identifyRunning = null;
        timeoutCancellation = null;
        try
        {
            identifyModule.Identify = false; // queued as IDENTIFY_DEVICE off by the generated device
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(TimeoutSeconds));
    }

    public async Task<bool> SetTimeoutSeconds(ushort timeoutSeconds)
    {
        return await base.SetValue(timeoutSeconds);
    }
}
