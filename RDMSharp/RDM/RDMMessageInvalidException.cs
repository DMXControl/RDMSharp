using System;
using System.Linq;

namespace RDMSharp
{
    public class RDMMessageInvalidException : Exception
    {
        public readonly RDMMessage RDMMessage;
        protected RDMMessageInvalidException(RDMMessage rdmMessage, string message) : base(message)
        {
            RDMMessage = rdmMessage;
        }
        internal static void ThrowIfInvalid(RDMMessage msg, ERDM_Command expectedCommand, params ERDM_Parameter[] expectedParameters)
        {
#if NETSTANDARD
            if (msg == null)
                throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
#else
            ArgumentNullException.ThrowIfNull(msg);
#endif

            if (expectedCommand.HasFlag(ERDM_Command.RESPONSE) && !msg.IsAck) throw new RDMMessageInvalidException(msg, $"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != expectedCommand) throw new RDMMessageInvalidException(msg, $"Command is not the expected Command: {expectedCommand}");
            if (expectedParameters.Length != 0 && !expectedParameters.Contains(msg.Parameter)) throw new RDMMessageInvalidException(msg, $"Parameter is not one of the expected Parameters: {string.Join(";", expectedParameters)}");
        }
        internal static void ThrowIfInvalidPDL(RDMMessage msg, ERDM_Command expectedCommand, ERDM_Parameter expectedParameter, params int[] expectedPDL)
        {
            ThrowIfInvalid(msg, expectedCommand, expectedParameter);
            if (!expectedPDL.Contains(msg.PDL)) throw new RDMMessageInvalidException(msg, $"PayloadDataLength is fitting the given Values {string.Join(";", expectedPDL)}");
        }
        internal static void ThrowIfInvalidPDL(RDMMessage msg, ERDM_Command expectedCommand, ERDM_Parameter[] expectedParameters, params int[] expectedPDL)
        {
            ThrowIfInvalid(msg, expectedCommand, expectedParameters);
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(msg, expectedPDL);
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(msg.ParameterData, expectedPDL);
        }
        internal static void ThrowIfInvalidPDLRange(RDMMessage msg, ERDM_Command expectedCommand, ERDM_Parameter expectedParameter, int expectedMinPDL, int expectedMaxPDL)
        {
            ThrowIfInvalid(msg, expectedCommand, expectedParameter);
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(msg, expectedMinPDL, expectedMaxPDL);
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(msg.ParameterData, expectedMinPDL, expectedMaxPDL);
        }
    }
}