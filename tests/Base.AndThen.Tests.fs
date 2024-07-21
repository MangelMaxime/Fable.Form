module Tests.Base_AndThen

open Mocha
open Fable.Form
open Fable.Form.Simple
open Node

type ContentAction =
    | CreatePost of string
    | CreateQuestion of string * string

type ContentType =
    | Post
    | Question

type Values =
    {
        Title: string
        Body: string
        ContentType: string
    }

let titleField: Form.Form<Values, string, obj> =
    Form.textField
        {
            Parser = Ok
            Value = fun values -> values.Title
            Update = fun newValue values -> { values with Title = newValue }
            Error = always None
            Attributes =
                {
                    Label = "Title"
                    Placeholder = "Write a title"
                    HtmlAttributes = []
                }
        }

let bodyField =
    Form.textField
        {
            Parser = Ok
            Value = fun values -> values.Body
            Update = fun newValue values -> { values with Body = newValue }
            Error = always None
            Attributes =
                {
                    Label = "Body"
                    Placeholder = "Write a body"
                    HtmlAttributes = []
                }
        }

let contentTypeError = "Invalid content type"

let contentTypeField =
    Form.selectField
        {
            Parser =
                function
                | "post" -> Ok Post
                | "question" -> Ok Question
                | _ -> Error contentTypeError
            Value = fun values -> values.ContentType
            Update = fun newValue values -> { values with ContentType = newValue }
            Error = always None
            Attributes =
                {
                    Label = "Body"
                    Placeholder = "Write a body"
                    Options = [ "post", "Post"; "question", "Question" ]
                }
        }

let form =
    contentTypeField
    |> Base.andThen (
        function
        | Post -> Base.succeed CreatePost |> Base.append bodyField

        | Question ->
            Base.succeed (fun title body -> CreateQuestion(title, body))
            |> Base.append titleField
            |> Base.append bodyField
    )

let fill = Base.fill form

let emptyChildQuestionValues =
    {
        ContentType = "question"
        Title = ""
        Body = ""
    }

let validQuestionValues =
    {
        ContentType = "question"
        Title = "Some title"
        Body = "Some body"
    }

let emptyParentValues =
    {
        ContentType = ""
        Title = ""
        Body = ""
    }

let invalidParentValues =
    {
        ContentType = "invalid"
        Title = ""
        Body = ""
    }

describe
    "Base.andThen"
    (fun () ->

        describe
            "when the parent fields are valid"
            (fun () ->

                it
                    "contains the parent and the child fields"
                    (fun () ->
                        let filledForm = fill emptyChildQuestionValues

                        Assert.strictEqual (filledForm.Fields.Length, 3)
                    )

                describe
                    "and when the child fields are valid"
                    (fun () ->

                        it
                            "results in the correct output"
                            (fun () ->
                                let filledForm = fill validQuestionValues

                                Assert.deepStrictEqual (
                                    filledForm.Result,
                                    Ok(CreateQuestion("Some title", "Some body"))
                                )
                            )

                    )

                describe
                    "and when the child fields are invalid"
                    (fun () ->

                        it
                            "results in the errors of the child fields"
                            (fun () ->
                                let filledForm = fill emptyChildQuestionValues

                                Assert.deepStrictEqual (
                                    filledForm.Result,
                                    Error(Error.RequiredFieldIsEmpty, [ Error.RequiredFieldIsEmpty ])
                                )
                            )

                    )

                it
                    "is not empty"
                    (fun () ->
                        let filledForm = fill emptyChildQuestionValues

                        Assert.deepStrictEqual (filledForm.IsEmpty, false)
                    )

            )

        describe
            "when the parent fields are empty"
            (fun () ->

                it
                    "is empty"
                    (fun () ->
                        let filledForm = fill emptyParentValues

                        Assert.strictEqual (filledForm.IsEmpty, true)
                    )

            )

        describe
            "when the parent fields is invalid"
            (fun () ->

                it
                    "contains only the parent fields"
                    (fun () ->
                        let filledForm = fill invalidParentValues

                        Assert.strictEqual (filledForm.Fields.Length, 1)
                    )

                it
                    "results in only the parent errors"
                    (fun () ->
                        let filledForm = fill invalidParentValues

                        Assert.deepStrictEqual (
                            filledForm.Result,
                            Error(Error.ValidationFailed contentTypeError, [])
                        )
                    )

                it
                    "is not empty"
                    (fun () ->
                        let filledForm = fill invalidParentValues

                        Assert.strictEqual (filledForm.IsEmpty, false)
                    )

            )

    )
