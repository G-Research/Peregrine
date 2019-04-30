namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module ValueEnumerators =

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
                | ValueSome (x :: xs) -> this.currentHead <- ValueSome xs // move forward one
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
