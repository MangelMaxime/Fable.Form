namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma

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
            /// A list of HTML attributes to add to the generated field
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

            let inputFunc =
                match inputType with
                | TextRaw -> Bulma.input.text

                | TextPassword -> Bulma.input.password

                | TextEmail -> Bulma.input.email

                | TextColor -> Bulma.input.color

                | TextDate -> Bulma.input.date

                | TextDateTimeLocal -> Bulma.input.datetimeLocal

                | TextNumber -> Bulma.input.number

                | TextSearch -> Bulma.input.search

                | TextTel -> Bulma.input.tel

                | TextTime -> Bulma.input.time

                | TextArea -> Bulma.textarea

            let autoComplete =
                match config.Attributes.AutoComplete with
                | Some value -> value
                | None -> "off"

            inputFunc [
                prop.onChange config.OnChange

                match config.OnBlur with
                | Some onBlur -> prop.onBlur (fun _ -> onBlur ())
                | None -> ()

                prop.disabled config.Disabled
                prop.readOnly config.IsReadOnly
                prop.value config.Value
                prop.placeholder config.Attributes.Placeholder
                if config.ShowError && config.Error.IsSome then
                    color.isDanger

                prop.autoComplete autoComplete
            ]
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
