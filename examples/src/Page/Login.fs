module Page.Login.Component

open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Feliz

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Email : string
        Password : string
        RememberMe : bool
    }

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the Login example, we just need to keep track of the Form model state
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
    | LogIn of EmailAddress.T * string * bool

let init () =
    {
        Email = ""
        Password = ""
        RememberMe = false
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
    | LogIn (_email, _password, _rememberMe) ->
        printfn "%A" _password
        // For this example, we just set a message in the Form view
        { model with
            State = Form.View.Success "You have been logged in successfully"
        }
        , Cmd.none


/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form : Form.Form<Values, Msg, _> =
    let emailField =
        Form.textField
            {
                Parser =
                    EmailAddress.tryParse
                Value =
                    fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with Email = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Email"
                        Placeholder = "some@email.com"
                        HtmlAttributes = [
                            prop.autoComplete "email"
                        ]
                    }
            }

    let passwordField =
        Form.passwordField
            {
                Parser = Ok
                Value =
                    fun values -> values.Password
                Update =
                    fun newValue values ->
                        { values with Password = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Password"
                        Placeholder = "Your password"
                        HtmlAttributes = [
                            prop.autoComplete "current-password"
                        ]
                    }
            }

    let rememberMe =
        Form.checkboxField
            {
                Parser = Ok
                Value =
                    fun values -> values.RememberMe
                Update =
                    fun newValue values ->
                        { values with RememberMe = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Text = "Remember me"
                    }
            }


    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit =
        fun email password rememberMe ->
            LogIn (email, password, rememberMe)

    Form.succeed onSubmit
        |> Form.append emailField
        |> Form.append passwordField
        |> Form.append rememberMe

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = Form.View.Action.SubmitOnly "Sign in"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        model

let information : DemoInformation.T =
    {
        Title = "Login"
        Route = Router.Route.Login
        Description = "A simple login form with 3 fields"
        Remark = None
        Code =
            """
Form.succeed onSubmit
|> Form.append emailField
|> Form.append passwordField
|> Form.append rememberMe
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
