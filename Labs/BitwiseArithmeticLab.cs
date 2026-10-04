namespace LogPulse.Engine.Labs;

/// <summary>Educational experiments for binary representation, bitwise operations, and ALU-style flags.</summary>
public static class BitwiseArithmeticLab
{
    /// <summary>Result of a 32-bit two's-complement addition simulated one bit at a time.</summary>
    public readonly record struct AdditionResult(int Result, bool Zero, bool Negative, bool Carry, bool Overflow);

    /// <summary>
    /// Adds two 32-bit bit patterns with a ripple-carry model and calculates Z, N, C, and V flags.
    /// This models the logic for learning; it does not control the host CPU's physical ALU.
    /// </summary>
    public static AdditionResult AddWithFlags(int left, int right)
    {
        uint a = unchecked((uint)left);
        uint b = unchecked((uint)right);
        uint sum = 0;
        uint carry = 0;
        bool carryIntoSignBit = false;
        bool carryOutOfSignBit = false;

        for (int bit = 0; bit < 32; bit++)
        {
            uint aBit = (a >> bit) & 1u;
            uint bBit = (b >> bit) & 1u;

            if (bit == 31)
                carryIntoSignBit = carry != 0;

            uint sumBit = aBit ^ bBit ^ carry;
            sum |= sumBit << bit;

            uint nextCarry = (aBit & bBit) | (carry & (aBit ^ bBit));

            if (bit == 31)
                carryOutOfSignBit = nextCarry != 0;

            carry = nextCarry;
        }

        int signedResult = unchecked((int)sum);
        bool overflow = carryIntoSignBit ^ carryOutOfSignBit;

        return new AdditionResult(
            signedResult,
            sum == 0,
            (sum & 0x80000000u) != 0,
            carry != 0,
            overflow);
    }

    /// <summary>Runs an interactive signed-addition experiment and displays the binary representation and ALU-style flags.</summary>
    public static void RunInteractive()
    {
        Console.WriteLine("\n=== 32-bit Binary Addition and Flags ===");
        int left = ReadInt("Enter signed 32-bit integer A: ");
        int right = ReadInt("Enter signed 32-bit integer B: ");

        AdditionResult result = AddWithFlags(left, right);
        Console.WriteLine($"A (decimal): {left}");
        Console.WriteLine($"A (binary) : {ToBinary(left)}");
        Console.WriteLine($"B (decimal): {right}");
        Console.WriteLine($"B (binary) : {ToBinary(right)}");
        Console.WriteLine($"Result     : {result.Result}");
        Console.WriteLine($"Result bits: {ToBinary(result.Result)}");
        Console.WriteLine($"Z (Zero)   : {Flag(result.Zero)}");
        Console.WriteLine($"N (Negative): {Flag(result.Negative)}");
        Console.WriteLine($"C (Carry)  : {Flag(result.Carry)}");
        Console.WriteLine($"V (Overflow): {Flag(result.Overflow)}");
        Console.WriteLine("Note: Carry and signed overflow are different conditions.");
    }

    private static string ToBinary(int value) =>
        Convert.ToString(unchecked((uint)value), 2).PadLeft(32, '0');

    private static string Flag(bool value) => value ? "1" : "0";

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
                return value;
            Console.WriteLine("Please enter a valid 32-bit signed integer.");
        }
    }
}
