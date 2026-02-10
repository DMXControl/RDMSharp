using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMRealTimeClockTest
{
    [Test]
    public void CompareTests()
    {
        RDMRealTimeClock rtc1 = new RDMRealTimeClock(year: 2023, month: 5, day: 20, hour: 15, minute: 30, second: 45);
        RDMRealTimeClock rtc2 = new RDMRealTimeClock(year: 2023, month: 4, day: 20, hour: 15, minute: 30, second: 45);
        RDMRealTimeClock rtc3 = new RDMRealTimeClock(year: 2024, month: 6, day: 21, hour: 16, minute: 31, second: 46);
        Assert.That(rtc1.CompareTo(rtc2), Is.EqualTo(1));
        Assert.That(rtc1.CompareTo(rtc3), Is.EqualTo(-1));
        Assert.That(rtc1.CompareTo(rtc1), Is.EqualTo(0));
    }

    [Test]
    public void ExceptionTests()
    {
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(year: 2000); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(month: 13); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(month: 0); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(day: 32); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(day: 0); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(hour: 25); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(minute: 60); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(second: 60); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(year: 2000, month: 2, day: 30); });
        Assert.Throws(typeof(ArgumentException), () => { new RDMRealTimeClock(year: 2023, month: 5, day: 20, hour: 15, minute: 30, second: 45).CompareTo(DateTime.Now); });
    }
}