using LogPulse.Engine.Models;

namespace LogPulse.Engine.Services;

/// <summary>
/// Reads and analyzes log lines incrementally, without storing every parsed entry.
/// </summary>
public static class LogAnalyzer
{
    /// <summary>
    /// Analyzes a sequence of raw lines in a single pass.
    /// Memory usage for parsed entries remains bounded because each line is discarded
    /// after its statistics have been updated.
    /// </summary>
    /// <param name="lines">Raw log lines from a file, stream, or test collection.</param>
    /// <returns>Aggregate report for all valid and invalid lines encountered.</returns>
    public static LogSummaryReport Analyze(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        long totalLines = 0;
        long processed = 0;
        long invalid = 0;
        long errorCount = 0;
        long totalResponseTime = 0;
        int maxResponseTime = 0;

        foreach (string line in lines)
        {
            totalLines++;

            if (!LogParser.TryParse(line, out LogEntry? entry) || entry is null)
            {
                invalid++;
                continue;
            }

            processed++;
            totalResponseTime = checked(totalResponseTime + entry.ResponseTimeMs);

            if (entry.ResponseTimeMs > maxResponseTime)
                maxResponseTime = entry.ResponseTimeMs;

            if (entry.IsErrorOrCritical)
                errorCount++;
        }

        double errorPercentage = processed == 0
            ? 0.0
            : (double)errorCount / processed * 100.0;

        double averageResponseTime = processed == 0
            ? 0.0
            : (double)totalResponseTime / processed;

        return new LogSummaryReport
        {
            TotalLinesRead = totalLines,
            TotalLogsProcessed = processed,
            InvalidLinesSkipped = invalid,
            ErrorOrCriticalCount = errorCount,
            ErrorPercentage = errorPercentage,
            AverageResponseTimeMs = averageResponseTime,
            MaxResponseTimeMs = maxResponseTime
        };
    }

    /// <summary>
    /// Analyzes a file line by line using File.ReadLines, avoiding loading the full file into memory.
    /// </summary>
    /// <param name="path">Path to the UTF-8 or text log file.</param>
    /// <returns>Aggregate report for the file.</returns>
    public static LogSummaryReport AnalyzeFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Analyze(File.ReadLines(path));
    }
}
