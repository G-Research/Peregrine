namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerables =
    
    [<Struct>]
    type SkippedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        (count : int, enumerable : 'enumerable)
        =
        interface ValueSeq<'a, Enumerators.SkippingEnumerator<'a, 'enumerator>> with
            member this.GetEnumerator () =
                let enumerator = enumerable.GetEnumerator ()
                new Enumerators.SkippingEnumerator<'a, _>(count, enumerator)
    
    [<Struct>]
    type TruncatedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        (count : int, enumerable : 'enumerable)
        =
        interface ValueSeq<'a, Enumerators.TruncatingEnumerator<'a, 'enumerator>> with
            member this.GetEnumerator () =
                let enumerator = enumerable.GetEnumerator ()
                new Enumerators.TruncatingEnumerator<'a, _>(count, enumerator)

