namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Sutil.Bulma.Fields
open Fable.Form.Simple.Sutil.Bulma.Helpers.Focus
open Browser
open Browser.Types

module DaisyTextInput =

    // Re-usable helper functions for rendering common field elements
    module View =

        let fieldLabel (label: string) =
            Html.div [
                prop.className "tw-daisy-label"
                Html.div [
                    prop.className "tw-daisy-label-text"
                    prop.text label
                ]
            ]

        let errorMessage (message: string) =
            Html.div [
                prop.className "tw-daisy-label"
                Html.div [
                    prop.className "tw-daisy-label-text-alt !tw-text-red-500"
                    prop.text message
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
                    |> Option.defaultValue (
                        Html.div [
                            prop.className "tw-daisy-label"
                        ]
                    )

                else
                    Html.div [
                        prop.className "tw-daisy-label"
                    ]

        let wrapInFieldContainer (children: Core.SutilElement list) =
            Html.div [
                prop.className "tw-daisy-form-control w-full"
                yield! children
            ]

        let withLabelAndError
            (label: string)
            (showError: bool)
            (error: Error.Error option)
            (fieldAsHtml: Core.SutilElement)
            : Core.SutilElement
            =
            [
                fieldLabel label
                fieldAsHtml
                errorMessageAsHtml showError error
            ]
            |> wrapInFieldContainer

    type Field<'Values>(innerField: TextField.InnerField<'Values>) =
        // For the demo, we only want to customize the rendering of raw text fields
        inherit TextField.Field<'Values>(innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, TextField.Attributes>) =
            let autoComplete =
                match config.Attributes.AutoComplete with
                | Some value -> value
                | None -> "off"

            Html.input [
                prop.className [
                    "tw-daisy-input tw-daisy-input-bordered w-full"
                    if config.ShowError && config.Error.IsSome then
                        "tw-daisy-input-error"
                ]

                // Sutil 2 redraw the HTML on each update, so we need to save and restore
                // the focused field and selection manually
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

                Ev.onInput (fun event ->
                    let inputElement = event.target :?> HTMLInputElement
                    config.OnChange inputElement.value
                )

                match config.OnBlur with
                | Some onBlur -> Ev.onBlur (fun _ -> onBlur ())
                | None -> ()

                prop.disabled config.Disabled
                prop.readOnly config.IsReadOnly
                prop.value config.Value

                match config.Attributes.Placeholder with
                | Some placeholder -> prop.placeholder placeholder
                | None -> ()

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
