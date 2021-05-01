namespace Fable.Form.Simple.Field

module SelectField =

    open Fable.Form

    type Attributes =
        {
            Label: string
            Placeholder: string
            Options: (string * string) list
        }

    type SelectField<'Values> = Field.Field<Attributes,string,'Values>

    val form<'Values,'Field,'Output> :
        ((SelectField<'Values> -> 'Field) -> Base.FieldConfig<Attributes,string,'Values,'Output> -> Base.Form<'Values,'Output,'Field>)
