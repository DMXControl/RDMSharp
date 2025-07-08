using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RDMSharp.RDM.Device.Module
{
    public abstract class AbstractModule : IModule
    {
        protected static ILogger Logger = Logging.CreateLogger<AbstractModule>();

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly string _name;
        public string Name { get => _name; }

        private readonly IReadOnlyCollection<ERDM_Parameter> _supportedParameters;

        public IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get => _supportedParameters; }

        protected AbstractGeneratedRDMDevice ParentDevice { get; private set; }

        public AbstractModule(string name, params ERDM_Parameter[] supportedParameters)
        {
            this._name = name;
            this._supportedParameters = supportedParameters;
        }
        public virtual RDMMessage? HandleRequest(RDMMessage message)
        {
            if (!this.IsHandlingParameter(message.Parameter))
                return null;

            ERDM_NackReason? nackReason = null;
            try
            {
                return handleRequest(message);
            }
            catch (System.Exception ex)
            {
                nackReason = ERDM_NackReason.HARDWARE_FAULT;
            }
            return new RDMMessage(nackReason ?? ERDM_NackReason.UNKNOWN_PID)
            {
                DestUID = message.SourceUID,
                SourceUID = message.SourceUID,
                Command = ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE,
                Parameter = message.Parameter
            };
        }
        protected virtual RDMMessage? handleRequest(RDMMessage message)
        {
            throw new System.NotImplementedException("This method should be overridden in derived classes to handle specific requests.");
        }

        public virtual bool IsHandlingParameter(ERDM_Parameter parameter)
        {
            return false;
        }

        internal void SetParentDevice(AbstractGeneratedRDMDevice device)
        {
            if (ParentDevice is not null)
                return;
            ParentDevice = device;
            device.ParameterValueAdded += Device_ParameterValueAdded;
            device.ParameterValueChanged += Device_ParameterValueChanged;
            OnParentDeviceChanged(ParentDevice);
        }

        private void Device_ParameterValueAdded(object sender, AbstractRDMCache.ParameterValueAddedEventArgs e)
        {
            ParameterChanged(e.Parameter, e.Value, e.Index);
        }
        private void Device_ParameterValueChanged(object sender, AbstractRDMCache.ParameterValueChangedEventArgs e)
        {
            ParameterChanged(e.Parameter, e.NewValue, e.Index);
        }
        protected abstract void ParameterChanged(ERDM_Parameter parameter, object? newValue, object? index);

        protected abstract void OnParentDeviceChanged(AbstractGeneratedRDMDevice device);

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}