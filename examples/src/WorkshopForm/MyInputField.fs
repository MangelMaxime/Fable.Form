namespace Fable.Form.WorkshopForm.Field

open Fable.Form

module MyInputField =

    type Attributes =
        {
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
        }

    type MyInputField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((MyInputField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty
