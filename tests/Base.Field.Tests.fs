module Tests.Base_Field

open Mocha
open Fable.Form
open Fable.Form.Simple

type Attributes = { A: int; B: string }

let customField = Base.field System.String.IsNullOrEmpty id

let invalidString = "invalid"

let externalErrorString = "external_error"

let attributes = { A = 1; B = "Some attributes" }

let form =
    customField
        {
            Parser =
                fun value ->
                    if value = invalidString then
                        Error "invalid input"
                    else
                        Ok value
            Value = id
            Update = fun value _ -> value
            Error =
                fun value ->
                    if value = externalErrorString then
                        Some "External error"
                    else
                        None
            Attributes = attributes
        }

let fill = Base.fill form

let withFieldAndError fn (result: Base.FilledForm<_, _>) =
    match result.Fields with
    | [ field ] -> fn field

    | _ -> Assert.fail "fields do not contain a single field"

let withField fn =
    withFieldAndError ((fun field -> field.State) >> fn)

let withFieldError fn =
    withFieldAndError ((fun field -> field.Error) >> fn)

[<RequireQualifiedAccess>]
module Expect =
    let strictEqual expected actual =
        // Arguments order is inverted making it easier to pipe the actual value
        Assert.strictEqual (actual, expected)

    let deepStrictEqual expected actual =
        // Arguments order is inverted making it easier to pipe the actual value
        Assert.deepStrictEqual (actual, expected)

    // Alias of deepStrictEqual
    let equal = deepStrictEqual

let getValue (x: Field.Field<Attributes, string, string>) : string = x.Value

let getAttributes (x: Field.Field<Attributes, string, string>) : Attributes = x.Attributes

describe
    "Base.field"
    (fun () ->

        describe
            "when filled"
            (fun () ->

                it
                    "contains a single field"
                    (fun () ->
                        let filledForm = fill ""

                        Assert.strictEqual (filledForm.Fields.Length, 1)
                    )

                it
                    "builds the field with its current value"
                    (fun () ->
                        let value = "hello"
                        let filledForm = fill value

                        filledForm |> withField (getValue >> Expect.strictEqual value)
                    )

                it
                    "builds the field with an update helper"
                    (fun () ->
                        fill "hello"
                        |> withField (fun field ->
                            field.Update "Hello world" |> Expect.strictEqual "Hello world"
                        )
                    )

                it
                    "builds the field with its attributes"
                    (fun () ->
                        fill "" |> withField (getAttributes >> Expect.strictEqual attributes)
                    )

            )

        describe
            "when filled with a valid value"
            (fun () ->

                it
                    "there is no field error"
                    (fun () -> fill "hello" |> withFieldError (Expect.strictEqual None))

                it
                    "result is the correct output"
                    (fun () ->
                        fill "hello"
                        |> fun filledForm -> filledForm.Result
                        |> Expect.equal (Ok "hello")
                    )

            )

        describe
            "when filled with a empty value"
            (fun () ->

                it
                    "field error is RequiredFieldIsEmpty"
                    (fun () ->
                        fill ""
                        |> withFieldError (
                            Expect.deepStrictEqual (Some Error.RequiredFieldIsEmpty)
                        )
                    )

                it
                    "result is a RequiredFieldIsEmpty error"
                    (fun () ->
                        fill ""
                        |> fun filledForm -> filledForm.Result
                        |> Expect.equal (Error(Error.RequiredFieldIsEmpty, []))
                    )

                it
                    "form is empty"
                    (fun () ->
                        fill "" |> (fun filledForm -> filledForm.IsEmpty) |> Expect.equal true
                    )

            )

        describe
            "when filled with an invalid value"
            (fun () ->

                it
                    "field error is ValidationFailed"
                    (fun () ->
                        fill invalidString
                        |> withFieldError (
                            Expect.equal (Some(Error.ValidationFailed "invalid input"))
                        )
                    )

                it
                    "result is a ValidationFailed error"
                    (fun () ->
                        fill invalidString
                        |> fun filledForm -> filledForm.Result
                        |> Expect.equal (Error(Error.ValidationFailed "invalid input", []))
                    )

                it
                    "form is not empty"
                    (fun () ->
                        fill invalidString
                        |> fun filledForm -> filledForm.IsEmpty
                        |> Expect.equal false
                    )

            )

        describe
            "when there is an external error"
            (fun () ->

                it
                    "field error is External"
                    (fun () ->
                        fill externalErrorString
                        |> withFieldError (Expect.equal (Some(Error.External "External error")))
                    )

                it
                    "result is an External error"
                    (fun () ->
                        fill externalErrorString
                        |> fun filledForm -> filledForm.Result
                        |> Expect.equal (Error(Error.External "External error", []))
                    )

                it
                    "form is not empty"
                    (fun () ->
                        fill externalErrorString
                        |> fun filledForm -> filledForm.IsEmpty
                        |> Expect.equal false
                    )

            )

    )
