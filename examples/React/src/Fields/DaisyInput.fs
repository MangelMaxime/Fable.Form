namespace Fable.Form.Simple.Bulma.Fields.DaisyTextInput

open System
open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Bulma.Fields

module DaisyTextInput =

    // Re-usable helper functions for rendering common field elements
    module View =

        let fieldLabel (label: string) =
            Html.div [
                prop.className "label"
                prop.children [
                    Html.div [
                        prop.className "label-text"
                        prop.text label
                    ]
                ]
            ]

        let errorMessage (message: string) =
            Html.div [
                prop.className "label"
                prop.children [
                    Html.div [
                        prop.className "label-text-alt !text-red-500"
                        prop.text message
                    ]
                ]
            ]

        let errorMessageAsHtml (showError: bool) (error: Error.Error option) =
            match error with
            | Some(Error.External externalError) -> errorMessage externalError

            | _ ->
                if showError then
                    error
                    |> Option.map Form.View.errorToString
                    |> Option.map errorMessage
                    |> Option.defaultValue (Bulma.help [])

                else
                    Bulma.help []

        let wrapInFieldContainer (children: ReactElement list) =
            Html.div [
                prop.className "form-control w-full"
                prop.children children
            ]

        let withLabelAndError
            (label: string)
            (showError: bool)
            (error: Error.Error option)
            (fieldAsHtml: ReactElement)
            : ReactElement
            =
            [
                fieldLabel label
                fieldAsHtml
                errorMessageAsHtml showError error
            ]
            |> wrapInFieldContainer

    type Field<'Values>(innerField: TextField.InnerField<'Values>) =
        // For the demo, we only want to customize the rendering of raw text fields
        inherit TextField.Field<'Values>(TextField.TextRaw, innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, TextField.Attributes>) =
            let autoComplete =
                match config.Attributes.AutoComplete with
                | Some value -> value
                | None -> "off"

            Html.input [
                prop.className [
                    "input input-bordered w-full"
                    if config.ShowError && config.Error.IsSome then
                        "input-error"
                ]
                prop.onChange config.OnChange

                match config.OnBlur with
                | Some onBlur -> prop.onBlur (fun _ -> onBlur ())
                | None -> ()

                prop.disabled config.Disabled
                prop.readOnly config.IsReadOnly
                prop.value config.Value
                prop.placeholder config.Attributes.Placeholder

                prop.autoComplete autoComplete
            ]
            |> View.withLabelAndError config.Attributes.Label config.ShowError config.Error

// Expose the function that the user will use to create a signature field
// It follows the same pattern as the other fields in Fable.Form.Simple.Bulma (Form.xxxField)
[<RequireQualifiedAccess>]
module Form =

    let daysiTextInputField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output>
        =
        TextField.form (fun field -> DaisyTextInput.Field field) config
