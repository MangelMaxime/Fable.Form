module Page.FormList.Component

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Logic
open Fable.Form.Simple.Bulma

/// <summary>
/// Type used to represent a Book in the domain logic
/// <para>This is how a book is represented outside of the form</para>
/// </summary>
type Book =
    {
        Title: string
        Author: string
        Summary: string
    }

/// <summary>
/// Type used to represent a Book in the form
/// </summary>
type BookValues =
    {
        Title: string
        Author: string
        Summary: string
    }

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    { Name: string; Books: BookValues list }

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // Used when the form has been submitted with success
    | FormFilled of string * Book list

type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<Values>
    // Message sent when the form is submitted
    | Submit of string * Book list
    // Message sent when the user ask to reset the demo
    | ResetDemo

let init () =
    {
        Name = ""
        Books =
            [
                {
                    Title = "The warded man"
                    Author = "Peter V. Brett"
                    Summary =
                        "The Painted Man, book one of the Demon Cycle, is a captivating and thrilling fantasy adventure, pulling the reader into a world of demons, darkness and heroes."
                }
            ]
    }
    |> Form.View.idle
    |> FillingForm,
    Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | FormFilled _ -> model, Cmd.none

    | Submit(name, books) ->
        match model with
        | FillingForm _ -> FormFilled(name, books), Cmd.none

        | FormFilled _ -> model, Cmd.none

    | ResetDemo -> init ()

let private bookForm (index: int) =
    let titleField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Title
                Update = fun newValue values -> { values with Title = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Name of book #" + string (index + 1)
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    let authorField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Author
                Update = fun newValue values -> { values with Author = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Author of book #" + string (index + 1)
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    let summary =
        Form.textareaField
            {
                Parser = Ok
                Value = fun values -> values.Summary
                Update = fun newValue values -> { values with Summary = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Summary of book #" + string (index + 1)
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    let onSubmit title author summary =
        {
            Title = title
            Author = author
            Summary = summary
        }
        : Book

    Form.succeed onSubmit
    |> Form.append titleField
    |> Form.append authorField
    |> Form.append summary

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let private form: Form.Form<Values, Msg, _> =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Name
                Update = fun newValue values -> { values with Name = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Name"
                        Placeholder = "Your name"
                        HtmlAttributes = []
                    }
            }

    let onSubmit name books = Submit(name, books)

    Form.succeed onSubmit
    |> Form.append nameField
    |> Form.append (
        Form.list
            {
                Default =
                    {
                        Title = ""
                        Author = ""
                        Summary = ""
                    }
                Value = fun values -> values.Books
                Update = fun newValue values -> { values with Books = newValue }
                Attributes =
                    {
                        Label = "Books"
                        Add = Some "Add book"
                        Delete = Some "Remove book"
                    }
            }
            bookForm
    )

// Function used to render a book when the form has been submitted
let private renderBook (rank: int) (book: Book) =
    Html.tr
        [
            Html.td [ Html.b (string (rank + 1)) ]
            Html.td book.Title
            Html.td book.Author
            Html.td book.Summary
        ]

// Function used to render the filled view (when the form has been submitted)
let private renderFilledView (name: string) (books: Book list) dispatch =
    Bulma.content
        [

            Bulma.message
                [
                    color.isSuccess

                    prop.children
                        [
                            Bulma.messageBody
                                [
                                    Html.text "Thank you "
                                    Html.b name
                                    Html.text " for creating those "
                                    Html.b (string (List.length books))
                                    Html.text " book(s)"
                                ]
                        ]

                ]

            Bulma.table
                [
                    table.isStriped
                    prop.className "is-vcentered-cells"

                    prop.children
                        [
                            Html.thead
                                [
                                    Html.tr
                                        [
                                            Html.th "#"
                                            Html.th "Title"
                                            Html.th "Author"
                                            Html.th "Description"
                                        ]
                                ]

                            Html.tableBody (List.mapi renderBook books)
                        ]
                ]

            Bulma.text.p
                [
                    text.hasTextCentered

                    prop.children
                        [
                            Bulma.button.button
                                [
                                    prop.onClick (fun _ -> dispatch ResetDemo)
                                    color.isPrimary

                                    prop.text "Reset the demo"
                                ]
                        ]
                ]

        ]

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = Form.View.Action.SubmitOnly "Submit"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | FormFilled(name, books) -> renderFilledView name books dispatch

let information: DemoInformation.T =
    {
        Title = "Form list"
        Remark = None
        Route = Router.Route.FormList
        Description = "A form where you can add and remove a list of forms"
        Code =
            """
Form.succeed onSubmit
    |> Form.append nameField
    |> Form.append (
        Form.list
            {
                Default =
                    {
                        Title = ""
                        Author = ""
                        Summary = ""
                    }
                Value =
                    fun values -> values.Books
                Update =
                    fun newValue values ->
                        { values with Books = newValue }
                Attributes =
                    {
                        Label = "Books"
                        Add = Some "Add book"
                        Delete = Some "Remove book"
                    }
            }
            bookForm
    )
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
