using System.Collections.Generic;

namespace RDMSharp.RDM.Device.Module;

public interface IParameterIDModule : IModule
{
    IReadOnlyCollection<ERDM_Parameter> SupportedParameters { get; }

    RDMMessage HandleRequest(RDMMessage message);
    bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command);
}
