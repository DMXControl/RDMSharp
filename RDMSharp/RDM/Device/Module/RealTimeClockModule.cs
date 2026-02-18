using RDMSharp.PayloadObject;
using System;

namespace RDMSharp.RDM.Device.Module;

public sealed class RealTimeClockModule : AbstractModule
{
    private const string _moduleName = "RealTimeClock";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.REAL_TIME_CLOCK;

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
        set
        {
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.REAL_TIME_CLOCK, new RDMRealTimeClock(value.Value));

            if (ParentRemoteDevice is not null)
                _ = ParentRemoteDevice.SetParameter(ERDM_Parameter.REAL_TIME_CLOCK, new RDMRealTimeClock(value.Value));
        }
    }
    public RealTimeClockModule() : base(
        _moduleName,
        _moduleParameter)
    {
    }
    public RealTimeClockModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.RealTimeClock = DateTime.Now;
        GlobalTimers.Instance.PresentUpdateTimerElapsed += Instance_PresentUpdateTimerElapsed;
    }

    private void Instance_PresentUpdateTimerElapsed(object sender, EventArgs e)
    {
        this.RealTimeClock = DateTime.Now;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.REAL_TIME_CLOCK:
                OnPropertyChanged(nameof(RealTimeClock));
                break;
        }
    }
}