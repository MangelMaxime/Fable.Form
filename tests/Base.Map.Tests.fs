module Tests.Base_Map_Tests

open Mocha
open Fable.Form
open Fable.Form.Simple

type Values =
        {
            Password : string
        }

let passwordError =
    "The password should have at least 8 characters"

let passwordField =
    Form.passwordField
        {
            Parser =
                fun value ->
                    if String.length value >= 8 then
                        Ok value
                    else
                        Error passwordError
            Value =
                fun values -> values.Password
            Update =
                fun newValue values -> { values with Password = newValue }
            Error = always None
            Attributes =
               {
                    Label = "Password"
                    Placeholder = "Type your password"
               }
        }

describe "Base.map" (fun () ->
    it "applies the given function to the form output" (fun () ->
        let form =
            Base.map String.length passwordField

        { Password = "12345678" }
        |> Base.fill form
        |> fun form ->
            Assert.deepStrictEqual(form.Result, Ok 8)
    )

    it "should report the errors if the form validation fails" (fun () ->
        let form =
            Base.map String.length passwordField

        { Password = "1234567" }
        |> Base.fill form
        |> fun form ->
            Assert.deepStrictEqual(form.Result, Error (Error.ValidationFailed passwordError, []))
    )
)
