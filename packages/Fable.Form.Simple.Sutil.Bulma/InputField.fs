namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma.Helpers.Focus
open Browser
open Browser.Types

module InputField =

    [<NoComparison; NoEquality>]
    type Config =
        {
            OnChange: string -> unit
            OnBlur: (unit -> unit) option
            Disabled: bool
            IsReadOnly: bool
            Value: string
            Error: Error.Error option
            ShowError: bool
            Label: string
            InputFunc: Core.SutilElement list -> Core.SutilElement
            FieldId: string
            ExtraInputProps: Core.SutilElement list
        }

    /// <summary>
    /// Generic input field renderer used to mutualize the rendering of input fields
    ///
    /// ATTENTION: For some input fields, you may not want to use this function.
    ///
    /// For example, date fields needs to use uncontrolled input fields.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    let renderField (config: Config) =

        config.InputFunc [
            Ev.onMount (fun ev ->
                let input = (ev.target :?> HTMLInputElement)

                if FocusedField.Instance.fieldId = config.FieldId then
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
                        config.FieldId,
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

            if config.ShowError && config.Error.IsSome then
                color.isDanger

            yield! config.ExtraInputProps
        ]
        |> Helpers.View.withLabelAndError config.Label config.ShowError config.Error
