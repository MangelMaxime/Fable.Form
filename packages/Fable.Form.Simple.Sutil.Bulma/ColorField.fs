namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html

module ColorField =

    type Field<'Values>(innerField: ColorField.InnerField<'Values>) =
        inherit IStandardField<'Values, string, ColorField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<string, ColorField.Attributes>) =
            CoreElements.fragment [

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
                        InputFunc = bulma.input.text
                        ExtraInputProps =
                            [
                                match config.Attributes.Suggestions with
                                | Some _ -> prop.custom ("list", config.Attributes.FieldId)
                                | None -> ()
                            ]
                    }

                match config.Attributes.Suggestions with
                | Some suggestions ->
                    Html.datalist [
                        prop.id config.Attributes.FieldId

                        yield! suggestions |> List.map Html.option
                    ]
                | None -> Html.none
            ]
