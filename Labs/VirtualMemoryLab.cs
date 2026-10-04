namespace LogPulse.Engine.Labs;

/// <summary>Demonstrates page-fault counting and the expected-access-time equation.</summary>
public static class VirtualMemoryLab
{
    /// <summary>Runs a FIFO page-replacement trace and calculates a simplified effective access time.</summary>
    public static void Run()
    {
        Console.WriteLine("\n=== Virtual Memory: FIFO and EAT ===");
        int[] references = { 1, 2, 3, 1, 4, 1, 2, 5, 1, 2, 3, 4 };
        int frames = 3;
        int faults = CountFifoPageFaults(references, frames);

        Console.WriteLine($"Reference string: {string.Join(", ", references)}");
        Console.WriteLine($"Frame count     : {frames}");
        Console.WriteLine($"FIFO page faults: {faults}");

        Console.WriteLine("\nEnter EAT values or press Enter to use the example values.");
        double p = ReadDouble("Page-fault probability p (0..1) [0.001]: ", 0.001);
        double memoryNs = ReadDouble("Normal memory access time in ns [100]: ", 100);
        double faultNs = ReadDouble("Page-fault service time in ns [8000000]: ", 8_000_000);

        if (!double.IsFinite(p) || !double.IsFinite(memoryNs) || !double.IsFinite(faultNs) ||
            p < 0 || p > 1 || memoryNs < 0 || faultNs < 0)
        {
            Console.WriteLine("Invalid values. Probability must be 0..1 and times must be non-negative.");
            return;
        }

        double eat = (1.0 - p) * memoryNs + p * faultNs;
        Console.WriteLine($"\nEAT = (1 - p) × memory + p × page-fault-service");
        Console.WriteLine($"EAT = {eat:N2} ns");
        Console.WriteLine("The equation is a simplified model; the meaning of each variable must match the model used in your course notes.");
        Console.WriteLine("This page-replacement exercise is a software simulation, not a measurement of the operating system's real page faults.");
    }

    /// <summary>Counts page faults in a reference sequence using FIFO replacement.</summary>
    public static int CountFifoPageFaults(IReadOnlyList<int> references, int frameCount)
    {
        ArgumentNullException.ThrowIfNull(references);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(frameCount);

        var pagesInMemory = new HashSet<int>();
        var fifoQueue = new Queue<int>();
        int faults = 0;

        foreach (int page in references)
        {
            if (pagesInMemory.Contains(page))
                continue;

            faults++;
            if (pagesInMemory.Count == frameCount)
            {
                int evicted = fifoQueue.Dequeue();
                pagesInMemory.Remove(evicted);
            }

            fifoQueue.Enqueue(page);
            pagesInMemory.Add(page);
        }

        return faults;
    }

    private static double ReadDouble(string prompt, double defaultValue)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        return double.TryParse(input, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out double value)
            ? value
            : double.NaN;
    }
}
