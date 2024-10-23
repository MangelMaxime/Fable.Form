(**
---
layout: standard
title: Customize the View
toc: false
---
**)

(*** hide ***)

(**

It can happen that a field has the right logic for you, but you are using a different CSS framework.

In this case, you can customize the view of the field to match your CSS framework
without having to rewrite all the code.

## Step by step guide

For example, it allows you to re-use the `Attributes`, and its pipeline builder API, etc.

*)

(*** hide ***)

#r "nuget: Feliz"
#r "nuget: Feliz.Bulma"
#r "./../../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.dll"
#r "./../../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.dll"
#r "./../../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Fields.Html.dll"
#r "./../../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Bulma.dll"

(**

In this example, we are going to customize the view of a text field.

<ul class="textual-steps">
<li>

Create a new file named `TextField.fs`.

</li>
<li>

Create the namespace for hosting your custom field and open the required modules

*)
namespace MyForm.Fields

open Fable.Form
open Feliz
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma.Fields

module TextField =

    (**
</li>
<li>

Create a new class which inherits from `TextField.Field<'Values>`, and override the `RenderField` method.

:::info
The example below is truncated for brevity.

Make you sure you handle all the properties of the `config` parameter in your implementation.
:::
*)

    type Field<'Values>(innerField: TextField.InnerField<'Values>) =

        inherit TextField.Field<'Values>(innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, TextField.Attributes>) =
            Html.div [
                Html.p config.Attributes.Label

                Html.input [
                    prop.onChange config.OnChange

                    // Handle the different properties

                    prop.disabled config.Disabled
                    prop.readOnly config.IsReadOnly
                    prop.value config.Value
                ]

                match config.Error with
                | Some(Error.External externalError) -> Html.p externalError
                | _ ->
                    if config.ShowError then
                        config.Error
                        |> Option.map Form.View.errorToString
                        |> Option.defaultValue ""
                        |> Html.p
            ]

(**
</li>
<li>

In another field named `Form.fs`, create the field which will be used by the user to create
an instance of the custom field.

*)
namespace MyForm

open Fable.Form
open MyForm.Fields
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma.Fields

[<RequireQualifiedAccess>]
module Form =

    let textField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> TextField.Field field) config

(**

:::info
Using the same name as the original field allows you to shadow the original field.

So if you open your namespace after `Fable.Form.Simple.Bulma`, you will use your custom field
instead of the original one and don't have to change anything in your code.
:::
*)
