using System;

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
        public const int DefaultNonQueuedUpdateTime = 10000; // 10 seconds
        public const int DefaultUpdateTimerInterval = 10000; // 10 seconds
        public int QueuedUpdateTime { get; set; } = DefaultQueuedUpdateTime;
        public int NonQueuedUpdateTime { get; set; } = DefaultNonQueuedUpdateTime;
        private int parameterUpdateTimerInterval = DefaultUpdateTimerInterval;
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
        private System.Timers.Timer parameterUpdateTimer = null;

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

        public void ResetAllTimersToDefault()
        {
            QueuedUpdateTime = DefaultQueuedUpdateTime;
            NonQueuedUpdateTime = DefaultNonQueuedUpdateTime;
            ParameterUpdateTimerInterval = DefaultUpdateTimerInterval;
        }

        private void ParameterUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            parameterUpdateTimerElapsed?.InvokeFailSafe(sender, EventArgs.Empty);
        }
    }
}
