namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Core.JsInterop
open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Browser
open Browser.Types
open Helpers.Focus

module TextField =

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// Placeholder to display when the field is empty
            /// </summary>
            Placeholder: string
            /// <summary>
            /// `auto-complete` attribute value
            /// </summary>
            AutoComplete: string option
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    /// <summary>
    /// Represents the type of a TextField
    /// </summary>
    type TextType =
        | TextColor
        | TextDate
        | TextDateTimeLocal
        | TextEmail
        // Not supported yet because there are not cross browser support Firefox doesn't support it for example
        // and there is no polyfill for it
        // | TextMonth
        | TextNumber
        | TextPassword
        // TODO:
        // | TextRange
        | TextSearch
        | TextTel
        // Match for input="text"
        | TextRaw
        | TextTime
        // Not supported yet because there are not cross browser support Firefox doesn't support it for example
        // and there is no polyfill for it
        // | TextWeek
        | TextArea

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

    type Field<'Values>(inputType: TextType, innerField: InnerField<'Values>) =
        inherit IStandardField<'Values, string, Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(inputType, Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, Attributes>) =
            let onBlur =
                match config.OnBlur with
                | Some onBlur -> fun _ -> onBlur ()
                | None -> ignore

            let inputFunc: children: seq<Core.SutilElement> -> Core.SutilElement =
                match inputType with
                | TextRaw -> bulma.input.text

                | TextPassword -> bulma.input.password

                | TextEmail -> bulma.input.email

                | TextColor -> bulma.input.color

                | TextDate -> bulma.input.date

                | TextDateTimeLocal -> bulma.input.datetimeLocal

                | TextNumber -> bulma.input.number

                | TextSearch -> bulma.input.search

                | TextTel -> bulma.input.tel

                | TextTime -> bulma.input.time

                | TextArea -> bulma.textarea

            let autoComplete =
                match config.Attributes.AutoComplete with
                | Some value -> value
                | None -> "off"

            inputFunc [
                prop.id config.Attributes.FieldId

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
                    let value: string = event.target?value
                    config.OnChange value
                )

                Ev.onBlur onBlur

                prop.disabled config.Disabled

                if config.IsReadOnly then
                    prop.readOnly config.IsReadOnly

                prop.value config.Value
                prop.placeholder config.Attributes.Placeholder
                if config.ShowError && config.Error.IsSome then
                    color.isDanger

                prop.autoComplete autoComplete
            ]
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
