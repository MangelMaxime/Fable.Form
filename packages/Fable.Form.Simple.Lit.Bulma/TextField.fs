namespace Fable.Form.Simple.Lit.Bulma.Fields

open Fable.Core.JsInterop
open Fable.Form
open Lit
open Fable.Form.Simple.Lit.Bulma

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
            let inputType =
                match inputType with
                | TextRaw -> "text"
                | TextPassword -> "password"
                | TextEmail -> "email"
                | TextColor -> "color"
                | TextDate -> "date"
                | TextDateTimeLocal -> "datetime-local"
                | TextNumber -> "number"
                | TextSearch -> "search"
                | TextTel -> "tel"
                | TextTime -> "time"
                | TextArea -> "textarea"

            let cls =
                [
                    "input"
                    if config.ShowError && config.Error.IsSome then
                        "is-danger"
                ]
                |> String.concat " "

            let onBlur =
                match config.OnBlur with
                | Some onBlur -> fun _ -> onBlur ()
                | None -> ignore

            let autoComp =
                match config.Attributes.AutoComplete with
                | Some value -> value
                | None -> "off"

            html
                $"""
                <input
                    class={cls}
                    type={inputType}
                    .placeholder={config.Attributes.Placeholder}
                    .value={config.Value}
                    .disabled={config.Disabled}
                    .readOnly={config.IsReadOnly}
                    .autocomplete={autoComp}

                    @input={fun (e: Browser.Types.KeyboardEvent) -> config.OnChange e.target?value}
                    @blur={onBlur}
                />
            """
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
