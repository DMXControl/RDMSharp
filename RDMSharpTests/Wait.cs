using NUnit.Framework.Constraints;
using System.Diagnostics;

namespace RDMSharpTests;

/// <summary>
/// Waits for a condition instead of a fixed time, so a test only waits as long as needed.
/// </summary>
internal static class Wait
{
    public const int DefaultTimeout = 5000;
    private const int PollInterval = 10;

    /// <summary>
    /// Waits until <paramref name="condition"/> is true or the timeout is reached.
    /// On timeout it simply returns, the assertions following the wait then report the actual failure.
    /// Exceptions in the condition (e.g. a value that is not there yet) count as "not yet".
    /// </summary>
    public static async Task UntilAsync(Func<bool> condition, int timeout = DefaultTimeout)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!evaluate(condition))
        {
            if (stopwatch.ElapsedMilliseconds >= timeout)
                return;
            await Task.Delay(PollInterval);
        }
    }

    /// <summary>
    /// Evaluates an NUnit constraint without asserting it, so it can be used inside <see cref="UntilAsync"/>.
    /// A failed <c>Assert.That</c> can't be used there, because NUnit records the failure even if the exception is caught.
    /// </summary>
    public static bool Holds<TActual>(TActual actual, IResolveConstraint constraint)
    {
        return constraint.Resolve().ApplyTo(actual).IsSuccess;
    }

    private static bool evaluate(Func<bool> condition)
    {
        try
        {
            return condition();
        }
        catch
        {
            return false;
        }
    }
}
