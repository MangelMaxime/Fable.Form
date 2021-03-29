module Form.Base.TextField

module Base = Form.Base.Core

open Form

type Attributes =
    {
        Label : string
        Placeholder : string
    }

type TextField<'Values> = Field.Field<Attributes, string, 'Values>

let form<'Values, 'Field, 'Output> : ((TextField<'Values> -> 'Field) -> Base.FieldConfig<Attributes, string, 'Values, 'Output> -> Base.Form<'Values, 'Output, 'Field>) =
    Base.field
        System.String.IsNullOrEmpty
