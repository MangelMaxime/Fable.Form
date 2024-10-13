namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html

module RadioField =

    type Field<'Values>(innerField: RadioField.InnerField<'Values>) =

        inherit
            IStandardField<'Values, RadioField.OptionItem option, RadioField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField
            (config: StandardRenderFieldConfig<RadioField.OptionItem option, RadioField.Attributes>)
            =

            let radio (optionItem: RadioField.OptionItem) =
                Bulma.input.labels.radio [
                    Bulma.input.radio [
                        prop.name config.Attributes.Label
                        prop.isChecked (Some optionItem = config.Value: bool)
                        prop.disabled config.Disabled

                        // RadioField can't really be set to readonly in HTML
                        // So we need to not listen to the onChange event
                        prop.readOnly config.IsReadOnly
                        if not config.IsReadOnly then
                            prop.onChange (fun (_: bool) -> config.OnChange(Some optionItem))

                        match config.OnBlur with
                        | Some onBlur -> prop.onBlur (fun _ -> onBlur ())

                        | None -> ()
                    ]

                    Html.text optionItem.Text
                ]

            Bulma.control.div [
                config.Attributes.Options |> List.map radio |> prop.children
            ]
            |> Html.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
