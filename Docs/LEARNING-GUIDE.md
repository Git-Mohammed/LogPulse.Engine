# LogPulse.Engine — Deep Learning Guide

This guide explains how to use the project as a learning environment rather than a collection of menu options. It complements the [README](../README.md), [Coverage Matrix](Coverage-Matrix.md), and [Roadmap](Roadmap.md).

## 1. Start with a real question

The application begins with a practical problem: a system produces event records, and an engineer needs a trustworthy summary. Before improving speed, the analyzer must be correct. A fast report that silently skips valid rows or includes malformed data is not useful.

For every new feature, define the input, expected result, invalid cases, and observable outcome. Keep a tiny dataset that you can solve by hand; then use larger inputs only after correctness is established.

## 2. Learn the data path

A log record starts as text. Text has no automatic guarantee of valid timestamps, recognized values, or sensible response times. Parsing turns the raw representation into typed data. Validation decides whether the data meets the program's rules. Aggregation updates counters and summary values. Reporting turns those values into information an operator can act on.

This division is not a direct copy of CPU hardware. It is a software workflow that makes processing responsibilities visible and testable.

Ask yourself:

- What should happen if a field is missing?
- What if the timestamp is invalid?
- Should a negative response time be accepted?
- Should an invalid row stop the whole report or be skipped and counted?
- What denominator should the error percentage use?
- How can I independently verify the report?

## 3. Binary arithmetic: bits have a width

Binary arithmetic is not simply arithmetic written with 0s and 1s. Hardware registers have finite width; a signed value is interpreted according to its representation, commonly two's complement. The same bit pattern can have different meanings depending on whether it is interpreted as signed or unsigned.

The adder lab traces bit-level addition and flags in a model. Distinguish:

- **Carry:** a bit carried out of the most significant position in the fixed-width addition.
- **Signed overflow:** the mathematical signed result falls outside the representable signed range.
- **C# checked overflow:** the runtime throws for supported integral operations/conversions that overflow in a checked context.

