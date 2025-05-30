using System.Collections.Concurrent;

namespace RDMSharpTests.Devices.Mock
{
    internal static class SendReceivePipelineImitateRealConditions
    {
        private static byte[]? data;
        private static SemaphoreSlim? semaphoreSlim;
        private static SemaphoreSlim? semaphoreSlim2;
        private static ConcurrentQueue<Task> queue = new ConcurrentQueue<Task>();
        public static async Task RDMMessageSend(RDMMessage rdmMessage)
        {
            if (!rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                if (!rdmMessage.SourceUID.IsValidDeviceUID)
                    rdmMessage.SourceUID = new UID(0x1fff, 0x44444444);

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
                    queue.Enqueue(Task.Run(async () =>
                    {
                        await semaphoreSlim2.WaitAsync();
                        var newData = rdmMessage.BuildMessage();
                        var oldData = data;
                        var combined = new byte[Math.Max(newData.Length, oldData?.Length ?? 0)];
                        for (int i = 0; i < combined.Length; i++)
                        {
                            byte n = (byte)(newData.Length > i ? newData[i] : 0);
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                            byte o = (byte)((oldData?.Length ?? 0) > i ? oldData[i] : 0);
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                            combined[i] = (byte)(n | o);
                        }
                        data = combined;
                        semaphoreSlim2.Release();
                    }));
                }

                if (semaphoreSlim.CurrentCount == 1)
                {
                    await semaphoreSlim.WaitAsync();
                    await Task.Delay(3);
                    while (queue.TryDequeue(out Task? waitOnMee)) // wait for all other MockDevices to put their Data on the Line, in real World this would be the time is not nessecary to wait for the other devices because they all act simultan but in the Test-Environment it needs time to itterate throu all Devices
                        if (waitOnMee != null)
                            await waitOnMee;
                    await semaphoreSlim2.WaitAsync();

                    RDMMessageReceivedResponse?.InvokeFailSafe(null, data);
                    data = null;
                    queue.Clear();
                    semaphoreSlim2.Release();
                    semaphoreSlim.Release();
                }
            }
            else
            {
                RDMMessageReceivedRequest?.InvokeFailSafe(null, rdmMessage);
            }
        }
        public static event EventHandler<RDMMessage>? RDMMessageReceivedRequest;
        public static event EventHandler<byte[]>? RDMMessageReceivedResponse;
    }
}
