namespace Peregrine.ValueTypes

open System

[<RequireQualifiedAccess>]
module ValueOption =

    [<CompiledName("Get")>]
    let get (option : 'a voption) : 'a =
        option.Value

    [<CompiledName("OfNullable")>]
    let ofNullable (nullable : 'a Nullable) : 'a voption =
        if nullable.HasValue then ValueSome nullable.Value else ValueNone

    [<CompiledName("ToNullable")>]
    let toNullable (valueOption : 'a voption) : 'a Nullable =
        match valueOption with
        | ValueSome value -> Nullable value
        | ValueNone -> Nullable()

    [<CompiledName("DefaultValue")>]
    let defaultValue (value : 'a) (option : 'a voption) : 'a =
        match option with
        | ValueSome actualValue -> actualValue
        | ValueNone -> value

    [<CompiledName("Map")>]
    let map (mapping : 'a -> 'U) (option : 'a voption) : 'U voption =
        match option with
        | ValueSome value -> value |> mapping |> ValueSome
        | ValueNone -> ValueNone
