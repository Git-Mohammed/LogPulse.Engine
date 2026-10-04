namespace LogPulse.Engine.Models;

/// <summary>
/// Represents one validated log event parsed from an input line.
/// </summary>
public sealed class LogEntry
{
    /// <summary>Gets or sets the event timestamp, normalized to UTC.</summary>
    public DateTime TimestampUtc { get; set; }

    /// <summary>Gets or sets the event severity, such as INFO, WARN, ERROR, or CRITICAL.</summary>
    public string Severity { get; set; } = "INFO";

    /// <summary>Gets or sets the service or component that emitted the event.</summary>
    public string SourceModule { get; set; } = string.Empty;

    /// <summary>Gets or sets the event message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the measured response time in milliseconds.</summary>
    public int ResponseTimeMs { get; set; }

    /// <summary>Returns true when the severity represents an error-level event.</summary>
    public bool IsErrorOrCritical =>
        string.Equals(Severity, "ERROR", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Severity, "CRITICAL", StringComparison.OrdinalIgnoreCase);
}
