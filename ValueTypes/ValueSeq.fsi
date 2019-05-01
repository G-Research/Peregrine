namespace Peregrine.ValueTypes

open System.Collections.Generic

/// This module contains functions for operating on ValueSeqs, which are value-type enumerables
[<RequireQualifiedAccess>]
module ValueSeq =

    /// Count the number of element in the sequence
    [<CompiledName("Count")>]
    val count<'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        source : ValueSeq<'a, 'enumerator>
        -> int

    /// If the sequence is non-empty, get its first element, otherwise return ValueNone
    [<CompiledName("TryHead")>]
    val tryHead<'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        source : ValueSeq<'a, 'enumerator>
        -> 'a voption

    /// Iterate through the sequence, applying the given action to each element
    [<CompiledName("Iter")>]
    val iter<'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        action : ('a -> unit) ->
        source : ValueSeq<'a, 'enumerator>
        -> unit

    /// Iterate through the ValueSeq, applying the given action to each element. The action is passed the element's index
    /// in addition to the element itself
    [<CompiledName("Iteri")>]
    val iteri<'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        action : (int -> 'a -> unit) -> ValueSeq<'a, 'enumerator> -> unit

    /// Fold over the given ValueSeq
    [<CompiledName("Fold")>]
    val fold<'state, 'a, 'enumerable, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>> :
        folder : ('state -> 'a -> 'state) ->
        initState : 'state ->
        source : 'enumerable
        -> 'state

    /// Fold over the given ValueSeq, passing in the current index to the folder function
    [<CompiledName("Foldi")>]
    val foldi<'state, 'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        folder : ('state -> int -> 'a -> 'state) ->
        initState : 'state ->
        source : ValueSeq<'a, 'enumerator>
        -> 'state

    /// Fold over at most n elements from the given ValueSeq.
    [<CompiledName("Foldn")>]
    val foldn<'state, 'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        n : int ->
        folder : ('state -> 'a -> 'state) ->
        initState : 'state ->
        source : ValueSeq<'a, 'enumerator>
        -> 'state

    /// Fold over the given ValueSeq, skipping over the first n-1 elements.
    /// If there are less than n elements then this is the same as folding over an empty ValueSeq.
    [<CompiledName("FoldFromN")>]
    val foldFromN<'state, 'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        n : int ->
        folder : ('state -> 'a -> 'state) ->
        initState : 'state ->
        source : ValueSeq<'a, 'enumerator>
        -> 'state

    /// Fold over the given ValueSeq, as long as the predicate function holds.
    /// The predicate function is evaluated *before* the folder.
    [<CompiledName("FoldWhile")>]
    val foldWhile<'state, 'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        predicate : ('state -> 'a -> bool) ->
        folder : ('state -> 'a -> 'state) ->
        initState : 'state ->
        source : ValueSeq<'a, 'enumerator>
        -> 'state

    [<CompiledName("TakeWhile")>]
    val takeWhile :
        predicate : ('a -> bool) ->
        source : #ValueSeq<'a, 'enumerator>
        -> #ValueSeq<'a, 'enumerator>
    
    /// Return a ValueSeq that ignores the first 'count' elements of the passed in ValueSeq.
    /// If the provided ValueSeq has less than 'count' elements, the empty sequence is reuturned.
    [<CompiledName("Skip")>]
    val skip<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>> :
        count : int ->
        source : 'enumerable
        -> Enumerables.SkippedValueSeq<'a, 'enumerator, 'enumerable>

    /// Return a ValueSeq only iterates over the first 'count' elements of the passed in ValueSeq.
    /// If the provided ValueSeq has less than 'count' elements, then this is in effect a no-op.
    [<CompiledName("Truncate")>]
    val truncate<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>> :
        count : int ->
        source : 'enumerable
        -> Enumerables.TruncatedValueSeq<'a, 'enumerator, 'enumerable>
    
    /// Returns a ValueSeq enumerable backed by the given array
    [<CompiledName("OfArray")>]
    val ofArray<'a> :
        array : 'a array
        -> ValueSeq<'a, Enumerators.ArrayEnumerator<'a>>

    /// Returns a ValueSeq enumerable backed by the given list
    [<CompiledName("OfList")>]
    val ofList<'a> :
        list : 'a list
        -> ValueSeq<'a, Enumerators.ListEnumerator<'a>>

    /// Wrap up the value-type sequence as a regular seq enumerable
    /// Note: this definitely allocates when used
    [<CompiledName("ToSeq")>]
    val toSeq<'a, 'enumerator when 'enumerator :> IEnumerator<'a> and 'enumerator : struct> :
        source : ValueSeq<'a, 'enumerator>
        -> 'a seq



