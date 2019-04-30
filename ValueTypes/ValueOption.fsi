namespace Peregrine.ValueTypes

open System

/// Functions for manipulating value-type options that use the built-in ValueOption
/// These exist because the F# core library lacks these standard functions
[<RequireQualifiedAccess>]
module ValueOption =

    /// Get the value of a 'ValueSome' option. An InvalidOperationException is raised if the option is a 'ValueNone'
    [<CompiledName("Get")>]
    val get<'a> : 'a voption -> 'a

    /// Given a System.Nullable convert it into a ValueOption
    [<CompiledName("OfNullable")>]
    val ofNullable<'a when 'a : (new : unit -> 'a) and 'a :> ValueType and 'a : struct> : 'a Nullable -> 'a voption

    /// Given a ValueOption convert it to a System.Nullable
    [<CompiledName("ToNullable")>]
    val toNullable<'a when 'a : (new : unit -> 'a) and 'a :> ValueType and 'a : struct> : 'a voption -> 'a Nullable

    /// Get the value of this ValueOption, or the given default value if it is a ValueNone
    [<CompiledName("DefaultValue")>]
    val defaultValue<'a> : 'a -> 'a voption -> 'a

    /// Apply the mapping to the optional value; return ValueNone if there is no value
    [<CompiledName("Map")>]
    val map<'a, 'b> : ('a -> 'b) -> 'a voption -> 'b voption