module Page.Login

open Warded.Simple
open Elmish
open Feliz
open Feliz.Bulma

type EmailAddress = EmailAddress of string

type FormValues =
    {
        Email : string
        Password : string
        RememberMe : bool
    }

type Model =
    Form.View.Model<FormValues>

type Msg =
    | FormChanged of Model
    | LogIn of EmailAddress * string * bool

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
    | FormChanged newModel ->
        printfn "%A" newModel.Values

        newModel
        , Cmd.none

    | LogIn (email, password, rememberMe) ->
        { model with
            State = Form.View.Success "You have been logged in successfully"
        }
        , Cmd.none

let form : Form.Form<FormValues, Msg> =
    let emailField =
        Form.textField
            {
                Parser =
                    fun value ->
                        if value.Contains("@") then
                            Ok value
                        else
                            Error "The e-mail address must contain a '@' symbol"
                Value = (fun values -> values.Email)
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
                Parser = Ok
                Value = (fun values -> values.Password)
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

    let rememberMe =
        Form.checkboxField
            {
                Parser = Ok
                Value = (fun values -> values.RememberMe)
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

    let formOutput =
        fun email password rememberMe ->
            LogIn (EmailAddress email, password, rememberMe)

    Form.succeed formOutput
        |> Form.append emailField
        |> Form.append passwordField
        |> Form.append rememberMe

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            Dispatch = dispatch
            OnChange = FormChanged
            Action = "Submit"
            Loading = "Loading"
            Validation = Form.View.Validation.ValidateOnSubmit
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
