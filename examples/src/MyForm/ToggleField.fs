namespace MyForm.Field

open Fable.Form

module ToggleField =

    type Attributes =
        {
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
        }

    type ToggleField<'Values> = Field.Field<Attributes, bool, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((ToggleField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, bool, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun _ -> false)
