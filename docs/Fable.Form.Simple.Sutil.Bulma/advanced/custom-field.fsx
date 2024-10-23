(**
---
layout: standard
title: Create a Field
---
**)

(**

`Fable.Form.Simple.Sutil.Bulma` allows you to create your own custom fields.

For example, you can create a `SignatureField`, `AutoCompleteField`, `ColorField`, etc.

[Click here to see a `SignatureField` in action.](/Fable.Form/examples/sutil/index.html#custom-field)

## Different types of fields

2 types of fields are available:

- `IStandardField` - most of the fields will implement this interface which
provides a ready to use configuration for the rendering

    Examples:

    - TextField
    - CheckboxField
    - SelectField

- `IGenericField` - low level API for when you don't need the standard configuration
or want more control over the logic

    Examples:

    - List
    - Section
    - Group

We are going to focus on `IStandardField` in this example, but you can look at the source code
to learn more about `IGenericField`.

## Step by step guide

For this guide, we are going to create a standard text input field.

<ul class="textual-steps">
<li>

Create a new file named `MyTextField.fs`.

</li>
<li>

Create the namespace for hosting your custom field and open the required modules

> I am using a namespace because it allows it to be used in multiple files
> and I like to use `[<RequireQualifiedAccess>]` on some modules to control/improve the
> user experience at some places.
>
> Feel free to experiment with different approaches and see what works best for you.
*)

(*** hide ***)

#r "nuget: Sutil"
#r "./../../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.dll"
#r "./../../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.dll"
#r "./../../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Fields.Html.dll"
#r "./../../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Sutil.Bulma.dll"

(**
*)

namespace MyForm.Fields

open System
open Sutil
open Fable.Form
open Sutil.Bulma
open Browser
open Browser.Types
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma.Helpers.Focus

module MyTextField =

    (**
</li>
<li>

Create an `Attributes` type to hold the configuration of the field, it needs to
implements `Field.IAttributes`.

> Here I am using a record, but you can use a class or interface if you prefer.
*)

    type Attributes =
        {
            FieldId: string
            Label: string
            Placeholder: string option
        }

        interface Field.IAttributes with

            member this.GetFieldId() = this.FieldId

    (**
</li>
<li>

Define the inner field type which will be used to create the field.

Here we say that the field will use our `Attributes` type to pass additional configuration
and will use a `string` as the value type to represent the field value in the view.

*)

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    (**
</li>
<li>

Create the `form` function which will be used to create the field.

What is important here is that we pass the `isEmpty` function to the `Base.field` function
so this where we determine how to detect if the field is empty.
*)

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field String.IsNullOrEmpty

    (**
</li>
<li>

Actual implementation of the field, we inherit from `IStandardField` because we want to have access
to the `StandardRenderFieldConfig` which provides a ready to use configuration for the rendering.

:::info
This makes the code simpler because you don't need to worry about how to forward `OnChange`, `Value`, etc.
:::
*)
    type Field<'Values>(innerField: InnerField<'Values>) =
        inherit IStandardField<'Values, string, Attributes>(innerField)

        (**
</li>
<li>

Interface with `IField` to be able to map the field values most of the time you will just copy this code
*)
        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        (**
</li>
<li>

Override the `RenderField` method to provide the rendering logic for the field.

For a standard field, you need to handle all the following:

- `Attributes: 'Attributes` - Any view related configuration you have in your attributes
- `Disabled: bool` - If the field is disabled
- `Error: option<Error.Error>` - If the field has an error
- `IsReadOnly: bool` - If the field is read-only
- `OnBlur: option<(unit -> unit)>` - Event to call when the field loses focus
- `OnChange: 'Value -> unit` - Event to call when the field value changes
- `ShowError: bool` - If the field should show an error
- `Value: 'Value` - The value of the field

:::info
The `Helpers.View` module provides several helper functions to make it easier to render the field.

It is encourage to have a look at this module or how other fields are implemented to get a better understanding.
:::

:::danger{title="IMPORTANT"}
Because of how Sutil 2 works, we need to handle the focus and selection manually.

This is done by saving the focused field and the selection when the field is unmounted and
restoring it when the field is mounted.

This will be fixed in Sutil 3.
:::
*)
        override _.RenderField(config: StandardRenderFieldConfig<string, Attributes>) =

            bulma.input.text [
                Ev.onMount (fun ev ->
                    let input = (ev.target :?> HTMLInputElement)

                    if FocusedField.Instance.fieldId = config.Attributes.FieldId then
                        FocusedField.Instance.fieldId <- ""
                        input.focus ()

                        FocusedField.Instance.selection
                        |> Option.iter (fun (selectionStart, selectionEnd) ->
                            input.selectionStart <- selectionStart
                            input.selectionEnd <- selectionEnd
                        )
                )

                Ev.onUnmount (fun ev ->
                    let input = (ev.target :?> HTMLInputElement)

                    if document.activeElement = input then
                        FocusedField.SaveFocused(
                            config.Attributes.FieldId,
                            input.selectionStart,
                            input.selectionEnd
                        )
                )

                Ev.onChange config.OnChange

                match config.OnBlur with
                | Some onBlur -> Ev.onBlur (fun _ -> onBlur ())
                | None -> ()

                prop.disabled config.Disabled
                prop.readOnly config.IsReadOnly
                prop.value config.Value

                if config.ShowError && config.Error.IsSome then
                    color.isDanger

                match config.Attributes.Placeholder with
                | Some placeholder -> prop.placeholder placeholder
                | None -> ()
            ]
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error

(**
</li>
<li>

We could stop here, but let's add some helper functions to make it easier to create the field.

Indeed, if your `Attributes` add 10 properties with a majority of them being optional,
it can be a bit cumbersome to create the field.

For Fable.Form, I decided to go with a pipeline builder style because it is easy to read and
intellisense is well supported on all IDE.
*)

type MyTextField =

    static member create(fieldId: string) : MyTextField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
        }

    static member withLabel (label: string) (attributes: MyTextField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: MyTextField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

(**
</li>
<li>

In another field named `Form.fs`, create the field which will be used by the user to create
an instance of the custom field.
*)

namespace MyForm

open Fable.Form
open MyForm.Fields
open Fable.Form.Simple.Sutil.Bulma

[<RequireQualifiedAccess>]
module Form =

    let myTextField
        (config: Base.FieldConfig<MyTextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        MyTextField.form (fun field -> MyTextField.Field field) config

(**

Like with any other field, the user will use `Form.myTextField` to create an instance of the custom field.

*)

(*** hide ***)

module Usage =

    type Values =
        {
            Email: string
        }

    (**
    *)

    open Fable.Form.Simple
    open Fable.Form.Simple.Sutil.Bulma
    open Fable.Form.Simple.Fields.Html
    open MyForm

    Form.myTextField
        {
            Parser = Ok
            Value = fun values -> values.Email
            Update =
                fun newValue values ->
                    { values with
                        Email = newValue
                    }
            Error = fun _ -> None
            Attributes =
                MyTextField.create "email"
                |> MyTextField.withLabel "Email"
                |> MyTextField.withPlaceholder "some@email.com"
        }

(**

</li>
</ul>

You now know how to create a custom field for `Fable.Form.Simple.Sutil.Bulma`.

Like mentionned earlier, a field can be anything you want, so feel free to implement
any field you need and share it with the community.
*)
