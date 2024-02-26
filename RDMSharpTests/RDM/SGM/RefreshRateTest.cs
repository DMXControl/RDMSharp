using RDMSharp.ParameterWrapper.SGM;
using System.Collections.Concurrent;
using System.Net;

namespace RDMSharpTest.RDM.SGM
{
    public class RefreshRateTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RefreshRate refreshRateRaw = new RefreshRate((byte)1);
            Assert.That(refreshRateRaw.RawValue, Is.EqualTo(1));
            Assert.That(refreshRateRaw.Frequency, Is.EqualTo(RefreshRate.FREQUENCY_MULTIPLYER));
            RefreshRate refreshRateFreq = new RefreshRate(RefreshRate.FREQUENCY_MULTIPLYER);
            Assert.That(refreshRateFreq.Frequency, Is.EqualTo(RefreshRate.FREQUENCY_MULTIPLYER));
            Assert.That(refreshRateFreq.RawValue, Is.EqualTo(1));
            Assert.That(refreshRateFreq, Is.EqualTo(refreshRateRaw));

            for (byte b = 2; b < byte.MaxValue - 16; b += 8)
            {
                refreshRateRaw = new RefreshRate(b);
                Assert.That(refreshRateRaw.RawValue, Is.EqualTo(b));
                Assert.That(refreshRateRaw.Frequency, Is.EqualTo(RefreshRate.FREQUENCY_MULTIPLYER / b));
                refreshRateFreq = new RefreshRate(RefreshRate.FREQUENCY_MULTIPLYER / b);
                Assert.That(refreshRateFreq.Frequency, Is.EqualTo(RefreshRate.FREQUENCY_MULTIPLYER / b));
                Assert.That(refreshRateFreq.RawValue, Is.EqualTo(b));
                Assert.That(refreshRateFreq, Is.EqualTo(refreshRateRaw));
            }

            refreshRateRaw = new RefreshRate(0);
            Assert.That(refreshRateRaw.RawValue, Is.EqualTo(0));
            Assert.That(refreshRateRaw.Frequency, Is.EqualTo(0));

            var src = refreshRateRaw.ToString();
            Assert.That(src, Is.Not.Null);

            ConcurrentDictionary<RefreshRate, string> dict = new ConcurrentDictionary<RefreshRate, string>();

            for (byte i1 = 1; i1 < 246; i1 += 8)
            {
                RefreshRate address = new RefreshRate(i1);
                var res = dict.TryAdd(address, address.ToString());
                Assert.That(res, Is.True);
            }
            Assert.That(refreshRateRaw.Equals(1), Is.False);
            Assert.That(refreshRateRaw.Equals((object)new RefreshRate(0)), Is.True);
            Assert.That(refreshRateRaw.Equals(new RefreshRate(0)), Is.True);
            Assert.That(refreshRateRaw.Equals(new RefreshRate((byte)1)), Is.False);
            Assert.That(new RefreshRate((uint)RefreshRate.FREQUENCY_MULTIPLYER).Equals(new RefreshRate((uint)(RefreshRate.FREQUENCY_MULTIPLYER - 1))), Is.False);
        }
    }
}