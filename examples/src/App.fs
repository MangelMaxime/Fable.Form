module Examples

open Feliz
open Elmish


type Values =
    {
        Email : string
        Password : string
        // RememberMe : bool
    }

type Model =
    Form.View.Model<Values>

// type EmailAddress = string //EmailAddress of string

type Msg =
    | FormChanged of Model
    // | LogIn of EmailAddress * string * bool
    | LogIn of string * string

// let init =
//     {
//         Email = ""
//         Password = ""
//         RememberMe = false
//     }
//     |> Form.View.idle

let init () =
    {
        Email = ""
        Password = ""
        // RememberMe = false
    }
    |> Form.View.idle
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    | FormChanged newModel ->
        newModel
        , Cmd.none

    // | LogIn (emailAddress, password, rememberMe) ->
    | LogIn (emailAddress, password) ->
        { model with
            State = Form.View.Success "You have been logged in successfully"
        }
        , Cmd.none

// : Form.Form<Values, (EmailAddress * string * bool -> Msg), obj> =

let emailField =
    Form.Core.textField
        {
            Parser = Ok
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
    Form.Core.textField
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
                    Label = "Email"
                    Placeholder = "some@email.com"
                }
        }

let form : Form.Core.Form<Values, Msg> =
    Form.Core.succeed (fun email password -> LogIn (email, password))
        |> Form.Core.append emailField
        |> Form.Core.append passwordField

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

open Elmish.React

#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.withReactSynchronous "root"
|> Program.run
