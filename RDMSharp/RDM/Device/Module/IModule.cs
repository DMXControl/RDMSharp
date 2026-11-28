using System.ComponentModel;

namespace RDMSharp.RDM.Device.Module;

public interface IModule : INotifyPropertyChanged
{
    string Name { get; }
}
