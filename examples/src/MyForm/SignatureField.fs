namespace MyForm.Field

open Fable.Form

module SignatureField =

    type Attributes =
        {
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
        }

    type SignatureField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((SignatureField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty
