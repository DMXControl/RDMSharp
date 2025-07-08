using System;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class RealTimeClockModule : AbstractModule
    {
        public DateTime? RealTimeClock
        {
            get
            {
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.REAL_TIME_CLOCK, out object res))
                {
                    if (res is DateTime dateTime)
                        return dateTime;
                    if (res is RDMRealTimeClock rdmRealTimeClock)
                        return rdmRealTimeClock.Date;
                }
                return null;
            }
            internal set
            {
                if (ParentDevice is not null)
                    ParentDevice.trySetParameter(ERDM_Parameter.REAL_TIME_CLOCK, new RDMRealTimeClock(value.Value));
            }
        }
        public RealTimeClockModule() : base(
            "RealTimeClock",
            ERDM_Parameter.REAL_TIME_CLOCK)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.RealTimeClock = DateTime.Now;
            GlobalTimers.Instance.PresentUpdateTimerElapsed += Instance_PresentUpdateTimerElapsed;
        }

        private void Instance_PresentUpdateTimerElapsed(object sender, EventArgs e)
        {
            this.RealTimeClock= DateTime.Now;
        }
    }
}