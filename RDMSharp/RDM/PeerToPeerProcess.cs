using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static RDMSharp.Metadata.JSON.Command;

namespace RDMSharp
{
    public class PeerToPeerProcess
    {
        private static readonly ILogger Logger = Logging.CreateLogger<PeerToPeerProcess>();

        public enum EPeerToPeerProcessState
        {
            Waiting,
            Running,
            Finished,
            Failed
        }
        public readonly ERDM_Command Command;
        public readonly UID UID;
        public readonly SubDevice SubDevice;
        public ParameterBag ParameterBag { get; private set; }
        public readonly DataTreeBranch RequestPayloadObject;
        public DataTreeBranch ResponsePayloadObject { get; private set; } = DataTreeBranch.Unset;

        public MetadataJSONObjectDefine Define { get; private set; }
        public EPeerToPeerProcessState State { get; private set; } = EPeerToPeerProcessState.Waiting;

        public Exception Exception { get; private set; }
        public ERDM_NackReason[] NackReason { get; private set; }

        private RDMMessage request = null;
        private RDMMessage response = null;
        public Func<RDMMessage, Task> BeforeSendMessage;
        public Func<RDMMessage, Task> ResponseMessage;
        public byte MessageCounter => response?.MessageCounter ?? 0;
        private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

        public PeerToPeerProcess(ERDM_Command command, UID uid, SubDevice subDevice, ParameterBag parameterBag, DataTreeBranch? payloadObject = null)
        {
            if (command != ERDM_Command.GET_COMMAND)
                if (command != ERDM_Command.SET_COMMAND)
                    throw new ArgumentException($"{nameof(command)} should be {ERDM_Command.GET_COMMAND} or {ERDM_Command.SET_COMMAND}");

            Command = command;
            UID = uid;
            SubDevice = subDevice;
            ParameterBag = parameterBag;
            RequestPayloadObject = payloadObject ?? DataTreeBranch.Unset;

            Logger?.LogTrace($"Create PeerToPeerProcess: {Command} UID: {UID} SubDevice: {SubDevice} Parameter: {ParameterBag.PID} PayloadObject: {RequestPayloadObject.ToString()}");

            //if (ParameterBag.PID != ERDM_Parameter.QUEUED_MESSAGE)
            Define = MetadataFactory.GetDefine(ParameterBag);
        }

