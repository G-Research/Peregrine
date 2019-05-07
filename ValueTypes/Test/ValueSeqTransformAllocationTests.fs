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

        member val AddFunc = (+) |> id
        member val Not4Func = (fun x -> not(x = 4)) |> id
        member val Times2Func = (fun x -> x * 2) |> id
        
        [<Benchmark>]
        member this.Skip () =
            let last3 = ValueSeq.skip 2 arrayBackedData.Value
            let last2 = ValueSeq.skip 1 last3
            let _ = ValueSeq.fold this.AddFunc 0 last2
            ()

        [<Benchmark>]
        member this.Truncate () =
            let first4 = ValueSeq.truncate 4 arrayBackedData.Value
            let _ = ValueSeq.fold this.AddFunc 0 first4
            ()
        
        [<Benchmark>]
        member this.TakeWhile () =
            let until4 = ValueSeq.takeWhile this.Not4Func arrayBackedData.Value
            let _ = ValueSeq.fold this.AddFunc 0 until4
            ()
            
        [<Benchmark>]
        member this.Map () =
            let times2 = ValueSeq.map this.Times2Func arrayBackedData.Value
            let _ = ValueSeq.fold this.AddFunc 0 times2
            ()

    [<Test>]
    let runTests () = BenchmarkDotNetHelpers.assertAllocationFree<ValueSeqTransformAllocationBenchmark> ()