using System;

namespace RDMSharp.ParameterWrapper
{
    public interface IRDMParameterWrapper : IEquatable<IRDMParameterWrapper>
    {
        string Name { get; }
        string Description { get; }
        ERDM_Parameter Parameter { get; }
        ERDM_CommandClass CommandClass { get; }

        bool SupportSubDevices { get; }
    }
}