using System.Globalization;
using LogPulse.Engine.Models;

namespace LogPulse.Engine.Services;

/// <summary>
/// Converts raw pipe-delimited log lines into validated LogEntry objects.
/// </summary>
public static class LogParser
{
    private const string TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    /// <summary>
    /// Tries to parse one line without throwing for expected format errors.
    /// The message field may contain additional pipe characters.
    /// </summary>
    /// <param name="line">Raw line formatted as Timestamp|Severity|Module|ResponseTimeMs|Message.</param>
    /// <param name="entry">Parsed entry when successful; otherwise null.</param>
    /// <returns>True if the line is valid; otherwise false.</returns>
    public static bool TryParse(string? line, out LogEntry? entry)
    {
        entry = null;

        if (string.IsNullOrWhiteSpace(line))
            return false;

        string[] parts = line.Split(new[] { '|' }, 5);
        if (parts.Length != 5)
            return false;

        if (!DateTime.TryParseExact(
                parts[0],
                TimestampFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTime timestampUtc))
        {
            return false;
        }

        string severity = parts[1].Trim();
        string source = parts[2].Trim();

        if (severity.Length == 0 || source.Length == 0)
            return false;

        if (!int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int responseTimeMs))
            return false;

        if (responseTimeMs < 0)
            return false;

        entry = new LogEntry
        {
            TimestampUtc = timestampUtc,
            Severity = severity,
            SourceModule = source,
            ResponseTimeMs = responseTimeMs,
            Message = parts[4]
        };

        return true;
    }
}
