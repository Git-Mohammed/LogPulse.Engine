using LogPulse.Engine.Services;

namespace LogPulse.Engine.Labs;

/// <summary>Contains lightweight dependency-free sanity checks for core educational examples.</summary>
public static class SelfTestLab
{
    /// <summary>Runs core assertions and prints the result of each check.</summary>
    public static void Run()
    {
        Console.WriteLine("=== LogPulse basic self-tests ===");
        int passed = 0;
        int failed = 0;

        Check("Valid line parses", () =>
            LogParser.TryParse("2025-01-01T12:00:00Z|ERROR|Api|25|Request failed", out var entry)
            && entry is not null && entry.Severity == "ERROR" && entry.ResponseTimeMs == 25,
            ref passed, ref failed);

        Check("Malformed timestamp is rejected", () =>
            !LogParser.TryParse("not-a-date|ERROR|Api|25|Bad timestamp", out _),
            ref passed, ref failed);

        Check("Negative response time is rejected", () =>
            !LogParser.TryParse("2025-01-01T12:00:00Z|ERROR|Api|-1|Bad time", out _),
            ref passed, ref failed);

        Check("Analyzer separates valid and invalid lines", () =>
        {
            string[] lines =
            {
                "2025-01-01T12:00:00Z|ERROR|Api|20|Failure",
                "2025-01-01T12:00:01Z|INFO|Auth|10|Signed in",
                "invalid line"
            };
            var report = LogAnalyzer.Analyze(lines);
            return report.TotalLinesRead == 3
                && report.TotalLogsProcessed == 2
                && report.InvalidLinesSkipped == 1
                && report.ErrorOrCriticalCount == 1
                && Math.Abs(report.AverageResponseTimeMs - 15.0) < 0.0001;
        }, ref passed, ref failed);

        Check("ALU model detects signed overflow", () =>
        {
            var overflow = BitwiseArithmeticLab.AddWithFlags(int.MaxValue, 1);
            var zeroWithCarry = BitwiseArithmeticLab.AddWithFlags(-1, 1);
            return overflow.Result == int.MinValue && overflow.Overflow && !overflow.Zero && overflow.Negative
                && zeroWithCarry.Result == 0 && zeroWithCarry.Zero && zeroWithCarry.Carry && !zeroWithCarry.Overflow;
        }, ref passed, ref failed);

        Check("Booth multiplies negative values correctly", () =>
            BoothMultiplierLab.Multiply(-7, 5, 8, printTrace: false) == -35
            && BoothMultiplierLab.Multiply(-128, -128, 8, printTrace: false) == 16_384,
            ref passed, ref failed);

        Console.WriteLine($"\nPassed: {passed}; Failed: {failed}");
        if (failed == 0)
            Console.WriteLine("All included checks passed.");
    }

    private static void Check(string name, Func<bool> test, ref int passed, ref int failed)
    {
        try
        {
            bool success = test();
            Console.WriteLine($"[{(success ? "PASS" : "FAIL")}] {name}");
            if (success) passed++; else failed++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FAIL] {name}: {ex.Message}");
            failed++;
        }
    }
}
