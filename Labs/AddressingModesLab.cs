namespace LogPulse.Engine.Labs;

/// <summary>Demonstrates immediate, direct, and indirect addressing using a tiny educational memory array.</summary>
public static class AddressingModesLab
{
    /// <summary>Prints examples showing how an operand's interpretation changes with the addressing mode.</summary>
    public static void Run()
    {
        Console.WriteLine("\n=== Addressing Modes: Immediate, Direct, Indirect ===");
        int[] memory = new int[16];
        memory[2] = 5;
        memory[5] = 99;

        int immediateOperand = 42;
        int directAddress = 2;
        int directValue = memory[directAddress];
        int indirectPointerAddress = 2;
        int indirectTargetAddress = memory[indirectPointerAddress];
        int indirectValue = memory[indirectTargetAddress];

        Console.WriteLine($"Immediate: operand itself is the value -> {immediateOperand}");
        Console.WriteLine($"Direct   : address 2 -> M[2] = {directValue}");
        Console.WriteLine($"Indirect : address 2 -> M[2] = {indirectTargetAddress}, then M[{indirectTargetAddress}] = {indirectValue}");
        Console.WriteLine("This is a teaching example using an integer array, not the host CPU's actual addressing circuitry.");
    }
}
