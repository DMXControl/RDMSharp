using RDMSharp.Metadata;

namespace RDMSharpTests;

/// <summary>
/// Builds request payloads with the existing metadata parsing logic (the same way a remote device does),
/// so tests don't have to build the bytes by hand.
/// </summary>
internal static class PayloadTestTools
{
    /// <summary>
    /// The Parameter Data of a SET request for <paramref name="parameter"/> with <paramref name="value"/>.
    /// </summary>
    public static byte[] SetRequestData(ERDM_Parameter parameter, object value)
    {
        var parameterBag = new ParameterBag(parameter);
        var dataTreeBranch = DataTreeBranch.FromObject(value, null, parameterBag, ERDM_Command.SET_COMMAND);
        return MetadataFactory.SetRequestMessageData(parameterBag, dataTreeBranch).SelectMany(data => data).ToArray();
    }
}
