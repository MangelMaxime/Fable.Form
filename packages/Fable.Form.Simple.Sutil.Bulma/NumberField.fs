namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html

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
                    InputFunc = bulma.input.number
                    FieldId = config.Attributes.FieldId
                    Label = config.Attributes.Label
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
