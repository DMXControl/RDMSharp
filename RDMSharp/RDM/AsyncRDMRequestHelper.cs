using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp
{
    public class AsyncRDMRequestHelper
    {
        private static ILogger Logger = null;
        private static Random random = new Random();
        ConcurrentDictionary<RDMMessage, RDMMessage> buffer = new ConcurrentDictionary<RDMMessage, RDMMessage>();
        Func<RDMMessage, Task> _sendMethode;
        public AsyncRDMRequestHelper(Func<RDMMessage,Task> sendMethode)
        {
            _sendMethode = sendMethode;
        }


        public bool ReceiveMethode(RDMMessage rdmMessage)
        {
            if(rdmMessage.Command== ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
            {
                var o = buffer.FirstOrDefault(b=>b.Key.Parameter == rdmMessage.Parameter);
                buffer.AddOrUpdate(o.Key, rdmMessage, (x, y) => rdmMessage);
            }
            //None Queued Parameters
            var obj = buffer.Where(b=>b.Key.Parameter!= ERDM_Parameter.QUEUED_MESSAGE).FirstOrDefault(b => b.Value == null && rdmMessage.Parameter == b.Key.Parameter && rdmMessage.TransactionCounter == b.Key.TransactionCounter && b.Key.DestUID == rdmMessage.SourceUID && b.Key.SourceUID == rdmMessage.DestUID);
            if (obj.Key != null)
            {
                buffer.AddOrUpdate(obj.Key, rdmMessage, (x, y) => rdmMessage);
                return true;
            }
            //Queued Parameters
            obj = buffer.Where(b => b.Key.Parameter == ERDM_Parameter.QUEUED_MESSAGE).FirstOrDefault(b => b.Value == null && rdmMessage.TransactionCounter == b.Key.TransactionCounter && b.Key.DestUID == rdmMessage.SourceUID && b.Key.SourceUID == rdmMessage.DestUID);
            if (obj.Key != null)
            {
                buffer.AddOrUpdate(obj.Key, rdmMessage, (x, y) => rdmMessage);
                return true;
            }
            return false;
        }


        public async Task<RequestResult> RequestParameter(RDMMessage requerst)
        {
            try
            {
                buffer.TryAdd(requerst, null);
                RDMMessage response = null;
                await _sendMethode.Invoke(requerst);
                int count = 0;
                do
                {
                    buffer.TryGetValue(requerst, out response);
                    if (response != null)
                        break;
                    await Task.Delay(10);
                    if (requerst.Command == ERDM_Command.NONE)
                    {
                        throw new Exception("Command is not set");
                    }
                    count++;
                    if (count % 300 == 299)
                    {
                        await Task.Delay(TimeSpan.FromTicks(random.Next(33, 777)));
                        await _sendMethode.Invoke(requerst);
                        await Task.Delay(TimeSpan.FromTicks(random.Next(33, 777)));
                    }
                    if (count > 10 && requerst.Command == ERDM_Command.DISCOVERY_COMMAND)
                        break;

                    if (count == 3000)
                        return new RequestResult(requerst);
                }
                while (response == null);
                buffer.TryRemove(requerst, out response);
                return new RequestResult(requerst, response);
            }
            catch( Exception ex)
            {
                Logger?.LogError(string.Empty, ex);
            }
            return new RequestResult(requerst);
        }
    }
}