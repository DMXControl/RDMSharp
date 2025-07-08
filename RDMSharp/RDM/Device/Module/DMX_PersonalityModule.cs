using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class DMX_PersonalityModule : AbstractModule
    {
        private byte _currentPersonality;
        public byte? CurrentPersonality
        {
            get
            {
                if (ParentDevice is null)
                    return _currentPersonality;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DMX_PERSONALITY, out res))
                    if (res is RDMDMXPersonality personality)
                        return personality.CurrentPersonality;
                return _currentPersonality;
            }
            set
            {
                if (!value.HasValue)
                    throw new NullReferenceException($"{CurrentPersonality} can't be null if {ERDM_Parameter.DMX_PERSONALITY} is Supported");
                if (value.Value == 0)
                    throw new ArgumentOutOfRangeException($"{CurrentPersonality} can't 0 if {ERDM_Parameter.DMX_PERSONALITY} is Supported");

                if (!this.Personalities.Any(p => p.ID == value.Value))
                    throw new ArgumentOutOfRangeException($"No Personality found with ID: {value.Value}");

                _currentPersonality = value.Value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.DMX_PERSONALITY, new RDMDMXPersonality(value.Value, PersonalitiesCount));
            }
        }
        public IReadOnlyCollection<RDMDMXPersonalityDescription> PersonalityDesriptions
        {
            get
            {
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, out object res))
                    if (res is ConcurrentDictionary<object, object> dict)
                        return dict.Select(d => d.Value).OfType<RDMDMXPersonalityDescription>().ToList().AsReadOnly();
                return Array.Empty<RDMDMXPersonalityDescription>();
            }
        }
        public readonly IReadOnlyCollection<GeneratedPersonality> Personalities;
        public readonly byte PersonalitiesCount;
        private ushort currentPersonalityFootprint;
        public ushort CurrentPersonalityFootprint
        {
            get
            {
                return currentPersonalityFootprint;
            }
            private set
            {
                if (currentPersonalityFootprint == value)
                    return;
                currentPersonalityFootprint = value;
                OnPropertyChanged();
            }
        }

        public DMX_PersonalityModule(byte currentPersonality, params GeneratedPersonality[] personalities) : base(
            "DMX_Personality",
            ERDM_Parameter.DMX_PERSONALITY,
            ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION)
        {
            if (!personalities.Any(p => p.ID == currentPersonality))
                throw new ArgumentOutOfRangeException($"No Personality found with ID: {currentPersonality}");

            _currentPersonality = currentPersonality;
            Personalities = (personalities ?? Array.Empty<GeneratedPersonality>()).ToList().AsReadOnly();
            PersonalitiesCount = (byte)Personalities.Count;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            if (Personalities is not null)
            {
                if (Personalities.Count >= byte.MaxValue)
                    throw new ArgumentOutOfRangeException($"There to many {Personalities}! Maximum is {byte.MaxValue - 1}");

                if (Personalities.Count != 0)
                {
                    var persDesc = new ConcurrentDictionary<object, object>();
                    foreach (var gPers in Personalities)
                        if (!persDesc.TryAdd(gPers.ID, (RDMDMXPersonalityDescription)gPers))
                            throw new Exception($"{gPers.ID} already used as {nameof(gPers.ID)}");

                    device.setParameterValue(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, persDesc);
                }
            }
            this.CurrentPersonality = _currentPersonality;
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DMX_PERSONALITY:
                    OnPropertyChanged(nameof(CurrentPersonality));
                    byte? val = null;
                    if (newValue is RDMDMXPersonality personality)
                        val = personality.OfPersonalities;
                    else if (newValue is byte b)
                        val = b;
                    if (val.HasValue)
                    {
                        CurrentPersonalityFootprint = Personalities.FirstOrDefault(p => p.ID == val.Value)?.SlotCount ?? 0;
                        return;
                    }
                    CurrentPersonalityFootprint = 0;
                    break;
                case ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION:
                    OnPropertyChanged(nameof(Personalities));
                    break;
            }
        }
        public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
        {
            if (parameter == ERDM_Parameter.DMX_PERSONALITY)
                return command == ERDM_Command.SET_COMMAND;
            return base.IsHandlingParameter(parameter, command);
        }
        protected override RDMMessage handleRequest(RDMMessage message)
        {
            if (message.Command == ERDM_Command.SET_COMMAND)
                if (message.Value is byte b)
                {
                    CurrentPersonality = b;

                    return new RDMMessage()
                    {
                        DestUID = message.SourceUID,
                        SourceUID = message.DestUID,
                        Parameter = message.Parameter,
                        Command = ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE,
                    };
                }
            return null;
        }
    }
}