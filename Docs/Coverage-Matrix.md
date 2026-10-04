# Computer Architecture Coverage Matrix

This project is a learning baseline, not a claim that every course topic has been fully implemented. Use this matrix to track progress against the lecture sources. Update the status only after the relevant source material has been checked and the learning activity tested.

**Status key:** `Implemented demo` = runnable simplified model; `Applied` = used in the actual log-processing workflow; `Conceptual` = explained with limitations; `Next lab` = reserved for a future focused implementation.

| Topic from the course map | Current treatment | Project location | Important boundary / next step |
|---|---|---|---|
| Structure, function, processing/storage/movement/control | Applied at software-workflow level | `Services/LogAnalyzer.cs`, `Program.cs` | These responsibilities are analogous concepts, not physical CPU units. |
| CPU, CU, ALU, registers, internal bus | Conceptual plus a small CPU model | `InstructionCycleLab.cs`, `BitwiseArithmeticLab.cs` | No access to physical CPU registers or buses. |
| PC, IR, MAR, MBR, accumulator | Implemented demo (partial register set) | `InstructionCycleLab.cs` | I/O AR and I/O BR are not modeled yet. |
| Von Neumann vs Harvard and shared-bus bottleneck | Conceptual | `ArchitectureConceptLab.cs` | No hardware architecture is inferred from an application timing test. |
| Flynn taxonomy: SISD, SIMD, MISD, MIMD | Conceptual; SIMD-capable vector example; task parallelism example | `ArchitectureConceptLab.cs`, `PerformanceLab.cs` | `Vector<T>` does not by itself implement a complete SIMD processor. Parallel tasks do not prove a specific hardware topology. |
| RISC vs CISC and Load/Store | Conceptual | `ArchitectureConceptLab.cs` | Native disassembly can be a later experiment on a known runtime/CPU combination. |
| ISA vs microarchitecture; MIPS R/I/J formats | Conceptual only | `ArchitectureConceptLab.cs` | A future MIPS encoding/decoder lab should use the exact instruction format described in the source. |
| Instruction cycle and micro-operations / RTL | Simplified fetch/decode/execute model | `InstructionCycleLab.cs` | Not a cycle-accurate implementation; RTL signal tables remain a future lab. |
| Hardwired vs microprogrammed control and control signals | Conceptual only | `ArchitectureConceptLab.cs` | The source's exact signal equations (such as C1..C13/C5) must be transcribed and checked before modeling. |
| Zero-, one-, two-, and three-address instruction formats | Next lab | Coverage plan | Add examples from the source and compare instruction sequences; do not infer formats from C# variables. |
| Immediate, direct, indirect addressing | Implemented demo | `AddressingModesLab.cs` | Uses a C# array as illustrative memory, not CPU address circuitry. |
| Binary arithmetic, carry, signed overflow | Implemented demo | `BitwiseArithmeticLab.cs` | Uses a bit-by-bit teaching adder. Its flags are a model; they do not reveal flags from the host CPU instruction. |
| Booth signed multiplication and arithmetic shift | Implemented demo with register trace | `BoothMultiplierLab.cs` | Supports signed widths from 2 to 16 bits to keep the teaching model bounded. |
| Memory hierarchy and temporal/spatial locality | Timing experiment | `MemoryAndCacheLab.cs` | Elapsed time is measured; cache misses are not directly counted. Repeat runs and control the environment. |
| Direct-mapped cache: index/tag/offset | Implemented simplified simulation | `MemoryAndCacheLab.cs` | Educational cache configuration, not a query of the host CPU's real cache. |
| MESI coherence and false sharing / padding | Conceptual; future controlled experiment | `ArchitectureConceptLab.cs`, `Docs/Roadmap.md` | Struct size alone does not guarantee base alignment or prove improved performance. |
| Virtual memory, page tables, EAT, page faults, thrashing | FIFO page-fault and EAT model | `VirtualMemoryLab.cs` | Simulated page references and supplied probabilities are not operating-system measurements. Verify variable definitions against the course formula. |
| Programmed I/O, polling, interrupts, DMA modes | Conceptual | `ArchitectureConceptLab.cs` | Async I/O is not equivalent to a hardware DMA implementation. |
| Zero-copy and data transfer | Conceptual boundary note | `ArchitectureConceptLab.cs` | Memory mapping does not guarantee zero-copy end-to-end. Add a measured file-I/O comparison later. |
| Five-stage pipeline and ideal speedup formula | Implemented idealized calculator and chart | `PipelineLab.cs` | Assumes no stalls, hazards, cache misses, or branch penalties. |
| RAW / WAR / WAW hazards and loop unrolling | RAW dependency demonstration only | `PipelineLab.cs` | WAR/WAW and cycle-accurate stall/forwarding modeling remain future labs. Loop unrolling should be measured, not assumed faster. |
| Branch prediction and branch penalty | Conceptual; future benchmark | `ArchitectureConceptLab.cs`, `Docs/Roadmap.md` | Sorting vs random data only measures elapsed time; it does not directly count mispredictions. |
| Superscalar issue and split cache (e.g. historical Pentium design) | Conceptual | `ArchitectureConceptLab.cs` | Requires a carefully scoped historical model or architecture-specific analysis to simulate faithfully. |
| Log ingestion, parsing, filtering, aggregation | Applied | `Services/LogParser.cs`, `Services/LogAnalyzer.cs` | Uses streaming `File.ReadLines` and a single-pass accumulator. |
| Scalar vs SIMD, sequential vs parallel aggregation | Implemented comparison experiments | `PerformanceLab.cs` | Results depend on hardware, JIT, runtime, array size, warm-up, and scheduling; no speedup is guaranteed. |

## Coverage discipline

1. Read the relevant lecture section before marking a topic complete.
2. Preserve any exact definitions, equations, signal tables, diagrams, and examples from the supplied course sources in study notes.
3. Keep physical hardware concepts separate from software analogies and simulations.
4. Record measurements with the test machine, OS, .NET version, input size, and repeated runs when doing performance work.
5. Never label the course 100% covered based only on this starter repository. Complete the matrix against the actual lecture sources.
