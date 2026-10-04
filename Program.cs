using System.Globalization;
using LogPulse.Engine.Labs;
using LogPulse.Engine.Services;

namespace LogPulse.Engine;

/// <summary>Console entry point for the LogPulse application and computer-architecture learning labs.</summary>
internal static class Program
{
    /// <summary>Displays the menu and dispatches the selected application task or educational lab.</summary>
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        bool exit = false;

        while (!exit)
        {
            PrintMenu();
            Console.Write("Select an option: ");
            string? input = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (input?.Trim())
                {
                    case "1": AnalyzeBundledSample(); break;
                    case "2": AnalyzeCustomFile(); break;
                    case "3": BitwiseArithmeticLab.RunInteractive(); break;
                    case "4": MemoryAndCacheLab.RunLocalityBenchmark(); break;
                    case "5": PerformanceLab.RunScalarVsSimd(); break;
                    case "6": PerformanceLab.RunSequentialVsParallel(); break;
                    case "7": MemoryAndCacheLab.RunDirectMappedCacheDemo(); break;
                    case "8": VirtualMemoryLab.Run(); break;
                    case "9": InstructionCycleLab.Run(); break;
                    case "10": PipelineLab.Run(); break;
                    case "11": PipelineLab.RunRawDependencyDemo(); break;
                    case "12": BoothMultiplierLab.RunInteractive(); break;
                    case "13": AddressingModesLab.Run(); break;
                    case "14": ArchitectureConceptLab.Run(); break;
                    case "15": SelfTestLab.Run(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Unknown option. Choose a listed number."); break;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File I/O error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"Arithmetic overflow: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            if (!exit)
            {
                Console.WriteLine();
                Console.Write("Press Enter to return to the menu...");
                Console.ReadLine();
                Console.WriteLine();
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("========================================================");
        Console.WriteLine("        LogPulse.Engine - Learning Console");
        Console.WriteLine("========================================================");
        Console.WriteLine("REAL APPLICATION");
        Console.WriteLine("  1. Analyze bundled sample logs");
        Console.WriteLine("  2. Analyze a log file by path");
        Console.WriteLine("COMPUTER ARCHITECTURE LABS");
        Console.WriteLine("  3. Binary addition and ALU-style flags");
        Console.WriteLine("  4. Sequential vs randomized memory access");
        Console.WriteLine("  5. Scalar vs Vector<T> summation");
        Console.WriteLine("  6. Sequential vs parallel aggregation");
        Console.WriteLine("  7. Direct-mapped cache simulation");
        Console.WriteLine("  8. Virtual memory, FIFO faults, and EAT");
        Console.WriteLine("  9. Toy CPU: fetch/decode/execute trace");
        Console.WriteLine(" 10. Ideal five-stage pipeline model");
        Console.WriteLine(" 11. RAW dependency demonstration");
        Console.WriteLine(" 12. Booth signed multiplication");
        Console.WriteLine(" 13. Addressing modes demonstration");
        Console.WriteLine(" 14. Architecture concepts and modeling boundaries");
        Console.WriteLine(" 15. Run basic self-tests");
        Console.WriteLine("  0. Exit");
        Console.WriteLine("--------------------------------------------------------");
    }

    private static void AnalyzeBundledSample()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "sample-logs.txt");
        if (!File.Exists(path))
        {
            Console.WriteLine("The sample file was not found. Generating a local sample file...");
            path = Path.Combine(Environment.CurrentDirectory, "Data", "sample-logs.txt");
            SampleLogGenerator.WriteFile(path, 100);
        }

        Console.WriteLine($"Analyzing: {path}");
        var report = LogAnalyzer.AnalyzeFile(path);
        PrintReport(report);
    }

    private static void AnalyzeCustomFile()
    {
        Console.Write("Enter the full path to a pipe-delimited log file: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("No file path was entered.");
            return;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine("The specified file does not exist.");
            return;
        }

        var report = LogAnalyzer.AnalyzeFile(path);
        PrintReport(report);
    }

    private static void PrintReport(Models.LogSummaryReport report)
    {
        Console.WriteLine("------------------- LOG SUMMARY -------------------");
        Console.WriteLine($"Total lines read         : {report.TotalLinesRead}");
        Console.WriteLine($"Valid logs processed     : {report.TotalLogsProcessed}");
        Console.WriteLine($"Invalid lines skipped    : {report.InvalidLinesSkipped}");
        Console.WriteLine($"ERROR / CRITICAL count   : {report.ErrorOrCriticalCount}");
        Console.WriteLine($"Error percentage         : {report.ErrorPercentage.ToString("F2", CultureInfo.InvariantCulture)}%");
        Console.WriteLine($"Average response time    : {report.AverageResponseTimeMs.ToString("F2", CultureInfo.InvariantCulture)} ms");
        Console.WriteLine($"Maximum response time    : {report.MaxResponseTimeMs} ms");
        Console.WriteLine("---------------------------------------------------");
    }
}
