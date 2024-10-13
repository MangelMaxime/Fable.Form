namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Fable.Form
open Sutil
open Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html

module CheckboxField =

    type Field<'Values>(innerField: CheckboxField.InnerField<'Values>) =

        inherit IStandardField<'Values, bool, CheckboxField.Attributes>(innerField)

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                Field(Field.mapValues update innerField)

        override _.RenderField(config: StandardRenderFieldConfig<bool, CheckboxField.Attributes>) =

            bulma.control.div [
                bulma.inputLabels.checkbox [
                    bulma.input.checkbox [
                        // Checkbox can't really be set to readonly in HTML
                        // So we need to not listen to the onChange event
                        prop.readOnly config.IsReadOnly
                        if not config.IsReadOnly then
                            Ev.onChange config.OnChange

                        match config.OnBlur with
                        | Some onBlur -> Ev.onBlur (fun _ -> onBlur ())

                        | None -> ()
                        prop.disabled config.Disabled

                        prop.isChecked config.Value
                    ]

                    Html.text config.Attributes.Text
                ]
            ]
            |> List.singleton
            |> Helpers.View.wrapInFieldContainer
