module Page.Composability.WithConfiguration

open Warded.Simple
open Elmish
open Page.Composability.WithConfiguration

// Student
// Teacher

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Name : string
        Address : AddressForm.Values
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
    | Submit of User.Name.T * Address.T

let init () =
    {
        Name = ""
        Address = AddressForm.blank
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
                Parser =
                    User.Name.tryParse
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

    let formOutput user address =
        Submit (user, address)

    Form.succeed formOutput
        |> Form.append nameField
        |> Form.append (
            AddressForm.form
                {
                    Value =
                        fun values -> values.Address
                    Update =
                        fun newValue values ->
                            { values with Address = newValue }
                }
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
        AddressForm.form
            {
                Get =
                    fun values -> values.Address
                Update =
                    fun newValue values ->
                        { values with Address = newValue }
            }
    )
    """

let githubLink =
    Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__