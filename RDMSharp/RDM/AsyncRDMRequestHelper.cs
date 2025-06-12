using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp
{
    public class AsyncRDMRequestHelper : IDisposable
    {
        private static readonly ILogger Logger = Logging.CreateLogger<AsyncRDMRequestHelper>();
        private static readonly Random random = new Random();
        private readonly ConcurrentDictionary<int, AsyncBufferBag> buffer = new ConcurrentDictionary<int, AsyncBufferBag>();
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
            this.IsDisposing = false;
        }

        public bool ReceiveMessage(RDMMessage rdmMessage)
        {
            if (this.IsDisposing || this.IsDisposed)
                return false;

            if (rdmMessage.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
            {
                var o = buffer.FirstOrDefault(b => b.Value.Request.Parameter == rdmMessage.Parameter);
                if (o.Value == null)
                    return false;

                updateBag(o.Value, rdmMessage);
                return true;
            }
            //By Key
            var key = generateKey(rdmMessage);
            if (buffer.TryGetValue(key, out AsyncBufferBag bag))
                if (checkNonQueued(bag.Request, rdmMessage))
                {
                    updateBag(bag, rdmMessage);
                    return true;
                }

            //None Queued Parameters
            var obj = buffer.Where(b => b.Value.Request.Parameter != ERDM_Parameter.QUEUED_MESSAGE).FirstOrDefault(b => b.Value.Response == null && checkNonQueued(b.Value.Request, rdmMessage));
            if (obj.Value != null)
            {
                updateBag(obj.Value, rdmMessage);
                return true;
            }
            //Queued Parameters
            obj = buffer.Where(b => b.Value.Request.Parameter == ERDM_Parameter.QUEUED_MESSAGE).FirstOrDefault(b => b.Value.Response == null && checkQueued(b.Value.Request, rdmMessage));
            if (obj.Value != null)
            {
                updateBag(obj.Value, rdmMessage);
                return true;
            }

            void updateBag(AsyncBufferBag bag, RDMMessage response)
            {
                bag.SetResponse(response);
                buffer.AddOrUpdate(bag.Key, bag, (key, oldValue) => bag);
            }
            return false;

            bool checkNonQueued(RDMMessage request, RDMMessage response)
            {
                if (request.Parameter != response.Parameter)
                    return false;
                if (request.TransactionCounter != response.TransactionCounter)
                    return false;
                if (request.SubDevice != response.SubDevice)
                    return false;
                if (request.SourceUID != response.DestUID)
                    return false;
                if (request.DestUID != response.SourceUID)
                    return false;
                if ((request.Command | ERDM_Command.RESPONSE) != response.Command)
                    return false;

                return true;
            }
            bool checkQueued(RDMMessage request, RDMMessage response)
            {
                if (request.TransactionCounter != response.TransactionCounter)
                    return false;
                if (request.SubDevice != response.SubDevice)
                    return false;
                if (request.SourceUID != response.DestUID)
                    return false;
                if (request.DestUID != response.SourceUID)
                    return false;
                if ((request.Command | ERDM_Command.RESPONSE) != response.Command)
                    return false;

                return true;
            }
        }


        public async Task<RequestResult> RequestMessage(RDMMessage request)
        {
            try
            {
                int key = generateKey(request);
                if (request.SubDevice.IsBroadcast)
                {
                    Logger?.LogTrace($"Send Subdevice-Broadcast Request: {request?.ToString()}");
                    await _sendMethode.Invoke(request);
                    return new RequestResult(request, null, TimeSpan.Zero); // Broadcasts are not expected to return a response.
                }
                if (!buffer.TryAdd(key, new AsyncBufferBag(key, request)))
                {
                    key += random.Next();
                    buffer.TryAdd(key, new AsyncBufferBag(key, request));
                }
                RDMMessage response = null;
                Logger?.LogTrace($"Send Request: {request?.ToString()}");
                await _sendMethode.Invoke(request);
                int count = 0;
                do
                {
                    if (this.IsDisposing || this.IsDisposed)
                        return new RequestResult(request);

                    buffer.TryGetValue(key, out AsyncBufferBag bag);
                    response = bag?.Response;
                    if (response != null)
                        break;
                    await Task.Delay(5, _cts.Token);
                    if (request.Command == ERDM_Command.NONE)
                    {
                        throw new Exception("Command is not set");
                    }
                    count++;
                    if (count % 300 == 299)
                    {
                        await Task.Delay(TimeSpan.FromTicks(random.Next(33, 777)), _cts.Token);
                        Logger?.LogTrace($"Retry Request: {request?.ToString()} ElapsedTime: {bag?.ElapsedTime}");
                        await _sendMethode.Invoke(request);
                        await Task.Delay(TimeSpan.FromTicks(random.Next(33, 777)), _cts.Token);
                    }
                    if (count > 3 && request.Command == ERDM_Command.DISCOVERY_COMMAND)
                    {
                        Logger?.LogTrace($"Discovery request exceeded timeout");
                        break;
                    }

                    if (count == 3000)
                    {
                        Logger?.LogTrace($"Timeout Request: {request?.ToString()} ElapsedTime: {bag?.ElapsedTime}");
                        return new RequestResult(request);
                    }
                }
                while (response == null);
                buffer.TryRemove(key, out AsyncBufferBag bag2);
                response = bag2.Response;
                var result = new RequestResult(request, response, bag2.ElapsedTime);
                Logger?.LogTrace($"Successful Request: {request?.ToString()} Response: {response?.ToString()} ElapsedTime: {bag2?.ElapsedTime}");
                return result;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex);
            }
            return new RequestResult(request);
        }
        private int generateKey(RDMMessage request)
        {
            var command = (ERDM_Command)((byte)request.Command & ~(byte)ERDM_Command.RESPONSE);
            if (command == ERDM_Command.DISCOVERY_COMMAND)
                return random.Next();

            int key = (request.SourceUID.GetHashCode() + request.DestUID.GetHashCode())*111111111
                + (9 + request.SubDevice.GetHashCode()) * 45123
                + (123 + request.TransactionCounter.GetHashCode()) * 931
                + request.Parameter.GetHashCode()*67
                + command.GetHashCode()*7;
            return key;
        }

        private class AsyncBufferBag
        {
            public readonly int Key;

            public readonly RDMMessage Request;
            public readonly DateTime RequestTimestamp;

            public RDMMessage Response { get; private set; }
            public DateTime? ResponseTimestamp { get; private set; }

            private TimeSpan? elapsedTime;
            public TimeSpan ElapsedTime
            {
                get
                {
                    if (elapsedTime.HasValue)
                        return elapsedTime.Value;

                    return DateTime.UtcNow - RequestTimestamp; ;
                }
                private set
                {
                    elapsedTime = value;
                }
            }

            public AsyncBufferBag(int key, RDMMessage request)
            {
                Request = request;
                Response = null;
                Key = key;
                RequestTimestamp = DateTime.UtcNow;
            }
            public void SetResponse(RDMMessage response)
            {
                Response = response;
                ResponseTimestamp = DateTime.UtcNow;
                ElapsedTime = ResponseTimestamp.Value - RequestTimestamp;
            }
        }
    }
}