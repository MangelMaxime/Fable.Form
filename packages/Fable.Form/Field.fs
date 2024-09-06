module Fable.Form.Field

type IAttributes =
    abstract GetFieldId: unit -> string

[<NoComparison; NoEquality>]
type Field<'Attributes, 'Value, 'Values when 'Attributes :> IAttributes> =
    {
        Value: 'Value
        Update: 'Value -> 'Values
        Attributes: 'Attributes
    }

let mapValues
    (fn: 'A -> 'B)
    (field: Field<'Attributes, 'Value, 'A>)
    : Field<'Attributes, 'Value, 'B>
    =

    {
        Value = field.Value
        Update = field.Update >> fn
        Attributes = field.Attributes
    }
