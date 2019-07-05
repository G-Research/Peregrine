namespace Peregrine.ValueTypes.Test

open NUnit.Framework
open FsUnit
open Peregrine.ValueTypes

[<RequireQualifiedAccess; TestFixture>]
module ValueSeqTransformTests =

    //
    // ValueSeq.skip tests
    //

    [<Test>]
    let ``Test skipping over a longer than n sequence`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.skip 2
        |> ValueSeq.toSeq
        |> should equal [ 4; 8 ]

    [<Test>]
    let ``Test skipping over a shorter than n sequence`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.skip 6
        |> ValueSeq.toSeq
        |> should be Empty

    [<Test>]
    let ``Test a skip of zero`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.skip 0
        |> ValueSeq.toSeq
        |> should equal [ 1; 2; 4; 8 ]

    [<Test>]
    let ``Test skpping over an empty sequnce`` () =
        []
        |> ValueSeq.ofList
        |> ValueSeq.skip 1
        |> ValueSeq.toSeq
        |> should be Empty

    //
    // ValueSeq.truncate tests
    //

    [<Test>]
    let ``Test truncating over a longer than n sequence`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.truncate 2
        |> ValueSeq.toSeq
        |> should equal [ 1; 2 ]

    [<Test>]
    let ``Test truncating over a shorter than n sequence`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.truncate 6
        |> ValueSeq.toSeq
        |> should equal [ 1; 2; 4; 8 ]

    [<Test>]
    let ``Test truncating to zero`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.truncate 0
        |> ValueSeq.toSeq
        |> should be Empty

    [<Test>]
    let ``Test truncating empty sequence`` () =
        []
        |> ValueSeq.ofList
        |> ValueSeq.truncate 1
        |> ValueSeq.toSeq
        |> should be Empty

    //
    // Combined ValueSeq.skip and ValueSeq.truncate tests
    //

    [<Test>]
    let ``Test skipping and truncating a sequence`` () =
        [| 1; 2; 4; 8 |]
        |> ValueSeq.ofArray
        |> ValueSeq.skip 1
        |> ValueSeq.truncate 2
        |> ValueSeq.toSeq
        |> should equal [2; 4]
        
    //
    // ValueSeq.takeWhile tests
    //
    
    [<TestCase(true)>]
    [<TestCase(false)>]
    let ``Test taking from an empty sequence`` predicateValue =
        []
        |> ValueSeq.ofList
        |> ValueSeq.takeWhile (fun _ -> predicateValue)
        |> ValueSeq.toSeq
        |> should be Empty
        
    [<Test>]
    let ``Test taking everything`` () =
        [1; 2; 4; 8]
        |> ValueSeq.ofList
        |> ValueSeq.takeWhile (fun _ -> true)
        |> ValueSeq.toSeq
        |> should equal [1; 2; 4; 8]
        
    [<Test>]
    let ``Test taking nothing`` () =
        [1; 2; 4; 8]
        |> ValueSeq.ofList
        |> ValueSeq.takeWhile (fun _ -> false)
        |> ValueSeq.toSeq
        |> should be Empty
        
    [<Test>]
    let ``Test taking some items`` () =
        [1; 2; 4; 8]
        |> ValueSeq.ofList
        |> ValueSeq.takeWhile (fun x -> not (x = 4))
        |> ValueSeq.toSeq
        |> should equal [1; 2]
        
    //
    // ValueSeq.map tests
    //
    
    [<Test>]
    let ``Test mapping over some sequence`` () =
        [1; 2; 4; 8]
        |> ValueSeq.ofList
        |> ValueSeq.map (fun x -> x * 2)
        |> ValueSeq.toSeq
        |> should equal [2; 4; 8; 16]
        
    [<Test>]
    let ``Test mapping over the empty sequence`` () =
        []
        |> ValueSeq.ofList
        |> ValueSeq.map id
        |> ValueSeq.toSeq
        |> should be Empty

    //
    // ValueSeq.scan tests
    //

    [<Test>]
    let ``Test summing over a list with ValueSeq.scan`` () =
        [1; 2; 4; 8]
        |> ValueSeq.ofList
        |> ValueSeq.scan (+) 0
        |> should equal [1; 3; 7; 15]

    [<Test>]
    let ``Test ValueSeq.scan over an empty list`` () =
        []
        |> ValueSeq.ofList
        |> ValueSeq.scan (+) 0
        |> should be Empty