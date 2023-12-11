using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp
{
    public class AsyncRDMRequestHelper
    {
        ConcurrentDictionary<RDMMessage, RDMMessage> buffer = new ConcurrentDictionary<RDMMessage, RDMMessage>();
        Action<RDMMessage> _sendMethode;
        public AsyncRDMRequestHelper(Action<RDMMessage> sendMethode)
        {
            _sendMethode = sendMethode;
        }


        public bool ReceiveMethode(RDMMessage rdmMessage)
        {
            var obj = buffer.FirstOrDefault(b => b.Value == null && rdmMessage.TransactionCounter == b.Key.TransactionCounter && b.Key.DestUID == rdmMessage.SourceUID && b.Key.SourceUID == rdmMessage.DestUID);
            if (obj.Key != null)
            {
                buffer.AddOrUpdate(obj.Key, rdmMessage, (x, y) => rdmMessage);
                return true;
            }
            return false;
        }


        public async Task<RDMMessage> RequestParameter(RDMMessage requerst)
        {
            buffer.TryAdd(requerst, null);
            RDMMessage resopnse = null;
            _sendMethode.Invoke(requerst);
            do
            {
                buffer.TryGetValue(requerst, out resopnse);
                await Task.Delay(10);
            }
            while (resopnse == null);
            buffer.TryRemove(requerst, out resopnse);
            return resopnse;
        }
    }
}