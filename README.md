# LogPulse.Engine — Learn Computer Architecture Through a Real C# Application

> **Purpose:** Learn how computer architecture and organization concepts influence software behavior, memory use, data processing, and performance decisions—by building and inspecting a small application instead of memorizing definitions alone.

`LogPulse.Engine` is a .NET 8 console application with two complementary parts:

1. **A useful log analyzer** that reads log files, validates records, counts errors, and calculates response-time statistics.
2. **Focused learning labs** that demonstrate selected architecture concepts, run simple performance comparisons, or model hardware behavior explicitly.

The project is designed for learning and experimentation. It is **not** a production log platform, a hardware profiler, or a cycle-accurate CPU simulator. Its main value is the connection it creates between a software problem, a computer-systems concept, an experiment, and an engineering conclusion.

**Recommended reading order:** start with this README, then use the [Deep Learning Guide](Docs/LEARNING-GUIDE.md) while running the labs. Keep the [Coverage Matrix](Docs/Coverage-Matrix.md) and [Learning Roadmap](Docs/Roadmap.md) open as you progress.

---

## 1. What should you learn from this project?

By working through the project rather than merely running it, you should build the ability to:

- Explain the path from a file on storage to data parsed and processed by a C# program.
- Distinguish input, parsing, validation, processing, aggregation, and reporting.
- Understand why reading data incrementally can reduce the amount of input retained in managed memory.
- Recognize that a program's performance depends on data layout, memory access patterns, computation, allocation, runtime behavior, and operating-system activity—not simply on the number of C# statements.
- Understand binary representation, carry, signed overflow, and the difference between a teaching ALU model and the arithmetic instructions executed by the host CPU.
- Explore temporal and spatial locality, cache hits and misses, and how cache models explain behavior without directly inspecting the physical cache.
- Compare scalar numeric work with SIMD-capable code and sequential work with parallel work, while checking correctness and measuring the actual result.
- Understand what a simplified CPU, instruction-cycle trace, address-mode demonstration, or pipeline model can teach—and what it cannot prove about the real machine.
- Learn to form a performance hypothesis, measure a baseline, change one variable, repeat the test, and report only conclusions supported by evidence.
- Decide when a concept belongs in application code, a controlled benchmark, a mathematical model, or a separate educational simulation.

The goal is **not** to apply every hardware concept directly inside the log analyzer. The goal is to place every concept in the right learning context.

## 2. Three kinds of learning activity

Keep these categories separate when interpreting the project.

| Activity type | What it means | Example in this project | What it can establish |
|---|---|---|---|
| **Real application behavior** | Code performs a useful task on input data. | Parse log records and aggregate statistics. | Whether the application produces correct results for the tested input. |
| **Measured experiment** | Two implementations or access patterns are compared under specified conditions. | Sequential vs randomized array access; scalar vs vector summation. | Observed elapsed-time differences for those runs and that environment. |
| **Teaching model** | Code represents a simplified hardware or operating-system concept. | Direct-mapped cache, FIFO page replacement, toy CPU, ideal pipeline. | The behavior implied by the model's rules and assumptions—not the internal state of your physical CPU or operating system. |

A fourth activity may be a **conceptual explanation** when a topic is not sensibly represented by the application or current lab. A concept should never be marked complete merely because its name appears in a comment or menu.

---

## 3. Requirements

- Visual Studio Community with the **.NET 8 SDK** installed, or the .NET 8 SDK installed separately.
- Windows, Linux, or macOS for the console application.
- A terminal is optional; Visual Studio is sufficient for building, running, and stepping through the code.

Timing results may differ between operating systems, CPU models, runtime versions, power states, and background workloads. This is expected and should be recorded rather than hidden.

## 4. Open, build, and run

1. Extract the project archive.
2. Open `LogPulse.Engine.sln` or `LogPulse.Engine.csproj` in Visual Studio Community.
3. Confirm that the project targets `net8.0` and that the .NET 8 SDK is installed.
4. Build the project.
5. Run with **Ctrl+F5** to start without attaching the debugger, or use **F5** when you want to step through code.
6. Select a menu option and read its notes before interpreting the output.

You can also run it from a terminal in the project directory:

```bash
dotnet --info
dotnet build
dotnet run
```

If the build fails, address the first compiler error before investigating later errors; many follow-on diagnostics are consequences of the first one. Do not assume the project has been verified on your machine until it builds and runs successfully there.

---

## 5. The application workflow: from raw text to a useful report

