using RDMSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RDMSharp
{
    public interface IRDMDevice : IDisposable, INotifyPropertyChanged
    {
        RDMUID UID { get; }
        DateTime LastSeen { get; }
        bool Present { get; }

        RDMDeviceInfo DeviceInfo { get; }
        IRDMDeviceModel DeviceModel { get; }

        IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues { get; }
        IReadOnlyDictionary<byte, RDMSensorValue> SensorValues { get; }
        IReadOnlyDictionary<ushort, Slot> Slots { get; }

        bool IsDisposing { get; }
        bool IsDisposed { get; }
    }
}
