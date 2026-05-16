using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RDMSharp.Tests")]
namespace RDMSharp;

public class GlobalTimers
{
    private static readonly ILogger Logger = Logging.CreateLogger<GlobalTimers>();
    private static GlobalTimers instance = null;
    public static GlobalTimers Instance
    {
        get
        {
            if (instance == null)
                instance = new GlobalTimers();
            return instance;
        }
    }

    public const int DefaultQueuedUpdateTime = 10000; // 10 seconds
    public const int DefaultNonQueuedUpdateTime = 60000; // 60 seconds
    public const int DefaultUpdateDelayBetweenRequests = 50; // 50 milliseconds
    public const int DefaultUpdateDelayBetweenQueuedUpdateRequests = 200; // 200 milliseconds
    public const int DefaultUpdateDelayBetweenNonQueuedUpdateRequests = 500; // 500 milliseconds
    public const int DefaultPresentLostTime = 15000; // 15 seconds

    public const int DefaultDiscoveryTimeout = 5; // 5 milliseconds

    public const int DefaultParameterUpdateTimerInterval = 7500; // 7.5 seconds
    public const int DefaultPresentUpdateTimerInterval = 500; // 0.5 seconds
    public const int DefaultRealTimeClockUpdateTimerInterval = 1800000; // 30 minutes
    public int QueuedUpdateTime { get; set; } = DefaultQueuedUpdateTime;
    public int NonQueuedUpdateTime { get; set; } = DefaultNonQueuedUpdateTime;
    public int UpdateDelayBetweenRequests { get; set; } = DefaultUpdateDelayBetweenRequests;
    public int UpdateDelayBetweenQueuedUpdateRequests { get; set; } = DefaultUpdateDelayBetweenQueuedUpdateRequests;
    public int UpdateDelayBetweenNonQueuedUpdateRequests { get; set; } = DefaultUpdateDelayBetweenNonQueuedUpdateRequests;
    public int PresentLostTime { get; set; } = DefaultPresentLostTime;

    public int DiscoveryTimeout { get; set; } = DefaultDiscoveryTimeout;

    private int parameterUpdateTimerInterval = DefaultParameterUpdateTimerInterval;
    public int ParameterUpdateTimerInterval
    {
        get
        {
            return parameterUpdateTimerInterval;
        }
        set
        {
            parameterUpdateTimerInterval = value;
            if (parameterUpdateTimer != null)
                parameterUpdateTimer.Interval = value;
        }
    }

    private int presentUpdateTimerInterval = DefaultPresentUpdateTimerInterval;
    public int PresentUpdateTimerInterval
    {
        get
        {
            return presentUpdateTimerInterval;
        }
        set
        {
            presentUpdateTimerInterval = value;
            if (presentUpdateTimer != null)
                presentUpdateTimer.Interval = value;
        }
    }

    private int realTimeClockUpdateTimerInterval = DefaultRealTimeClockUpdateTimerInterval;
    public int RealTimeClockUpdateTimerInterval
    {
        get
        {
            return realTimeClockUpdateTimerInterval;
        }
        set
        {
            realTimeClockUpdateTimerInterval = value;
            if (realTimeClockUpdateTimer != null)
                realTimeClockUpdateTimer.Interval = value;
        }
    }

    private System.Timers.Timer parameterUpdateTimer = null;
    private System.Timers.Timer presentUpdateTimer = null;
    private System.Timers.Timer realTimeClockUpdateTimer = null;

    private event EventHandler parameterUpdateTimerElapsed;
    public event EventHandler ParameterUpdateTimerElapsed
    {
        add
        {
            if (parameterUpdateTimer == null)
                initializeParameterUpdateTimer();

            parameterUpdateTimerElapsed += value;
        }
        remove
        {
            parameterUpdateTimerElapsed -= value;

            if (parameterUpdateTimer != null && parameterUpdateTimerElapsed == null)
                destroyParameterUpdateTimer();
        }
    }
    private event EventHandler presentUpdateTimerElapsed;
    public event EventHandler PresentUpdateTimerElapsed
    {
        add
        {
            if (presentUpdateTimerElapsed == null)
                initializePresentUpdateTimer();

            presentUpdateTimerElapsed += value;
        }
        remove
        {
            presentUpdateTimerElapsed -= value;

            if (presentUpdateTimerElapsed != null && presentUpdateTimerElapsed == null)
                destroyPresentUpdateTimer();
        }
    }

    private event EventHandler realTimeClockUpdateTimerElapsed;
    public event EventHandler RealTimeClockUpdateTimerElapsed
    {
        add
        {
            if (realTimeClockUpdateTimerElapsed == null)
                initializeRealTimeClockUpdateTimer();

            realTimeClockUpdateTimerElapsed += value;
        }
        remove
        {
            realTimeClockUpdateTimerElapsed -= value;

            if (realTimeClockUpdateTimerElapsed != null && realTimeClockUpdateTimerElapsed == null)
                destroyRealTimeClockUpdateTimer();
        }
    }
    private void initializeParameterUpdateTimer()
    {
        Logger?.LogInformation("InitializeParameterUpdateTimer");
        if (parameterUpdateTimer != null)
            return;
        parameterUpdateTimer = new System.Timers.Timer(ParameterUpdateTimerInterval);
        parameterUpdateTimer.Elapsed += ParameterUpdateTimer_Elapsed;
        parameterUpdateTimer.Enabled = true;
    }
    private void destroyParameterUpdateTimer()
    {
        Logger?.LogCritical("DestroyParameterUpdateTimer");
        if (parameterUpdateTimer == null)
            return;
        parameterUpdateTimer.Enabled = false;
        parameterUpdateTimer.Elapsed -= ParameterUpdateTimer_Elapsed;
        parameterUpdateTimer.Dispose();
        parameterUpdateTimer = null;
    }

