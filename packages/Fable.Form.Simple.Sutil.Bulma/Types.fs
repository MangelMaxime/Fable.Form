namespace Fable.Form.Simple.Sutil.Bulma

open Sutil.Core
open Fable.Form
open Fable.Form.Simple

/// <summary>
/// DUs used to represents the different of Field supported by Fable.Form.Studio
/// </summary>
type IField<'Values> =

    abstract MapFieldValues: update: ('Values -> 'NewValues) -> IField<'NewValues>
// update: ('Values -> 'NewValues -> 'NewValues) -> values: 'NewValues -> IField<'NewValues>

and FilledField<'Values> = Base.FilledField<IField<'Values>>

/// <summary>
/// Represents a form using Fable.Form.Studio representation
/// </summary>
type Form<'Values, 'Output> = Base.Form<'Values, 'Output, IField<'Values>>

[<NoComparison; NoEquality>]
type StandardRenderFieldConfig<'Value, 'Attributes> =
    {
        OnChange: 'Value -> unit
        OnBlur: (unit -> unit) option
        Disabled: bool
        IsReadOnly: bool
        Value: 'Value
        Error: Error.Error option
        ShowError: bool
        Attributes: 'Attributes
    }

(***

Fable cannot type test against interfaces, so we need to use abstract classes.

However, when type testing against classes:

```fs
match field.State with
| :? IStandardField<'Values, 'Value, 'Attributes> as standardField ->
    // Do something with standardField

| :? IGenericField<'Values> as genericField ->
    // Do something with genericField

| _ ->
    // Invalid type
```

Fable generate a warning 'Generic args are ignored in type testing'.
Unfortunately, we cannot suppress this warning, and it is emitted once per compilation.

This means that any Fable.Form.Simple.Bulma user will see this warning...

To work around this issue, it possible to dynimcally cast the instance:

```fs
try
    let standardField = field.State :?> IStandardField<'Values, 'Value, 'Attributes>

    let attributes = standardField.InnerField.Attributes

    let config = ...

    standardField.RenderField config

with _ ->
    try
        let genericField = field.State :?> IGenericField<'Values>

        genericField.RenderField dispatch fieldConfig field

    with _ ->
        // Invalid type
```

The code above will not generate any warning, and works if we call an API that is specific to
the casted type.

In the case above, `InnerField` only exists in `IStandardField`, so if we get an instance of `IGenericField`,
we will get an exception which then have a chance to handle, and so on.

However, I am not a fan of this approach, as it feels dirty to use exceptions for control flow.

The solution I found, is to use a DU that we get access to via implementing a class `IRendererField`.

And then, we make the abstract class `IStandardField` and `IGenericField` inherit from `IRendererField`
and specify the type of field renderer they are.

This makes the code feel cleaner, emit no warning and closer to type testing against classes/interfaces.

```fsharp
try
    let rendererField = field.State :?> IRendererField

    match rendererField.FieldRendererType with
    | FieldRendererType.Standard ->

        let standardField = field.State :?> IStandardField<'Values, 'Value, 'Attributes>

        let attributes = standardField.InnerField.Attributes

        let config = ...

        standardField.RenderField config

    | FieldRendererType.Generic ->
        let genericField = field.State :?> IGenericField<'Values>

        genericField.RenderField dispatch fieldConfig field

with _ ->
    failwith "Field not implemented, please implement the field `IStandardField<'Values>` or `IGenericField<'Values>`"
```
*)

[<RequireQualifiedAccess>]
[<Fable.Core.EraseAttribute>]
type FieldRendererType =
    | Standard
    | Generic

type IRendererField(rendererType) =
    member _.FieldRendererType: FieldRendererType = rendererType

[<AbstractClass>]
type IStandardField<'Values, 'Value, 'Attributes when 'Attributes :> Field.IAttributes>
    (innerField: Field.Field<'Attributes, 'Value, 'Values>)
    =
    inherit IRendererField(FieldRendererType.Standard)

    abstract RenderField: StandardRenderFieldConfig<'Value, 'Attributes> -> SutilElement

    member _.InnerField: Field.Field<'Attributes, 'Value, 'Values> = innerField

[<AbstractClass>]
type IGenericField<'Values>() =
    inherit IRendererField(FieldRendererType.Generic)

    abstract RenderField:
        Form.View.FieldConfig<'Values, 'Msg> -> FilledField<'Values> -> SutilElement
