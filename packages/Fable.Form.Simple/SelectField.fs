module Fable.Form.Simple.Field.SelectField

open Fable.Form

type Attributes =
    {
        Label: string
        Placeholder: string
        Options: (string * string) list
    }

type SelectField<'Values> = Field.Field<Attributes, string, 'Values>

let form<'Values, 'Field, 'Output>
    : ((SelectField<'Values> -> 'Field)
          -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
          -> Base.Form<'Values, 'Output, 'Field>) =
    Base.field System.String.IsNullOrEmpty
