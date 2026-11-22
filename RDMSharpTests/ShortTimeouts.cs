namespace RDMSharpTests;

/// <summary>
/// A request is given up after 400 ms instead of 10 s (and sent again after 150 ms), so a test of a device that does not answer does not wait for the
/// real timeouts. The timers are set back when it is disposed.
/// </summary>
internal sealed class ShortTimeouts : IDisposable
{
    private readonly int requestTimeout = GlobalTimers.Instance.RequestTimeout;
    private readonly int requestRetryInterval = GlobalTimers.Instance.RequestRetryInterval;
    private readonly int silentParameterRetryInterval = GlobalTimers.Instance.SilentParameterRetryInterval;

    /// <param name="silentParameterRetryInterval">Time in milliseconds after which a parameter the device did not answer is asked for again.</param>
    public ShortTimeouts(int silentParameterRetryInterval = 500)
    {
        GlobalTimers.Instance.RequestTimeout = 400;
        GlobalTimers.Instance.RequestRetryInterval = 150;
        GlobalTimers.Instance.SilentParameterRetryInterval = silentParameterRetryInterval;
    }

    public void Dispose()
    {
        GlobalTimers.Instance.RequestTimeout = requestTimeout;
        GlobalTimers.Instance.RequestRetryInterval = requestRetryInterval;
        GlobalTimers.Instance.SilentParameterRetryInterval = silentParameterRetryInterval;
    }
}
