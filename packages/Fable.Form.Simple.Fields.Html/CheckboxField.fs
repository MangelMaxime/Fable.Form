namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module CheckboxField =

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            Text: string
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, bool, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, bool, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun _ -> false)

type CheckboxField =

    static member create(fieldId: string) : CheckboxField.Attributes =
        {
            FieldId = fieldId
            Text = ""
        }

    static member withText (text: string) (attributes: CheckboxField.Attributes) =
        { attributes with
            Text = text
        }
