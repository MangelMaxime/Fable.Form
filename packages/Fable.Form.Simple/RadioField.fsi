namespace Fable.Form.Simple.Field

module RadioField =

    open Fable.Form

    type Attributes =
        { Label: string
          Options: (string * string) list }

    type RadioField<'Values> = Field.Field<Attributes, string, 'Values>

    val form<'Values, 'Field, 'Output> :
        ((RadioField<'Values> -> 'Field)
            -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
            -> Base.Form<'Values, 'Output, 'Field>)
