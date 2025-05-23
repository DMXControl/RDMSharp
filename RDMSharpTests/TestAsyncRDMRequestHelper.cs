namespace RDMSharpTests
{
    public class TestAsyncRDMRequestHelper
    {
        private AsyncRDMRequestHelper? asyncRDMRequestHelper;
        private bool hold = false;
        private SemaphoreSlim? hold_Semaphore;

        [SetUp]
        public void Setup()
        {
            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendMethode);
            hold_Semaphore = new SemaphoreSlim(1);
        }
        [TearDown]
        public void Teardown()
        {
            asyncRDMRequestHelper?.Dispose();
            asyncRDMRequestHelper = null;
            hold_Semaphore?.Dispose();
            hold_Semaphore = null;
        }

        private async Task sendMethode(RDMMessage rdmMessage)
        {
            if (!hold || hold_Semaphore == null)
                return;

            if(hold_Semaphore.CurrentCount==0)
            {
                await hold_Semaphore.WaitAsync(5000);
                hold_Semaphore.Release();
            }
            // Do nothing, just simulating sending a message
        }

        [Test, Order(1)]
        public async Task TestSimpleBackAndForth()
        {
            await testPackage(new UID(3, 56), new UID(55, 90), 1, SubDevice.Root, ERDM_Parameter.DMX_START_ADDRESS, ERDM_Command.GET_COMMAND, new byte[] { 0x00, 0x01 });
            await testPackage(new UID(3, 56), new UID(55, 90), 1, new SubDevice(34), ERDM_Parameter.DMX_START_ADDRESS, ERDM_Command.GET_COMMAND, new byte[] { 0x00, 0x04 });
            await testPackage(new UID(3, 56), new UID(55, 90), 1, SubDevice.Root, ERDM_Parameter.DMX_START_ADDRESS, ERDM_Command.SET_COMMAND, new byte[] { 0x00, 0x04 });
            await testPackage(new UID(3, 56), new UID(55, 90), 1, SubDevice.Broadcast, ERDM_Parameter.DMX_START_ADDRESS, ERDM_Command.SET_COMMAND, new byte[] { 0x00, 0x04 });

            await testPackage(new UID(3, 56), new UID(55, 90), 1, SubDevice.Root, ERDM_Parameter.DMX_START_ADDRESS, ERDM_Command.GET_COMMAND, new byte[] { 0x00, 0x01 },1000);
        }
        [Test, Order(3), CancelAfter(10000)]
        public async Task TestDelayedResponse()
        {
            await testPackage(new UID(3, 56), new UID(55, 90), 1, SubDevice.Root, ERDM_Parameter.DMX_START_ADDRESS, ERDM_Command.GET_COMMAND, new byte[] { 0x00, 0x01 }, 8000);
        }

        [Test, Order(6)]
        public async Task TestSimultanRequests()
        {
            await hold_Semaphore!.WaitAsync();
            hold=true;
            const int delay = 1000;
            UID[] sourceUIDs = new UID[] { new UID(3, 56), new UID(3, 33), new UID(40, 33) };
            UID[] destUIDs = new UID[] { new UID(65, 90), new UID(55, 2), new UID(55, 82) };
            SubDevice[] subDevices = new SubDevice[] { SubDevice.Root, new SubDevice(34), new SubDevice(59) };
            ERDM_Parameter[] parameters = new ERDM_Parameter[] { ERDM_Parameter.DMX_START_ADDRESS, ERDM_Parameter.DMX_PERSONALITY, ERDM_Parameter.DISPLAY_INVERT };
            ERDM_Command[] commands = new ERDM_Command[] { ERDM_Command.GET_COMMAND, ERDM_Command.SET_COMMAND };

            List<Task> tasks = new List<Task>();

            foreach (UID sourceUID in sourceUIDs)
                foreach (UID destUID in destUIDs)
                    foreach (SubDevice subDevice in subDevices)
                        foreach (ERDM_Parameter parameter in parameters)
                            foreach (ERDM_Command command in commands)
                            {
                                byte[]? parameterData = null;
                                if(parameter == ERDM_Parameter.DMX_START_ADDRESS && command == ERDM_Command.GET_COMMAND)
                                    parameterData = new byte[] { 0x00, 0x01 };
                                else if (parameter == ERDM_Parameter.DMX_PERSONALITY && command == ERDM_Command.GET_COMMAND)
                                    parameterData = new byte[] { 0x01, 0x02 };
                                else if (parameter == ERDM_Parameter.DISPLAY_INVERT && command == ERDM_Command.GET_COMMAND)
                                    parameterData = new byte[] { 0x01 };
                                tasks.Add(testPackage(sourceUID, destUID, (byte)(tasks.Count % byte.MaxValue), subDevice, parameter, command, new byte[] { 0x00, 0x01 }, delay));
                            }
            hold_Semaphore?.Release();
            await Task.WhenAll(tasks);
        }

        [Test, Order(7)]
        public async Task TestSimultanRequestsSameTransactionID()
        {
            await hold_Semaphore!.WaitAsync();
            hold = true;
            const int delay = 1000;
            UID[] sourceUIDs = new UID[] { new UID(3, 56), new UID(3, 33), new UID(40, 33) };
            UID[] destUIDs = new UID[] { new UID(65, 90), new UID(55, 2), new UID(55, 82) };
            SubDevice[] subDevices = new SubDevice[] { SubDevice.Root, new SubDevice(34), new SubDevice(59) };
            ERDM_Parameter[] parameters = new ERDM_Parameter[] { ERDM_Parameter.DMX_START_ADDRESS, ERDM_Parameter.DMX_PERSONALITY, ERDM_Parameter.DISPLAY_INVERT };
            ERDM_Command[] commands = new ERDM_Command[] { ERDM_Command.GET_COMMAND, ERDM_Command.SET_COMMAND };

            List<Task> tasks = new List<Task>();

            foreach (UID sourceUID in sourceUIDs)
                foreach (UID destUID in destUIDs)
                    foreach (SubDevice subDevice in subDevices)
                        foreach (ERDM_Parameter parameter in parameters)
                            foreach (ERDM_Command command in commands)
                            {
                                byte[]? parameterData = null;
                                if (parameter == ERDM_Parameter.DMX_START_ADDRESS && command == ERDM_Command.GET_COMMAND)
                                    parameterData = new byte[] { 0x00, 0x01 };
                                else if (parameter == ERDM_Parameter.DMX_PERSONALITY && command == ERDM_Command.GET_COMMAND)
                                    parameterData = new byte[] { 0x01, 0x02 };
                                else if (parameter == ERDM_Parameter.DISPLAY_INVERT && command == ERDM_Command.GET_COMMAND)
                                    parameterData = new byte[] { 0x01 };
                                tasks.Add(testPackage(sourceUID, destUID, 1, subDevice, parameter, command, new byte[] { 0x00, 0x01 }, delay));
                            }
            hold_Semaphore?.Release();
            await Task.WhenAll(tasks);
        }

        #region
        private async Task testPackage(UID sourceUID, UID destUID, byte transactionCounter, SubDevice subDevice, ERDM_Parameter parameter, ERDM_Command command, byte[] parameterData, int responseDelay = 100)
        {
            Assert.That(sourceUID, Is.Not.EqualTo(destUID));
            Assert.That(command.HasFlag(ERDM_Command.RESPONSE), Is.False);
            await testPackage(
                new RDMMessage()
                {
                    SourceUID = sourceUID,
                    DestUID = destUID,
                    TransactionCounter = transactionCounter,
                    SubDevice = subDevice,
                    Parameter = parameter,
                    Command = command
                }, new RDMMessage()
                {
                    DestUID = sourceUID,
                    SourceUID = destUID,
                    TransactionCounter = transactionCounter,
                    SubDevice = subDevice,
                    Parameter = parameter,
                    Command = command | ERDM_Command.RESPONSE,
                    ParameterData = parameterData
                }, responseDelay);
        }
        private async Task testPackage(RDMMessage request, RDMMessage response, int responseDelay = 100)
        {
            Task task = Task.Run(async () =>
            {
                var result = await asyncRDMRequestHelper!.RequestMessage(request);
                validate(request, result);
            });
            await Task.Delay(responseDelay); // Simulate some delay before the next task

            asyncRDMRequestHelper!.ReceiveMessage(response);

            await task;
        }
        private void validate(RDMMessage request, RequestResult result)
        {
            string failMessage = $"Request: {request.ToString()} Response: {result.Response?.ToString()}";

            if (request.Command == ERDM_Command.SET_COMMAND && request.SubDevice.IsBroadcast)
            {
                Assert.That(result.Success, Is.True, failMessage);
                Assert.That(result.Response, Is.Null, failMessage);
                return;
            }

            Assert.That(result.Success, Is.True, failMessage);
            Assert.That(result.Response, Is.Not.Null, failMessage);
            Assert.That(result.Response.DestUID, Is.EqualTo(request.SourceUID), failMessage);
            Assert.That(result.Response.SourceUID, Is.EqualTo(request.DestUID), failMessage);
            Assert.That(result.Response.SubDevice, Is.EqualTo(request.SubDevice), failMessage);
            Assert.That(result.Response.TransactionCounter, Is.EqualTo(request.TransactionCounter), failMessage);
            Assert.That(result.Response.Command, Is.EqualTo(request.Command | ERDM_Command.RESPONSE), failMessage);

            if (request.Command == ERDM_Command.GET_COMMAND && request.Parameter != ERDM_Parameter.QUEUED_MESSAGE)
                Assert.That(result.Response.Parameter, Is.EqualTo(request.Parameter), failMessage);
        }
        #endregion
    }
}