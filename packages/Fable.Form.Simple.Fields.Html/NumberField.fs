namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module NumberField =

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
            AutoComplete: string option
            Max: float option
            Min: float option
            Step: float option
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

type NumberField =

    static member create(fieldId: string) : NumberField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            AutoComplete = None
            Max = None
            Min = None
            Step = None
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: NumberField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: NumberField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member withAutoComplete (autoComplete: string) (attributes: NumberField.Attributes) =
        { attributes with
            AutoComplete = Some autoComplete
        }

    static member withMax(max: float) =
        fun (attributes: NumberField.Attributes) ->
            { attributes with
                Max = Some max
            }

    static member withMax(max: int) =
        fun (attributes: NumberField.Attributes) ->
            { attributes with
                Max = Some(float max)
            }

    static member withMin(min: float) =
        fun (attributes: NumberField.Attributes) ->
            { attributes with
                Min = Some min
            }

    static member withMin(min: int) =
        fun (attributes: NumberField.Attributes) ->
            { attributes with
                Min = Some(float min)
            }

    static member withStep(step: float) =
        fun (attributes: NumberField.Attributes) ->
            { attributes with
                Step = Some step
            }

    static member withStep(step: int) =
        fun (attributes: NumberField.Attributes) ->
            { attributes with
                Step = Some(float step)
            }

    static member withAutoFocus (autoFocus: bool) (attributes: NumberField.Attributes) =
        { attributes with
            AutoFocus = autoFocus
        }
