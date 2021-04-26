module Tests.Base_Meta

open Mocha
open Fable.Form
open Fable.Form.Simple

type Values =
    {
        Password : string
        RepeatPassword : string
    }

let repeatPasswordError =
    "The password should have at least 8 characters"

let repeatPasswordField =
    Base.meta
        (fun values ->
            Form.passwordField
                {
                    Parser =
                        fun value ->
                            if value = values.Password then
                                Ok ()
                            else
                                Error repeatPasswordError
                    Value =
                        fun values -> values.RepeatPassword
                    Update =
                        fun newValue values -> { values with RepeatPassword = newValue }
                    Error = always None
                    Attributes =
                       {
                            Label = "Repeat password"
                            Placeholder = "Type your password again"
                       }
                }
        )

let fill =
    Base.fill repeatPasswordField

let validValues =
    {
        Password = "123"
        RepeatPassword = "123"
    }

let invalidValues =
    {
        Password = "123"
        RepeatPassword = "456"
    }

let emptyValues =
    {
        Password = ""
        RepeatPassword = ""
    }

describe "Base.meta" (fun () ->

    describe "when filled" (fun () ->

        it "contains the correct field" (fun () ->
            let filledForm =
                fill emptyValues

            Assert.strictEqual(
                filledForm.Fields.Length,
                1
            )
        )

        it "provides access to the values of the form" (fun () ->
            let validForm =
                fill validValues

            let invalidForm =
                fill invalidValues

            Assert.deepStrictEqual(
                validForm.Result,
                Ok ()
            )

            Assert.deepStrictEqual(
                invalidForm.Result,
               Error (Error.ValidationFailed repeatPasswordError, [ ])
            )
        )

    )

)
