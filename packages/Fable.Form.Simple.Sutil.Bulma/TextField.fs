namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Core.JsInterop
open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Browser
open Browser.Types

module TextField =

    type FocusedInfo =
        {
            FieldId: string
            IsFocused: bool
            SelectionStart: int
        }

    let mutable focusedInfo: FocusedInfo option = None

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
                CoreElements.onMount
                    (fun ev ->
                        let input = (ev.target :?> HTMLInputElement)

                        match focusedInfo with
                        | Some info when info.FieldId = config.Attributes.FieldId ->
                            input.focus ()
                            input.selectionStart <- info.SelectionStart
                            focusedInfo <- None
                        | _ -> ()
                    )
                    [
                        CoreElements.EventModifier.Once
                    ]

                CoreElements.onUnmount
                    (fun ev ->
                        let input = (ev.target :?> HTMLInputElement)

                        printfn "Unfocused"

                        if document.activeElement = input then
                            printfn "Focused"

                            focusedInfo <-
                                Some
                                    {
                                        FieldId = config.Attributes.FieldId
                                        IsFocused = true
                                        SelectionStart = input.selectionStart
                                    }
                    )
                    [
                        CoreElements.EventModifier.Once
                    ]

                prop.id config.Attributes.FieldId

                // Note: Compared to React, we need to register to both onInput and onBlur events
                // to mimic React onChange event
                Ev.onInput (fun event ->
                    let value: string = event.target?value
                    config.OnChange value
                )

                // TODO: Redo no need to call config.OnChange onBlur
                // last input has already been sent
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
