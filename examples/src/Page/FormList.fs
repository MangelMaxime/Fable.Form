module Page.FormList.Component

open Fable.Form.Simple
open Fable.Form.Simple.Feliz.Bulma
open Elmish

// Student
// Teacher

/// <summary>
/// Type used to represent a Book
/// </summary>
type Book =
    {
        Title : string
        Author : string
        Summary : string
    }

/// <summary>
/// Type used to represent a Book in the form
/// </summary>
type BookValues =
    {
        Title : string
        Author : string
        Summary : string
    }

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Name : string
        Books : BookValues list
    }

/// <summary>
/// Represents the model of your Elmish component
/// </summary>
type Model =
    Form.View.Model<Values>

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occure in the form
    | FormChanged of Model
    // Used when the user submit the form
    | Submit of string * Book list

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
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        newModel
        , Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    // | LogIn (email, password, rememberMe) ->
    //     // For the example, we just set a message in the Form view
    //     { model with
    //         State = Form.View.Success "You have been logged in successfully"
    //     }
    //     , Cmd.none

let bookForm (index : int) =
    let titleField =
        Form.textField
            {
                Parser = Ok
                Value =
                    fun values -> values.Title
                Update =
                    fun newValue values ->
                        { values with Title = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Name of book #" + string (index + 1)
                        Placeholder = ""
                    }
            }

    let authorField =
        Form.textField
            {
                Parser = Ok
                Value =
                    fun values -> values.Author
                Update =
                    fun newValue values ->
                        { values with Author = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Author of book #" + string (index + 1)
                        Placeholder = ""
                    }
            }

    let summary =
        Form.textareaField
            {
                Parser = Ok
                Value =
                    fun values -> values.Summary
                Update =
                    fun newValue values ->
                        { values with Summary = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Summary of book #" + string (index + 1)
                        Placeholder = ""
                    }
            }

    let formOutput title author summary =
        {
            Title = title
            Author = author
            Summary = summary
        } : Book


    Form.succeed formOutput
        |> Form.append titleField
        |> Form.append authorField
        |> Form.append summary

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form : Form.Form<Values, Msg> =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value =
                    fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with Name = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Name"
                        Placeholder = "Your name"
                    }
            }

    let formOutput name books =
        Submit (name, books)

    Form.succeed formOutput
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

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = "Submit"
            Loading = "Loading"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        model

let code =
    """
Form.succeed formOutput
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

let githubLink =
    Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__

let title =
    "Form list"

let remark =
    None
