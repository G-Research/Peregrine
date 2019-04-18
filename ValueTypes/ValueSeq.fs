namespace Peregrine.ValueTypes

open System.Collections.Generic

/// For an enumerable whose enumerator is a struct (to avoid allocation), by implementing this interface, consuming
/// code can be written to take any enumerable without allocating memory due to boxing the struct into an IEnumerator.
///
/// The F# and C# compilers can take code that uses IEnumerable and emit code that doesn't allocate (see e.g. the
/// 'foreach' keyword in C#). Unfortunately we cannot enforce usage of IEnumerable to these cases so this interface
/// exists to explicitly call out the places where we care about not allocating heap objects, and allows us to avoid it.
type ValueSeq<'a, 'enumerator
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct>
    =
    abstract member GetEnumerator : unit -> 'enumerator

[<RequireQualifiedAccess>]
module ValueSeq =

    /// Given a ValueSeq get an iterator for it.
    /// This is defined as a concrete type that implements IEnumerator<T> and as such, no boxing occurs. Note that since
    /// this returns a value-type in order to use the IEnumerator<T> methods it must be stored in a mutable variable, e.g.:
    /// let mutable iter = source |> getIterator
    let private getIterator (enumerable : #ValueSeq<'a,_>) : #IEnumerator<'a> =
        enumerable.GetEnumerator ()

    [<CompiledName("Count")>]
    let count (source : ValueSeq<_,_>) : int =
        let mutable iter = source |> getIterator
        let mutable counter = 0

        while iter.MoveNext() do
            counter <- counter + 1
        counter

    [<CompiledName("TryHead")>]
    let tryHead (source : ValueSeq<'a,_>) : 'a voption =
        let mutable iter = source |> getIterator
        if iter.MoveNext () then ValueSome iter.Current else ValueNone

    [<CompiledName("Iter")>]
    let iter (action : 'a -> unit) (source : ValueSeq<'a,_>) : unit =
        let mutable iter = source |> getIterator
        while iter.MoveNext() do iter.Current |> action

    [<CompiledName("Iteri")>]
    let iteri (action : int -> 'a -> unit) (source : ValueSeq<'a,_>) : unit =
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

    [<CompiledName("Foldi")>]
    let foldi (folder : 'state -> int -> 'a -> 'state) (state : 'state) (source : ValueSeq<'a,_>) : 'state =
        let mutable iter = source |> getIterator
        let mutable currentState = state
        let mutable index = -1

        while iter.MoveNext() do
            index <- index + 1
            currentState <- folder currentState index iter.Current
        currentState

    [<CompiledName("Foldn")>]
    let foldn (n : int) (folder : 'state -> 'a -> 'state) (state: 'state) (source : ValueSeq<'a,_>) : 'state =
        let mutable iter = source |> getIterator
        let mutable currentState = state
        let mutable counter = 0

        while counter < n && iter.MoveNext() do
            counter <- counter + 1
            currentState <- folder currentState iter.Current
        currentState

    [<CompiledName("FoldFromN")>]
    let foldFromN (n : int) (folder : 'state -> 'a -> 'state) (state: 'state) (source : ValueSeq<'a,_>) : 'state =
        let mutable iter = source |> getIterator
        let mutable currentState = state
        let mutable counter = 0

        // Iterate until at the (n-1)th element
        while counter < (n-1) && iter.MoveNext() do
            counter <- counter + 1

        // Now fold until there are no more elements
        while iter.MoveNext() do
            currentState <- folder currentState iter.Current
        currentState

    [<CompiledName("FoldWhile")>]
    let foldWhile
        (predicate : 'state -> 'a -> bool)
        (folder : 'state -> 'a -> 'state)
        (state: 'state)
        (source : ValueSeq<'a,_>) :
        'state
        =
        let mutable iter = source |> getIterator
        let mutable currentState = state

        while iter.MoveNext() && (predicate currentState iter.Current) do
            currentState <- folder currentState iter.Current
        currentState


    type ArrayEnumerable<'a> (array : 'a array) =
        interface ValueSeq<'a, ValueEnumerators.ArrayEnumerator<'a>> with
            member this.GetEnumerator () = new ValueEnumerators.ArrayEnumerator<'a>(array)

    [<CompiledName("OfArray")>]
    let ofArray (array : 'a array) : ValueSeq<'a,_>=
        array |> ArrayEnumerable :> _


    type ListEnumerable<'a> (list : 'a list) =
        interface ValueSeq<'a, ValueEnumerators.ListEnumerator<'a>> with
            member this.GetEnumerator () = new ValueEnumerators.ListEnumerator<'a>(list)

    [<CompiledName("OfList")>]
    let ofList (list : 'a list) : ValueSeq<'a,_> =
        list |> ListEnumerable :> _

    [<CompiledName("ToSeq")>]
    let toSeq (source : ValueSeq<'a,'enumerator>) : 'a seq =
        seq {
            let mutable iter = source |> getIterator
            while iter.MoveNext() do yield iter.Current
        }
