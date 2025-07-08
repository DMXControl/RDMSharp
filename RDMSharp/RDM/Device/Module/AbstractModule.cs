using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module
{
    public abstract class AbstractModule : IModule
    {
        protected static ILogger Logger = Logging.CreateLogger<AbstractModule>();
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
        //public virtual RDMMessage? HandleRequest(RDMMessage message)
        //{
        //    ERDM_NackReason? nackReason = null;
        //    try
        //    {
        //        return handleRequest(message);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        nackReason = ERDM_NackReason.HARDWARE_FAULT;
        //    }
        //    return new RDMMessage(nackReason ?? ERDM_NackReason.UNKNOWN_PID)
        //    {
        //        DestUID = message.SourceUID,
        //        SourceUID = message.SourceUID,
        //        Command = ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE,
        //        Parameter = message.Parameter
        //    };
        //}
        //protected abstract RDMMessage? handleRequest(RDMMessage message);

        public bool IsHandlingParameter(ERDM_Parameter parameter)
        {
            return _supportedParameters.Contains(parameter);
        }

        internal void SetParentDevice(AbstractGeneratedRDMDevice device)
        {
            if (ParentDevice is not null)
                return;
            ParentDevice = device;
            OnParentDeviceChanged(ParentDevice);
        }
        protected abstract void OnParentDeviceChanged(AbstractGeneratedRDMDevice device);
    }
}