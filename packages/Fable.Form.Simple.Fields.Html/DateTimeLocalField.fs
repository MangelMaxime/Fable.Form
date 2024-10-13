namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module DateTimeLocalField =

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// A list of HTML attributes to add to the generated field
            /// </summary>
            Max: System.DateTime option
            Min: System.DateTime option
            Step: int option
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

type DateTimeLocalField =

    static member create(fieldId: string) : DateTimeLocalField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Max = None
            Min = None
            Step = None
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: DateTimeLocalField.Attributes) =
        { attributes with
            Label = label
        }

    static member withMax (max: System.DateTime) (attributes: DateTimeLocalField.Attributes) =
        { attributes with
            Max = Some max
        }

    static member withMin (min: System.DateTime) (attributes: DateTimeLocalField.Attributes) =
        { attributes with
            Min = Some min
        }

    static member withStep (step: int) (attributes: DateTimeLocalField.Attributes) =
        { attributes with
            Step = Some step
        }

    static member withAutoFocus(attributes: DateTimeLocalField.Attributes) =
        { attributes with
            AutoFocus = true
        }
