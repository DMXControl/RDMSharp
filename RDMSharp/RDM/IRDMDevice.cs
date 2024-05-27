using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RDMSharp
{
    public interface IRDMDevice : IDisposable, INotifyPropertyChanged
    {
        UID UID { get; }
        DateTime LastSeen { get; }
        bool Present { get; }

        RDMDeviceInfo DeviceInfo { get; }

        IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues { get; }
        IReadOnlyDictionary<byte, Sensor> Sensors { get; }
        IReadOnlyDictionary<ushort, Slot> Slots { get; }

        bool IsDisposing { get; }
        bool IsDisposed { get; }
    }
}
