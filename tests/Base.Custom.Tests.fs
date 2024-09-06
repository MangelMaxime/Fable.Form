module Tests.Base_Custom

open Mocha
open Fable.Form
open Fable.Form.Simple

type CustomField = CustomField

let invalidValue = "invalid"

let form =
    Base.custom (fun value ->
        {
            State = CustomField
            Result =
                if value = invalidValue then
                    Error(
                        Error.ValidationFailed "Error #1",
                        [
                            Error.ValidationFailed "Error #2"
                            Error.ValidationFailed "Error #3"
                        ]
                    )
                else
                    Ok "valid"
            IsEmpty = false
        }
    )

let fill = Base.fill form

describe
    "Base.custom"
    (fun () ->

        it
            "it returns the correct result when the value is valid"
            (fun () ->
                let filledForm = fill "hello"

                Assert.deepStrictEqual (
                    filledForm,
                    {
                        Fields =
                            [
                                {
                                    State = CustomField
                                    Error = None
                                    IsDisabled = false
                                }
                            ]
                        Result = Ok "valid"
                        IsEmpty = false
                    }
                )

            )

        it
            "returns the errors when the value is invalid"
            (fun () ->
                let filledForm = fill invalidValue

                Assert.deepStrictEqual (
                    filledForm,
                    {
                        Fields =
                            [
                                {
                                    State = CustomField
                                    Error = Some(Error.ValidationFailed "Error #1")
                                    IsDisabled = false
                                }
                            ]
                        Result =
                            Error(
                                Error.ValidationFailed "Error #1",
                                [
                                    Error.ValidationFailed "Error #2"
                                    Error.ValidationFailed "Error #3"
                                ]
                            )
                        IsEmpty = false
                    }
                )
            )

    )
