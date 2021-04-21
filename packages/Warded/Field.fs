module Warded.Field

type Field<'Attributes, 'Value, 'Values> =
    {
        Value : 'Value
        Update : 'Value -> 'Values
        Attributes : 'Attributes
    }

let mapValues
    (fn : 'A -> 'B)
    (field : Field<'Attributes, 'Value, 'A>)
    : Field<'Attributes, 'Value, 'B> =

    {
        Value = field.Value
        Update = field.Update >> fn
        Attributes = field.Attributes
    }
