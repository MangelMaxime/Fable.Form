module Fable.Form.Simple.Field.RadioField

open Fable.Form

type Attributes =
    {
        Label: string
        Options: (string * string) list
    }

type RadioField<'Values> = Field.Field<Attributes, string, 'Values>

let form<'Values, 'Field, 'Output>
    : ((RadioField<'Values> -> 'Field)
          -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
          -> Base.Form<'Values, 'Output, 'Field>) =
    Base.field System.String.IsNullOrEmpty
