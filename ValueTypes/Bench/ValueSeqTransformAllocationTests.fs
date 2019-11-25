namespace Peregrine.ValueTypes.Bench

open NBench
open Peregrine.ValueTypes

type ValueSeqTransformAllocationTests ()  =
    // This is the data we use for the tests
    let data = ValueSeq.ofArray [|1; 2; 4; 8; 16|]
    // Functions that we pass into the test methods - we have to pass them to id to make
    // sure they're realised before the test code is executed
    let add = (+) |> id
    let times2 = (fun x -> x * 2) |> id
    let timesi = (fun i x -> i * x) |> id
    let not4 = (fun x -> not(x = 4)) |> id
    // Give us somewhere to store the result, lest it be optimised away
    let mutable result = ValueNone
    
    [<PerfBenchmark(Description="Check that ValueSeq.truncate doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.Truncate() : unit =
        let first4 = ValueSeq.truncate 4 data
        result <- ValueSome (ValueSeq.fold add 0 first4)
        
    [<PerfBenchmark(Description="Check that ValueSeq.skip doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.Skip () : unit =
        let last3 = ValueSeq.skip 2 data
        let last2 = ValueSeq.skip 1 last3
        result <- ValueSome (ValueSeq.fold add 0 last2)
        
    [<PerfBenchmark(Description="Check that ValueSeq.takeWhile doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.TakeWhile () =
        let until4 = ValueSeq.takeWhile not4 data
        result <- ValueSome (ValueSeq.fold add 0 until4)
          
    [<PerfBenchmark(Description="Check that ValueSeq.map doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.Map () =
        let times2 = ValueSeq.map times2 data
        result <- ValueSome (ValueSeq.fold add 0 times2)
        
    [<PerfBenchmark(Description="Check that ValueSeq.mapi doesn't allocate",
                    NumberOfIterations = 1,
                    RunMode = RunMode.Throughput,
                    TestMode = TestMode.Test,
                    SkipWarmups = true)>]
    [<MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.ExactlyEqualTo, 0.0)>]
    member this.MapIndexed () =
        let timesi = ValueSeq.mapi timesi data
        result <- ValueSome (ValueSeq.fold add 0 timesi)
        
    [<PerfCleanup>]
    member this.Teardown() =
        result <- ValueNone