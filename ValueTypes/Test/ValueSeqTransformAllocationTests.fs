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
            let last3 = ValueSeq.skip 2 arrayBackedData.Value
            let last2 = ValueSeq.skip 1 last3
            let _ = ValueSeq.fold this.FoldAction 0 last2
            ()

        [<Benchmark>]
        member this.Truncate () =
            let first4 = ValueSeq.truncate 4 arrayBackedData.Value
            let _ = ValueSeq.fold this.FoldAction 0 first4
            ()
        
        member val Predicate = (fun x -> not(x = 4)) |> id
            
        [<Benchmark>]
        member this.TakeWhile () =
            let until4 = ValueSeq.takeWhile this.Predicate arrayBackedData.Value
            let _ = ValueSeq.fold this.FoldAction 0 until4
            ()

    [<Test>]
    let runTests () = BenchmarkDotNetHelpers.assertAllocationFree<ValueSeqTransformAllocationBenchmark> ()