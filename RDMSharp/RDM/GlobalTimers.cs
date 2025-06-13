using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RDMSharp.Tests")]
namespace RDMSharp
{
    public class GlobalTimers
    {
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

        public const int DefaultQueuedUpdateTime = 4000; // 4 seconds
        public const int DefaultNonQueuedUpdateTime = 4000; // 4 seconds
        public const int DefaultUpdateDelayBetweenRequests = 50; // 50 milliseconds
        public const int DefaultUpdateDelayBetweenQueuedUpdateRequests = 200; // 200 milliseconds
        public const int DefaultUpdateDelayBetweenNonQueuedUpdateRequests = 200; // 200 milliseconds
        public const int DefaultPresentLostTime = 15000; // 15 seconds

        public const int DefaultParameterUpdateTimerInterval = 10000; // 10 seconds
        public const int DefaultPresentUpdateTimerInterval = 500; // 0.5 seconds
        public int QueuedUpdateTime { get; set; } = DefaultQueuedUpdateTime;
        public int NonQueuedUpdateTime { get; set; } = DefaultNonQueuedUpdateTime;
        public int UpdateDelayBetweenRequests { get; set; } = DefaultUpdateDelayBetweenRequests;
        public int UpdateDelayBetweenQueuedUpdateRequests { get; set; } = DefaultUpdateDelayBetweenQueuedUpdateRequests;
        public int UpdateDelayBetweenNonQueuedUpdateRequests { get; set; } = DefaultUpdateDelayBetweenNonQueuedUpdateRequests;
        public int PresentLostTime { get; set; } = DefaultPresentLostTime;

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

        private System.Timers.Timer parameterUpdateTimer = null;
        private System.Timers.Timer presentUpdateTimer = null;

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
        private void initializeParameterUpdateTimer()
        {
            if (parameterUpdateTimer != null)
                return;
            parameterUpdateTimer = new System.Timers.Timer(ParameterUpdateTimerInterval);
            parameterUpdateTimer.Elapsed += ParameterUpdateTimer_Elapsed;
            parameterUpdateTimer.Enabled = true;
        }
        private void destroyParameterUpdateTimer()
        {
            if (parameterUpdateTimer == null)
                return;
            parameterUpdateTimer.Enabled = false;
            parameterUpdateTimer.Elapsed -= ParameterUpdateTimer_Elapsed;
            parameterUpdateTimer.Dispose();
            parameterUpdateTimer = null;
        }

        private void initializePresentUpdateTimer()
        {
            if (parameterUpdateTimer != null)
                return;
            presentUpdateTimer = new System.Timers.Timer(PresentUpdateTimerInterval);
            presentUpdateTimer.Elapsed += PresentUpdateTimer_Elapsed;
            presentUpdateTimer.Enabled = true;
        }
        private void destroyPresentUpdateTimer()
        {
            if (parameterUpdateTimer == null)
                return;
            presentUpdateTimer.Enabled = false;
            presentUpdateTimer.Elapsed -= PresentUpdateTimer_Elapsed;
            presentUpdateTimer.Dispose();
            presentUpdateTimer = null;
        }
        public void ResetAllTimersToDefault()
        {
            QueuedUpdateTime = DefaultQueuedUpdateTime;
            NonQueuedUpdateTime = DefaultNonQueuedUpdateTime;
            UpdateDelayBetweenRequests = DefaultUpdateDelayBetweenRequests;
            UpdateDelayBetweenQueuedUpdateRequests = DefaultUpdateDelayBetweenQueuedUpdateRequests;
            UpdateDelayBetweenNonQueuedUpdateRequests = DefaultUpdateDelayBetweenNonQueuedUpdateRequests;
            PresentLostTime = DefaultPresentLostTime;

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

            ParameterUpdateTimerInterval = 15;
            PresentUpdateTimerInterval = 1000;
        }

        private void ParameterUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Parallelisierung: Alle Handler parallel ausführen, falls mehrere abonniert sind
            var handlers = parameterUpdateTimerElapsed?.GetInvocationList();
            if (handlers != null)
            {
                System.Threading.Tasks.Parallel.ForEach(handlers, handler =>
                {
                    try
                    {
                        ((EventHandler)handler)?.Invoke(sender, EventArgs.Empty);
                    }
                    catch
                    {
                    }
                });
            }
        }
        private void PresentUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var handlers = presentUpdateTimerElapsed?.GetInvocationList();
            if (handlers != null)
            {
                System.Threading.Tasks.Parallel.ForEach(handlers, handler =>
                {
                    try
                    {
                        ((EventHandler)handler)?.Invoke(sender, EventArgs.Empty);
                    }
                    catch
                    {
                    }
                });
            }
        }
    }
}
