namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module TextareaField =

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
            Cols: int option
            Rows: int option
            IsWrapHard: bool
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

type TextareaField =

    static member create(fieldId: string) : TextareaField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            AutoComplete = None
            AutoFocus = false
            Cols = None
            Rows = None
            IsWrapHard = false
        }

    static member withLabel (label: string) (attributes: TextareaField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: TextareaField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member withAutoComplete (autoComplete: string) (attributes: TextareaField.Attributes) =
        { attributes with
            AutoComplete = Some autoComplete
        }

    static member withAutoFocus(attributes: TextareaField.Attributes) =
        { attributes with
            AutoFocus = true
        }

    static member withCols (cols: int) (attributes: TextareaField.Attributes) =
        { attributes with
            Cols = Some cols
        }

    static member withRows (rows: int) (attributes: TextareaField.Attributes) =
        { attributes with
            Rows = Some rows
        }

    static member withWrapHard(attributes: TextareaField.Attributes) =
        { attributes with
            IsWrapHard = true
        }

    static member withWrapSoft(attributes: TextareaField.Attributes) =
        { attributes with
            IsWrapHard = false
        }
