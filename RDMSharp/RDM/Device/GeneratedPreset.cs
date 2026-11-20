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

    public GeneratedPreset(ushort id, double upFadeTime = 0, double downFadeTime = 0, double waitTime = 0)
    {
        if (id == 0)
            throw new ArgumentOutOfRangeException($"{0} is not allowed as {id}");
        this.id = id;
        UpFadeTime = upFadeTime;
        DownFadeTime = downFadeTime;
        WaitTime = waitTime;
    }

    public override string ToString()
    {
        return $"Preset [{ID}] {{ UpFadeTime={UpFadeTime}, DownFadeTime={DownFadeTime}, WaitTime={WaitTime} }}";
    }
}
