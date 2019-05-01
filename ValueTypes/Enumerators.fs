namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerators =

    [<Struct>]
    type 'a ArrayEnumerator =
        val mutable private currentIndex : int
        val private array : 'a array

        new (array : 'a array) = {
            array = array
            currentIndex = -1
        }

        interface 'a IEnumerator with
            /// Advance the index to the next element if it is a valid index into the array
            member this.MoveNext () : bool =
                if this.currentIndex + 1 < (this.array |> Array.length) then
                    this.currentIndex <- this.currentIndex + 1
                    true
                else
                    false

            member this.Reset () : unit =
                this.currentIndex <- -1

            /// Return the element at the current index; rely on Array.get to throw when the index is invalid
            member this.Current : 'a =
                Array.get this.array this.currentIndex

            member this.Current : obj =
                Array.get this.array this.currentIndex |> box

            member __.Dispose () = ()


    [<Struct>]
    type 'a ListEnumerator =
        val private list : 'a list
        val mutable private currentHead : 'a list voption

        new (list : 'a list) = {
            list = list
            currentHead = ValueNone
        }

        interface 'a IEnumerator with
            /// Set the 'currentHead' field to point to the next item in the list
            member this.MoveNext () : bool =
                match this.currentHead with
                | ValueNone -> this.currentHead <- ValueSome this.list // initialisation
                | ValueSome (_ :: xs) -> this.currentHead <- ValueSome xs // move forward one
                | ValueSome [] -> () // iterator at the end of the list

                this.currentHead.Value |> List.isEmpty |> not

            member this.Reset () : unit =
                this.currentHead <- ValueNone

            /// Return the element at the point where we are in the list; if the iterator hasn'a been initialised then
            /// ValueOption.get will raise an exception
            member this.Current : 'a =
                this.currentHead |> ValueOption.get |> List.head

            member this.Current : obj =
                this.currentHead |> ValueOption.get |> List.head |> box

            member __.Dispose () = ()

    [<Struct>]
    type SkippingEnumerator<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private skip : int
        val mutable private enumerator : 'enumerator
        val mutable private skippingDone : bool

        new (skip : int, enumerator : 'enumerator) = {
            skip = skip
            enumerator = enumerator
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
    type TruncatingEnumerator<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private truncateTo : int
        val mutable private enumerator : 'enumerator
        val mutable private counter : int

        new (truncate : int, enumerator : 'enumerator) = {
            truncateTo = truncate
            enumerator = enumerator
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
    type PredicateEnumerator<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private predicate : 'a -> bool
        val mutable private enumerator : 'enumerator
        val mutable private moreItems : bool
        
        new (predicate : 'a -> bool, enumerator : 'enumerator) = {
            predicate = predicate
            enumerator = enumerator
            moreItems = true
        }
        
        interface 'a IEnumerator with
            member this.MoveNext() : bool =
                this.moreItems <- this.moreItems &&
                                  this.enumerator.MoveNext() &&
                                  this.predicate this.enumerator.Current
                this.moreItems
                    
            member this.Reset () : unit =
                    this.enumerator.Reset()
                    this.moreItems <- true
                    
            member this.Current : 'a = this.enumerator.Current

            member this.Current : obj = this.enumerator.Current |> box

            member __.Dispose () = ()
