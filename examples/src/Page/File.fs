module Page.File.Component

open Elmish
open Fable.Core

open Fable.Form.Simple
open Fable.Form.Simple.Field
open Fable.Form.Simple.Bulma


/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Files : Browser.Types.File array
    }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the File example, we just need to keep track of the Form model state
/// </summary>
type Model =
    Form.View.Model<Values>

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occur in the form
    | FormChanged of Model
    // Used when the user submit the form
    | SendFiles of Browser.Types.File []

let init () =
    {
        Files = Array.empty
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
    | SendFiles files ->
        // For this example, we just print the file names in the console
        files |> Array.iter (fun f -> printfn $"File: {f.name}")

        { model with
            State = Form.View.Success "Files names printed in the console"
        }
        , Cmd.none

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form : Form.Form<Values, Msg, _> =
    let fileField =
        Form.fileField
            {
                Parser =
                    Ok
                Value =
                    fun values -> values.Files
                Update =
                    fun newValue values ->
                        { values with Files = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Choose some files"
                        Accept = FileField.FileType.Specific [".pdf"]
                        FileIconClassName = FileField.FileIconClassName.Default
                        Multiple = true
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit =
        fun (files: Browser.Types.File array) ->
            SendFiles files

    Form.succeed onSubmit
    |> Form.append fileField

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = Form.View.Action.SubmitOnly "Send"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        model

let information : DemoInformation.T =
    {
        Title = "File upload"
        Route = Router.Route.File
        Description = "A simple file upload form"
        Remark = None
        Code =
            """
Form.succeed onSubmit
|> Form.append fileField
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
