using RDMSharp.Metadata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static RDMSharp.Metadata.JSON.Command;

namespace RDMSharp
{
    public class PeerToPeerProcess
    {
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

        private RDMMessage request = null;
        private RDMMessage response = null;
        public byte MessageCounter => response?.MessageCounter ?? 0;
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

            //if (ParameterBag.PID != ERDM_Parameter.QUEUED_MESSAGE)
            Define = MetadataFactory.GetDefine(ParameterBag);
        }

        public async Task Run(AsyncRDMRequestHelper asyncRDMRequestHelper)
        {
            await run(asyncRDMRequestHelper);
        }

        private async Task run(AsyncRDMRequestHelper asyncRDMRequestHelper)
        {
            try
            {
                if (State != EPeerToPeerProcessState.Waiting)
                    return;

                if (asyncRDMRequestHelper == null)
                    throw new ArgumentNullException(nameof(asyncRDMRequestHelper));

                State = EPeerToPeerProcessState.Running;

                ECommandDublicate commandRequest = ECommandDublicate.GetRequest;
                if (Command == ERDM_Command.SET_COMMAND)
                    commandRequest = ECommandDublicate.SetRequest;

                ECommandDublicate commandResponse = ECommandDublicate.GetResponse;
                if (Command == ERDM_Command.SET_COMMAND)
                    commandResponse = ECommandDublicate.SetResponse;

                byte[] parameterData = //ParameterBag.PID != ERDM_Parameter.QUEUED_MESSAGE ? 
                    MetadataFactory.ParsePayloadToData(Define, commandRequest, RequestPayloadObject);// : null;
                request = new RDMMessage()
                {
                    Command = Command,
                    DestUID = UID,
                    SubDevice = SubDevice,
                    Parameter = ParameterBag.PID,
                    ParameterData = parameterData
                };
                List<byte> bytes = new List<byte>();
                while (State == EPeerToPeerProcessState.Running)
                {
                    var responseResult = await asyncRDMRequestHelper.RequestMessage(request);
                    if (!responseResult.Success ||
                        (responseResult.Response is not null && responseResult.Response.ResponseType == ERDM_ResponseType.NACK_REASON))
                    {
                        State = EPeerToPeerProcessState.Failed;
                        return;
                    }
                    response = responseResult.Response;
                    if (response.ResponseType == ERDM_ResponseType.ACK_TIMER && response.Value is AcknowledgeTimer timer)
                    {
                        await Task.Delay(timer.EstimidatedResponseTime);
                        request.Parameter = ERDM_Parameter.QUEUED_MESSAGE;
                        //Send Message on next loop
                        continue;
                    }
                    bytes.AddRange(response.ParameterData);
                    if (response.ResponseType == ERDM_ResponseType.ACK)
                    {
                        if (request.Parameter == ERDM_Parameter.QUEUED_MESSAGE)
                        {
                            ParameterBag = new ParameterBag(response.Parameter, ParameterBag.ManufacturerID, ParameterBag.DeviceModelID, ParameterBag.SoftwareVersionID);
                            Define = MetadataFactory.GetDefine(ParameterBag);                        
                        }
                        ResponsePayloadObject = MetadataFactory.ParseDataToPayload(Define, commandResponse, bytes.ToArray());
                        State = EPeerToPeerProcessState.Finished;
                        return;
                    }
                    if (response.ResponseType == ERDM_ResponseType.ACK_OVERFLOW)
                    {
                        //Do nothing else send another Request
                        //Send Message on next loop
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                this.Exception = e;
                State = EPeerToPeerProcessState.Failed;
            }
        }
    }
}