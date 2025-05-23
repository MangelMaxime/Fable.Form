namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma

module SearchField =

    type Field<'Values>(innerField: SearchField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, SearchField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, SearchField.Attributes>) =
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
                    InputFunc = Bulma.input.text
                    ExtraInputProps =
                        [
                            match config.Attributes.SpellCheck with
                            | SearchField.SpellCheck.Default -> ()
                            | SearchField.SpellCheck.True -> prop.spellcheck true
                            | SearchField.SpellCheck.False -> prop.spellcheck false

                            prop.autoFocus config.Attributes.AutoFocus

                            match config.Attributes.Placeholder with
                            | Some placeholder -> prop.placeholder placeholder
                            | None -> ()

                            match config.Attributes.AutoComplete with
                            | Some autoComplete -> prop.autoComplete autoComplete
                            | None -> ()
                        ]
                }
