namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma

module NumberField =

    type Field<'Values>(innerField: NumberField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, NumberField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, NumberField.Attributes>) =
            InputField.renderField
                {
                    OnChange = config.OnChange
                    OnBlur = config.OnBlur
                    Disabled = config.Disabled
                    IsReadOnly = config.IsReadOnly
                    Value = config.Value
                    Error = config.Error
                    ShowError = config.ShowError
                    Label = config.Attributes.Label
                    InputFunc = Bulma.input.number
                    ExtraInputProps =
                        [
                            prop.autoFocus config.Attributes.AutoFocus

                            match config.Attributes.Placeholder with
                            | Some placeholder -> prop.placeholder placeholder
                            | None -> ()

                            match config.Attributes.AutoComplete with
                            | Some autoComplete -> prop.autoComplete autoComplete
                            | None -> ()

                            match config.Attributes.Max with
                            | Some max -> prop.max max
                            | None -> ()

                            match config.Attributes.Min with
                            | Some min -> prop.min min
                            | None -> ()

                            match config.Attributes.Step with
                            | Some step -> prop.step step
                            | None -> ()
                        ]
                }
