using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata
{
    public class TestParameterBag
    {

        [Test]
        public void TestMany()
        {
            Assert.Throws(typeof(NotSupportedException), () => new ParameterBag());
            HashSet<ParameterBag> parameterBags = new HashSet<ParameterBag>();
            Assert.DoesNotThrow(() => parameterBags.Add(new ParameterBag(ERDM_Parameter.CURVE)));


            ERDM_Parameter pid = ERDM_Parameter.ADD_TAG;
            ParameterBag bag = new ParameterBag(pid);
            parameterBags.Add(bag);
            Assert.Multiple(() =>
            {
                Assert.That(bag, Is.Not.Default);
                Assert.That(bag.PID, Is.EqualTo(pid));
                Assert.That(bag.ManufacturerID, Is.EqualTo(0));
                Assert.That(bag.DeviceModelID, Is.EqualTo(null));
                Assert.That(bag.SoftwareVersionID, Is.EqualTo(null));
                Assert.That(bag.ToString(), Is.EqualTo(pid.ToString()));
            });

            pid = (ERDM_Parameter)0x8943;
            bag = new ParameterBag(pid, 432, 678, 42);
            parameterBags.Add(bag);
            Assert.Multiple(() =>
            {
                Assert.That(bag, Is.Not.Default);
                Assert.That(bag.PID, Is.EqualTo(pid));
                Assert.That(bag.ManufacturerID, Is.EqualTo(432));
                Assert.That(bag.DeviceModelID, Is.EqualTo(678));
                Assert.That(bag.SoftwareVersionID, Is.EqualTo(42));
                Assert.That(bag.ToString(), Is.EqualTo($"{pid} ManufacturerID: {432}, DeviceModelID: {678}, SoftwareVersionID: {42}"));
            });

            bag = new ParameterBag(pid, 432, 678);
            parameterBags.Add(bag);
            Assert.Multiple(() =>
            {
                Assert.That(bag, Is.Not.Default);
                Assert.That(bag.PID, Is.EqualTo(pid));
                Assert.That(bag.ManufacturerID, Is.EqualTo(432));
                Assert.That(bag.DeviceModelID, Is.EqualTo(678));
                Assert.That(bag.SoftwareVersionID, Is.EqualTo(null));
                Assert.That(bag.ToString(), Is.EqualTo($"{pid} ManufacturerID: {432}, DeviceModelID: {678}"));
            });

            bag = new ParameterBag(pid, 432);
            parameterBags.Add(bag);
            Assert.Multiple(() =>
            {
                Assert.That(bag, Is.Not.Default);
                Assert.That(bag.PID, Is.EqualTo(pid));
                Assert.That(bag.ManufacturerID, Is.EqualTo(432));
                Assert.That(bag.DeviceModelID, Is.EqualTo(null));
                Assert.That(bag.SoftwareVersionID, Is.EqualTo(null));
                Assert.That(bag.ToString(), Is.EqualTo($"{pid} ManufacturerID: {432}"));
            });

            Assert.Throws(typeof(ArgumentNullException), () => new ParameterBag(pid));
        }


        [Test]
        public void TestEqualMethodes()
        {
            Assert.Multiple(() =>
            {
                Assert.That(new ParameterBag(ERDM_Parameter.CURVE) == new ParameterBag(ERDM_Parameter.CURVE), Is.True);
                Assert.That(new ParameterBag(ERDM_Parameter.CURVE) == new ParameterBag(ERDM_Parameter.DIMMER_INFO), Is.False);

                Assert.That(new ParameterBag(ERDM_Parameter.CURVE) != new ParameterBag(ERDM_Parameter.CURVE), Is.False);
                Assert.That(new ParameterBag(ERDM_Parameter.CURVE) != new ParameterBag(ERDM_Parameter.DIMMER_INFO), Is.True);

                Assert.That(new ParameterBag(ERDM_Parameter.CURVE).Equals(new ParameterBag(ERDM_Parameter.CURVE)), Is.True);
                Assert.That(new ParameterBag(ERDM_Parameter.CURVE).Equals(new ParameterBag(ERDM_Parameter.DIMMER_INFO)), Is.False);

                Assert.That(new ParameterBag(ERDM_Parameter.CURVE).Equals((object)new ParameterBag(ERDM_Parameter.CURVE)), Is.True);
                Assert.That(new ParameterBag(ERDM_Parameter.CURVE).Equals((object)new ParameterBag(ERDM_Parameter.DIMMER_INFO)), Is.False);

                Assert.That(new ParameterBag(ERDM_Parameter.CURVE).Equals(null), Is.False);

                ERDM_Parameter pid = (ERDM_Parameter)0x8555;
                Assert.That(new ParameterBag(pid, 1), Is.Not.EqualTo(new ParameterBag(pid, 2)));
                Assert.That(new ParameterBag(pid, 1), Is.Not.EqualTo(new ParameterBag(pid, 1, 444)));
                Assert.That(new ParameterBag(pid, 1, 444), Is.Not.EqualTo(new ParameterBag(pid, 1, 444, 5)));
#pragma warning disable NUnit2009
                Assert.That(new ParameterBag(pid, 1, 444, 5), Is.EqualTo(new ParameterBag(pid, 1, 444, 5)));
#pragma warning restore NUnit2009
            });
        }
    }
}