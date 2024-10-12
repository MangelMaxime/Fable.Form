namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma

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
                    InputFunc = Bulma.textarea
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
                            | true -> prop.wrap.hard
                            | false -> prop.wrap.soft
                        ]
                }
