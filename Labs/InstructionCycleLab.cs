namespace LogPulse.Engine.Labs;

/// <summary>A tiny accumulator-based CPU model used to visualize fetch/decode/execute steps.</summary>
public static class InstructionCycleLab
{
    private enum OpCode { Load, Add, Store, JumpIfZero, Halt }
    private sealed record Instruction(OpCode Op, int Operand = 0)
    {
        public override string ToString() => Op switch
        {
            OpCode.Load => $"LOAD {Operand}",
            OpCode.Add => $"ADD {Operand}",
            OpCode.Store => $"STORE {Operand}",
            OpCode.JumpIfZero => $"JZ {Operand}",
            OpCode.Halt => "HALT",
            _ => Op.ToString()
        };
    }

    /// <summary>Runs a fixed toy program and prints the register values after every instruction.</summary>
    public static void Run()
    {
        Console.WriteLine("\n=== Toy CPU: Fetch / Decode / Execute ===");
        int[] memory = new int[16];
        memory[0] = 10;
        memory[1] = 5;
        memory[3] = 7;

        Instruction[] program =
        {
            new(OpCode.Load, 0),
            new(OpCode.Add, 1),
            new(OpCode.Store, 2),
            new(OpCode.Load, 2),
            new(OpCode.Add, 3),
            new(OpCode.Store, 4),
            new(OpCode.Halt)
        };

        int pc = 0;
        int mar = 0;
        int mbr = 0;
        int accumulator = 0;
        string ir = "<empty>";
        int steps = 0;
        bool halted = false;

        Console.WriteLine("Step | PC | IR       | MAR | MBR | AC | Action");
        Console.WriteLine("------------------------------------------------");

        while (!halted && pc >= 0 && pc < program.Length && steps < 100)
        {
            int instructionAddress = pc;
            Instruction instruction = program[pc];
            ir = instruction.ToString();
            pc++;
            string action;

            switch (instruction.Op)
            {
                case OpCode.Load:
                    mar = instruction.Operand;
                    EnsureValidAddress(mar, memory.Length);
                    mbr = memory[mar];
                    accumulator = mbr;
                    action = $"AC <- M[{mar}]";
                    break;

                case OpCode.Add:
                    mar = instruction.Operand;
                    EnsureValidAddress(mar, memory.Length);
                    mbr = memory[mar];
                    accumulator = unchecked(accumulator + mbr);
                    action = $"AC <- AC + M[{mar}]";
                    break;

                case OpCode.Store:
                    mar = instruction.Operand;
                    EnsureValidAddress(mar, memory.Length);
                    mbr = accumulator;
                    memory[mar] = mbr;
                    action = $"M[{mar}] <- AC";
                    break;

                case OpCode.JumpIfZero:
                    if (accumulator == 0)
                        pc = instruction.Operand;
                    action = accumulator == 0 ? $"PC <- {pc}" : "No jump";
                    break;

                case OpCode.Halt:
                    halted = true;
                    action = "Stop execution";
                    break;

                default:
                    throw new InvalidOperationException("Unsupported toy instruction.");
            }

            Console.WriteLine($"{steps + 1,4} | {instructionAddress,2} | {ir,-8} | {mar,3} | {mbr,3} | {accumulator,2} | {action}");
            steps++;
        }

        Console.WriteLine("\nFinal memory values:");
        Console.WriteLine($"M[2] = {memory[2]} (10 + 5)");
        Console.WriteLine($"M[4] = {memory[4]} (15 + 7)");
        Console.WriteLine("This pedagogical model simplifies real hardware: memory and instructions are C# arrays, and each instruction is an object.");
    }

    private static void EnsureValidAddress(int address, int memoryLength)
    {
        if (address < 0 || address >= memoryLength)
            throw new InvalidOperationException($"Toy CPU memory address {address} is out of range.");
    }
}
