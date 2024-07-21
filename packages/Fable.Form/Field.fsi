module Fable.Form.Field

[<NoComparison; NoEquality>]
type Field<'Attributes, 'Value, 'Values> =
    { Value: 'Value
      Update: 'Value -> 'Values
      Attributes: 'Attributes }

val mapValues: fn: ('A -> 'B) -> field: Field<'Attributes, 'Value, 'A> -> Field<'Attributes, 'Value, 'B>
