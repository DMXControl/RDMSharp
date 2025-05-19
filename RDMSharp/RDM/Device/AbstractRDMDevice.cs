using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractRDMDevice : AbstractRDMCache, IRDMDevice
    {
        private protected static readonly ILogger Logger = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly UID uid;
        public UID UID => uid;

        private readonly SubDevice subdevice;
        public SubDevice Subdevice => subdevice;

        public abstract RDMDeviceInfo DeviceInfo { get; }
        public abstract IReadOnlyDictionary<byte, Sensor> Sensors { get; }
        public abstract IReadOnlyDictionary<ushort, Slot> Slots { get; }

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
                throw new NotSupportedException($"A SubDevice {this.Subdevice} cannot have SubDevices.");

            if (this.Subdevice.IsBroadcast)
                throw new NotSupportedException($"A SubDevice cannot be Broadcast.");


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

            if (this.Subdevice.IsRoot)
                asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMRequestMessage);

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
                    sd.asyncRDMRequestHelper = this.asyncRDMRequestHelper;
                    sd.performInitialize();
                }
        }


        #region SendReceive Pipeline
        private async Task sendRDMRequestMessage(RDMMessage rdmMessage)
        {
            rdmMessage.DestUID = UID;
            await SendRDMMessage(rdmMessage);
        }
        protected abstract Task SendRDMMessage(RDMMessage rdmMessage);

        protected async Task ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            if (!this.Subdevice.IsRoot && !rdmMessage.SubDevice.IsBroadcast)
                return;

            if (this.IsDisposed || IsDisposing)
                return;
            try
            {
                if (rdmMessage.SubDevice.IsBroadcast)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var sd in this.subDevices)
                        tasks.Add(OnReceiveRDMMessage(rdmMessage));
                    await Task.WhenAll(tasks);
                    return;
                }
                AbstractRDMDevice sds = null;
                if (rdmMessage.SubDevice.IsRoot)
                    sds = this;
                else
                    sds = this.subDevices?.OfType<AbstractRDMDevice>().FirstOrDefault(sd => sd.Subdevice == rdmMessage.SubDevice);

                if (sds != null)
                    await sds.OnReceiveRDMMessage(rdmMessage);
                else
                    this.asyncRDMRequestHelper.ReceiveMessage(rdmMessage);

            }
            catch (Exception e)
            {
                Logger?.LogError(e, string.Empty);
            }
        }
        protected abstract Task OnReceiveRDMMessage(RDMMessage rdmMessage);
        #endregion


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
            asyncRDMRequestHelper.Dispose();
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