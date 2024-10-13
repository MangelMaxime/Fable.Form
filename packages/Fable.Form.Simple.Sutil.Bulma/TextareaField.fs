namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html

module TextareaField =

    type Field<'Values>(innerField: TextareaField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, TextareaField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField
            (config: StandardRenderFieldConfig<string, TextareaField.Attributes>)
            =

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
                    FieldId = config.Attributes.FieldId
                    InputFunc = bulma.textarea
                    ExtraInputProps =
                        [
                            match config.Attributes.Placeholder with
                            | Some placeholder -> prop.placeholder placeholder
                            | None -> ()

                            match config.Attributes.AutoComplete with
                            | Some value -> prop.autoComplete value
                            | None -> ()

                            prop.autoFocus config.Attributes.AutoFocus

                            match config.Attributes.Rows with
                            | Some value -> prop.rows value
                            | None -> ()

                            match config.Attributes.Cols with
                            | Some value -> prop.cols value
                            | None -> ()

                            match config.Attributes.IsWrapHard with
                            | true -> prop.custom ("wrap", "hard")
                            | false -> prop.custom ("wrap", "soft")
                        ]
                }
