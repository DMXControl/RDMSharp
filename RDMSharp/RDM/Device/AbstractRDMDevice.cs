using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RDMSharp
{
    public abstract class AbstractRDMDevice : AbstractRDMCache, IRDMDevice
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly UID uid;
        public UID UID => uid;

        private readonly SubDevice subdevice;
        public SubDevice Subdevice => subdevice;

        public abstract RDMDeviceInfo DeviceInfo { get; }
        public abstract IReadOnlyDictionary<byte, Sensor> Sensors { get; }
        public abstract IReadOnlyDictionary<ushort, Slot> Slots { get; }
        public abstract IReadOnlyDictionary<int, RDMStatusMessage> StatusMessages { get; }

        private List<IRDMDevice> subDevices;
        protected IList<IRDMDevice> SubDevices_Internal { get => subDevices; }
        public IReadOnlyCollection<IRDMDevice> SubDevices => SubDevices_Internal?.AsReadOnly();


        public new bool IsDisposing { get; private set; }
        public new bool IsDisposed { get; private set; }
        public bool IsInitialized { get; private set; }
        public abstract bool IsGenerated { get; }


        protected AbstractRDMDevice(UID uid, SubDevice? subDevice = null, IRDMDevice[] subDevices = null)
        {
            this.uid = uid;
            this.subdevice = subDevice ?? SubDevice.Root;
            if (subDevices != null && !this.Subdevice.IsRoot)
                throw new NotSupportedException($"A SubDevice {this.Subdevice} can't have SubDevices.");

            if (this.Subdevice.IsBroadcast)
                throw new NotSupportedException($"A SubDevice can't be Broadcast.");



            if (this.Subdevice == SubDevice.Root)
            {
                this.subDevices = new List<IRDMDevice>();
                this.subDevices.Add(this);

                if (subDevices != null)
                    this.subDevices.AddRange(subDevices);

                if (this.subDevices.Distinct().Count() != this.subDevices.Count)
                    throw new InvalidOperationException($"The SubDevices of {this.UID} are not unique.");

                performInitialize();
            }
        }

        protected void performInitialize(RDMDeviceInfo deviceInfo=null)
        {
            if (this.IsInitialized)
                return;

            initialize(deviceInfo);
            this.IsInitialized = true;
        }

        protected virtual void initialize(RDMDeviceInfo deviceInfo = null)
        {
            if (this.Subdevice.IsRoot)
                foreach (AbstractRDMDevice sd in this.subDevices)
                {
                    if (sd.Subdevice.IsRoot)
                        continue;
                    sd.performInitialize();
                }
        }


        protected virtual void OnPropertyChanged(string property)
        {
            this.PropertyChanged.InvokeFailSafe(this, new PropertyChangedEventArgs(property));
        }


        public virtual IReadOnlyDictionary<ERDM_Parameter, object> GetAllParameterValues()
        {
            return this.ParameterValues;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose-Methoden müssen SuppressFinalize aufrufen", Justification = "<Ausstehend>")]
        public new void Dispose()
        {
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;

            try
            {
                OnDispose();
            }
            catch { }
            finally
            {
                IsDisposed = true;
                IsDisposing = false;
            }
        }
        protected abstract void OnDispose();

        public override string ToString()
        {
            if (!this.Subdevice.IsRoot)
                return $"[{UID}] ({this.Subdevice})";
            return $"[{UID}]";
        }
    }
}