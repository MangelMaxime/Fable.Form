module Examples.Shared.Forms.FormList

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
#endif

#if EXAMPLE_LIT
open Fable.Form.Simple.Lit.Bulma
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
#endif

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
    {
        Name: string
        Books: BookValues list
    }

let init =
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

let private bookForm (index: int) =
    let titleField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Title
                Update =
                    fun newValue values ->
                        { values with
                            Title = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = $"book-title-{index}"
                        Label = "Name of book #" + string (index + 1)
                        Placeholder = ""
                        AutoComplete = None
                    }
            }

    let authorField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Author
                Update =
                    fun newValue values ->
                        { values with
                            Author = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = $"book-author-{index}"
                        Label = "Author of book #" + string (index + 1)
                        Placeholder = ""
                        AutoComplete = None
                    }
            }

    let summary =
        Form.textareaField
            {
                Parser = Ok
                Value = fun values -> values.Summary
                Update =
                    fun newValue values ->
                        { values with
                            Summary = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = $"book-summary-{index}"
                        Label = "Summary of book #" + string (index + 1)
                        Placeholder = ""
                        AutoComplete = None
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
let form: Form<Values, _> =
    let nameField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with
                            Name = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "name"
                        Label = "Name"
                        Placeholder = "Your name"
                        AutoComplete = None
                    }
            }

    let onSubmit name books = (name, books)

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
                Update =
                    fun newValue values ->
                        { values with
                            Books = newValue
                        }
                Attributes =
                    {
                        Label = "Books"
                        Add = Some "Add book"
                        Delete = Some "Remove book"
                    }
            }
            bookForm
    )

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "Form list"
        Remark = None
        Route = SharedRouter.Route.FormList
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
