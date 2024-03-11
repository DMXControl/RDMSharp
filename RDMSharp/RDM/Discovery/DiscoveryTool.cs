using System.ComponentModel;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractDiscoveryTool : INotifyPropertyChanged
    {
        private AsyncRDMRequestHelper asyncRDMRequestHelper;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool DiscoveryInProgress { get; private set; }

        public AbstractDiscoveryTool()
        {
            asyncRDMRequestHelper = new AsyncRDMRequestHelper(SendRDMMessage);
        }

        protected abstract Task SendRDMMessage(RDMMessage rdmMessage);

        protected void ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            asyncRDMRequestHelper.ReceiveMethode(rdmMessage);
        }

        public async Task FullDiscovery()
        {
            if (DiscoveryInProgress) return;
            DiscoveryInProgress = true;
            RDMMessage unmuteBroadcastMessage = new RDMMessage();
            unmuteBroadcastMessage.Parameter = ERDM_Parameter.DISC_UN_MUTE;
            unmuteBroadcastMessage.DestUID = RDMUID.Broadcast;
            await SendRDMMessage(unmuteBroadcastMessage);


            RDMMessage discMessage = new RDMMessage();
            discMessage.Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH;
            await asyncRDMRequestHelper.RequestParameter(discMessage);
        }
        public void UpdateDiscovery()
        {

        }
    }
}
