namespace Fable.Form.Simple.Field

module TextField =

    open Fable.Form

    type Attributes =
        {
            Label: string
            Placeholder: string
        }

    type TextField<'Values> = Field.Field<Attributes,string,'Values>

    val form<'Values,'Field,'Output> :
        ((TextField<'Values> -> 'Field) -> Base.FieldConfig<Attributes,string,'Values,'Output> -> Base.Form<'Values,'Output,'Field>)
