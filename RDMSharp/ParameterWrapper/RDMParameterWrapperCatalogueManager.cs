using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class RDMParameterWrapperCatalogueManager
    {
        private static RDMParameterWrapperCatalogueManager instance;

        public static RDMParameterWrapperCatalogueManager GetInstance()
        {
            if (instance == null)
                instance = new RDMParameterWrapperCatalogueManager();

            return instance;
        }
        private HashSet<IRDMParameterWrapper> parameterWrappers = new HashSet<IRDMParameterWrapper>();
        public ReadOnlyCollection<IRDMParameterWrapper> ParameterWrappers
        {
            get
            {
                return this.parameterWrappers.ToList().AsReadOnly();
            }
        }
        private RDMParameterWrapperCatalogueManager()
        {
            var type = typeof(IRDMParameterWrapper);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);
            foreach (Type t in types)
                this.RegisterParameterWrapper(t);
        }
        public IRDMParameterWrapper GetRDMParameterWrapperByID(ERDM_Parameter parameter, EManufacturer? manufacturer = null, ushort? deviceModelId = null)
        {
            if (!manufacturer.HasValue && !deviceModelId.HasValue)
                return parameterWrappers.SingleOrDefault(pw => pw.Parameter == parameter);

            if (manufacturer.HasValue)
            {
                var wrappers = parameterWrappers.OfType<IRDMManufacturerParameterWrapper>().Where(pw => pw.Manufacturer == manufacturer && pw.Parameter == parameter);
                if (deviceModelId.HasValue)
                    return wrappers.OfType<IRDMDeviceModelIdParameterWrapper>().SingleOrDefault(pw => pw.DeviceModelIds.Contains(deviceModelId.Value));
                else
                    return wrappers.SingleOrDefault();
            }

            return null;
        }

        public void RegisterParameterWrapper(Type type)
        {
            if (typeof(IRDMParameterWrapper).IsAssignableFrom(type))
                this.parameterWrappers.Add((IRDMParameterWrapper)Activator.CreateInstance(type));
        }

        public object ParameterDataObjectFromMessage(RDMMessage message)
        {
            var wrapper = parameterWrappers.FirstOrDefault(pw => pw.Parameter == message.Parameter);
            if (wrapper == null)
                return null;

            switch (message.Command)
            {
                case ERDM_Command.GET_COMMAND when wrapper is IRDMGetParameterWrapperRequest @getRequest:
                    return getRequest.GetRequestParameterDataToObject(message.ParameterData);

                case ERDM_Command.GET_COMMAND_RESPONSE when wrapper is IRDMGetParameterWrapperResponse @getResponse:
                    return @getResponse.GetResponseParameterDataToObject(message.ParameterData);

                case ERDM_Command.SET_COMMAND when wrapper is IRDMSetParameterWrapperRequest @setRequest:
                    return @setRequest.SetRequestParameterDataToObject(message.ParameterData);

                case ERDM_Command.SET_COMMAND_RESPONSE when wrapper is IRDMSetParameterWrapperResponse @setResponse:
                    return @setResponse.SetResponseParameterDataToObject(message.ParameterData);
            }

            return null;
        }
    }
}
