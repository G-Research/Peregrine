namespace Peregrine.ValueTypes.Test

open System
open NUnit.Framework
open FsUnit
open Peregrine.ValueTypes

[<TestFixture>]
module ValueOptionTests =

    [<Test>]
    let ``Test ValueOption to and from Nullable conversion of None`` () =
        let none = ValueOption<int>.ValueNone
        let asNullable = none |> ValueOption.toNullable

        asNullable.HasValue |> should equal false

        asNullable |> ValueOption.ofNullable |> should equal none

    [<Test>]
    let ``Test ValueOption to and from Nullable conversion of Some value`` () =
        let asNullable = ValueSome 1 |> ValueOption.toNullable

        asNullable.HasValue |> should equal true
        asNullable.Value |> should equal 1

        asNullable |> ValueOption.ofNullable |> should equal (ValueSome 1)

    [<Test>]
    let ``Test ValueOption.get of ValueSome returns the value`` () =
        ValueSome 1 |> ValueOption.get |> should equal 1

    [<Test>]
    let ``Test ValueOption.get of ValueNone throws an exception`` () =
        (fun () -> ValueNone |> ValueOption.get |> ignore) |> should throw typeof<InvalidOperationException>

    [<Test>]
    let ``Test ValueOption.defaultValue of ValueSome returns the value`` () =
        ValueSome 1 |> ValueOption.defaultValue 0 |> should equal 1

    [<Test>]
    let ``Test ValueOption.defaultValue of ValueNone returns default value`` () =
        ValueNone |> ValueOption.defaultValue 0 |> should equal 0

    [<Test>]
    let ``Test ValueOption.map of ValueSome applies the mapping`` () =
        ValueSome 1 |> ValueOption.map float |> should equal (ValueSome 1.)

    [<Test>]
    let ``Test ValueOption.map of ValueNone gives ValueNone`` () =
        ValueNone |> ValueOption.map float |> should equal ValueOption<float>.ValueNone