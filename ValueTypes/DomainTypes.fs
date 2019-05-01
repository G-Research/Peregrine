namespace Peregrine.ValueTypes

open System.Collections.Generic

/// For an enumerable whose enumerator is a struct (to avoid allocation), by implementing this interface, consuming
/// code can be written to take any enumerable without allocating memory due to boxing the struct into an IEnumerator.
///
/// The F# and C# compilers can take code that uses IEnumerable and emit code that doesn't allocate (see e.g. the
/// 'foreach' keyword in C#). Unfortunately we cannot enforce usage of IEnumerable to these cases so this interface
/// exists to explicitly call out the places where we care about not allocating heap objects, and allows us to avoid it.
type ValueSeq<'a, 'enumerator
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct>
    =
    abstract member GetEnumerator : unit -> 'enumerator


