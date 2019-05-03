namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerators =

    /// Value-type enumerator for an array
    [<Struct>]
    type 'a ArrayEnumerator =
        val private array : 'a array
        val mutable private currentIndex : int
        new : 'a array -> 'a ArrayEnumerator
        interface 'a IEnumerator

    /// Value-type enumerator for a list
    [<Struct>]
    type 'a ListEnumerator =
        val private list : 'a list
        val mutable private currentHead : 'a list voption
        new : 'a list -> 'a ListEnumerator
        interface 'a IEnumerator

    /// Value-type enumerator that skips the first n elements of another value-type enumerator
    [<Struct>]
    type SkippingEnumerator<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private skip : int
        val mutable private enumerator : 'enumerator
        val mutable private skippingDone : bool
        new : int * 'enumerator -> SkippingEnumerator<'a, 'enumerator>
        interface 'a IEnumerator
    
    /// Value-type enumerator that truncates to the first n elements of another value-type enumerator    
    [<Struct>]
    type TruncatingEnumerator<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private truncateTo : int
        val mutable private enumerator : 'enumerator
        val mutable private counter : int
        new : int * 'enumerator -> TruncatingEnumerator<'a, 'enumerator>
        interface 'a IEnumerator