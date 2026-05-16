using System.Collections.Concurrent;
using System.ComponentModel;

namespace RDMSharp.RDM.Device;

public class SupportedParameterMetadata : INotifyPropertyChanged
{
    public readonly ERDM_Parameter Parameter;
    public readonly bool IsNonBlueprint;
    public readonly bool IsBlueprintModel;
    public readonly bool IsBlueprintModelPersonality;

    public readonly bool IsManufacturerInternal;

    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isSupported = true;
    public bool IsSupported
    {
        get
        {
            return _isSupported;
        }
        private set
        {
            if (_isSupported == value)
                return;
            _isSupported = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSupported)));
        }
    }
    private ERDM_CommandClass _unsupportedCommandClasses = ERDM_CommandClass.NONE;
    public ERDM_CommandClass UnsupportedCommandClasses
    {
        get
        {
            return _unsupportedCommandClasses;
        }
        private set
        {
            if (_unsupportedCommandClasses == value)
                return;
            _unsupportedCommandClasses = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnsupportedCommandClasses)));
        }
    }

    public bool IsManufacturerSpecific
    {
        get
        {
            return ((int)Parameter & 0x8000) != 0;
        }
    }

    private object _parameterDescription = null;
    public object ParameterDescription
    {
        get
        {
            return _parameterDescription;
        }
        private set
        {
            if (_parameterDescription is not null)
                return;
            _parameterDescription = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterDescription)));
        }
    }

    private string _name = null;
    public string Name
    {
        get
        {
            return _name;
        }
        private set
        {
            if (_name is not null)
                return;
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }

    private int? _parameterUpdateTimeMilliseconds = null;
    public int ParameterUpdateTimeMilliseconds
    {
        get
        {
            return _parameterUpdateTimeMilliseconds ?? -1;
        }
        internal set
        {
            if (_parameterUpdateTimeMilliseconds.HasValue)
                return;
            _parameterUpdateTimeMilliseconds = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterUpdateTimeMilliseconds)));
        }
    }

    private ConcurrentDictionary<int, ERDM_NackReason> NackReasons = new();

    private SupportedParameterMetadata(
        in ERDM_Parameter parameter,
        in bool blueprintModel = false,
        in bool blueprintModelPersonality = false,
        in bool manufacturerInternal = false)
    {
        Parameter = parameter;
        IsBlueprintModel = blueprintModel;
        IsBlueprintModelPersonality = blueprintModelPersonality;
        IsNonBlueprint = !blueprintModel && !blueprintModelPersonality;
        IsManufacturerInternal = manufacturerInternal;
    }

    public static SupportedParameterMetadata Create(
        in ERDM_Parameter parameter,
        in bool manufacturerInternal = false)
    {
        return new SupportedParameterMetadata(parameter, manufacturerInternal: manufacturerInternal);
    }
    public static SupportedParameterMetadata CreateBlueprintModel(
        in ERDM_Parameter parameter,
        in bool manufacturerInternal = false)
    {
        return new SupportedParameterMetadata(parameter, blueprintModel: true, manufacturerInternal: manufacturerInternal);
    }
    public static SupportedParameterMetadata CreateBlueprintModelPersonality(
        in ERDM_Parameter parameter,
        in bool manufacturerInternal = false)
    {
        return new SupportedParameterMetadata(parameter, blueprintModelPersonality: true, manufacturerInternal: manufacturerInternal);
    }

    internal void HandleNack(RDMMessage message)
    {
        if (message.NackReason.HasValue)
        {
            NackReasons.TryAdd(NackReasons.Count, message.NackReason.Value);
            switch (message.NackReason.Value)
            {
                case ERDM_NackReason.UNKNOWN_PID:
                    this.IsSupported = false;
                    break;
                case ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS:
                    if (message.Command == ERDM_Command.GET_COMMAND_RESPONSE)
                        this.UnsupportedCommandClasses |= ERDM_CommandClass.GET;
                    if (message.Command == ERDM_Command.SET_COMMAND_RESPONSE)
                        this.UnsupportedCommandClasses |= ERDM_CommandClass.SET;
                    break;
            }
        }
    }

    internal void SetParameterDescription(object parameterDescription)
    {
        ParameterDescription = parameterDescription;
    }

    internal void SetName(string name)
    {
        Name = name;
    }

    internal void SetParameterUpdateTime(int milliseconds)
    {
        ParameterUpdateTimeMilliseconds = milliseconds;
    }

    public sealed override string ToString()
    {
        return $"Parameter: {Parameter}, IsSupported: {IsSupported}, IsBlueprintModel: {IsBlueprintModel}, IsBlueprintModelPersonality: {IsBlueprintModelPersonality}, IsNonBlueprint: {IsNonBlueprint}, IsManufacturerInternal: {IsManufacturerInternal}";
    }
}