    private void initializePresentUpdateTimer()
    {
        Logger?.LogInformation("InitializePresentUpdateTimer");
        if (parameterUpdateTimer != null)
            return;
        presentUpdateTimer = new System.Timers.Timer(PresentUpdateTimerInterval);
        presentUpdateTimer.Elapsed += PresentUpdateTimer_Elapsed;
        presentUpdateTimer.Enabled = true;
    }
    private void destroyPresentUpdateTimer()
    {
        Logger?.LogCritical("DestroyPresentUpdateTimer");
        if (parameterUpdateTimer == null)
            return;
        presentUpdateTimer.Enabled = false;
        presentUpdateTimer.Elapsed -= PresentUpdateTimer_Elapsed;
        presentUpdateTimer.Dispose();
        presentUpdateTimer = null;
    }

    private void initializeRealTimeClockUpdateTimer()
    {
        Logger?.LogInformation("InitializeRealTimeClockUpdateTimer");
        if (parameterUpdateTimer != null)
            return;
        realTimeClockUpdateTimer = new System.Timers.Timer(RealTimeClockUpdateTimerInterval);
        realTimeClockUpdateTimer.Elapsed += RealTimeClockUpdateTimer_Elapsed;
        realTimeClockUpdateTimer.Enabled = true;
    }
    private void destroyRealTimeClockUpdateTimer()
    {
        Logger?.LogCritical("DestroyRealTimeClockUpdateTimer");
        if (parameterUpdateTimer == null)
            return;
        realTimeClockUpdateTimer.Enabled = false;
        realTimeClockUpdateTimer.Elapsed -= RealTimeClockUpdateTimer_Elapsed;
        realTimeClockUpdateTimer.Dispose();
        realTimeClockUpdateTimer = null;
    }


    public void ResetAllTimersToDefault()
    {
        Logger?.LogCritical("ResetAllTimersToDefault");
        QueuedUpdateTime = DefaultQueuedUpdateTime;
        NonQueuedUpdateTime = DefaultNonQueuedUpdateTime;
        UpdateDelayBetweenRequests = DefaultUpdateDelayBetweenRequests;
        UpdateDelayBetweenQueuedUpdateRequests = DefaultUpdateDelayBetweenQueuedUpdateRequests;
        UpdateDelayBetweenNonQueuedUpdateRequests = DefaultUpdateDelayBetweenNonQueuedUpdateRequests;
        PresentLostTime = DefaultPresentLostTime;

        DiscoveryTimeout = DefaultDiscoveryTimeout;

        ParameterUpdateTimerInterval = DefaultParameterUpdateTimerInterval;
        PresentUpdateTimerInterval = DefaultPresentUpdateTimerInterval;
    }
    internal void InternalAllTimersToTestSpeed()
    {
        ResetAllTimersToDefault();
        QueuedUpdateTime = 30;
        NonQueuedUpdateTime = 30;
        UpdateDelayBetweenRequests = 0;
        UpdateDelayBetweenQueuedUpdateRequests = 0;
        UpdateDelayBetweenNonQueuedUpdateRequests = 0;
        PresentLostTime = 10000;

        DiscoveryTimeout = 15;

        ParameterUpdateTimerInterval = 15;
        PresentUpdateTimerInterval = 1000;
    }

    private void ParameterUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            var handlers = parameterUpdateTimerElapsed?.GetInvocationList();
            if (handlers != null)
            {
                System.Threading.Tasks.Parallel.ForEach(handlers, handler =>
                {
                    try
                    {
                        ((EventHandler)handler)?.Invoke(sender, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex, "Error in ParameterUpdateTimerElapsed handler");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in ParameterUpdateTimer_Elapsed");
        }
    }
    private void PresentUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            var handlers = presentUpdateTimerElapsed?.GetInvocationList();
            if (handlers != null)
            {
                System.Threading.Tasks.Parallel.ForEach(handlers, handler =>
                {
                    try
                    {
                        ((EventHandler)handler)?.InvokeFailSafe(sender, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex, "Error in PresentUpdateTimerElapsed handler");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in PresentUpdateTimer_Elapsed");
        }
    }
    private void RealTimeClockUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            Random random = new Random();
            var handlers = realTimeClockUpdateTimerElapsed?.GetInvocationList();
            if (handlers != null)
            {
                System.Threading.Tasks.Parallel.ForEachAsync(handlers, async (handler, token) =>
                {
                    await Task.Delay(random.Next(1000, 15000)); //Add some randomness to distribute the load on the bus.
                    try
                    {
                        ((EventHandler)handler)?.Invoke(sender, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex, "Error in RealTimeClockUpdateTimerElapsed handler");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in RealTimeClockUpdateTimer_Elapsed");
        }
    }
}
