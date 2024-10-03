using RDMSharp.Metadata;
using System;

namespace RDMSharp
{
    internal class PeerToPeerProcess
    {
        public PeerToPeerProcess(ERDM_Command command, ERDM_Parameter parameter, object payloadObject = null)
        {
            if (command != ERDM_Command.GET_COMMAND)
                if (command != ERDM_Command.SET_COMMAND)
                    throw new ArgumentException($"{nameof(command)} should be {ERDM_Command.GET_COMMAND} or {ERDM_Command.SET_COMMAND}");


            //MetadataFactory.
        }
    }
}