using System;

namespace RDMSharp
{
    public class GlobalTimers
    {
        private static GlobalTimers? instance = null;
        public static GlobalTimers Instance
        {
            get
            {
                if (instance == null)
                    instance = new GlobalTimers();
                return instance;
            }
        }

        public int ParameterUpdateTimerInterval { get; set; } = 1000;
        private System.Timers.Timer? parameterUpdateTimer = null;

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

        private void ParameterUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            parameterUpdateTimerElapsed?.InvokeFailSafe(sender, EventArgs.Empty);
        }
    }
}
