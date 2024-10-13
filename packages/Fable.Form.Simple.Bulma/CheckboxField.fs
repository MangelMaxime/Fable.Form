namespace Fable.Form.Simple.Bulma.Fields

open Fable.Form
open Feliz
open Feliz.Bulma
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma

module CheckboxField =

    type Field<'Values>(innerField: CheckboxField.InnerField<'Values>) =

        inherit IStandardField<'Values, bool, CheckboxField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<bool, CheckboxField.Attributes>) =

            Bulma.control.div [
                Bulma.input.labels.checkbox [
                    prop.children [
                        Bulma.input.checkbox [
                            // Checkbox can't really be set to readonly in HTML
                            // So we need to not listen to the onChange event
                            prop.readOnly config.IsReadOnly
                            if not config.IsReadOnly then
                                prop.onChange config.OnChange

                            match config.OnBlur with
                            | Some onBlur -> prop.onBlur (fun _ -> onBlur ())

                            | None -> ()
                            prop.disabled config.Disabled

                            prop.isChecked config.Value
                        ]

                        Html.text config.Attributes.Text
                    ]
                ]
            ]
            |> List.singleton
            |> Html.View.wrapInFieldContainer
