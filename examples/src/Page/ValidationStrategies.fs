module Page.ValidationStrategies

open Warded.Simple
open Elmish
open Feliz
open Feliz.Bulma

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        ValidationStrategy : string
        Email : string
        Password : string
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
    | Submit of EmailAddress.T * User.Password.T

let init () =
    {
        ValidationStrategy = "onBlur"
        Email = ""
        Password = ""
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
    | Submit (email, password) ->
        // For the example, we just set a message in the Form view
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
let form : Form.Form<Values, Msg> =
    let validationStrategiesField =
        Form.radioField
            {
                Parser = Ok
                Value =
                    fun values -> values.ValidationStrategy
                Update =
                    fun newValue values ->
                        { values with ValidationStrategy = newValue }
                Error =
                    always None
                Attributes =
                    {
                        Label = "Validation strategy"
                        Options =
                            [
                                "onSubmit", "Validate on form submit"
                                "onBlur", "Validate on field blur"
                            ]
                    }

            }

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
                    }
            }

    let passwordField =
        Form.passwordField
            {
                Parser =
                    User.Password.tryParse
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
                    }
            }


    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let formOutput =
        fun _ email password ->
            Submit (email, password)

    Form.succeed formOutput
        |> Form.append validationStrategiesField
        |> Form.append emailField
        |> Form.append passwordField

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = "Submit"
            Loading = "Loading"
            Validation =
                if model.Values.ValidationStrategy = "onSubmit" then
                    Form.View.ValidateOnSubmit
                else
                    Form.View.ValidateOnBlur
        }
        form
        model

let code =
    """
Form.succeed formOutput
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append rememberMe
    """

let githubLink =
    Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__

let title =
    "Validation strategies"

let remark =
    Bulma.content [
        text.hasTextCentered

        prop.children [
            Html.text "This feature depends on the view implementation, here it is offered by "
            Html.b "Fable.Form.Simple"
            Html.text " package"
        ]
    ]
    |> Some


