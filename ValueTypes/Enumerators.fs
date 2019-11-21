namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerators =

    [<Struct>]
    type 'a Array =
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
    type 'a List =
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
    type Skipping<'a, 'enumerator
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
    type Truncating<'a, 'enumerator
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
    type Predicated<'a, 'enumerator
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

    [<Struct>]
    type Mapped<'a, 'b, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private mapping : 'a -> 'b
        val mutable private enumerator : 'enumerator
        
        new (mapping : 'a -> 'b, enumerator : 'enumerator) = {
            mapping = mapping
            enumerator = enumerator
        }
        
        interface 'b IEnumerator with
            member this.MoveNext() : bool = this.enumerator.MoveNext()
                
            member this.Reset () : unit = this.enumerator.Reset()
                
            member this.Current : 'b = this.enumerator.Current |> this.mapping
            
            member this.Current : obj = this.enumerator.Current |> this.mapping |> box
            
            member this.Dispose () = this.enumerator.Dispose()
    
    [<Struct>]
    type MapIndexed<'a, 'b, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private mapping : int -> 'a -> 'b
        val mutable private enumerator : 'enumerator
        val mutable index : int
        
        new (mapping : int -> 'a -> 'b, enumerator : 'enumerator) = {
            mapping = mapping
            enumerator = enumerator
            index = -1
        }
        
        interface 'b IEnumerator with
            member this.MoveNext () : bool =
                if this.enumerator.MoveNext () then
                    this.index <- this.index + 1
                    true
                else
                    false
            
            member this.Reset () : unit =
                this.enumerator.Reset ()
                this.index <- -1
            
            member this.Current : 'b =
                this.mapping this.index this.enumerator.Current
            
            member this.Current : obj =
                this.mapping this.index this.enumerator.Current |> box
            
            member this.Dispose () =
                this.enumerator.Dispose ()
    
    [<Struct>]
    type Scanned<'a, 'state, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private folder : 'state -> 'a -> 'state
        val private init : 'state
        val mutable private enumerator : 'enumerator
        val mutable private state : 'state ValueOption

        new (folder : 'state -> 'a -> 'state, init : 'state, enumerator : 'enumerator) = {
            folder = folder
            init = init
            enumerator = enumerator
            state = ValueNone
        }

        interface 'state IEnumerator with
            member this.MoveNext() : bool =
                match this.state with
                | ValueNone ->
                    this.state <- ValueSome this.init
                    true
                | ValueSome state ->
                    if this.enumerator.MoveNext() then
                        this.state <- ValueSome (this.folder state this.enumerator.Current)
                        true
                    else
                        false

            member this.Reset() : unit =
                this.state <- ValueNone
                this.enumerator.Reset()

            member this.Current : 'state = this.state.Value

            member this.Current : obj = this.state.Value |> box

            member this.Dispose () = this.enumerator.Dispose()
