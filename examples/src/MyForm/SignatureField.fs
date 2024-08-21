namespace Fable.Form.MyForm.Field

open Fable.Form

module SignatureField =

    type Attributes =
        {
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
        }

    type Value =
        {
            History: string list
            CurrentSignature: string
        }

        static member Default =
            {
                History = []
                CurrentSignature = ""
            }

    type SignatureField<'Values> = Field.Field<Attributes, Value, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((SignatureField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, Value, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun value -> System.String.IsNullOrEmpty value.CurrentSignature)
