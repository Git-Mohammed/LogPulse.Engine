namespace LogPulse.Engine.Labs;

/// <summary>Small simulations for memory locality and a direct-mapped cache.</summary>
public static class MemoryAndCacheLab
{
    /// <summary>
    /// Compares sequential and randomized access over the same array elements.
    /// Timings are illustrative and do not directly report hardware cache-miss counts.
    /// </summary>
    public static void RunLocalityBenchmark()
    {
        Console.WriteLine("\n=== Memory Locality Experiment ===");
        const int size = 1_000_000;
        int[] values = new int[size];
        int[] shuffledIndexes = new int[size];

        for (int i = 0; i < size; i++)
        {
            values[i] = (i % 97) + 1;
            shuffledIndexes[i] = i;
        }

        var random = new Random(1234);
        for (int i = shuffledIndexes.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (shuffledIndexes[i], shuffledIndexes[j]) = (shuffledIndexes[j], shuffledIndexes[i]);
        }

        // Warm-up reduces (but does not eliminate) one-time runtime effects.
        _ = SumSequential(values);
        _ = SumRandomOrder(values, shuffledIndexes);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        long sequentialSum = SumSequential(values);
        stopwatch.Stop();
        TimeSpan sequentialTime = stopwatch.Elapsed;

        stopwatch.Restart();
        long randomSum = SumRandomOrder(values, shuffledIndexes);
        stopwatch.Stop();
        TimeSpan randomTime = stopwatch.Elapsed;

        Console.WriteLine($"Elements read       : {size:N0}");
        Console.WriteLine($"Sequential sum      : {sequentialSum:N0}");
        Console.WriteLine($"Random-order sum    : {randomSum:N0}");
        Console.WriteLine($"Sequential elapsed  : {sequentialTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Random-order elapsed: {randomTime.TotalMilliseconds:F2} ms");
        Console.WriteLine("Interpretation: compare repeated runs; JIT, CPU state, OS scheduling, and index-array reads affect timings.");
        Console.WriteLine("This measures elapsed time, not cache misses directly.");
    }

    /// <summary>Runs a toy direct-mapped cache simulation for a fixed address trace.</summary>
    public static void RunDirectMappedCacheDemo()
    {
        Console.WriteLine("\n=== Direct-Mapped Cache Demo ===");
        int blockSizeBytes = ReadPositiveInt("Block size in bytes (default 4): ", 4);
        int lineCount = ReadPositiveInt("Number of cache lines (default 4): ", 4);

        int[] addresses = { 0, 4, 8, 0, 16, 0, 4, 32, 8, 36, 0 };
        int[] tags = new int[lineCount];
        bool[] valid = new bool[lineCount];
        int hits = 0;
        int misses = 0;

        Console.WriteLine("Address | Block | Slot | Tag | Result");
        Console.WriteLine("--------------------------------------");

        foreach (int address in addresses)
        {
            int blockNumber = address / blockSizeBytes;
            int slot = blockNumber % lineCount;
            int tag = blockNumber / lineCount;
            bool hit = valid[slot] && tags[slot] == tag;

            if (hit)
            {
                hits++;
            }
            else
            {
                misses++;
                valid[slot] = true;
                tags[slot] = tag;
            }

            Console.WriteLine($"{address,7} | {blockNumber,5} | {slot,4} | {tag,3} | {(hit ? "HIT" : "MISS")}");
        }

        Console.WriteLine($"\nHits: {hits}; Misses: {misses}; Hit rate: {(double)hits / addresses.Length:P2}");
        Console.WriteLine("This is a simplified cache model; it does not inspect your CPU's real caches.");
    }

    private static long SumSequential(int[] values)
    {
        long sum = 0;
        for (int i = 0; i < values.Length; i++)
            sum += values[i];
        return sum;
    }

    private static long SumRandomOrder(int[] values, int[] indexes)
    {
        long sum = 0;
        for (int i = 0; i < indexes.Length; i++)
            sum += values[indexes[i]];
        return sum;
    }

    private static int ReadPositiveInt(string prompt, int defaultValue)
    {
        Console.Write(prompt);
        string? text = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(text))
            return defaultValue;
        return int.TryParse(text, out int value) && value > 0 ? value : defaultValue;
    }
}
