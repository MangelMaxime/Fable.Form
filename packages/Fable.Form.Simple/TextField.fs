namespace Fable.Form.Simple.Field

open Fable.Form

module TextField =

    type Attributes<'Attributes> =
        {
            Label: string
            Placeholder: string
            HtmlAttributes: 'Attributes list
        }

    type TextField<'Values, 'Attributes> = Field.Field<Attributes<'Attributes>, string, 'Values>

    let form<'Values, 'Attributes, 'Field, 'Output>
        : ((TextField<'Values, 'Attributes> -> 'Field)
              -> Base.FieldConfig<Attributes<'Attributes>, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty
