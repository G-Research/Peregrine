namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerables =

    [<Struct; NoEquality; NoComparison>]
    type 'a ArrayValueSeq =
        new : 'a array -> 'a ArrayValueSeq
        interface ValueSeq<'a, 'a Enumerators.Array>
    
    [<Struct; NoEquality; NoComparison>]
    type 'a ListValueSeq =
        new : 'a list -> 'a ListValueSeq
        interface ValueSeq<'a, 'a Enumerators.List>
    
    [<Struct; NoEquality; NoComparison>]
    type SkippedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        =
        new : int * 'enumerable -> SkippedValueSeq<'a, 'enumerator, 'enumerable>
        interface ValueSeq<'a, Enumerators.Skipping<'a, 'enumerator>>
    
    [<Struct; NoEquality; NoComparison>]
    type TruncatedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        =
        new : int * 'enumerable -> TruncatedValueSeq<'a, 'enumerator, 'enumerable>
        interface ValueSeq<'a, Enumerators.Truncating<'a, 'enumerator>>
        
    [<Struct; NoEquality; NoComparison>]
    type PredicatedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        =
        new : ('a -> bool) * 'enumerable -> PredicatedValueSeq<'a, 'enumerator, 'enumerable>
        interface ValueSeq<'a, Enumerators.Predicated<'a, 'enumerator>>

    [<Struct; NoEquality; NoComparison>]
    type MappedValueSeq<'a, 'b, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        =
        new : ('a -> 'b) * 'enumerable -> MappedValueSeq<'a, 'b, 'enumerator, 'enumerable>
        interface ValueSeq<'b, Enumerators.Mapped<'a, 'b, 'enumerator>>
