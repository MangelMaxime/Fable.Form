namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module ColorField =

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// List of predefined colors to display
            /// </summary>
            Suggestions: string list option
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

type ColorField =

    static member create(fieldId: string) : ColorField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Suggestions = None
        }

    static member withLabel (label: string) (attributes: ColorField.Attributes) =
        { attributes with
            Label = label
        }

    static member withSuggestions (suggestions: string list) (attributes: ColorField.Attributes) =
        { attributes with
            Suggestions = Some suggestions
        }
