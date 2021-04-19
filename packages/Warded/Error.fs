module Warded.Error


type Error =
    | RequiredFieldIsEmpty
    | ValidationFailed of string
    | External of string

