namespace Fable.Form.Simple.Field

module CheckboxField =

    open Fable.Form

    type Attributes =
        {
            Text: string
        }

    type CheckboxField<'Values> = Field.Field<Attributes,bool,'Values>

    val form<'Values,'Field,'Output> :
        ((CheckboxField<'Values> -> 'Field) -> Base.FieldConfig<Attributes,bool,'Values,'Output> -> Base.Form<'Values,'Output,'Field>)
