namespace Fable.Form.Simple.Fields.Html

open Fable.Form

module SelectField =

    [<AllowNullLiteral>]
    type OptionItem =
        abstract member Key: string
        abstract member Text: string

    type Value = OptionItem option

    [<NoComparison>]
    type Attributes =
        {
            FieldId: string
            Label: string
            Placeholder: string option
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

type SelectField =

    static member create(fieldId: string) : SelectField.Attributes =
        {
            FieldId = fieldId
            Label = ""
            Placeholder = None
            Options = []
            AutoFocus = false
        }

    static member withLabel (label: string) (attributes: SelectField.Attributes) =
        { attributes with
            Label = label
        }

    static member withPlaceholder (placeholder: string) (attributes: SelectField.Attributes) =
        { attributes with
            Placeholder = Some placeholder
        }

    static member withOptions
        (options: SelectField.OptionItem list)
        (attributes: SelectField.Attributes)
        =
        { attributes with
            Options = options
        }

    static member withBasicsOptions
        (options: (string * string) list)
        (attributes: SelectField.Attributes)
        =
        { attributes with
            Options =
                options
                |> List.map (fun (key, text) ->
                    { new SelectField.OptionItem with
                        member _.Key = key
                        member _.Text = text
                    }
                )
        }

    static member withAutoFocus(attributes: SelectField.Attributes) =
        { attributes with
            AutoFocus = true
        }
