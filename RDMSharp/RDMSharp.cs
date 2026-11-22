using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp;

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
    public event EventHandler<RDMMessage> ResponseReceivedEvent;
    public event EventHandler<RequestReceivedEventArgs> RequestReceivedEvent;

    private readonly ConcurrentDictionary<UID, byte> transactionCounters = new ConcurrentDictionary<UID, byte>();
    private readonly ConcurrentDictionary<UID, SemaphoreSlim> transactionLock = new ConcurrentDictionary<UID, SemaphoreSlim>();
    private readonly ConcurrentBag<AbstractRemoteRDMDevice> remoteDeviceCache = new ConcurrentBag<AbstractRemoteRDMDevice>();

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
        RDMMessage _response = null;
        var e = new RequestReceivedEventArgs(request);
        var handlers = RequestReceivedEvent?.GetInvocationList() ?? Array.Empty<Delegate>();

        // Parallel ausführen, aber Reihenfolge der Responses beachten
        ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount // Optional: Setze die maximale Parallelität
        };
        Parallel.ForEach(handlers, parallelOptions, (handler, state) =>
        {
            handler.InvokeFailSafe(this, e);
            if (e.Response is not null)
            {
                if (request.Command != ERDM_Command.DISCOVERY_COMMAND || request.DestUID.IsBroadcast)
                {
                    _response = e.Response;
                    state.Stop(); // Beende Parallel.ForEach, wenn eine Response gefunden wurde
                }
            }
        });

        response = _response;
        return response is not null;
    }

    public static async Task Initialize(UID controllerUID, Func<RDMMessage, Task> sendMethode)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("RDMSharp instance already exists. Use Instance property to access it.");
        }
        _instance = new RDMSharp(controllerUID, sendMethode);
        await MetadataFactory.AwaitInitialize();
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
    internal byte getTransactionCounter(UID uid)
    {
        if (transactionCounters.TryGetValue(uid, out byte tc))
        {
            tc++;
            transactionCounters.AddOrUpdate(uid, (a) => tc, (b, c) => tc);
            return tc;
        }
        else
        {
            transactionCounters.TryAdd(uid, 0);
            return 0;
        }
    }
    /// <summary>
    /// Waits until no other transaction runs for the UID (one request at a time).
    /// </summary>
    /// <returns>
    /// false if the wait timed out: the caller can go on, but it does not hold the lock and must not release it
    /// (that would let two transactions run at once from then on).
    /// </returns>
    internal async Task<bool> lockTransaktion(UID uid, TimeSpan? timeout = null)
    {
        // the semaphore is created once, even if several transactions of a new UID start at the same time
        var semaphore = transactionLock.GetOrAdd(uid, _ => new SemaphoreSlim(1, 1));

        if (timeout.HasValue)
            return await semaphore.WaitAsync(timeout.Value);

        await semaphore.WaitAsync();
        return true;
    }
    internal void unlockTransaktion(UID uid)
    {
        if (transactionLock.TryGetValue(uid, out SemaphoreSlim semaphore))
        {
            semaphore.Release();
        }
    }

    internal void AddRemoteDeviceToCache(AbstractRemoteRDMDevice abstractRemoteRDMDevice)
    {
        remoteDeviceCache.Add(abstractRemoteRDMDevice);
    }
    public bool TryGetRemoteDeviceFromCache(UID uid, SubDevice subDevice, out AbstractRemoteRDMDevice abstractRemoteRDMDevice)
    {
        abstractRemoteRDMDevice = null;
        try
        {
            abstractRemoteRDMDevice = remoteDeviceCache.Where(rd => rd.UID.Equals(uid)).Where(rd => rd.Subdevice.Equals(subDevice)).FirstOrDefault(rd => !(rd.IsDisposed || rd.IsDisposing));
        }
        catch { return false; }
        return abstractRemoteRDMDevice is not null;
    }
}