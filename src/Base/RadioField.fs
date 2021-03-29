module Form.Base.RadioField

module Base = Form.Base.Core

open Form

type Attributes =
    {
        Label : string
        Placeholder : string
    }

type RadioField<'Values> = Field.Field<Attributes, string, 'Values>
