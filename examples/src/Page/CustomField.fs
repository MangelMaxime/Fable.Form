module Page.CustomField.Component

open Elmish
open Fable.Form.Simple.View
open Fable.Form.Simple.Bulma
open Feliz
open MyForm
// open Fable.Form.Simple

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values = { UserName: string; Signature: string }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the Login example, we just need to keep track of the Form model state
/// </summary>
type Model = Form.View.Model<Values>

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occur in the form
    | FormChanged of Model
    // Used when the user submit the form
    | Enter of string * string

let init () =
    { UserName = ""; Signature = "" } |> Form.View.idle, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel -> newModel, Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    | Enter(userName, signature) ->
        printfn "User name: %s, Signature: %s" userName signature
        // For this example, we just set a message in the Form view
        { model with
            State = Form.View.Success "TODO"
        },
        Cmd.none

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form.Form<Values, Msg, _> =
    let userNameField =
        Form.textField
            {
                Parser = Ok
                Value = _.UserName
                Update = fun newValue values -> { values with UserName = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "UserName"
                        Placeholder = ""
                        HtmlAttributes = [ prop.autoComplete "email" ]
                    }
            }

    let acceptTermsField =
        Form.signatureField
            {
                Parser = Ok
                Value = _.Signature
                Update = fun newValue values -> { values with Signature = newValue }
                Error = fun _ -> None
                Attributes = { Label = "Accept the terms" }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun email rememberMe -> Enter(email, rememberMe)

    Form.succeed onSubmit
    |> Form.append userNameField
    |> Form.append acceptTermsField

let view (model: Model) (dispatch: Dispatch<Msg>) =
    MyForm.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = Form.View.Action.SubmitOnly "Sign in"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        model

let information: DemoInformation.T =
    {
        Title = "Custom Field"
        Route = Router.Route.CustomField
        Description =
            "This example shows how to create your own custom field if the default fields are not enough for your needs."
        Remark = None
        Code =
            """
Learn more by looking at the source code of the custom field component.
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
