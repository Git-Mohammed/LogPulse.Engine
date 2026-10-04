namespace LogPulse.Engine.Labs;

/// <summary>Illustrates ideal five-stage pipelining and simple register-dependency detection.</summary>
public static class PipelineLab
{
    private static readonly string[] Stages = { "IF", "ID", "EX", "MEM", "WB" };

    /// <summary>Displays ideal pipelined versus non-pipelined cycle counts and a timing chart.</summary>
    public static void Run()
    {
        Console.WriteLine("\n=== Five-Stage Pipeline Model ===");
        int instructionCount = ReadPositiveInt("Instruction count [8]: ", 8);
        const int stageCount = 5;

        long nonPipelinedCycles = (long)instructionCount * stageCount;
        long idealPipelinedCycles = instructionCount == 0 ? 0 : instructionCount + stageCount - 1;
        double speedup = idealPipelinedCycles == 0 ? 0 : (double)nonPipelinedCycles / idealPipelinedCycles;

        Console.WriteLine($"Stages                  : {stageCount}");
        Console.WriteLine($"Instructions            : {instructionCount}");
        Console.WriteLine($"Non-pipelined cycles    : {nonPipelinedCycles}");
        Console.WriteLine($"Ideal pipelined cycles  : {idealPipelinedCycles}");
        Console.WriteLine($"Idealized speedup       : {speedup:F2}x");
        Console.WriteLine("\nTiming chart (ideal; no stalls, hazards, or branch penalties):");
        PrintTimingChart(instructionCount, stageCount);
        Console.WriteLine("This is a mathematical teaching model, not a trace of the host CPU pipeline.");
    }

    /// <summary>Reports simple read-after-write dependencies in an abstract instruction list.</summary>
    public static void RunRawDependencyDemo()
    {
        Console.WriteLine("\n=== RAW Dependency Demo ===");
        var instructions = new[]
        {
            new InstructionInfo("I1: R1 = R2 + R3", "R1", new[] { "R2", "R3" }),
            new InstructionInfo("I2: R4 = R1 + R5", "R4", new[] { "R1", "R5" }),
            new InstructionInfo("I3: R6 = R4 + R1", "R6", new[] { "R4", "R1" }),
            new InstructionInfo("I4: R7 = R8 + R9", "R7", new[] { "R8", "R9" })
        };

        for (int current = 0; current < instructions.Length; current++)
        {
            for (int previous = 0; previous < current; previous++)
            {
                string? written = instructions[previous].Destination;
                if (written is not null && instructions[current].Sources.Contains(written, StringComparer.Ordinal))
                {
                    Console.WriteLine($"RAW dependency: {instructions[previous].Text} -> {instructions[current].Text} (register {written})");
                }
            }
        }

        Console.WriteLine("This checks register-name dependencies in a toy instruction list; it does not model forwarding or exact stall counts.");
    }

    private static void PrintTimingChart(int instructionCount, int stageCount)
    {
        if (instructionCount > 12)
        {
            Console.WriteLine("Chart omitted for readability when more than 12 instructions are requested.");
            return;
        }

        int cycles = instructionCount == 0 ? 0 : instructionCount + stageCount - 1;
        Console.Write("Instr. ");
        for (int cycle = 1; cycle <= cycles; cycle++)
            Console.Write($"| C{cycle,-4}");
        Console.WriteLine();

        for (int instruction = 0; instruction < instructionCount; instruction++)
        {
            Console.Write($"I{instruction + 1,-6}");
            for (int cycle = 0; cycle < cycles; cycle++)
            {
                int stageIndex = cycle - instruction;
                string cell = stageIndex >= 0 && stageIndex < stageCount ? Stages[stageIndex] : "";
                Console.Write($"| {cell,-5}");
            }
            Console.WriteLine();
        }
    }

    private static int ReadPositiveInt(string prompt, int defaultValue)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return defaultValue;
        return int.TryParse(input, out int value) && value > 0 ? value : defaultValue;
    }

    private sealed record InstructionInfo(string Text, string? Destination, string[] Sources);
}
