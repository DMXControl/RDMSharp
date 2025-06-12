using System;
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

        public readonly UID ControllerUID;
        public readonly Func<RDMMessage, Task> SendMessage;
        public readonly AsyncRDMRequestHelper AsyncRDMRequestHelper;
        public event EventHandler<RDMMessage>? ResponseReceivedEvent;
        public event EventHandler<RequestReceivedEventArgs>? RequestReceivedEvent;

        private RDMSharp(UID controllerUID, Func<RDMMessage, Task> sendMessage)
        {
            _instance = this ?? throw new InvalidOperationException("RDMSharp instance already exists. Use Instance property to access it.");

            this.ControllerUID = controllerUID;

            SendMessage = sendMessage ?? throw new ArgumentNullException(nameof(sendMessage), "SendMethode can't be null.");
            AsyncRDMRequestHelper = new AsyncRDMRequestHelper(async (rdmMessage) =>
            {
                rdmMessage.SourceUID = ControllerUID;
                await SendMessage.Invoke(rdmMessage);
            });
        }
        public void ResponseReceived(RDMMessage rdmMessage)
        {
            if (!AsyncRDMRequestHelper.ReceiveMessage(rdmMessage))
                ResponseReceivedEvent?.InvokeFailSafe(this, rdmMessage);
        }
        public bool RequestReceived(RDMMessage request, out RDMMessage response)
        {

            var e = new RequestReceivedEventArgs(request);
            RequestReceivedEvent.InvokeFailSafe(this, e);
            if (request.Command != ERDM_Command.DISCOVERY_COMMAND || request.DestUID.IsBroadcast)
            {
                if (e.Response is not null)
                {
                    response = e.Response;
                    return true;
                }
            }
            response = null;
            return false;
        }

        public static void Initialize(UID controllerUID, Func<RDMMessage, Task> sendMethode)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("RDMSharp instance already exists. Use Instance property to access it.");
            }
            _instance = new RDMSharp(controllerUID, sendMethode);
        }

        public class RequestReceivedEventArgs : EventArgs
        {
            public RDMMessage Request { get; }
            public RDMMessage Response { get; set; }
            public RequestReceivedEventArgs(RDMMessage request)
            {
                Request = request ?? throw new ArgumentNullException(nameof(request), "Request can't be null.");
                Response = null;
            }
        }
    }
}