using System.Threading.Tasks;

namespace RDMSharp.RDM
{
    public abstract class AbstractSendReceivePipeline
    {
        public abstract Task SendMesage(RDMMessage message);
        public abstract Task ReceiveMesage(RDMMessage message);
    }
}