The main application is deliberately small. Its normal data path is:

```text
Log file
   │
   ▼
File.ReadLines
   │  Produces lines incrementally rather than first loading the entire file as one string.
   ▼
LogParser.TryParse
   │  Validates each record and converts text fields to typed values.
   ▼
LogAnalyzer.Analyze
   │  Accumulates counts and response-time statistics in one pass.
   ▼
LogSummaryReport
   │  Holds the final statistics.
   ▼
Console report
```

### Why this path matters

A real log analyzer must do more than split strings. It must decide what counts as a valid record, how to treat malformed timestamps or invalid numbers, how to measure response times, and whether the report denominator includes rejected rows.

This project distinguishes:

- **Lines read:** all input lines encountered, including invalid lines.
- **Valid logs processed:** records that passed parsing and validation.
- **Invalid lines skipped:** rows that were rejected.
- **Error/critical count:** valid records whose severity is `ERROR` or `CRITICAL`.
- **Error percentage:** error/critical count divided by valid records processed.
- **Average response time:** the arithmetic mean among valid records.
- **Maximum response time:** the largest response time among valid records.

If no valid records are processed, percentage and average are reported as zero to avoid division by zero. That is a reporting choice for this starter application, not a universal rule for analytics systems; another application might represent an undefined statistic as `null` or display “N/A”.

### Input format

The parser expects five pipe-delimited fields:

```text
TimestampUtc|Severity|SourceModule|ResponseTimeMs|Message
2025-01-01T12:00:00Z|ERROR|ApiGateway|510|Upstream service returned 503
```

The timestamp format is intentionally strict: `yyyy-MM-dd'T'HH:mm:ss'Z'`. The parser splits into at most five fields, so additional `|` characters stay inside the message field. Invalid timestamps, missing required fields, invalid response times, and negative response times are rejected.

This is a teaching format, not a universal log standard. Production systems may require adapters for JSON, Apache/Nginx formats, structured logging, different timestamp precision, time-zone offsets, escaped delimiters, and schema versioning.

### Important implementation detail: streaming

`LogAnalyzer.AnalyzeFile` uses `File.ReadLines` and accumulates statistics as it enumerates the file. It does not first build a list of every parsed `LogEntry`. This is a practical example of processing data incrementally. It does not mean that all memory use is constant: the runtime, file buffering, strings, parser allocations, and other work still consume memory. The exact benefit should be evaluated using representative files and measurements.

---

## 6. Menu: how to use each feature as a lesson

### 1. Analyze bundled sample logs

**Use it first.** Observe the complete workflow and learn what each report field means. Compare the total number of lines with the number of valid records and invalid records. Explain why those counts need not be identical.

**Learning questions**
- Why should malformed rows not crash analysis of an otherwise usable file?
- Why does the error percentage use valid processed records as its denominator?
- Which tests would reveal a bug in timestamp or response-time parsing?

### 2. Analyze a log file by path

Use a file of your own that follows the documented format. Begin with a tiny hand-written file whose expected output you can calculate by hand. Then increase its size.

**Learning questions**
- Does the analyzer produce the same statistics for a file and for the same lines supplied as an in-memory sequence?
- What should happen with an empty file?
- Can a message contain a pipe character?
- What information would be needed to support a new log format safely?

### 3. Binary addition and ALU-style flags

This lab implements a small, bit-by-bit teaching adder and reports result bits plus flags such as zero, negative, carry, and signed overflow.

**Learn:** fixed-width representation, two's complement intuition, carry-out, and the distinction between unsigned carry and signed overflow.

**Try:** add values near the 32-bit signed limits; compare a result that sets carry with a result that sets signed overflow. Do not assume these flags are read from the host CPU: they are calculated by the model.

Related C# behavior: `checked` and `unchecked` determine how certain integral operations and conversions behave when their result exceeds the destination range. This is related to arithmetic range, but it is not the same as reading a hardware ALU flag. See [C# checked and unchecked](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/checked-and-unchecked).

### 4. Sequential vs randomized memory access

This experiment reads values from an array in sequential order and in a randomized index order, then reports elapsed time.

**Learn:** spatial locality and why nearby data access can interact favorably with cache lines and prefetching.

**Do not conclude:** the timer directly counted cache misses. It did not. The randomized case also reads an index array, so the test measures the combined behavior of data access, index access, CPU, JIT, operating-system scheduling, and runtime state.

