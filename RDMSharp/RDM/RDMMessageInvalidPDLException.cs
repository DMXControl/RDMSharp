using System;
using System.Linq;

namespace RDMSharp
{
    public class RDMMessageInvalidPDLException : Exception
    {
        public readonly RDMMessage RDMMessage;
        public readonly byte[] PayloadData;
        private RDMMessageInvalidPDLException(RDMMessage rdmMessage, string message) : base(message)
        {
            RDMMessage = rdmMessage;
            PayloadData = rdmMessage.ParameterData;
        }
        private RDMMessageInvalidPDLException(byte[] payloadData, string message) : base(message)
        {
            RDMMessage = null;
            PayloadData = payloadData;
        }
        internal static void ThrowIfInvalidPDL(byte[] payloadData, params int[] expectedPDL)
        {
            if (!expectedPDL.Contains(payloadData.Length)) throw new RDMMessageInvalidPDLException(payloadData, $"PayloadDataLength is fitting the given Values {string.Join(";", expectedPDL)}");
        }
        internal static void ThrowIfInvalidPDL(RDMMessage msg, params int[] expectedPDL)
        {
#if NETSTANDARD
            if (msg == null)
                throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
#else
            ArgumentNullException.ThrowIfNull(msg);
#endif

            if (!expectedPDL.Contains(msg.PDL)) throw new RDMMessageInvalidPDLException(msg, $"PayloadDataLength is fitting the given Values {string.Join(";", expectedPDL)}");
        }
        internal static void ThrowIfInvalidPDLRange(byte[] payloadData, int expectedMinPDL, int expectedMaxPDL)
        {
            if (payloadData.Length < expectedMinPDL) throw new RDMMessageInvalidPDLException(payloadData, $"PayloadDataLength is fitting the given Range {payloadData.Length} < {expectedMinPDL}");
            if (payloadData.Length > expectedMaxPDL) throw new RDMMessageInvalidPDLException(payloadData, $"PayloadDataLength is fitting the given Range {payloadData.Length} > {expectedMaxPDL}");
        }
        internal static void ThrowIfInvalidPDLRange(RDMMessage msg, int expectedMinPDL, int expectedMaxPDL)
        {
#if NETSTANDARD
            if (msg == null)
                throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
#else
            ArgumentNullException.ThrowIfNull(msg);
#endif

            if (msg.PDL < expectedMinPDL) throw new RDMMessageInvalidPDLException(msg, $"PayloadDataLength is fitting the given Range {msg.PDL} < {expectedMinPDL}");
            if (msg.PDL > expectedMaxPDL) throw new RDMMessageInvalidPDLException(msg, $"PayloadDataLength is fitting the given Range {msg.PDL} > {expectedMaxPDL}");
        }
    }
}