namespace Peregrine.ValueTypes

open System.Collections.Generic

/// This module contains functions for operating on ValueSeqs, which are value-type enumerables
[<RequireQualifiedAccess>]
module ValueSeq =

    /// Count the number of element in the sequence
    [<CompiledName("Count")>]
    val count<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        source : 'enumerable ->
        int

    /// If the sequence is non-empty, get its first element, otherwise return ValueNone
    [<CompiledName("TryHead")>]
    val tryHead<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        source : 'enumerable ->
        'a voption

    /// Iterate through the sequence, applying the given action to each element
    [<CompiledName("Iterate")>]
    val iter<'a, 'enumerable, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        action : ('a -> unit) ->
        source : 'enumerable ->
        unit

    /// Iterate through the ValueSeq, applying the given action to each element. The action is passed the element's index
    /// in addition to the element itself
    [<CompiledName("IterateIndexed")>]
    val iteri<'a, 'enumerable, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        action : (int -> 'a -> unit) ->
        source : 'enumerable ->
        unit

    /// Fold over the given ValueSeq
    [<CompiledName("Fold")>]
    val fold<'state, 'a, 'enumerable, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        folder : ('state -> 'a -> 'state) ->
        initState : 'state ->
        source : 'enumerable ->
        'state

    /// Returns a ValueSeq that contains only elements of the provided ValueSeq for which the passed in predicate
    /// returned true. In other words: elements up until the first element that returned false.
    [<CompiledName("TakeWhile")>]
    val takeWhile<'a, 'enumerable, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        predicate : ('a -> bool) ->
        source : 'enumerable ->
        Enumerables.PredicatedValueSeq<'a, 'enumerator, 'enumerable>
    
    /// Return a ValueSeq that ignores the first 'count' elements of the passed in ValueSeq.
    /// If the provided ValueSeq has less than 'count' elements, the empty sequence is reuturned.
    [<CompiledName("Skip")>]
    val skip<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        count : int ->
        source : 'enumerable ->
        Enumerables.SkippedValueSeq<'a, 'enumerator, 'enumerable>

    /// Return a ValueSeq that only iterates over the first 'count' elements of the passed in ValueSeq.
    /// If the provided ValueSeq has less than 'count' elements, then this is in effect a no-op.
    [<CompiledName("Truncate")>]
    val truncate<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        count : int ->
        source : 'enumerable ->
        Enumerables.TruncatedValueSeq<'a, 'enumerator, 'enumerable>
    
    /// Return a ValueSeq that contains the elements of the passed in ValueSeq but with the mapping function applied.
    [<CompiledName("Map")>]
    val map<'a, 'b, 'enumerable, 'enumerator
        when 'enumerable :> ValueSeq<'a, 'enumerator>
        and 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        :
        mapping : ('a -> 'b) ->
        source : 'enumerable ->
        Enumerables.MappedValueSeq<'a, 'b, 'enumerator, 'enumerable>
            
    /// Returns a ValueSeq enumerable backed by the given array
    [<CompiledName("OfArray")>]
    val ofArray<'a> :
        array : 'a array ->
        'a Enumerables.ArrayValueSeq

    /// Returns a ValueSeq enumerable backed by the given list
    [<CompiledName("OfList")>]
    val ofList<'a> :
        list : 'a list ->
        'a Enumerables.ListValueSeq

    /// Wrap up the value-type sequence as a regular seq enumerable
    /// Note: this definitely allocates when used
    [<CompiledName("ToSeq")>]
    val toSeq<'enumerable, 'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        :
        source : 'enumerable ->
        'a seq



