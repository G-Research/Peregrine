namespace Peregrine.ValueTypes

open System.Collections.Generic

/// An enumerable whose enumerator is a struct.
/// By defining the enumerator as a type parameter, we can
type ValueSeq<'a, 'enumerator
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct>
    =
    abstract member GetEnumerator : unit -> 'enumerator

