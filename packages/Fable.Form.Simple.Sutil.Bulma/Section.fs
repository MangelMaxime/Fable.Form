namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Sutil
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma

module SectionField =

    type InnerField<'Values> = string * FilledField<'Values> list

    type Field<'Values>(title: string, fields: FilledField<'Values> list) =

        inherit IGenericField<'Values>()

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

                let newFields =
                    fields
                    |> List.map (fun filledField ->
                        {
                            State = filledField.State.MapFieldValues update
                            Error = filledField.Error
                            IsDisabled = filledField.IsDisabled
                            IsReadOnly = filledField.IsReadOnly
                        }
                        : FilledField<'NewValues>
                    )

                Field(title, newFields)

        override _.RenderField
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (_filledField: FilledField<'Values>)
            =

            let renderedFields =
                fields
                |> List.map (fun field ->
                    let f1 = Helpers.View.ignoreChildError field.Error field

                    Helpers.View.renderField
                        { fieldConfig with
                            Disabled = field.IsDisabled || fieldConfig.Disabled
                        }
                        f1
                )

            Html.fieldSet [
                prop.className "fieldset"

                Html.legend [
                    prop.text title
                ]

                yield! renderedFields
            ]
