using RDMSharpTests.Devices.Mock;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDMSharpTests
{
    [SetUpFixture]
    public class GlobalTestSetup
    {
        private readonly ConcurrentDictionary<long, RDMMessage> identifyer = new ConcurrentDictionary<long, RDMMessage>();
        private Random rnd = new Random();
        public static bool ImitateRealConditions { get; set; } = false; // Set to false to use the Mock SendReceivePipeline
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            GlobalTimers.Instance.InternalAllTimersToTestSpeed();
            RDMSharp.RDMSharp.Initialize(new UID(), async (rdmMessage) =>
            {
                await SendRDMMessage(rdmMessage);
            });
            SendReceivePipelineImitateRealConditions.RDMMessageReceivedResponse += (sender, data) =>
            {
                if (!ImitateRealConditions)
                    return;
                if (data == null)
                    return;
                RDMMessage rdmMessage = new RDMMessage(data);
                if (rdmMessage == null)
                    return;

                RDMSharp.RDMSharp.Instance.ResponseReceived(rdmMessage);
            };
            SendReceivePipelineImitateRealConditions.RDMMessageReceivedRequest += async (sender, rdmMessage) =>
            {
                if (!ImitateRealConditions)
                    return;
                if (rdmMessage == null)
                    return;

                if (RDMSharp.RDMSharp.Instance.RequestReceived(rdmMessage, out RDMMessage response))
                    await SendRDMMessage(response);
            };
            SendReceivePipeline.RDMMessageReceived += (sender, tuple) =>
            {
                if (ImitateRealConditions)
                    return;
                if (tuple == null)
                    return;
                RDMMessage rdmMessage = tuple.Item2;
                if (rdmMessage == null)
                    return;
                if (rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                    RDMSharp.RDMSharp.Instance.ResponseReceived(rdmMessage);
                else
                    if (RDMSharp.RDMSharp.Instance.RequestReceived(rdmMessage, out RDMMessage response))
                    _ = SendRDMMessage(response);
            };
        }

        private async Task SendRDMMessage(RDMMessage rdmMessage)
        {
            await Task.CompletedTask;
            if (ImitateRealConditions)
                _ = SendReceivePipelineImitateRealConditions.RDMMessageSend(rdmMessage); // in this case as responder we don't wait for comlpletion of the send, we just send it and forget it
            else
            {
                long i = rnd.Next(); // Generate a random identifier for the message
                SendReceivePipeline.RDMMessageSend(i, rdmMessage);
            }
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            // Wird einmal nach allen Tests ausgeführt
            Console.WriteLine("Global TearDown läuft");
        }
        public static bool IsRunningOnGitHubActions()
        {
            return Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        }
    }
}
