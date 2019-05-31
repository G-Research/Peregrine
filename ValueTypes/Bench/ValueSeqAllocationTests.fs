namespace Peregrine.ValueTypes.Test

open NBench
open Peregrine.ValueTypes

/// Tests to make sure that the ValueSeq functions genuinely do not allocate
/// Note: the functions we pass into the ValueSeq functions are class methods that have been forceably realised as a
/// closure by passing them through 'id', otherwise the closure is created late in the benchmark method, which allocates.
type ValueSeqAllocationTests () =
    // Data used for the tests
    let arrayBackedData = ValueSeq.ofArray [|1; 2; 4; 8; 16|]
    let listBackedData = ValueSeq.ofList [1; 2; 4; 8; 16]
    // Functions passed into the various ValueSeq functions under test
    let iterAccumulator =
        let mutable accumulator = 0
        (fun x -> accumulator <- accumulator + x) |> id
    let iteriAccumulator =
        let mutable accumulator = 0
        (fun i x -> accumulator <- accumulator + i + x) |> id
    let add = (+) |> id
    // Somewhere to hold onto our result, lest it be optimised away...
    let mutable result = ValueNone
    
    [<PerfBenchmark(Description="Check that iterating through an array using ValueSeq doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.LengthofArray () =
        result <- arrayBackedData |> ValueSeq.count |> ValueSome

    [<PerfBenchmark(Description="Check that iterating through a list using ValueSeq doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.LengthOfList () =
        result <- listBackedData |> ValueSeq.count |> ValueSome

    [<PerfBenchmark(Description="Check that ValueSeq.tryHead doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.TryHead () =
        result <- arrayBackedData |> ValueSeq.tryHead

    [<PerfBenchmark(Description="Check that ValueSeq.iter doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.Iter () =
        arrayBackedData |> ValueSeq.iter iterAccumulator

    [<PerfBenchmark(Description="Check that ValueSeq.iteri doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.Iteri () =
        arrayBackedData |> ValueSeq.iteri iteriAccumulator

    [<PerfBenchmark(Description="Check that ValueSeq.fold doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.Fold () =
        result <- arrayBackedData |> ValueSeq.fold add 0 |> ValueSome
        
    [<PerfCleanup>]
    member this.Teardown () =
        result <- ValueNone
