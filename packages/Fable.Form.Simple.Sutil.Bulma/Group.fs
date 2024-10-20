namespace Fable.Form.Simple.Sutil.Bulma.Fields

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma

module Group =

    type Field<'Values>(fields: FilledField<'Values> list) =

        inherit IGenericField<'Values>()

        interface IField<'Values> with

            member _.MapFieldValues(update: 'Values -> 'NewValues) : IField<'NewValues> =

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
                |> Field
                :> IField<'NewValues>

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

            bulma.field.div [
                bulma.columns (renderedFields |> List.map bulma.column)
            ]
