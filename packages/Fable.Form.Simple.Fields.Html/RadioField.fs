namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module RadioField =

    type OptionItem =
        abstract member Key: string
        abstract member Text: string

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            Label: string
            Options: OptionItem list
            AutoFocus: bool
        }

        interface Field.IAttributes with
            member this.GetFieldId() = this.FieldId

    type InnerField<'Values> = Field.Field<Attributes, OptionItem option, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, OptionItem option, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field _.IsNone

type RadioField =

    static member create(fieldId: string) : RadioField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Options = []
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: RadioField.Attributes) =
        { attributes with
            Label = label
        }

    static member withOptions
        (options: RadioField.OptionItem list)
        (attributes: RadioField.Attributes)
        =
        { attributes with
            Options = options
        }

    static member withBasicsOptions
        (options: (string * string) list)
        (attributes: RadioField.Attributes)
        =
        { attributes with
            Options =
                options
                |> List.map (fun (key, text) ->
                    { new RadioField.OptionItem with
                        member _.Key = key
                        member _.Text = text
                    }
                )
        }

    static member withAutoFocus(attributes: RadioField.Attributes) =
        { attributes with
            AutoFocus = true
        }
