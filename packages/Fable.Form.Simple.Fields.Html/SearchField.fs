namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module SearchField =

    [<RequireQualifiedAccess>]
    type SpellCheck =
        | Default
        | True
        | False

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
            SpellCheck: SpellCheck
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

type SearchField =

    static member create(fieldId: string) : SearchField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            AutoComplete = None
            SpellCheck = SearchField.SpellCheck.Default
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: SearchField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: SearchField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member withAutoComplete (autoComplete: string) (attributes: SearchField.Attributes) =
        { attributes with
            AutoComplete = Some autoComplete
        }

    static member withSpellCheck (spellCheck: bool) (attributes: SearchField.Attributes) =
        { attributes with
            SpellCheck =
                if spellCheck then
                    SearchField.SpellCheck.True
                else
                    SearchField.SpellCheck.False
        }

    static member withAutoFocus(attributes: SearchField.Attributes) =
        { attributes with
            AutoFocus = true
        }
