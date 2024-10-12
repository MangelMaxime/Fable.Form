namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module RangeField =

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            Suggestions: float list option
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

type RangeField =

    static member create(fieldId: string) : RangeField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Suggestions = None
            Max = None
            Min = None
            Step = None
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: RangeField.Attributes) =
        { attributes with
            Label = label
        }

    static member withSuggestions(rangeList: float list) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Suggestions = Some rangeList
            }

    static member withSuggestions(rangeList: int list) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Suggestions = Some(rangeList |> List.map float)
            }

    static member withMax(max: float) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Max = Some max
            }

    static member withMax(max: int) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Max = Some(float max)
            }

    static member withMin(min: float) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Min = Some min
            }

    static member withMin(min: int) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Min = Some(float min)
            }

    static member withStep(step: float) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Step = Some step
            }

    static member withStep(step: int) =
        fun (attributes: RangeField.Attributes) ->
            { attributes with
                Step = Some(float step)
            }

    static member withAutoFocus (autoFocus: bool) (attributes: RangeField.Attributes) =
        { attributes with
            AutoFocus = autoFocus
        }
