using Microsoft.Extensions.Logging;
using System;

namespace RDMSharp
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Vorlage muss ein statischer Ausdruck sein", Justification = "<Ausstehend>")]
    public static class LoggingTools
    {
        public static void LogTrace(this ILogger logger, Exception exception) => logger?.LogTrace(exception, message: string.Empty);
        public static void LogDebug(this ILogger logger, Exception exception) => logger?.LogDebug(exception, message: string.Empty);
        public static void LogInformation(this ILogger logger, Exception exception) => logger?.LogInformation(exception, message: string.Empty);
        public static void LogWarning(this ILogger logger, Exception exception) => logger?.LogWarning(exception, message: string.Empty);
        public static void LogError(this ILogger logger, Exception exception) => logger?.LogError(exception, message: string.Empty);
        public static void LogCritical(this ILogger logger, Exception exception) => logger?.LogCritical(exception, message: string.Empty);
    }
}
