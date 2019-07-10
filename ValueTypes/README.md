# Peregrine ValueTypes

[![Version](https://img.shields.io/nuget/v/PeregrineValueTypes.svg?label=NuGet)](https://www.nuget.org/packages/PeregrineValueTypes)

This library, which is part of the Peregrine series of code for real-time and performant applications, contains F# functions for manipulating data structures in ways that don't allocate. It tests this fact by using BenchmarkDotNet to assert that the number of allocations made by its functions are zero.

The core modules of this library are:
* `ValueOption` - provides functions similar to those in the `Option` module for working with the `ValueOption` struct.
* `ValueSeq` - provides `Seq`-like functions that can operate on enumerables without allocating through boxing.

## ValueOption

`FSharp.Core` as of versions 4.5.X provides a type called `ValueOption` (aliased as `voption`) that works with value types and is itself a value type. The functions available in the `ValueOption` module are very minimal, so the module in this library fills the hole.

It is intended that in the future, the core F# library will have these functions and so this module will not be required, but until then, or if working with older versions of the core library, it will be needed.

## ValueSeq

In .NET, collections that you can iterate over are called _enumerables_ and implement the following interface:
```
type IEnumerable<'a> = 
    abstract member GetEnumerator : unit -> IEnumerator<'a>
```
Every time you want to iterate over an enumerable the `GetEnumerator()` method is called, returning a new _enumerator_, which implements the interface `IEnumerator<'a>`. This causes code to allocate whenever you want to iterate over a collection because the call to `GetEnumerator()` creates a new object. One solution to this is to implement your `IEnumerator<'a>` as a value-type (i.e. a `struct` rather than a `class`), causing it to live on the stack rather than the heap. Unfortunately this isn't the complete solution as whenever your value-type enumerator is passed to a function, such as `Seq.fold` it is *boxed* into an `IEnumerator<'a>`, which causes an allocation.

Boxing can be avoided in F# by passing an argument to a function using a type parameter that is constrained to be derived from another type. This library uses that fact to define a `ValueSeq` type, which is an _enumerable_ that only works with value-type _enumerators_. It looks like this:
```
type ValueSeq<'enumerator, 'a
    when 'enumerator :> IEnumerator<'a>
    and 'enumerator : struct>
    =
    abstract member GetEnumerator : unit -> 'enumerator
```
By ensuring that the type returned by `GetEnumerator` is a concrete value-type and functions that use it reference it as a type parameter, the code remains allocation free.