namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Core.JsInterop
open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma

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
            HtmlAttributes: Core.SutilElement list
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

            inputFunc [
                // Note: Compared to React, we need to register to both onInput and onBlur events
                // to mimic React onChange event
                Ev.onInput (fun event ->
                    printfn "OnInput"
                    let value: string = event.target?value
                    config.OnChange value
                )

                match config.OnBlur with
                | Some onBlur ->
                    Ev.onBlur (fun event ->
                        let value: string = event.target?value
                        config.OnChange value
                        onBlur ()
                    )
                | None ->
                    Ev.onBlur (fun event ->
                        let value: string = event.target?value
                        config.OnChange value
                    )

                prop.disabled config.Disabled

                if config.IsReadOnly then
                    prop.readOnly config.IsReadOnly

                prop.value config.Value
                prop.placeholder config.Attributes.Placeholder
                if config.ShowError && config.Error.IsSome then
                    color.isDanger

                yield! config.Attributes.HtmlAttributes
            ]
            |> Helpers.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
