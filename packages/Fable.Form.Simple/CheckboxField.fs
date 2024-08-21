module Fable.Form.Simple.Field.CheckboxField

open Fable.Form

type Attributes =
    {
        Text: string
    }

type CheckboxField<'Values> = Field.Field<Attributes, bool, 'Values>

let form<'Values, 'Field, 'Output>
    : ((CheckboxField<'Values> -> 'Field)
          -> Base.FieldConfig<Attributes, bool, 'Values, 'Output>
          -> Base.Form<'Values, 'Output, 'Field>) =
    Base.field (fun _ -> false)
