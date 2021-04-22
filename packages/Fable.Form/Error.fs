module Fable.Form.Error


type Error =
    | RequiredFieldIsEmpty
    | ValidationFailed of string
    | External of string

