namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module TimeField =

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
            Max: float option
            Min: float option
            Step: float option
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

type TimeField =

    static member create(fieldId: string) : TimeField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            AutoComplete = None
            AutoFocus = false
            Max = None
            Min = None
            Step = None
        }

    static member withLabel (label: string) (attributes: TimeField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: TimeField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member withAutoComplete (autoComplete: string) (attributes: TimeField.Attributes) =
        { attributes with
            AutoComplete = Some autoComplete
        }

    static member withAutoFocus(attributes: TimeField.Attributes) =
        { attributes with
            AutoFocus = true
        }

    static member withMax(max: float) =
        fun (attributes: TimeField.Attributes) ->
            { attributes with
                Max = Some max
            }

    static member withMax(max: int) =
        fun (attributes: TimeField.Attributes) ->
            { attributes with
                Max = Some(float max)
            }

    static member withMin(min: float) =
        fun (attributes: TimeField.Attributes) ->
            { attributes with
                Min = Some min
            }

    static member withMin(min: int) =
        fun (attributes: TimeField.Attributes) ->
            { attributes with
                Min = Some(float min)
            }

    static member withStep(step: float) =
        fun (attributes: TimeField.Attributes) ->
            { attributes with
                Step = Some step
            }

    static member withStep(step: int) =
        fun (attributes: TimeField.Attributes) ->
            { attributes with
                Step = Some(float step)
            }
