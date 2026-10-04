using System.Globalization;

namespace LogPulse.Engine.Services;

/// <summary>Creates deterministic sample logs for demonstrations and tests.</summary>
public static class SampleLogGenerator
{
    private static readonly string[] Severities = { "INFO", "WARN", "ERROR", "CRITICAL" };
    private static readonly string[] Modules = { "AuthService", "PaymentGateway", "DatabaseNode", "ApiGateway" };

    /// <summary>Generates sample lines using a fixed seed and fixed UTC start time.</summary>
    public static IEnumerable<string> GenerateLines(int count, int seed = 42)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        var random = new Random(seed);
        DateTime baseTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        for (int i = 0; i < count; i++)
        {
            string timestamp = baseTime.AddSeconds(-i)
                .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
            string severity = Severities[random.Next(Severities.Length)];
            string module = Modules[random.Next(Modules.Length)];
            int responseMs = random.Next(10, 801);
            yield return $"{timestamp}|{severity}|{module}|{responseMs}|Sample log event {i}";
        }
    }

    /// <summary>Writes generated sample lines to a file, creating its parent directory when needed.</summary>
    public static void WriteFile(string path, int count, int seed = 42)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string fullPath = Path.GetFullPath(path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllLines(fullPath, GenerateLines(count, seed));
    }
}
