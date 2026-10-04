# Learning Roadmap

The goal is to first build a correct, useful log analyzer, then use small controlled experiments and models to study architecture. Do not implement all topics in one giant class or add abstractions before a need appears.

## Milestone 1 — A working application

- Read a pipe-delimited log file line by line.
- Reject malformed rows without crashing the entire run.
- Count ERROR/CRITICAL events and calculate average/max response time.
- Test empty files, invalid timestamps, invalid numbers, negative durations, and messages containing `|`.

## Milestone 2 — Representation and arithmetic

- Use `BitwiseArithmeticLab` to study 32-bit representations, carry, zero, negative, and signed overflow.
- Use `BoothMultiplierLab` to trace A, Q and Q-1 across each multiplication iteration.
- Add tests that compare the Booth result with ordinary multiplication for values fitting the selected signed width.

## Milestone 3 — Memory and cache models

- Compare sequential and randomized reads over the same array.
- Inspect the direct-mapped cache demo and calculate block, slot, and tag by hand.
- Add temporal-locality experiments, then compare working-set sizes; do not claim to measure cache misses without hardware-counter support.

## Milestone 4 — Data parallelism

- Compare scalar `float` summation to `Vector<float>` and check numeric differences.
- Compare sequential aggregation with `Parallel.For` using local accumulators.
- Measure several runs and include warm-up; parallel work may lose on small workloads.
- Only then design a false-sharing experiment with verified layout/alignment assumptions.

## Milestone 5 — Files, virtual memory, and I/O

- Compare `File.ReadLines`, full-file loading, and selected memory-mapped access patterns using equivalent workloads.
- Track elapsed time and managed allocation, and clearly distinguish OS page faults, CPU cache misses, DMA, and actual end-to-end zero-copy claims.
- Extend the FIFO page-replacement model only if it matches the concepts and assumptions in the course notes.

## Milestone 6 — Processor models

- Extend the toy CPU with source-aligned instruction formats and addressing modes.
- Add RTL/control signal trace only after transcribing exact source tables and equations.
- Add pipeline data hazards, forwarding/stalls, branch penalty, and loop-unrolling examples as explicit models, not as claims about the host CPU's internals.
- Add a focused MESI state-machine lab using explicit assumptions.

## Performance experiment template

For every optimization, record:

1. The hypothesis.
2. The input size and data distribution.
3. The baseline implementation.
4. The modified implementation.
5. Correctness checks proving both versions calculate the same result.
6. Warm-up and number of measurements.
7. Elapsed time and allocations; hardware counters only when available.
8. Environment details and known sources of noise.
9. A conclusion that says what the evidence supports—and what it does not.
