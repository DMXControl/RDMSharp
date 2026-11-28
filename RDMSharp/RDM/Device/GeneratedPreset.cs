using System;
using System.ComponentModel;

namespace RDMSharp;

public class GeneratedPreset : IPreset, INotifyPropertyChanged
{
    private readonly ushort id;

    public event PropertyChangedEventHandler PropertyChanged;

    public ushort ID => id;

    private double upFadeTime;
    public double UpFadeTime
    {
        get => upFadeTime;
        set
        {
            if (upFadeTime != value)
            {
                upFadeTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpFadeTime)));
            }
        }
    }

    private double downFadeTime;
    public double DownFadeTime
    {
        get => downFadeTime;
        set
        {
            if (downFadeTime != value)
            {
                downFadeTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownFadeTime)));
            }
        }
    }

    private double waitTime;
    public double WaitTime
    {
        get => waitTime;
        set
        {
            if (waitTime != value)
            {
                waitTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WaitTime)));
            }
        }
    }

    private ERDM_PresetProgrammed programmed;
    public ERDM_PresetProgrammed Programmed
    {
        get => programmed;
        set
        {
            if (programmed != value)
            {
                programmed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Programmed)));
            }
        }
    }

    public bool IsReadOnly => Programmed == ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY;

    public GeneratedPreset(ushort id, double upFadeTime = 0, double downFadeTime = 0, double waitTime = 0, ERDM_PresetProgrammed programmed = ERDM_PresetProgrammed.NOT_PROGRAMMED)
    {
        if (id == 0 || id == 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(id), id, "0 and 0xFFFF are not allowed as Preset ID");
        this.id = id;
        UpFadeTime = upFadeTime;
        DownFadeTime = downFadeTime;
        WaitTime = waitTime;
        Programmed = programmed;
    }

    public override string ToString()
    {
        return $"Preset [{ID}] {{ UpFadeTime={UpFadeTime}, DownFadeTime={DownFadeTime}, WaitTime={WaitTime}, Programmed={Programmed} }}";
    }
}
