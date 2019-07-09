namespace Peregrine.ValueTypes.Test

open NUnit.Framework
open FsUnit

open Peregrine.ValueTypes

[<TestFixture; RequireQualifiedAccess>]
module ValueSeqTests =

    let testData =
        seq {
            yield [| 1; 2; 4; 8; 16 |], 31
            yield [| 2 |], 2
            yield Array.empty, 0
        }

    let makeTestCaseData (testData, expectedResult) =
        TestCaseData(box testData).Returns(expectedResult)

    let arraysWithLength = testData |> Seq.map (fun (array, _) -> array, array.Length) |> Seq.map makeTestCaseData

    let arraysForSumming = testData |> Seq.map makeTestCaseData

    [<TestCaseSource("arraysWithLength")>]
    let ``Test ValueSeq.length over an array`` (array : int array) =
        array |> ValueSeq.ofArray |> ValueSeq.count

    [<TestCaseSource("arraysForSumming")>]
    let ``Test ValueSeq.iter`` testValues =

        let mutable sum = 0

        // Create a ValueSeq from the array and sum up the values using the ValueSeq module's iter func
        let testValuesSeq = ValueSeq.ofArray testValues
        ValueSeq.iter (fun x -> sum <- sum + x) testValuesSeq
        sum

    [<TestCaseSource("arraysForSumming")>]
    let ``Test ValueSeq.iteri`` testValues =

        let mutable sum = 0
        let mutable counter = -1

        let testValuesSeq = ValueSeq.ofArray testValues

        // Use the iteri function to sum up the values in the test array; make sure the index is correct.
        ValueSeq.iteri
            (fun i x ->
                sum <- sum + x
                counter <- counter + 1
                i |> should equal counter
            ) testValuesSeq
        sum

    [<TestCaseSource("arraysForSumming")>]
    let ``Test ValueSeq.fold over an array`` array =
        array |> ValueSeq.ofArray |> ValueSeq.fold (+) 0

    [<Test>]
    let ``Test ValueSeq.foldn over a longer than n sequence`` () =
        let array = [| 1; 2; 4; 8; 16 |]
        array |> ValueSeq.ofArray |> ValueSeq.truncate 3 |> ValueSeq.fold (+) 0 |> should equal 7

    [<Test>]
    let ``Test ValueSeq.foldn over a length = n sequence`` () =
        let array = [| 1; 2; 4; 8; 16 |]
        array |> ValueSeq.ofArray |> ValueSeq.truncate 5 |> ValueSeq.fold (+) 0 |> should equal 31

    [<Test>]
    let ``Test ValueSeq.foldn over a shorter than n sequence`` () =
        let array = [| 1; 2; 4; 8; 16 |]
        array |> ValueSeq.ofArray |> ValueSeq.truncate 6 |> ValueSeq.fold (+) 0 |> should equal 31

    [<Test>]
    let ``Test ValueSeq.foldFromN over a longer than n sequence`` () =
        let array = [| 1; 2; 4; 8; 16 |]
        array |> ValueSeq.ofArray |> ValueSeq.skip 2 |> ValueSeq.fold (+) 0 |> should equal 28

    [<Test>]
    let ``Test ValueSeq.foldFromN over a length = n sequence`` () =
        let array = [| 1; 2; 4; 8; 16 |]
        array |> ValueSeq.ofArray |> ValueSeq.skip 4 |> ValueSeq.fold (+) 0 |> should equal 16

    [<Test>]
    let ``Test ValueSeq.foldFromN over a shorter than n sequence`` () =
        let array = [| 1; 2; 4; 8; 16 |]
        array |> ValueSeq.ofArray |> ValueSeq.skip 5 |> ValueSeq.fold (+) 0 |> should equal 0

    [<Test>]
    let ``TestValueSeq.foldWhile sum the values as long as the sum is less than the value`` () =
        let array = [| 1; 2; 3; 4 |]
        array |> ValueSeq.ofArray |> ValueSeq.takeWhile ((>) 4) |> ValueSeq.fold (+) 0 |> should equal 6

    [<Test>]
    let ``Test ValueSeq.tryHead with a value`` () =
        let result = [| 1 |] |> ValueSeq.ofArray |> ValueSeq.tryHead
        result |> should equal (ValueSome 1)

    [<Test>]
    let ``Test ValueSeq.tryHead with an empty sequence`` () =
        let result = Array.empty |> ValueSeq.ofArray |> ValueSeq.tryHead
        result |> should equal ValueNone

    [<Test>]
    let ``TestValueSeq.tryLast with a sequence`` () =
        [| 1; 2; 4 |] |> ValueSeq.ofArray |> ValueSeq.tryLast |> should equal (ValueSome 4)

    [<Test>]
    let ``Test ValueSeq.tryLast with an empty sequence`` () =
        let result = Array.empty |> ValueSeq.ofArray |> ValueSeq.tryHead
        result |> should equal ValueNone

    let listsForSumming =
        testData
        |> Seq.map (fun (array, result) -> array |> Array.toList, result)
        |> Seq.map makeTestCaseData

    [<TestCaseSource("listsForSumming")>]
    let ``Test ValueSeq.fold over a list`` list =
        list |> ValueSeq.ofList |> ValueSeq.fold (+) 0
