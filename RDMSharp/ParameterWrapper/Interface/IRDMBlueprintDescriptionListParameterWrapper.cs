namespace RDMSharp.ParameterWrapper
{
    public interface IRDMBlueprintDescriptionListParameterWrapper : IRDMBlueprintParameterWrapper, IRDMDescriptionParameterWrapper
    {
        ERDM_Parameter[] DescriptiveParameters { get; }
    }
}