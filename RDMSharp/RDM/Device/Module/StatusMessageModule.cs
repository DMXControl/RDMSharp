using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class StatusMessageModule : AbstractModule
    {
        private ConcurrentDictionary<int, RDMStatusMessage> statusMessages = new ConcurrentDictionary<int, RDMStatusMessage>();
        public IReadOnlyDictionary<int, RDMStatusMessage> StatusMessages { get { return statusMessages.AsReadOnly(); } }
        public StatusMessageModule() : base(
            "StatusMessage",
            ERDM_Parameter.STATUS_MESSAGES,
            ERDM_Parameter.CLEAR_STATUS_ID)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.ParentDevice.setParameterValue(ERDM_Parameter.STATUS_MESSAGES, new RDMStatusMessage[0]);
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
        }
        public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
        {
            if (parameter == ERDM_Parameter.CLEAR_STATUS_ID)
                return true;

            return base.IsHandlingParameter(parameter, command);
        }
        protected override RDMMessage handleRequest(RDMMessage message)
        {
            if (message.Parameter == ERDM_Parameter.CLEAR_STATUS_ID)
                if (message.Command == ERDM_Command.SET_COMMAND)
                {
                    if (message.PDL == 0)
                    {
                        try
                        {
                            statusMessages?.Clear();
                            OnPropertyChanged(nameof(StatusMessages));
                        }
                        catch (Exception ex)
                        {
                            Logger?.LogError(ex);
                            return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
                            {
                                DestUID = message.SourceUID,
                                SourceUID = message.DestUID,
                                Parameter = message.Parameter,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE
                            };
                        }
                        return new RDMMessage()
                        {
                            DestUID = message.SourceUID,
                            SourceUID = message.DestUID,
                            Parameter = message.Parameter,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        };
                    }
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        DestUID = message.SourceUID,
                        SourceUID = message.DestUID,
                        Parameter = message.Parameter,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE
                    };
                }
            return base.handleRequest(message);
        }
        public void AddStatusMessage(RDMStatusMessage statusMessage)
        {
            int id = 0;
            if (this.statusMessages.Count != 0)
                id = this.statusMessages.Max(s => s.Key) + 1;
            if (this.statusMessages.TryAdd(id, statusMessage))
            {
                this.ParentDevice.setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());
                OnPropertyChanged(nameof(StatusMessages));
            }
        }
        public void ClearStatusMessage(RDMStatusMessage statusMessage)
        {
            this.statusMessages.Where(s => s.Value.Equals(statusMessage)).ToList().ForEach(s =>
            {
                s.Value.Clear();
            });
            this.ParentDevice.setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());
            OnPropertyChanged(nameof(StatusMessages));
        }
        public void RemoveStatusMessage(RDMStatusMessage statusMessage)
        {
            bool succes = false;
            this.statusMessages.Where(s => s.Value.Equals(statusMessage)).ToList().ForEach(s =>
            {
                if (this.statusMessages.TryRemove(s.Key, out _))
                    succes = true;
            });
            if (succes)
            {
                this.ParentDevice.setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());
                OnPropertyChanged(nameof(StatusMessages));
            }
        }
    }
}