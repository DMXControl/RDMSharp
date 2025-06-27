using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMSendReceive_SlotOverflow
    {
        private MockGeneratedDevice_SlotOverflow? generated;
        private MockDevice? remote;
        private Random random = new Random();
        [SetUp]
        public void Setup()
        {
            var uid = new UID((ushort)random.Next(), (uint)random.Next());
            generated = new MockGeneratedDevice_SlotOverflow(uid);
            remote = new MockDevice(uid);
        }
        [TearDown]
        public void TearDown()
        {
            generated?.Dispose();
            generated = null;
            remote?.Dispose();
            remote = null;
        }

        [Test, Retry(3), Order(1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public async Task TestDevice1()
        {
            var parameterValuesRemote = remote!.GetAllParameterValues();
            var parameterValuesGenerated = generated!.GetAllParameterValues();

            Console.WriteLine($"Generated: {String.Join(", ", parameterValuesGenerated.OrderBy(p => p.Key).Select(x => $"{x.Key}"))}");
            Console.WriteLine($"Remote:    {String.Join(", ", parameterValuesRemote.OrderBy(p => p.Key).Select(x => $"{x.Key}"))}");

            Assert.Multiple(() =>
            {
                Assert.That(parameterValuesGenerated.Keys, Is.EquivalentTo(parameterValuesRemote.Keys));
                foreach (var parameter in parameterValuesGenerated.Keys)
                {
                    Assert.That(parameterValuesRemote.Keys, Contains.Item(parameter), $"Tested Parameter {parameter}");
                    if (parameterValuesGenerated[parameter] is Array)
                        Assert.That(parameterValuesGenerated[parameter], Is.EquivalentTo((Array)parameterValuesRemote[parameter]), $"Tested Parameter {parameter}");
                    else
                        Assert.That(parameterValuesGenerated[parameter], Is.EqualTo(parameterValuesRemote[parameter]), $"Tested Parameter {parameter}");
                }
                foreach (var parameter in parameterValuesRemote.Keys)
                {
                    Assert.That(parameterValuesGenerated.Keys, Contains.Item(parameter), $"Tested Parameter {parameter}");
                    if (parameterValuesRemote[parameter] is Array)
                        Assert.That(parameterValuesRemote[parameter], Is.EquivalentTo((Array)parameterValuesGenerated[parameter]), $"Tested Parameter {parameter}");
                    else
                        Assert.That(parameterValuesRemote[parameter], Is.EqualTo(parameterValuesGenerated[parameter]), $"Tested Parameter {parameter}");
                }
                Assert.That(parameterValuesRemote, Has.Count.EqualTo(parameterValuesGenerated.Count));
            });
        }
    }
}