**Experiment:** predict which order will be faster, run it several times, reverse the order of the two tests, and record whether the result is stable. Test multiple array sizes rather than only one. The [CS 61C cache notes](https://notes.cs61c.org/content/caches-ii/) explain locality, blocks, tags, and cache terminology.

### 5. Scalar vs `Vector<float>` summation

The lab compares a scalar sum with a `Vector<float>` implementation and reports whether hardware acceleration is available, the vector width, the output values, their difference, and elapsed times.

**Learn:** SIMD applies one operation across multiple data elements; it is especially useful when the same numeric operation repeats over a buffer.

**Important:** vectorization is not guaranteed to speed up every workload. Input size, vector width, memory traffic, JIT behavior, CPU, and measurement methodology matter. Floating-point addition is not perfectly associative, so the different summation order can produce a small numeric difference.

**Exercise:** check both results against an acceptable tolerance, run several sizes, and explain whether the measured time supports your hypothesis. Review Microsoft's [.NET SIMD guide](https://learn.microsoft.com/en-us/dotnet/standard/simd), which recommends testing correctness, edge cases, and performance before keeping the additional complexity.

### 6. Sequential vs parallel aggregation

This lab compares a sequential sum with a `Parallel.For`-based sum. It verifies the totals and measures elapsed time.

**Learn:** multiple workers can process independent pieces of a problem, but partitioning, scheduling, synchronization, and combining partial results have costs.

**Important:** parallel work can be slower when each iteration is cheap or the dataset is small. Correct parallel code must also consider shared mutable state and race conditions.

**Exercise:** repeat the comparison with small, medium, and large inputs. Record the point—if any—where the parallel version begins to help. Read Microsoft's [Task Parallel Library overview](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) and [parallelism pitfalls](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/potential-pitfalls-in-data-and-task-parallelism).

### 7. Direct-mapped cache simulation

The lab applies a simplified mapping rule to an explicit address trace and prints block, slot/index, tag, and hit/miss results.

**Learn:** why a cache needs both an index and a tag, and why different memory blocks can compete for the same cache slot.

**Exercise:** before running the program, calculate the slot and tag for two or three addresses by hand. Then change the address pattern and predict how the hit ratio changes.

**Boundary:** this is a deterministic model of a configured cache. It does not inspect or reproduce the host CPU's actual cache hierarchy, replacement policy, prefetcher, associativity, coherence traffic, or timing. The [CS 61C cache notes](https://notes.cs61c.org/content/caches-ii/) and [MIT computer architecture lecture notes](https://ocw.mit.edu/courses/6-823-computer-system-architecture-fall-2005/pages/lecture-notes/) are useful for deeper study.

### 8. Virtual memory, FIFO page replacement, and EAT

The lab processes a page-reference sequence using a FIFO replacement model and calculates a simplified effective access time from inputs.

**Learn:** how a page-reference pattern and a finite number of frames affect simulated page faults; how a probability-weighted equation combines possible access paths.

**Boundary:** simulated page faults are not operating-system page-fault measurements. The EAT formula must match the exact definitions used by the course notes. Before interpreting any result, write down the meaning and units of every variable.

**Exercise:** change the reference sequence and frame count, predict which pages will be evicted, and compare your hand calculation with the trace. Then vary the page-fault probability to observe how a costly rare event can affect the expected average.

### 9. Toy CPU: fetch, decode, execute

This is a simplified accumulator-based CPU model. It maintains a small memory array and registers such as a program counter, instruction register, memory address/data registers, and accumulator. The output traces the steps used to execute a tiny program.

**Learn:** how instruction execution can be described as a sequence of state transitions and data transfers.

**Boundary:** C# objects and arrays stand in for registers, memory, and instructions. The simulation is not cycle-accurate and does not show actual electrical signals, physical registers, or the host processor's internal operations.

**Exercise:** follow the value of every modeled register after each step. Predict the final accumulator and memory values before you run the lab.

### 10. Ideal five-stage pipeline model

The calculator illustrates an idealized pipeline with stages such as IF, ID, EX, MEM, and WB, and compares its theoretical cycle count with a non-pipelined model.

**Learn:** pipelining overlaps the execution of multiple instructions; its benefit is primarily higher throughput, not necessarily reducing the latency of an individual instruction by the same factor.

**Boundary:** the model assumes ideal stage behavior and omits real stalls, cache delays, branch penalties, forwarding details, and other processor-specific effects.

**Exercise:** calculate the cycle count for a small instruction count by hand. Then explain why real observed CPU performance will not necessarily match the ideal speedup.

### 11. RAW dependency demonstration

This lab scans a small, simplified instruction list for read-after-write dependencies by comparing register names.

**Learn:** a later instruction may need a value produced by an earlier instruction. Data dependencies can constrain overlap even when a CPU supports pipelining.

**Boundary:** finding a dependency does not by itself calculate the exact number of stalls. Forwarding, pipeline design, instruction timing, and scheduling affect the outcome. WAR and WAW analysis are not fully modeled by this simple demonstration.

### 12. Booth signed multiplication

The lab traces the state of A, Q, and Q-1 through Booth's signed multiplication algorithm and compares the modeled result with ordinary C# multiplication for the selected width.

**Learn:** how a fixed-width algorithm represents signed values, makes decisions from adjacent bits, and uses arithmetic shifts to reduce repeated additions/subtractions in its model.

**Exercise:** predict the operation from the pair of low-order bits at each step, then compare the final result with the reference multiplication. Always keep the chosen bit width and its signed range in mind.

### 13. Addressing modes

The demo contrasts immediate, direct, and indirect addressing using a C# array as illustrative memory.

**Learn:** whether an instruction contains the value itself, an address of the value, or an address that points to another address.

**Boundary:** an integer array illustrates the meaning of the modes; it is not the host CPU's address decoder or pointer circuitry.

### 14. Architecture concepts and modeling boundaries

This option provides explanatory notes for topics that are not implemented as a full runnable model in the current starter, including broader processor and I/O concepts.

Use it as a map for further learning, not as proof that every topic in the course has been fully implemented. The [MIT Computer System Architecture course](https://ocw.mit.edu/courses/6-823-computer-system-architecture-fall-2005/) provides university-level lecture notes on instruction sets, pipelining and hazards, caches, virtual memory, branch prediction, coherence, vector processing, and more.

### 15. Basic self-tests

Run these checks after modifying the parser, arithmetic labs, or models. They provide a quick regression check for selected behaviors.

**Important:** passing the included self-tests does not prove that the complete project is defect-free or that every topic is correct. Add tests for new behaviors and edge cases as the project grows.

---

## 7. How to study each lab effectively

Do not begin by pressing a menu option and reading the answer. Use the following cycle for every experiment:

1. **Connect to the course:** read the exact paragraph, diagram, or equation from your lecture notes.
2. **State the question:** what real problem or theoretical behavior are you trying to understand?
3. **Make a prediction:** write what you expect the model or benchmark to do and why.
4. **Understand the code:** identify inputs, state, operations, output, and assumptions before running it.
5. **Calculate a small case by hand:** use tiny data to check your mental model.
6. **Run and validate:** compare the program's output with your hand calculation or expected result.
7. **Change one variable:** for example, input size, memory access order, cache slot count, page count, or thread count.
8. **Record the observation:** preserve the inputs and output so you can compare runs.
9. **Explain the result:** separate what the test directly measured from what you infer about architecture.
10. **Write a short conclusion:** state what you learned, what surprised you, and what the experiment did not establish.

A good learning note contains three parts:

- **Concept:** the rule described by the course.
- **Experiment:** what the code did under specific conditions.
- **Conclusion:** which part of the rule the evidence illustrates, and which parts remain theoretical or unmeasured.

---

## 8. Responsible performance measurement

Performance engineering begins with a correct baseline, not with a guessed optimization. Microsoft's guidance recommends measuring first, finding a hot path, and measuring again after a change. See [C# performance guidance](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/) and [.NET profiling tools](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/profilers).

For useful comparisons:

- **Build Release mode** for performance runs. Debug builds and an attached debugger can change execution behavior.
- **Use identical input data** for each implementation and validate that both compute the intended result.
- **Warm up code paths** where JIT compilation can affect timing. Warm-up is necessary but does not make a hand-written benchmark fully rigorous.
- **Repeat measurements.** A single `Stopwatch` sample is only a demonstration. Report multiple samples or use a dedicated benchmarking harness.
- **Change one factor at a time.** Do not alter the data layout, algorithm, input size, and threading model in one experiment.
- **Record the environment:** CPU model, logical processor count, OS, .NET runtime/SDK, build configuration, input size, data distribution, and relevant background activity.
- **Measure memory separately from time.** Allocations, working set, GC activity, and elapsed time are different metrics.
- **Keep the output comparable.** If one version performs more work or calculates a different result, the timing comparison is not valid.
- **Expect noise.** CPU frequency changes, thermal behavior, other processes, JIT decisions, and memory state can affect results.

### What the project measures—and what it does not

| Observation | How to interpret it |
|---|---|
| `Stopwatch` elapsed time | Wall-clock time for the measured code region; not a direct CPU-cycle count. |
| Equal totals from two aggregation methods | Evidence that both methods agree for the tested input; not a proof for all possible inputs. |
| `Vector.IsHardwareAccelerated` and `Vector<T>.Count` | Information about the runtime's vector support and width; not proof that the vector loop is faster. |
| Simulated cache hits and misses | Results from the cache model's parameters and trace; not the host CPU's hardware counters. |
| Simulated page faults | Results from the selected page-replacement algorithm and reference string; not OS page-fault events. |
| Ideal pipeline speedup | A theoretical result under explicit assumptions; not measured execution speed on the host CPU. |
| `MemoryMappedFile` use | Access through a memory-mapped view managed by the OS memory manager; not a guarantee of end-to-end zero-copy or direct application control over DMA. |

For more serious .NET performance analysis, study [BenchmarkDotNet](https://benchmarkdotnet.org/GettingStarted.htm), Microsoft's [dotnet-counters](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters), and [dotnet-trace](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace). Access to specific hardware counters depends on the operating system, hardware, tools, and permissions.

---

## 9. Suggested learning roadmap

### Milestone 1 — Make the application correct

- Run the bundled sample.
- Read `Models/LogEntry.cs`, `Models/LogSummaryReport.cs`, `Services/LogParser.cs`, and `Services/LogAnalyzer.cs` in that order.
- Trace one input line from raw text to report contribution.
- Test empty files, malformed timestamps, missing fields, invalid numbers, negative response times, and messages containing `|`.
- Add one test before changing a behavior.

**Completion question:** can you explain where each report number comes from and how invalid input affects it?

### Milestone 2 — Understand fixed-width arithmetic

- Work through binary addition and signed overflow.
- Compare carry and signed overflow cases.
- Trace Booth's algorithm with small signed numbers.
- Add tests around the largest and smallest values supported by the chosen width.

**Completion question:** can you explain the bit pattern and the mathematical value without confusing the two?

### Milestone 3 — Learn memory behavior through models and experiments

- Calculate direct-mapped cache tags and indexes by hand.
- Compare sequential and randomized access over equal data.
- Vary the input size and repeat the experiment.
- Explain temporal and spatial locality separately.

**Completion question:** can you distinguish a measured time difference from a measured cache-miss count?

### Milestone 4 — Understand vectorization and parallelism

- Verify scalar and vector results before comparing speed.
- Check for expected floating-point rounding differences.
- Compare sequential and parallel aggregation with the same dataset.
- Try several input sizes; do not assume parallelism helps small workloads.
- Study shared state, local accumulators, synchronization, and race conditions.

**Completion question:** can you explain both the benefit and the overhead of parallel work?

### Milestone 5 — Files, virtual memory, and I/O

- Compare incremental reading with a deliberately full-file approach using equivalent input and result checks.
- Study virtual-memory/page-replacement models separately from OS measurements.
- Read the official memory-mapped-file documentation before adding a memory-mapped experiment.
- Do not describe async I/O as DMA or memory mapping as guaranteed zero-copy.

**Completion question:** can you explain which layer—application, runtime, OS, or hardware—controls each behavior?

### Milestone 6 — Processor models and the course coverage matrix

- Trace the toy CPU's modeled registers and memory after each instruction.
- Work through ideal pipeline timing and RAW dependency examples.
- Add more detailed instruction formats or control-signal traces only after matching the exact notation and tables in the course notes.
- Review `Docs/Coverage-Matrix.md` and mark coverage only after the exact source section has been checked and the learning activity completed.

**Completion question:** can you state the assumptions and limitations of each model without presenting it as a physical CPU trace?

---

## 10. Project structure

```text
LogPulse.ArchitectureLab/
├── LogPulse.Engine.sln
├── LogPulse.Engine.csproj
├── Program.cs
├── Models/
│   ├── LogEntry.cs
│   └── LogSummaryReport.cs
├── Services/
│   ├── LogParser.cs
│   ├── LogAnalyzer.cs
│   └── SampleLogGenerator.cs
├── Labs/
│   ├── AddressingModesLab.cs
│   ├── ArchitectureConceptLab.cs
│   ├── BitwiseArithmeticLab.cs
│   ├── BoothMultiplierLab.cs
│   ├── InstructionCycleLab.cs
│   ├── MemoryAndCacheLab.cs
│   ├── PerformanceLab.cs
│   ├── PipelineLab.cs
│   ├── SelfTestLab.cs
│   └── VirtualMemoryLab.cs
├── Data/
│   └── sample-logs.txt
└── Docs/
    ├── Coverage-Matrix.md
    └── Roadmap.md
```

The organization is intentionally modest: models represent data, services implement the useful log-processing workflow, and the `Labs` folder contains independent experiments and teaching models. The project should only gain additional files or abstractions when they make a concept easier to understand, test, or maintain.

---

## 11. Coverage: completeness must be demonstrated

The project includes a [coverage matrix](Docs/Coverage-Matrix.md) and a [roadmap](Docs/Roadmap.md). Treat them as living learning documents, not marketing claims.

A topic should be marked complete only when:

1. The related course material has been read.
2. Its definitions, diagrams, equations, and special cases have been recorded accurately.
3. The selected application, experiment, or model has been implemented or explained at the appropriate level.
4. The result has been tested or checked against a hand-worked example.
5. The limitations have been recorded.

Some topics in a computer-architecture course cannot honestly be implemented inside an ordinary log-processing program. They should remain focused teaching models or explanations rather than being forced into production code. The project is not a claim that every line or figure in the supplied course sources is already covered.

---

## 12. Professional learning resources

Use these references to deepen the relevant topic. Start with the course source first, then use the reference to clarify the concept or understand its practical boundary.

### Computer architecture and organization

- **UC Berkeley CS 61C — Cache Terminology and Locality:** a clear explanation of temporal/spatial locality, cache blocks, tags, hits, and misses.  
  https://notes.cs61c.org/content/caches-ii/
- **MIT OpenCourseWare — Computer System Architecture:** university lecture notes and problem sets spanning ISA design, microprogramming, pipeline hazards, caches, virtual memory, branch prediction, coherence, vector processing, and more.  
  https://ocw.mit.edu/courses/6-823-computer-system-architecture-fall-2005/
- **A Primer on Memory Consistency and Cache Coherence (Wisconsin):** a deeper reference for cache coherence and false sharing.  
  https://pages.cs.wisc.edu/~markhill/papers/primer2020_2nd_edition.pdf

### C# and .NET implementation details

- **SIMD and hardware intrinsics in .NET:** vectorization APIs, hardware acceleration, testing, and benchmarking cautions.  
  https://learn.microsoft.com/en-us/dotnet/standard/simd
- **Memory-mapped files in .NET:** mapped views, persisted/non-persisted files, and OS-managed memory mapping.  
  https://learn.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files
- **C# `checked` and `unchecked`:** integral arithmetic overflow behavior and checked conversions.  
  https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/checked-and-unchecked
- **Task Parallel Library (TPL):** APIs for task and data parallelism in .NET.  
  https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl
- **Potential pitfalls in data and task parallelism:** why parallel execution can be slower and how shared state introduces risk.  
  https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/potential-pitfalls-in-data-and-task-parallelism
- **C# performance guidance:** measure first, identify hot paths, and verify that changes improve memory and execution behavior.  
  https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/
- **BenchmarkDotNet — Getting Started:** a dedicated framework for repeatable .NET benchmarking.  
  https://benchmarkdotnet.org/GettingStarted.htm
- **.NET profiling tools:** overview of Visual Studio profiling and `dotnet-trace`.  
  https://learn.microsoft.com/en-us/dotnet/core/diagnostics/profilers
- **`dotnet-counters`:** runtime counter monitoring for early-stage performance investigation.  
  https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters
- **`dotnet-trace`:** collect diagnostic traces from a running .NET process.  
  https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace

---

## 13. Final principle

A useful systems engineer does not optimize by guessing, and a good student does not confuse a model with the machine it represents.

For every concept, ask:

1. What problem does this concept explain?
2. Where does it exist—in application code, the runtime, the operating system, or physical hardware?
3. Can I apply it directly, measure its effects, or only model it in this project?
4. What result do I expect before running the experiment?
5. What did I actually measure?
6. What conclusion is justified by the evidence?
7. What remains unknown?

**The objective of LogPulse.Engine is to turn course concepts into lasting engineering intuition: understand the mechanism, test a hypothesis, verify correctness, measure responsibly, and make decisions based on evidence.**
