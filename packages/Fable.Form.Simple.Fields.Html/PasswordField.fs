namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module PasswordField =

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

type PasswordField =

    static member create(fieldId: string) : PasswordField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            AutoComplete = None
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: PasswordField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: PasswordField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member withAutoComplete (autoComplete: string) (attributes: PasswordField.Attributes) =
        { attributes with
            AutoComplete = Some autoComplete
        }

    static member withAutoFocus(attributes: PasswordField.Attributes) =
        { attributes with
            AutoFocus = true
        }
