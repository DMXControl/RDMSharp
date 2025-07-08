using System.Collections.Generic;

namespace RDMSharp.RDM.Device.Module
{
    public interface IModule
    {
        string Name { get; }
        IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get; }

        //RDMMessage? HandleRequest(RDMMessage message);
        bool IsHandlingParameter(ERDM_Parameter parameter);
    }
}
