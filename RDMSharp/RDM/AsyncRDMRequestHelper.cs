using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp
{
    public class AsyncRDMRequestHelper : IDisposable
    {
        private static readonly ILogger Logger = null;
        private static readonly Random random = new Random();
        private readonly ConcurrentDictionary<int, Tuple<RDMMessage, RDMMessage>> buffer = new ConcurrentDictionary<int, Tuple<RDMMessage, RDMMessage>>();
        private readonly Func<RDMMessage, Task> _sendMethode;
        private CancellationTokenSource _cts;
        public bool IsDisposing, IsDisposed;
        public AsyncRDMRequestHelper(Func<RDMMessage, Task> sendMethode)
        {
            _cts = new CancellationTokenSource();
            _sendMethode = sendMethode;
        }

        public void Dispose()
        {
            if (this.IsDisposing || this.IsDisposed)
                return;
            this.IsDisposing = true;
            _cts.Cancel();
            buffer.Clear();
            _cts.Dispose();
            this.IsDisposed = true;
            this.IsDisposing= false;
        }

        public bool ReceiveMessage(RDMMessage rdmMessage)
        {
            if (this.IsDisposing || this.IsDisposed)
                return false;

            if (rdmMessage.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
            {
                var o = buffer.FirstOrDefault(b => b.Value.Item1.Parameter == rdmMessage.Parameter);
                if (o.Value == null)
                    return false;
                var tuple = new Tuple<RDMMessage, RDMMessage>(o.Value.Item1, rdmMessage);
                buffer.AddOrUpdate(o.Key, tuple, (x, y) => tuple);
                return true;
            }
            //None Queued Parameters
            var obj = buffer.Where(b => b.Value.Item1.Parameter != ERDM_Parameter.QUEUED_MESSAGE).FirstOrDefault(b => b.Value.Item2 == null && rdmMessage.Parameter == b.Value.Item1.Parameter && rdmMessage.TransactionCounter == b.Value.Item1.TransactionCounter && b.Value.Item1.DestUID == rdmMessage.SourceUID && b.Value.Item1.SourceUID == rdmMessage.DestUID);
            if (obj.Value != null)
            {
                var tuple = new Tuple<RDMMessage, RDMMessage>(obj.Value.Item1, rdmMessage);
                buffer.AddOrUpdate(obj.Key, tuple, (x, y) => tuple);
                return true;
            }
            //Queued Parameters
            obj = buffer.Where(b => b.Value.Item1.Parameter == ERDM_Parameter.QUEUED_MESSAGE).FirstOrDefault(b => b.Value.Item2 == null && rdmMessage.TransactionCounter == b.Value.Item1.TransactionCounter && b.Value.Item1.DestUID == rdmMessage.SourceUID && b.Value.Item1.SourceUID == rdmMessage.DestUID);
            if (obj.Value != null)
            {
                var tuple = new Tuple<RDMMessage, RDMMessage>(obj.Value.Item1, rdmMessage);
                buffer.AddOrUpdate(obj.Key, tuple, (x, y) => tuple);
                return true;
            }
            return false;
        }


        public async Task<RequestResult> RequestMessage(RDMMessage requerst)
        {
            try
            {
                int key = random.Next();
                buffer.TryAdd(key, new Tuple<RDMMessage, RDMMessage>(requerst, null));
                RDMMessage response = null;
                await _sendMethode.Invoke(requerst);
                int count = 0;
                do
                {
                    if (this.IsDisposing || this.IsDisposed)
                        return new RequestResult(requerst);

                    buffer.TryGetValue(key, out Tuple<RDMMessage, RDMMessage> tuple1);
                    response = tuple1.Item2;
                    if (response != null)
                        break;
                    await Task.Delay(5, _cts.Token);
                    if (requerst.Command == ERDM_Command.NONE)
                    {
                        throw new Exception("Command is not set");
                    }
                    count++;
                    if (count % 300 == 299)
                    {
                        await Task.Delay(TimeSpan.FromTicks(random.Next(33, 777)), _cts.Token);
                        await _sendMethode.Invoke(requerst);
                        await Task.Delay(TimeSpan.FromTicks(random.Next(33, 777)), _cts.Token);
                    }
                    if (count > 1 && requerst.Command == ERDM_Command.DISCOVERY_COMMAND)
                        break;

                    if (count == 3000)
                        return new RequestResult(requerst);
                }
                while (response == null);
                buffer.TryRemove(key, out Tuple<RDMMessage, RDMMessage> tuple2);
                response = tuple2.Item2;
                return new RequestResult(requerst, response);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex);
            }
            return new RequestResult(requerst);
        }
    }
}