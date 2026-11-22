namespace RDMSharpTests.RDM;

/// <summary>
/// The timers of <see cref="GlobalTimers"/> run while someone is subscribed: started by the first subscriber, stopped by the last one.
/// Each test uses its own instance, the shared <see cref="GlobalTimers.Instance"/> has subscribers of other tests.
/// </summary>
public class TestGlobalTimers
{
    private static void noop(object? sender, EventArgs e) { }

    [Test]
    public void TestRealTimeClockTimerStartsWhileParameterTimerIsRunning()
    {
        var timers = new GlobalTimers();
        timers.ParameterUpdateTimerElapsed += noop;
        timers.RealTimeClockUpdateTimerElapsed += noop;
        timers.PresentUpdateTimerElapsed += noop;
        try
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(timers.IsParameterUpdateTimerRunning, Is.True);
                Assert.That(timers.IsRealTimeClockUpdateTimerRunning, Is.True, "the real time clock timer must not depend on the parameter timer");
                Assert.That(timers.IsPresentUpdateTimerRunning, Is.True, "the present timer must not depend on the parameter timer");
            }
        }
        finally
        {
            timers.ParameterUpdateTimerElapsed -= noop;
            timers.RealTimeClockUpdateTimerElapsed -= noop;
            timers.PresentUpdateTimerElapsed -= noop;
        }
    }

    [Test]
    public void TestLastSubscriberStopsTheTimers()
    {
        var timers = new GlobalTimers();
        EventHandler other = (_, _) => { };
        timers.ParameterUpdateTimerElapsed += noop;
        timers.PresentUpdateTimerElapsed += noop;
        timers.PresentUpdateTimerElapsed += other;
        timers.RealTimeClockUpdateTimerElapsed += noop;

        timers.ParameterUpdateTimerElapsed -= noop;
        timers.PresentUpdateTimerElapsed -= noop;
        timers.RealTimeClockUpdateTimerElapsed -= noop;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(timers.IsParameterUpdateTimerRunning, Is.False);
            Assert.That(timers.IsPresentUpdateTimerRunning, Is.True, "one subscriber is left");
            Assert.That(timers.IsRealTimeClockUpdateTimerRunning, Is.False);
        }

        timers.PresentUpdateTimerElapsed -= other;
        Assert.That(timers.IsPresentUpdateTimerRunning, Is.False);

        // a new subscriber starts it again
        timers.PresentUpdateTimerElapsed += noop;
        Assert.That(timers.IsPresentUpdateTimerRunning, Is.True);
        timers.PresentUpdateTimerElapsed -= noop;
    }

    [Test, MaxTime(10000)]
    public async Task TestConcurrentSubscribersShareOneTimer()
    {
        // many devices subscribe at the same moment (parallel initialization): there must be one timer, so every handler runs once per tick
        var timers = new GlobalTimers { PresentUpdateTimerInterval = 100 };
        const int subscribers = 64;
        var counts = new int[subscribers];
        var handlers = Enumerable.Range(0, subscribers)
            .Select(i => (EventHandler)((_, _) => Interlocked.Increment(ref counts[i])))
            .ToArray();
        using var start = new ManualResetEventSlim();
        var threads = handlers.Select(h => new Thread(() => { start.Wait(); timers.PresentUpdateTimerElapsed += h; })).ToList();
        threads.ForEach(t => t.Start());
        start.Set();
        threads.ForEach(t => t.Join());
        try
        {
            await Task.Delay(1000);
        }
        finally
        {
            foreach (var h in handlers)
                timers.PresentUpdateTimerElapsed -= h;
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(timers.IsPresentUpdateTimerRunning, Is.False);
            // 1 s at 100 ms: about 10 ticks, more than 15 means a handler got several timers
            Assert.That(counts.Max(), Is.LessThanOrEqualTo(15));
            Assert.That(counts.Min(), Is.GreaterThan(0));
        }
    }
}
