namespace Peregrine.ValueTypes.Test

open NUnit.Framework
open BenchmarkDotNet.Attributes
open Peregrine.ValueTypes

/// Tests to make sure that the ValueSeq functions genuinely do not allocate
/// Note: the functions we pass into the ValueSeq functions are class methods that have been forceably realised as a
/// closure by passing them through 'id', otherwise the closure is created late in the benchmark method, which allocates.
[<RequireQualifiedAccess; TestFixture>]
module ValueSeqAllocationTests =

    [<Class; MemoryDiagnoser>]
    type ValueSeqAllocationBenchmark () =
        let mutable arrayBackedData = None
        let mutable listBackedData = None

        [<GlobalSetup>]
        member this.Setup () =
            arrayBackedData <- [|1; 2; 4; 8; 16|] |> ValueSeq.ofArray |> Some
            listBackedData <- [1; 2; 4; 8; 16] |> ValueSeq.ofList |> Some

        [<Benchmark>]
        member this.LengthofArray () =
            let length = ValueSeq.count arrayBackedData.Value
            ()

        [<Benchmark>]
        member this.LengthOfList () =
            let length = listBackedData.Value |> ValueSeq.count
            ()

        [<Benchmark>]
        member this.TryHead () =
            let head = arrayBackedData.Value |> ValueSeq.tryHead |> ValueOption.get
            ()

        member val IterAction =
            let mutable counter = 0
            (fun x -> counter <- counter + x) |> id

        [<Benchmark>]
        member this.Iter () =
            arrayBackedData.Value |> ValueSeq.iter this.IterAction

        member val IteriAction =
            let mutable counter = 0
            (fun x i -> counter <- counter + x) |> id

        [<Benchmark>]
        member this.Iteri () =
            arrayBackedData.Value |> ValueSeq.iteri this.IteriAction

        member val FoldAction =
            (+) |> id

        [<Benchmark>]
        member this.Fold () =
            let sum = arrayBackedData.Value |> ValueSeq.fold this.FoldAction 0
            ()

        [<Benchmark>]
        member this.Foldn () =
            let sum = arrayBackedData.Value |> ValueSeq.foldn 4 this.FoldAction 0
            ()

        [<Benchmark>]
        member this.FoldFromN () =
            let sum = arrayBackedData.Value |> ValueSeq.foldFromN 4 this.FoldAction 0
            ()

        member val FoldPredicate =
            (fun _ _ -> true) |> id

        [<Benchmark>]
        member this.FoldWhile () =
            let sum = arrayBackedData.Value |> ValueSeq.foldWhile this.FoldPredicate this.FoldAction 0
            ()

    [<Test>]
    let runTests () = BenchmarkDotNetHelpers.assertAllocationFree<ValueSeqAllocationBenchmark> ()
