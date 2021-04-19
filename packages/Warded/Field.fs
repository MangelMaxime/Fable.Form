module Warded.Field

type Field<'Attributes, 'Value, 'Values> =
    {
        Value : 'Value
        Update : 'Value -> 'Values
        Attributes : 'Attributes
    }
