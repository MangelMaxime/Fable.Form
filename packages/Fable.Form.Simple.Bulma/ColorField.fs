namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma

module ColorField =

    type Field<'Values>(innerField: ColorField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, ColorField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, ColorField.Attributes>) =
            React.fragment [

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
                                match config.Attributes.Suggestions with
                                | Some _ -> prop.list config.Attributes.FieldId
                                | None -> ()
                            ]
                    }

                match config.Attributes.Suggestions with
                | Some suggestions ->
                    Html.datalist [
                        prop.id config.Attributes.FieldId
                        suggestions |> List.map Html.option |> prop.children
                    ]
                | None -> Html.none
            ]
