namespace LogPulse.Engine.Labs;

/// <summary>Educational, fixed-width Booth multiplication for signed two's-complement integers.</summary>
public static class BoothMultiplierLab
{
    /// <summary>
    /// Multiplies two signed integers using Booth's algorithm with an (n+1)-bit A register,
    /// an n-bit Q register, and a Q-minus-one bit. Supports widths from 2 through 16 bits.
    /// </summary>
    public static long Multiply(int multiplicand, int multiplier, int bits, bool printTrace = true)
    {
        if (bits is < 2 or > 16)
            throw new ArgumentOutOfRangeException(nameof(bits), "Use a bit width from 2 through 16.");

        int min = -(1 << (bits - 1));
        int max = (1 << (bits - 1)) - 1;
        if (multiplicand < min || multiplicand > max)
            throw new ArgumentOutOfRangeException(nameof(multiplicand), $"Value must fit in signed {bits}-bit range [{min}, {max}].");
        if (multiplier < min || multiplier > max)
            throw new ArgumentOutOfRangeException(nameof(multiplier), $"Value must fit in signed {bits}-bit range [{min}, {max}].");

        long mask = (1L << bits) - 1;
        long maskA = (1L << (bits + 1)) - 1;
        long a = 0;
        // Booth requires an A register one bit wider than the operands.
        // Sign-extend M into that wider register to handle the most-negative value.
        long m = SignExtend(multiplicand, bits) & maskA;
        long q = multiplier & mask;
        int qMinus1 = 0;

        if (printTrace)
        {
            Console.WriteLine("Step | A (signed) | Q (signed) | Q-1 | Operation");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Init | {SignExtend(a, bits + 1),10} | {SignExtend(q, bits),9} | {qMinus1,3} | Initialize A=0");
        }

        for (int step = 1; step <= bits; step++)
        {
            int q0 = (int)(q & 1L);
            string operation;

            if (q0 == 0 && qMinus1 == 1)
            {
                a = (a + m) & maskA;
                operation = "A = A + M";
            }
            else if (q0 == 1 && qMinus1 == 0)
            {
                a = (a - m) & maskA;
                operation = "A = A - M";
            }
            else
            {
                operation = "No add/subtract";
            }

            long oldALsb = a & 1L;
            long signedA = SignExtend(a, bits + 1);
            a = (signedA >> 1) & maskA; // Arithmetic right shift preserves A's sign bit.
            q = ((oldALsb << (bits - 1)) | (q >> 1)) & mask;
            qMinus1 = q0;

            if (printTrace)
                Console.WriteLine($"{step,4} | {SignExtend(a, bits + 1),10} | {SignExtend(q, bits),9} | {qMinus1,3} | {operation}; arithmetic shift");
        }

        long combined = ((a & mask) << bits) | q; // Discard the extra A sign bit in the final 2n-bit product.
        long result = SignExtend(combined, bits * 2);

        if (printTrace)
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Booth result : {result}");
            Console.WriteLine($"C# reference : {(long)multiplicand * multiplier}");
            Console.WriteLine($"Results match: {result == (long)multiplicand * multiplier}");
        }

        return result;
    }

    /// <summary>Runs an interactive Booth multiplication demonstration.</summary>
    public static void RunInteractive()
    {
        Console.WriteLine("\n=== Booth Signed Multiplication ===");
        int bits = ReadInt("Bit width (2..16) [8]: ", 8);
        int left = ReadInt("Signed multiplicand M: ", 0);
        int right = ReadInt("Signed multiplier Q: ", 0);
        try
        {
            Multiply(left, right, bits);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static long SignExtend(long value, int bits)
    {
        long signBit = 1L << (bits - 1);
        long mask = (1L << bits) - 1;
        value &= mask;
        return (value ^ signBit) - signBit;
    }

    private static int ReadInt(string prompt, int defaultValue)
    {
        Console.Write(prompt);
        string? value = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        return int.TryParse(value, out int parsed) ? parsed : defaultValue;
    }
}
