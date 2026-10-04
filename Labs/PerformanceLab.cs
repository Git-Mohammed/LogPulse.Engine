using System.Diagnostics;
using System.Numerics;

namespace LogPulse.Engine.Labs;

/// <summary>Educational performance comparisons for scalar, SIMD, and parallel aggregation.</summary>
public static class PerformanceLab
{
    /// <summary>Compares scalar summation with Vector&lt;float&gt; summation and checks the results.</summary>
    public static void RunScalarVsSimd()
    {
        Console.WriteLine("\n=== Scalar vs Vector<T> (SIMD-capable) Sum ===");
        const int length = 2_000_000;
        float[] values = new float[length];
        for (int i = 0; i < values.Length; i++)
            values[i] = (i % 101) * 0.25f;

        // Warm up both implementations before timing.
        _ = ScalarSum(values);
        _ = VectorSum(values);

        var timer = Stopwatch.StartNew();
        float scalar = ScalarSum(values);
        timer.Stop();
        TimeSpan scalarTime = timer.Elapsed;

        timer.Restart();
        float vector = VectorSum(values);
        timer.Stop();
        TimeSpan vectorTime = timer.Elapsed;

        Console.WriteLine($"Vector.IsHardwareAccelerated: {Vector.IsHardwareAccelerated}");
        Console.WriteLine($"Vector<float>.Count         : {Vector<float>.Count}");
        Console.WriteLine($"Scalar result               : {scalar:F3}");
        Console.WriteLine($"Vector result               : {vector:F3}");
        Console.WriteLine($"Absolute difference         : {Math.Abs(scalar - vector):F6}");
        Console.WriteLine($"Scalar time                 : {scalarTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Vector time                 : {vectorTime.TotalMilliseconds:F2} ms");
        Console.WriteLine("Floating-point addition order can change rounding, so small result differences may be expected.");
        Console.WriteLine("A single run is not a reliable benchmark; use multiple runs and a benchmark harness for serious conclusions.");
    }

    /// <summary>Compares sequential and parallel summation and verifies the calculated totals.</summary>
    public static void RunSequentialVsParallel()
    {
        Console.WriteLine("\n=== Sequential vs Parallel Aggregation ===");
        const int length = 2_000_000;
        int[] values = new int[length];
        for (int i = 0; i < values.Length; i++)
            values[i] = i % 13;

        _ = SequentialSum(values); // Warm-up.

        var timer = Stopwatch.StartNew();
        long sequential = SequentialSum(values);
        timer.Stop();
        TimeSpan sequentialTime = timer.Elapsed;

        long parallel = 0;
        timer.Restart();
        Parallel.For(
            0,
            values.Length,
            () => 0L,
            (index, _, localSum) => localSum + values[index],
            localSum => Interlocked.Add(ref parallel, localSum));
        timer.Stop();
        TimeSpan parallelTime = timer.Elapsed;

        Console.WriteLine($"Items                 : {length:N0}");
        Console.WriteLine($"Sequential total      : {sequential:N0}");
        Console.WriteLine($"Parallel total        : {parallel:N0}");
        Console.WriteLine($"Results match         : {sequential == parallel}");
        Console.WriteLine($"Sequential time       : {sequentialTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Parallel time         : {parallelTime.TotalMilliseconds:F2} ms");
        Console.WriteLine("Parallel work has scheduling and coordination overhead; it can be slower for small workloads.");
    }

    private static float ScalarSum(float[] values)
    {
        float sum = 0;
        for (int i = 0; i < values.Length; i++)
            sum += values[i];
        return sum;
    }

    private static float VectorSum(float[] values)
    {
        int width = Vector<float>.Count;
        int i = 0;
        Vector<float> vectorSum = Vector<float>.Zero;

        for (; i <= values.Length - width; i += width)
            vectorSum += new Vector<float>(values, i);

        float sum = 0;
        for (int lane = 0; lane < width; lane++)
            sum += vectorSum[lane];

        for (; i < values.Length; i++)
            sum += values[i];

        return sum;
    }

    private static long SequentialSum(int[] values)
    {
        long sum = 0;
        for (int i = 0; i < values.Length; i++)
            sum += values[i];
        return sum;
    }
}