These concepts are related, but they are not interchangeable. Read the [C# overflow reference](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/checked-and-unchecked) after running the adder lab.

## 4. Memory hierarchy: locality is a pattern, not a guarantee

Temporal locality means recently accessed data may be accessed again soon. Spatial locality means nearby addresses may be accessed around the same time. Caches exploit these tendencies because a program often reuses both data and nearby memory locations.

The locality experiment compares the elapsed time of sequential and randomized access. The experiment illustrates why access pattern matters, but it does not directly count hardware cache misses. Random access also has extra index-array work; therefore, interpret the entire experiment, not just a single time number.

A useful next step is to vary:

- the array size;
- the order of the accesses;
- the number of repetitions;
- the data type and amount of work per element.

Predict the outcome before each run and write down the result. Learn the cache terminology from [CS 61C](https://notes.cs61c.org/content/caches-ii/) and deepen the architecture material with the [MIT lecture notes](https://ocw.mit.edu/courses/6-823-computer-system-architecture-fall-2005/).

## 5. SIMD: one operation over several values

SIMD (Single Instruction, Multiple Data) is a hardware execution style in which one instruction can operate on multiple data elements. In .NET, `Vector<T>` provides portable vector operations, while fixed-width vector APIs and hardware intrinsics offer other levels of control.

The scalar-vs-vector lab is designed to teach the workflow:

1. Write a clear scalar implementation.
2. Write a vectorized implementation for the same operation.
3. Verify both against an expected answer.
4. Compare numeric differences, especially for floating-point arithmetic.
5. Benchmark realistic input sizes.
6. Keep the vectorized version only if the complexity is justified by measured results.

Vectorization is most useful for suitable data-parallel work. It is not automatically beneficial for string parsing, irregular branches, or small inputs. Follow the official [.NET SIMD guide](https://learn.microsoft.com/en-us/dotnet/standard/simd).

## 6. Parallelism: more workers create coordination costs

Parallel aggregation shows that independent pieces of work can run on multiple workers. But the work must be large enough to pay for partitioning, scheduling, synchronization, and combining partial results. Shared mutable state may also introduce race conditions.

To learn correctly, compare the same input, validate the totals, repeat measurements, and test multiple sizes. A parallel version being slower is not a failed experiment; it is evidence that the workload or implementation did not benefit under those conditions.

References: [Task Parallel Library](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) and [parallelism pitfalls](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/potential-pitfalls-in-data-and-task-parallelism).

## 7. Cache models: calculate before you simulate

The direct-mapped cache lab has an explicit configuration and access sequence. It is useful because it exposes mapping rules: block, index/slot, and tag. It does not inspect the actual cache hierarchy of the computer running the program.

Before running it, calculate a few expected results by hand. For each address, determine its block number, selected slot, and tag according to the model. Then explain why a later address hits or misses.

The point is to understand a model's rules and limitations, not to claim that the model exactly reproduces real CPU caches with their associativity, prefetching, coherence, replacement logic, and timing.

## 8. Virtual memory: understand the model's variables

The FIFO lab receives a page-reference sequence and a frame count. It uses a queue to represent the order in which pages entered memory and counts faults when a referenced page is absent.

The EAT calculation is a mathematical model. Before using it, match each variable and unit to the formula in your course notes. Different teaching examples can define access paths differently; an equation should never be copied without its assumptions.

A simulated page fault in this lab is not a measured OS page fault. The lab teaches page-replacement reasoning and expected-cost calculations, not how the host OS manages a process's real page tables or storage I/O.

## 9. CPU and instruction-cycle model

The toy CPU makes the fetch/decode/execute idea concrete. Follow the program counter, instruction register, memory address register, memory buffer/data register, and accumulator through a tiny sequence of instructions.

At each step ask:

- Which address is being read?
- Which instruction was fetched?
- Which register changes?
- Which memory location changes?
- What will the next PC value be?

The model's memory and registers are C# data structures. They are not the machine's physical registers, and the trace is not a cycle-accurate record of the host CPU. The learning goal is to understand state transitions and data movement inside a simplified processor model.

## 10. Pipeline timing and dependencies

The ideal five-stage pipeline calculator helps separate throughput from single-instruction latency. In an ideal pipeline, instruction stages overlap. Real implementations may have stalls, forwarding, branch penalties, cache misses, resource conflicts, and different stage organization.

The RAW detector identifies a simple read-after-write dependency in toy instruction descriptions. A dependency alone does not determine exact stall cycles: the result depends on the implementation and whether forwarding is available. WAR and WAW are different dependency types and are not fully modeled by the current simple detector.

Treat the timing chart as a reasoned model, not as a trace of the CPU inside your PC.

## 11. Booth multiplication and address modes

Booth's algorithm lab prints each algorithmic iteration so you can connect sign representation, A/Q/Q-1, arithmetic shifting, and signed multiplication. Use small inputs first and compare the result against ordinary multiplication for the selected bit width.

The addressing-modes lab uses an integer array to illustrate immediate, direct, and indirect addressing. An array is a teaching stand-in for memory: the example explains the relationship between an operand and the value/address it represents, not physical address circuitry.

## 12. How to study measurements responsibly

For every performance experiment, record:

- the hypothesis;
- the input size and data distribution;
- the correctness check;
- the baseline and modified implementation;
- the build configuration and runtime;
- the number of warm-up and timed runs;
- elapsed time and allocations if measured;
- hardware counters only when a suitable tool actually reports them;
- what the experiment supports and what it cannot prove.

Use Release builds for meaningful performance experiments. A single `Stopwatch` result is useful for exploration but is not a robust benchmark. For serious comparisons, look at [BenchmarkDotNet](https://benchmarkdotnet.org/GettingStarted.htm), the [Visual Studio/.NET profiling tools](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/profilers), and [.NET counters](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters).

## 13. Final learning checklist

Before considering a lab complete, make sure you can:

- explain the concept without reading its description;
- state whether it applies to software, runtime, OS, or hardware;
- predict a small example before running it;
- explain the actual output;
- identify the assumptions in the model;
- distinguish direct measurements from inferences;
- describe one real engineering situation where the concept matters;
- update the course coverage matrix honestly.

The project succeeds when it improves your systems intuition and decision-making—not merely when every menu item runs.
