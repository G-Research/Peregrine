namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerables =

    [<Struct>]
    type SkippedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        =
        new : int * 'enumerable -> SkippedValueSeq<'a, 'enumerator, 'enumerable>
        interface ValueSeq<'a, Enumerators.SkippingEnumerator<'a, 'enumerator>>
    
    [<Struct>]
    type TruncatedValueSeq<'a, 'enumerator, 'enumerable
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct
        and 'enumerable :> ValueSeq<'a, 'enumerator>>
        =
        new : int * 'enumerable -> TruncatedValueSeq<'a, 'enumerator, 'enumerable>
        interface ValueSeq<'a, Enumerators.TruncatingEnumerator<'a, 'enumerator>>
        