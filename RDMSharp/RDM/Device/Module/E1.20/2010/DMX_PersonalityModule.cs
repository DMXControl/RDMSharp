using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DMX_PersonalityModule : AbstractModule
{
    private const string _moduleName = "DMX_Personality";
    private const string _moduleDisplayName = "DMX Personality";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.DMX_PERSONALITY,
        ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION
    };

    public override string DisplayName => _moduleDisplayName;

    private IPersonality currentPersonality;
    public IPersonality CurrentPersonality
    {
        get
        {
            return currentPersonality;
            //if (ParentGeneratedDevice is not null)
            //    return _generatedPersonalities?.FirstOrDefault(p => p.ID == CurrentPersonalityId);

            //else if (ParentRemoteDevice is not null)
            //    return ParentRemoteDevice.PersonalityModel.Personality;
            //return null;
        }
        private set
        {
            if (currentPersonality == value)
                return;
            currentPersonality = value;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DMX_PERSONALITY, new RDMDMXPersonality(value.ID, PersonalitiesCount));
            OnPropertyChanged();
        }
    }
    private byte _initialPersonalityId;
    //public byte? CurrentPersonalityId
    //{
    //    get
    //    {
    //        if (ParentDevice is null)
    //            return _currentPersonalityId;
    //        object res;
    //        if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DMX_PERSONALITY, out res))
    //            if (res is RDMDMXPersonality personality)
    //                return personality.CurrentPersonality;
    //        return _currentPersonalityId;
    //    }
    //    set
    //    {
    //        if (!value.HasValue)
    //            throw new NullReferenceException($"{CurrentPersonalityId} can't be null if {ERDM_Parameter.DMX_PERSONALITY} is Supported");
    //        if (value.Value == 0)
    //            throw new ArgumentOutOfRangeException($"{CurrentPersonalityId} can't 0 if {ERDM_Parameter.DMX_PERSONALITY} is Supported");


    //        _currentPersonalityId = value.Value;
    //        if (ParentGeneratedDevice is not null)
    //        {
    //            if (!this._generatedPersonalities.Any(p => p.ID == value.Value))
    //                throw new ArgumentOutOfRangeException($"No Personality found with ID: {value.Value}");
    //            ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DMX_PERSONALITY, new RDMDMXPersonality(value.Value, PersonalitiesCount));
    //        }
    //        if (ParentRemoteDevice is not null)
    //        {
    //            if (!this.PersonalityDesriptions.Any(p => p.PersonalityId == value.Value))
    //                throw new ArgumentOutOfRangeException($"No Personality found with ID: {value.Value}");
    //            _ = ParentRemoteDevice.SetParameter(ERDM_Parameter.DMX_PERSONALITY, value);
    //        }
    //    }
    //}
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
    public readonly IReadOnlyCollection<GeneratedPersonality> _generatedPersonalities = null;
    public IReadOnlyCollection<IPersonality> Personalities
    {
        get
        {
            if (_generatedPersonalities is not null)
                return _generatedPersonalities;

            return ParentRemoteDevice.DeviceModel.KnownPersonalityModels.Select(pm => pm.Personality).ToList().AsReadOnly();
        }
    }
    public readonly byte PersonalitiesCount;
    //private ushort currentPersonalityFootprint;
    //public ushort CurrentPersonalityFootprint
    //{
    //    get
    //    {
    //        return currentPersonalityFootprint;
    //    }
    //    private set
    //    {
    //        if (currentPersonalityFootprint == value)
    //            return;
    //        currentPersonalityFootprint = value;
    //        OnPropertyChanged();
    //    }
    //}

    public DMX_PersonalityModule(byte initialPersonalityId, params GeneratedPersonality[] personalities) : base(
        _moduleName,
        _moduleParameters)
    {
        if (!personalities.Any(p => p.ID == initialPersonalityId))
            throw new ArgumentOutOfRangeException($"No Personality found with ID: {initialPersonalityId}");

        _initialPersonalityId = initialPersonalityId;
        _generatedPersonalities = (personalities ?? Array.Empty<GeneratedPersonality>()).ToList().AsReadOnly();
        PersonalitiesCount = (byte)_generatedPersonalities.Count;

    }
    public DMX_PersonalityModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        CurrentPersonality = ParentRemoteDevice.PersonalityModel?.Personality;
        //currentPersonalityFootprint = ParentRemoteDevice.PersonalityModel.SlotCount;
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedPersonalities is not null)
        {
            if (_generatedPersonalities.Count >= byte.MaxValue)
                throw new ArgumentOutOfRangeException($"There to many {_generatedPersonalities}! Maximum is {byte.MaxValue - 1}");

            if (_generatedPersonalities.Count != 0)
            {
                var persDesc = new ConcurrentDictionary<object, object>();
                foreach (var gPers in _generatedPersonalities)
                    if (!persDesc.TryAdd(gPers.ID, new RDMDMXPersonalityDescription(gPers)))
                        throw new Exception($"{gPers.ID} already used as {nameof(gPers.ID)}");

                device.setParameterValue(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, persDesc);
            }
        }
        await SetPersonality(_initialPersonalityId);
    }
    protected override async void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.DMX_PERSONALITY:
                if (newValue is RDMDMXPersonality personality)
                {
                    if (ParentGeneratedDevice is not null)
                        await SetPersonality(personality.CurrentPersonality);

                    else if (ParentRemoteDevice is not null)
                    {
                        var pm = ParentRemoteDevice.DeviceModel.KnownPersonalityModels.FirstOrDefault(p => p.Personality?.ID == personality.CurrentPersonality);
                        if (pm is null)
                            pm = ParentRemoteDevice.DeviceModel.getPersonalityModel(ParentRemoteDevice, personality.CurrentPersonality);
                        if (!pm.IsInitialized)
                            await pm.Initialize();
                        CurrentPersonality = pm.Personality;
                    }
                }
                break;
            case ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION when ParentGeneratedDevice is not null:
                OnPropertyChanged(nameof(PersonalityDesriptions));
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
        if (message.Parameter == ERDM_Parameter.DMX_PERSONALITY)
            if (message.Command == ERDM_Command.SET_COMMAND)
            {
                if (message.Value is byte b)
                {
                    try
                    {
                        if (this._generatedPersonalities.FirstOrDefault(p => p.ID == b) is IPersonality pers)
                            CurrentPersonality = pers;
                        else
                        {
                            return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                            {
                                DestUID = message.SourceUID,
                                SourceUID = message.DestUID,
                                Parameter = message.Parameter,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE
                            };
                        }
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
    public async Task<bool> SetPersonality(byte personalityId)
    {
        if (ParentGeneratedDevice is not null)
        {
            if (this._generatedPersonalities.FirstOrDefault(p => p.ID == personalityId) is not IPersonality pers)
                throw new ArgumentOutOfRangeException($"No Personality found with ID: {personalityId}");
            CurrentPersonality = pers;
        }
        if (ParentRemoteDevice is not null)
        {
            if (!this.PersonalityDesriptions.Any(p => p.PersonalityId == personalityId))
                throw new ArgumentOutOfRangeException($"No Personality found with ID: {personalityId}");
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.DMX_PERSONALITY, personalityId);
        }
        return true;
    }
}