        public async Task Run(AsyncRDMRequestHelper asyncRDMRequestHelper = null)
        {
            asyncRDMRequestHelper ??= RDMSharp.Instance.AsyncRDMRequestHelper;
            if (State != EPeerToPeerProcessState.Waiting)
                return;
            if (SemaphoreSlim.CurrentCount == 0)
                return;
            await SemaphoreSlim.WaitAsync();
            State = EPeerToPeerProcessState.Running;
            try
            {
                ECommandDublicate commandRequest = ECommandDublicate.GetRequest;
                if (Command == ERDM_Command.SET_COMMAND)
                    commandRequest = ECommandDublicate.SetRequest;

                ECommandDublicate commandResponse = ECommandDublicate.GetResponse;
                if (Command == ERDM_Command.SET_COMMAND)
                    commandResponse = ECommandDublicate.SetResponse;

                byte[] parameterData = null;
                if (Define != null)
                    parameterData = MetadataFactory.ParsePayloadToData(Define, commandRequest, RequestPayloadObject).First();
                request = new RDMMessage()
                {
                    Command = Command,
                    DestUID = UID,
                    SubDevice = SubDevice,
                    Parameter = ParameterBag.PID,
                    ParameterData = parameterData
                };
                List<byte> bytes = new List<byte>();
                bool done = false;
                int counter = 0;
                do
                {
                    counter++;
                    if (BeforeSendMessage != null)
                        await BeforeSendMessage(request);
                    if (response?.ResponseType == ERDM_ResponseType.ACK)
                        return;
                    var responseResult = await asyncRDMRequestHelper.RequestMessage(request);
                    if (!responseResult.Success ||
                        (responseResult.Response is not null && responseResult.Response.ResponseType == ERDM_ResponseType.NACK_REASON))
                    {
                        State = EPeerToPeerProcessState.Failed;
                        NackReason= responseResult.Response.NackReason;
                        if (responseResult.Response is not null)
                            ResponseMessage?.InvokeFailSafe(responseResult.Response);
                        return;
                    }
                    response = responseResult.Response;
                    switch (responseResult.Response.ResponseType)
                    {
                        case ERDM_ResponseType.ACK_TIMER:
                        case ERDM_ResponseType.ACK_TIMER_HI_RES:
                            if (response.Value is IAcknowledgeTimer timer)
                            {
                                await Task.Delay(timer.EstimidatedResponseTime);
                                request.Parameter = ERDM_Parameter.QUEUED_MESSAGE;
                                //Send Message on next loop
                                continue;
                            }
                            break;
                        case ERDM_ResponseType.ACK:
                            bytes.AddRange(response.ParameterData);
                            done = true;
                            var _byteArray = bytes.ToArray();
                            bytes.Clear();
                            bytes = null;
                            if (request.Parameter == ERDM_Parameter.QUEUED_MESSAGE)
                            {
                                ParameterBag = new ParameterBag(response.Parameter, ParameterBag.ManufacturerID, ParameterBag.DeviceModelID, ParameterBag.SoftwareVersionID);
                                Define = MetadataFactory.GetDefine(ParameterBag);
                            }
                            if (Define != null)
                                ResponsePayloadObject = MetadataFactory.ParseDataToPayload(Define, commandResponse, _byteArray);
                            State = EPeerToPeerProcessState.Finished;

                            ResponseMessage?.InvokeFailSafe(responseResult.Response);
                            return;
                        case ERDM_ResponseType.ACK_OVERFLOW:
                            bytes.AddRange(response.ParameterData);
                            continue;

                    }
                    //if ((response.ResponseType == ERDM_ResponseType.ACK_TIMER ||
                    //    response.ResponseType == ERDM_ResponseType.ACK_TIMER_HI_RES) && response.Value is IAcknowledgeTimer timer)
                    //{
                    //    await Task.Delay(timer.EstimidatedResponseTime);
                    //    request.Parameter = ERDM_Parameter.QUEUED_MESSAGE;
                    //    //Send Message on next loop
                    //    continue;
                    //}
                    //if (response.ResponseType == ERDM_ResponseType.ACK)
                    //{
                    //    bytes.AddRange(response.ParameterData);
                    //    done = true;
                    //    var _byteArray = bytes.ToArray();
                    //    bytes.Clear();
                    //    bytes = null;
                    //    if (request.Parameter == ERDM_Parameter.QUEUED_MESSAGE)
                    //    {
                    //        ParameterBag = new ParameterBag(response.Parameter, ParameterBag.ManufacturerID, ParameterBag.DeviceModelID, ParameterBag.SoftwareVersionID);
                    //        Define = MetadataFactory.GetDefine(ParameterBag);
                    //    }
                    //    if (Define != null)
                    //        ResponsePayloadObject = MetadataFactory.ParseDataToPayload(Define, commandResponse, _byteArray);
                    //    State = EPeerToPeerProcessState.Finished;

                    //    ResponseMessage?.InvokeFailSafe(responseResult.Response);
                    //    return;
                    //}
                    //if (response.ResponseType == ERDM_ResponseType.ACK_OVERFLOW)
                    //{
                    //    bytes.AddRange(response.ParameterData);
                    //    //Do nothing else send another Request
                    //    //Send Message on next loop
                    //    continue;
                    //}
                }
                while (!done && State == EPeerToPeerProcessState.Running);
            }
            catch (Exception e)
            {
                Logger?.LogError(e);
                this.Exception = e;
                State = EPeerToPeerProcessState.Failed;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}