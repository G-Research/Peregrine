namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module ValueSeq =

    /// Given a ValueSeq get an iterator for it.
    /// This is defined as a concrete type that implements IEnumerator<T> and as such, no boxing occurs. Note that since
    /// this returns a value-type in order to use the IEnumerator<T> methods it must be stored in a mutable variable, e.g.:
    /// let mutable iter = source |> getIterator
    let private getIterator (enumerable : #ValueSeq<'a,_>) : #IEnumerator<'a> =
        enumerable.GetEnumerator ()

    [<CompiledName("Count")>]
    let count (source : #ValueSeq<_,_>) : int =
        let mutable iter = source |> getIterator
        let mutable counter = 0

        while iter.MoveNext() do
            counter <- counter + 1
        counter

    [<CompiledName("TryHead")>]
    let tryHead (source : #ValueSeq<'a,_>) : 'a voption =
        let mutable iter = source |> getIterator
        if iter.MoveNext () then ValueSome iter.Current else ValueNone

    [<CompiledName("Iterate")>]
    let iter (action : 'a -> unit) (source : #ValueSeq<'a,_>) : unit =
        let mutable iter = source |> getIterator
        while iter.MoveNext() do iter.Current |> action

    [<CompiledName("IterateIndexed")>]
    let iteri (action : int -> 'a -> unit) (source : #ValueSeq<'a,_>) : unit =
        let mutable iter = source |> getIterator
        let mutable index = -1
        while iter.MoveNext() do
            index <- index + 1
            action index iter.Current

    [<CompiledName("Fold")>]
    let fold
        (folder : 'state -> 'a -> 'state)
        (state : 'state)
        (source : #ValueSeq<'a, 'enumerator>)
        : 'state
        =
        let mutable currentState = state
        let mutable iter = source |> getIterator

        while iter.MoveNext() do
            currentState <- folder currentState iter.Current
        currentState

    [<CompiledName("TakeWhile")>]
    let takeWhile
        (predicate : 'a -> bool)
        (source : #ValueSeq<'a, 'enumerator>)
        : Enumerables.PredicatedValueSeq<_,_,_>
        =
        Enumerables.PredicatedValueSeq(predicate, source)
    
    [<CompiledName("Skip")>]
    let skip
        (count : int)
        (source : #ValueSeq<'a, 'enumerator>)
        : Enumerables.SkippedValueSeq<_,_,_>
        =
        Enumerables.SkippedValueSeq(count, source)
    
    [<CompiledName("Truncate")>]
    let truncate
        (count : int)
        (source : #ValueSeq<'a,_>)
        : Enumerables.TruncatedValueSeq<_,_,_>
        =
        Enumerables.TruncatedValueSeq(count, source)
    
    [<CompiledName("Map")>]
    let map
        (mapping : 'a -> 'b)
        (source : #ValueSeq<'a,_>)
        : Enumerables.MappedValueSeq<_,_,_,_>
        =
        Enumerables.MappedValueSeq(mapping, source)
    
    [<CompiledName("OfArray")>]
    let ofArray (array : 'a array) =
        Enumerables.ArrayValueSeq(array)

    [<CompiledName("OfList")>]
    let ofList (list : 'a list) =
        Enumerables.ListValueSeq(list)

    [<CompiledName("ToSeq")>]
    let toSeq (source : #ValueSeq<'a, _>) : 'a seq =
        seq {
            let mutable iter = source |> getIterator
            while iter.MoveNext() do yield iter.Current
        }
