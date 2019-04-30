namespace Peregrine.ValueTypes.Test

open BenchmarkDotNet.Attributes
open NUnit.Framework
open Peregrine.ValueTypes

[<RequireQualifiedAccess; TestFixture>]
module ValueSeqTransformAllocationTests =

    [<Class; MemoryDiagnoser>]
    type ValueSeqTransformAllocationBenchmark () =
        let mutable arrayBackedData = None

        [<GlobalSetup>]
        member this.Setup () =
            arrayBackedData <- [|1; 2; 4; 8; 16|] |> ValueSeq.ofArray |> Some

        member val FoldAction = (+) |> id

        [<Benchmark>]
        member this.Skip () =
            let last3 = Beta.ValueSeq.skip 2 arrayBackedData.Value
            let last2 = Beta.ValueSeq.skip 1 last3
            let sum = Beta.ValueSeq.fold2 this.FoldAction 0 last2
            ()

        [<Benchmark>]
        member this.Truncate () =
            let first4 = Beta.ValueSeq.truncate 4 arrayBackedData.Value
            let sum = Beta.ValueSeq.fold2 this.FoldAction 0 first4
            ()

    [<Test>]
    let runTests () = BenchmarkDotNetHelpers.assertAllocationFree<ValueSeqTransformAllocationBenchmark> ()