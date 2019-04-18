namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module ValueEnumerators =

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
