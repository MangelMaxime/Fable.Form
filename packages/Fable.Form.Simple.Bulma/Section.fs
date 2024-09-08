namespace Fable.Form.Simple.Bulma.Fields

open Elmish
open Feliz
open Fable.Form.Simple
open Fable.Form.Simple.Bulma

module SectionField =

    type InnerField<'Values> = string * FilledField<'Values> list

    type Field<'Values, 'Output, 'Value>(title: string, fields: FilledField<'Values> list) =

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
                        }
                        : FilledField<'NewValues>
                    )

                Field(title, newFields)

        override _.RenderField
            (dispatch: Dispatch<'Msg>)
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (_filledField: FilledField<'Values>)
            =

            let renderedFields =
                fields
                |> List.map (fun field ->
                    let f1 = Html.View.ignoreChildError field.Error field

                    Html.View.renderField
                        dispatch
                        { fieldConfig with
                            Disabled = field.IsDisabled || fieldConfig.Disabled
                        }
                        f1
                )

            Html.fieldSet [
                prop.className "fieldset"

                prop.children [
                    Html.legend [
                        prop.text title
                    ]

                    yield! renderedFields
                ]
            ]