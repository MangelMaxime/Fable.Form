module Examples.Shared.Forms.File

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Bulma.Fields
#endif

#if EXAMPLE_LIT
open Fable.Form.Simple.Lit.Bulma
open Fable.Form.Simple.Lit.Bulma.Fields
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma.Fields
#endif

/// <summary>
/// Type used to represent the form values
/// </summary>
[<NoComparison>]
type Values =
    {
        ArchiveName: string
        Files: Browser.Types.File array
    }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the File example, we just need to keep track of the Form model state
/// </summary>
[<NoComparison>]
type Model =
    // The form is being filled
    | FillingForm of Form.View.Model<Values>
    // The form has been submitted and the files have been printed in the console
    | FileUploaded of string list

let init =
    {
        ArchiveName = ""
        Files = Array.empty
    }
    |> Form.View.idle

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, Browser.Types.File array> =
    Form.fileField
        {
            Parser = Ok
            Value = fun values -> values.Files
            Update =
                fun newValue values ->
                    { values with
                        Files = newValue
                    }
            Error = fun _ -> None
            Attributes =
                {
                    FieldId = "invoices"
                    Label = "Invoices"
                    InputLabel = "Choose one or more PDF files"
                    Accept =
                        FileField.FileType.Specific [
                            ".pdf"
                        ]
                    FileIconClassName = FileField.DefaultIcon
                    Multiple = true
                }
        }

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "File upload"
        Route = SharedRouter.Route.File
        Description = "Showcases how to create a form with a file input"
        Remark = None
        Code =
            """
Form.fileField
    {
        // ...
    }
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
