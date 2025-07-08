using System.Collections.Generic;
using System.ComponentModel;

namespace RDMSharp.RDM.Device.Module
{
    public interface IModule: INotifyPropertyChanged
    {
        string Name { get; }
        IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get; }

        RDMMessage? HandleRequest(RDMMessage message);
        bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command);
    }
}
