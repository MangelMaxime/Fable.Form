namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Sutil.Bulma.Helpers.Focus
open Browser
open Browser.Types

module DateField =

    type Field<'Values>(innerField: DateField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, DateField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, DateField.Attributes>) =
            bulma.input.date [
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

                // We should use uncontrolled input for date field, but Sutil doesn't support it yet
                // Waiting for Sutil 3 to be released to check what the actual implementation needs to be
                prop.value config.Value

                if config.ShowError && config.Error.IsSome then
                    color.isDanger

                match config.Attributes.Max with
                | Some max -> prop.max max
                | None -> ()

                match config.Attributes.Min with
                | Some min -> prop.min min
                | None -> ()

                match config.Attributes.Step with
                | Some step -> prop.step step
                | None -> ()

                prop.autoFocus config.Attributes.AutoFocus
            ]
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
