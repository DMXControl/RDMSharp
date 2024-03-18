namespace RDMSharpTests.Devices.Mock
{
    internal static class SendReceivePipeline
    {
        private static readonly Random Random = new Random();
        public static long GetNewIdentifyer()
        {
            return Random.NextInt64();
        }
        public static void RDMMessageSend(long identifyer, RDMMessage rdmMessage)
        {
            if (!rdmMessage.DestUID.IsValidDeviceUID)
                rdmMessage.DestUID = new RDMUID(0x1fff, 44444444);
            if (!rdmMessage.SourceUID.IsValidDeviceUID)
                rdmMessage.SourceUID = new RDMUID(0x1fff, 0x44444444);

#if DEBUG
            Console.WriteLine(rdmMessage);
#endif

            RDMMessageRereived?.InvokeFailSafe(null, new Tuple<long, RDMMessage>(identifyer,rdmMessage));
        }
        public static event EventHandler<Tuple<long, RDMMessage>>? RDMMessageRereived;
    }
}
