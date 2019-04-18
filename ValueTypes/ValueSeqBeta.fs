namespace Peregrine.ValueTypes.Beta

open System.Collections.Generic
open Peregrine.ValueTypes

[<Struct>]
type SkippingEnumerator<'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> =
    val mutable private enumerator : 'enumerator
    val private skip : int
    val mutable private skippingDone : bool

    new (skip : int, enumerator : 'enumerator) = {
        enumerator = enumerator
        skip = skip
        skippingDone = false
    }

    interface 'a IEnumerator with
        member this.MoveNext () : bool =

            if not this.skippingDone then
                // Skip over the first n elements
                let mutable skipCounter = 0

                while skipCounter < this.skip && this.enumerator.MoveNext () do
                    skipCounter <- skipCounter + 1

                this.skippingDone <- true

            this.enumerator.MoveNext()

        member this.Reset () : unit =
            this.enumerator.Reset ()
            this.skippingDone <- false

        member this.Current : 'a = this.enumerator.Current

        member this.Current : obj = this.enumerator.Current |> box

        member __.Dispose () = ()

[<Struct>]
type SkippedValueSeq<'a, 'enumerator, 'enumerable
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct
    and 'enumerable :> ValueSeq<'a, 'enumerator>>
    (count : int, enumerable : 'enumerable)
    =
    interface ValueSeq<'a, SkippingEnumerator<'a, 'enumerator>> with
        member this.GetEnumerator () =
            let enumerator = enumerable.GetEnumerator ()
            new SkippingEnumerator<'a, _>(count, enumerator)

[<Struct>]
type TruncateEnumerator<'a, 'enumerator
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct> =
    val mutable private enumerator : 'enumerator
    val private truncateTo : int
    val mutable private counter : int

    new (truncate : int, enumerator : 'enumerator) = {
        enumerator = enumerator
        truncateTo = truncate
        counter = 0
    }

    interface 'a IEnumerator with
        member this.MoveNext () : bool =

            if this.counter < this.truncateTo then
                this.counter <- this.counter + 1
                this.enumerator.MoveNext()
            else
                false

        member this.Reset () : unit =
            this.enumerator.Reset ()
            this.counter <- 0

        member this.Current : 'a = this.enumerator.Current

        member this.Current : obj = this.enumerator.Current |> box

        member __.Dispose () = ()

[<Struct>]
type TruncatedValueSeq<'a, 'enumerator, 'enumerable
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct
    and 'enumerable :> ValueSeq<'a, 'enumerator>>
    (count : int, enumerable : 'enumerable)
    =
    interface ValueSeq<'a, TruncateEnumerator<'a, 'enumerator>> with
        member this.GetEnumerator () =
            let enumerator = enumerable.GetEnumerator ()
            new TruncateEnumerator<'a, _>(count, enumerator)

[<RequireQualifiedAccess>]
module ValueSeq =

    let skip
        (count : int)
        (source : #ValueSeq<'a,'enumerator>)
        : SkippedValueSeq<_,_,_>
        =
        SkippedValueSeq(count, source)

    let fold2
        (folder : 'state -> 'a -> 'state)
        (initState : 'state)
        (source : #ValueSeq<'a,'enumerator>)
        : 'state
        =
        let mutable currentState = initState
        let mutable iter = source.GetEnumerator ()

        while iter.MoveNext() do
            currentState <- folder currentState iter.Current
        currentState

    let truncate
        (count : int)
        (source : #ValueSeq<'a,_>)
        : TruncatedValueSeq<_,_,_>
        =
        TruncatedValueSeq(count, source)
