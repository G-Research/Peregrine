namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerables =

    [<Struct>]    
    type ArrayValueSeq<'a> (array : 'a array) =
        interface ValueSeq<'a, Enumerators.Array<'a>> with
            member this.GetEnumerator () = new Enumerators.Array<'a>(array)

    [<Struct>]    
    type ListValueSeq<'a> (list : 'a list) =
        interface ValueSeq<'a, Enumerators.List<'a>> with
            member this.GetEnumerator () = new Enumerators.List<'a>(list)
    
    [<Struct>]
    type SkippedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        (count : int, enumerable : 'enumerable)
        =
        interface ValueSeq<'a, Enumerators.Skipping<'a, 'enumerator>> with
            member this.GetEnumerator () =
                let enumerator = enumerable.GetEnumerator ()
                new Enumerators.Skipping<'a, _>(count, enumerator)
    
    [<Struct>]
    type TruncatedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        (count : int, enumerable : 'enumerable)
        =
        interface ValueSeq<'a, Enumerators.Truncating<'a, 'enumerator>> with
            member this.GetEnumerator () =
                let enumerator = enumerable.GetEnumerator ()
                new Enumerators.Truncating<'a, _>(count, enumerator)

    [<Struct>]
    type PredicatedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        (predicate : 'a -> bool, enumerable : 'enumerable)
        =
        interface ValueSeq<'a, Enumerators.Predicated<'a, 'enumerator>> with
            member this.GetEnumerator () =
                let enumerator = enumerable.GetEnumerator()
                new Enumerators.Predicated<'a, _>(predicate, enumerator)

    [<Struct>]
    type MappedValueSeq<'a, 'b, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        (mapping : 'a -> 'b, enumerable : 'enumerable)
        =
        interface ValueSeq<'b, Enumerators.Mapped<'a, 'b, 'enumerator>> with
            member this.GetEnumerator () =
                let enumerator = enumerable.GetEnumerator()
                new Enumerators.Mapped<'a, 'b, _>(mapping, enumerator)