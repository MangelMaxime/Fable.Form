namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module EmailField =

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// Placeholder to display when the field is empty
            /// </summary>
            Placeholder: string option
            /// <summary>
            /// A list of HTML attributes to add to the generated field
            /// </summary>
            AutoComplete: string option
            Multiple: bool option
            AutoFocus: bool
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

type EmailField =

    static member create(fieldId: string) : EmailField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            AutoComplete = None
            Multiple = None
            AutoFocus = false
        }

    static member inline withLabel (label: string) (attributes: EmailField.Attributes) =
        { attributes with
            Label = label
        }

    static member inline withPlaceholder (placeholder: string) (attributes: EmailField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member inline withAutoComplete
        (autoComplete: string)
        (attributes: EmailField.Attributes)
        =
        { attributes with
            AutoComplete = Some autoComplete
        }

    static member inline withMultiple (multiple: bool) (attributes: EmailField.Attributes) =
        { attributes with
            Multiple = Some multiple
        }

    static member inline withAutoFocus(attributes: EmailField.Attributes) =
        { attributes with
            AutoFocus = true
        }
