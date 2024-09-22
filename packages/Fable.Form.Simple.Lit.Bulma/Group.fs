namespace Fable.Form.Simple.Lit.Bulma.Fields

open Lit
open Fable.Form.Simple
open Fable.Form.Simple.Lit.Bulma

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
                    let f1 = Html.View.ignoreChildError field.Error field

                    Html.View.renderField
                        { fieldConfig with
                            Disabled = field.IsDisabled || fieldConfig.Disabled
                        }
                        f1
                )

            html
                $"""
                <div class="field">
                    <div class="columns">
                        {renderedFields}
                    </div>
                </div>
            """
