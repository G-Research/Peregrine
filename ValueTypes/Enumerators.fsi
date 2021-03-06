namespace Peregrine.ValueTypes

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Enumerators =

    /// Value-type enumerator for an array
    [<Struct>]
    type 'a Array =
        val private array : 'a array
        val mutable private currentIndex : int
        new : 'a array -> 'a Array
        interface 'a IEnumerator

    /// Value-type enumerator for a list
    [<Struct>]
    type 'a List =
        val private list : 'a list
        val mutable private currentHead : 'a list voption
        new : 'a list -> 'a List
        interface 'a IEnumerator

    /// Value-type enumerator that skips the first n elements of another value-type enumerator
    [<Struct>]
    type Skipping<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private skip : int
        val mutable private enumerator : 'enumerator
        val mutable private skippingDone : bool
        new : int * 'enumerator -> Skipping<'a, 'enumerator>
        interface 'a IEnumerator
    
    /// Value-type enumerator that truncates to the first n elements of another value-type enumerator    
    [<Struct>]
    type Truncating<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private truncateTo : int
        val mutable private enumerator : 'enumerator
        val mutable private counter : int
        new : int * 'enumerator -> Truncating<'a, 'enumerator>
        interface 'a IEnumerator
        
    /// Value-type enumerator that takes elements of another value-type enumerator as long as the provided predicate
    /// function returns true
    [<Struct>]
    type Predicated<'a, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private predicate : 'a -> bool
        val mutable private enumerator : 'enumerator
        val mutable private moreItems : bool
        new : ('a -> bool) * 'enumerator -> Predicated<'a, 'enumerator>
        interface 'a IEnumerator
        
    /// Value-type enumerator that transforms the elements of another value-type enumerator according to the given
    /// mapping function.
    [<Struct>]
    type Mapped<'a, 'b, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private mapping : 'a -> 'b
        val mutable private enumerator : 'enumerator
        new : ('a -> 'b) * 'enumerator -> Mapped<'a, 'b, 'enumerator>
        interface 'b IEnumerator

    /// Value-type enumerator that transforms the elements of another value-type enumerator according to the given
    /// mapping function and the elements' indices.
    [<Struct>]
    type MapIndexed<'a, 'b, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private mapping : int -> 'a -> 'b
        val mutable private enumerator : 'enumerator
        val mutable private index : int
        new : (int -> 'a -> 'b) * 'enumerator -> MapIndexed<'a, 'b, 'enumerator>
        interface 'b IEnumerator
    
    /// Value-type enumerator that folds over another value-type enumerator and gives a sequence of the folder state
    [<Struct>]
    type Scanned<'a, 'state, 'enumerator
        when 'enumerator :> IEnumerator<'a>
        and 'enumerator : struct>
        =
        val private folder : 'state -> 'a -> 'state
        val private init : 'state
        val mutable private enumerator : 'enumerator
        val mutable private state : 'state ValueOption
        new : ('state -> 'a -> 'state) * 'state * 'enumerator -> Scanned<'a, 'state, 'enumerator>
        interface 'state IEnumerator