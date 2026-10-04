namespace LogPulse.Engine.Models;

/// <summary>
/// Contains aggregate statistics calculated from a stream of log lines.
/// </summary>
public sealed class LogSummaryReport
{
    /// <summary>Total number of raw lines read, including invalid lines.</summary>
    public long TotalLinesRead { get; init; }

    /// <summary>Number of lines successfully parsed and analyzed.</summary>
    public long TotalLogsProcessed { get; init; }

    /// <summary>Number of lines rejected by the parser.</summary>
    public long InvalidLinesSkipped { get; init; }

    /// <summary>Number of valid entries with ERROR or CRITICAL severity.</summary>
    public long ErrorOrCriticalCount { get; init; }

    /// <summary>Percentage of valid entries that are ERROR or CRITICAL.</summary>
    public double ErrorPercentage { get; init; }

    /// <summary>Average response time across all valid entries, in milliseconds.</summary>
    public double AverageResponseTimeMs { get; init; }

    /// <summary>Maximum response time observed, in milliseconds.</summary>
    public int MaxResponseTimeMs { get; init; }
}
