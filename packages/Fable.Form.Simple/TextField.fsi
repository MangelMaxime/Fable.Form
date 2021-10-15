namespace Fable.Form.Simple.Field

module TextField =

    open Fable.Form

    type Attributes<'Attributes> =
        {
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// Placeholder to display when the field is empty
            /// </summary>
            Placeholder: string
            /// <summary>
            /// A list of HTML attributes to add to the generated field
            /// </summary>
            HtmlAttributes: 'Attributes list
        }

    type TextField<'Values, 'Attributes> = Field.Field<Attributes<'Attributes>,string,'Values>

    val form<'Values,'Attributes,'Field,'Output> :
        ((TextField<'Values,'Attributes> -> 'Field) -> Base.FieldConfig<Attributes<'Attributes>,string,'Values,'Output> -> Base.Form<'Values,'Output,'Field>)
