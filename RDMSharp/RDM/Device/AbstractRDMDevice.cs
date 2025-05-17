using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public abstract IReadOnlyCollection<IRDMDevice> SubDevices { get; }

        public new bool IsDisposing { get; private set; }
        public new bool IsDisposed { get; private set; }
        public bool IsInitializing { get; private set; }
        public bool IsInitialized { get; private set; }
        public abstract bool IsGenerated { get; }

        protected AbstractRDMDevice(UID uid) : this(uid, SubDevice.Root)
        {
        }

        protected AbstractRDMDevice(UID uid, SubDevice subDevice)
        {
            this.IsInitializing = true;

            this.uid = uid;
            this.subdevice = subDevice;

            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMRequestMessage);
            initialize();
            this.IsInitialized = true;
            this.IsInitializing = false;
        }

        protected virtual void initialize()
        {
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
            if (this.IsDisposed || IsDisposing)
                return;
            try
            {
                await OnReceiveRDMMessage(rdmMessage);
            }
            catch (Exception e)
            {
                Logger.LogError(e, string.Empty);
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
            return $"[{UID}]";
        }
    }
}