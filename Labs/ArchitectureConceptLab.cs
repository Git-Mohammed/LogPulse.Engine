namespace LogPulse.Engine.Labs;

/// <summary>Provides concise reference notes for architecture topics that are not directly implemented by the log analyzer.</summary>
public static class ArchitectureConceptLab
{
    /// <summary>Prints conceptual explanations and practical boundaries for selected course topics.</summary>
    public static void Run()
    {
        Console.WriteLine("\n=== Architecture Concept Reference ===\n");

        Print("System structure and four functions",
            "A computer coordinates processing, storage, data movement, and control. LogPulse has analogous software responsibilities, but software components are not physical CPU units.");
        Print("Von Neumann vs Harvard",
            "The classical distinction concerns instruction and data memory paths. A .NET application benchmark cannot, by itself, prove a CPU follows one classroom model or another.");
        Print("Flynn taxonomy",
            "SISD: single instruction/single data; SIMD: one instruction applied to multiple data lanes; MISD: multiple instructions/single data (uncommon); MIMD: multiple instruction streams on multiple data. Vector<T> and parallel workers illustrate related programming ideas, not complete architectural implementations.");
        Print("RISC vs CISC; ISA vs microarchitecture",
            "An ISA defines the software-visible machine instructions; microarchitecture is how a processor implements them. C# compiles to IL and is commonly JIT-compiled to native instructions for the target platform. One disassembly does not establish a general RISC/CISC performance comparison.");
        Print("CPU registers and control unit",
            "PC, IR, MAR, MBR, accumulator and control signals are hardware concepts. The toy CPU lab models their roles as software state for learning; it does not access physical registers.");
        Print("Instruction formats and addressing",
            "Zero-, one-, two-, and three-address instruction formats differ in where operands/results are implied or encoded. Immediate/direct/indirect modes describe how an operand or its address is obtained; see the Addressing Modes lab for a small memory-array example.");
        Print("Caches and locality",
            "Temporal locality means recently used data may be used again; spatial locality means nearby addresses may be accessed close together. The cache lab simulates a direct-mapped cache, while timing experiments only measure elapsed time unless hardware counters are collected separately.");
        Print("MESI and false sharing",
            "MESI models cache-line coherence states: Modified, Exclusive, Shared, Invalid. False sharing occurs when independent variables occupy the same cache line and are written by different cores. Correct experiments require careful layout and measurement; a 64-byte struct size alone does not prove alignment or improvement.");
        Print("Virtual memory and EAT",
            "Virtual memory maps virtual addresses to physical pages. The FIFO and EAT lab is a simplified educational model; its page-fault probabilities and service times are inputs, not measurements of the operating system.");
        Print("I/O, interrupts, polling, DMA, and zero-copy",
            "Polling checks for readiness repeatedly; interrupts notify the CPU through hardware/OS mechanisms; DMA allows capable devices to transfer data with limited CPU copying. Async I/O and memory-mapped files are software APIs and must not be casually equated with hardware DMA or guaranteed zero-copy.");
        Print("Pipeline, hazards, and superscalar execution",
            "Pipelining overlaps instruction stages. RAW/WAR/WAW describe dependency types. Superscalar processors may issue multiple eligible instructions per cycle. The pipeline lab is an idealized model and does not expose the host CPU's actual pipeline schedule.");
        Print("Booth multiplication and arithmetic flags",
            "Booth's algorithm is a fixed-width signed multiplication method; the Booth lab prints its register trace. The binary adder lab calculates carry and signed overflow as distinct flags.");

        Console.WriteLine("\nUse Docs/Coverage-Matrix.md to see which topics have a working lab and which remain theory/roadmap items.");
    }

    private static void Print(string heading, string body)
    {
        Console.WriteLine($"{heading}\n  {body}\n");
    }
}
