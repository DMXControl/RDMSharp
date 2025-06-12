using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDMSharp
{
    public class RDMSharp
    {
        private static RDMSharp _instance;
        public static RDMSharp Instance
        {
            get
            {
                return _instance;
            }
        }

        public readonly Func<RDMMessage, Task> SendMessage;
        public readonly AsyncRDMRequestHelper AsyncRDMRequestHelper;
        public event EventHandler<RDMMessage>? MessageReceivedEvent;

        private RDMSharp(Func<RDMMessage, Task> sendMessage)
        {
            _instance = this ?? throw new InvalidOperationException("RDMSharp instance already exists. Use Instance property to access it.");

            SendMessage = sendMessage ?? throw new ArgumentNullException(nameof(sendMessage), "SendMethode can't be null.");
            AsyncRDMRequestHelper = new AsyncRDMRequestHelper(SendMessage);
        }
        public void MessageReceived(RDMMessage rdmMessage)
        {
            MessageReceivedEvent?.InvokeFailSafe(this, rdmMessage);
        }

        public static void Initialize(Func<RDMMessage, Task> sendMethode)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("RDMSharp instance already exists. Use Instance property to access it.");
            }
            _instance = new RDMSharp(sendMethode);
        }
    }
}
