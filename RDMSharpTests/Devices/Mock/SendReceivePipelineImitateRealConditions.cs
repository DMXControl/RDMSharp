using System.Threading;

namespace RDMSharpTests.Devices.Mock
{
    internal static class SendReceivePipelineImitateRealConditions
    {
        private static byte[]? data;
        private static SemaphoreSlim? semaphoreSlim;
        private static SemaphoreSlim? semaphoreSlim2;
        public static async Task RDMMessageSend(RDMMessage rdmMessage)
        {
            if (!rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                if (!rdmMessage.SourceUID.IsValidDeviceUID)
                    rdmMessage.SourceUID = new RDMUID(0x1fff, 0x44444444);
#if DEBUG
            Console.WriteLine(rdmMessage);
#endif
            if (rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
            {
                if (semaphoreSlim == null)
                    semaphoreSlim = new SemaphoreSlim(1);
                if (semaphoreSlim2 == null)
                    semaphoreSlim2 = new SemaphoreSlim(1);
                if (data == null)
                    data = rdmMessage.BuildMessage();
                else
                {
                    await semaphoreSlim2.WaitAsync();
                    var newData= rdmMessage.BuildMessage();
                    var oldData = data;
                    var combined = new byte[Math.Max(newData.Length, oldData?.Length??0)];
                    for(int i = 0;i < combined.Length; i++)
                    {
                        byte n = (byte)(newData.Length > i ? newData[i] : 0);
                        byte o = (byte)((oldData?.Length ?? 0) > i ? oldData[i] : 0);
                        combined[i] = (byte)(n | o);
                    }
                    data = combined;
                    semaphoreSlim2.Release();
                }

                if (semaphoreSlim.CurrentCount == 1)
                {
                    await semaphoreSlim.WaitAsync();
                    await Task.Delay(5);
                    await semaphoreSlim2.WaitAsync();
                    RDMMessageRereivedResponse?.InvokeFailSafe(null, data);
                    data = null;
                    semaphoreSlim2.Release();
                    semaphoreSlim.Release();
                }
            }
            else
                RDMMessageRereivedRequest?.InvokeFailSafe(null, rdmMessage);
        }
        public static event EventHandler<RDMMessage>? RDMMessageRereivedRequest;
        public static event EventHandler<byte[]>? RDMMessageRereivedResponse;
    }
}
