module Page.SignUp

open Warded.Simple
open Elmish
open Feliz
open Feliz.Bulma

type FormResult =
    {
        Email : string
        Password : string
        AcceptTerms : bool
        MakePublic : string
    }

type FormValues =
    {
        Email : string
        Password : string
        RepeatPassword : string
        AcceptTerms : bool
        MakePublic : string
    }


type Model =
    Form.View.Model<FormValues>

// type EmailAddress = string //EmailAddress of string

type Msg =
    | FormChanged of Model
    // | LogIn of EmailAddress * string * bool
    | LogIn of FormResult

let init () =
    {
        Email = ""
        Password = ""
        RepeatPassword = ""
        AcceptTerms = false
        MakePublic = ""
    }
    |> Form.View.idle
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    | FormChanged newModel ->
        printfn "%A" newModel.Values

        newModel
        , Cmd.none

    | LogIn values ->
        printfn "User login"
        { model with
            State = Form.View.Success "You have been logged in successfully"
        }
        , Cmd.none

// : Form.Form<Values, (EmailAddress * string * bool -> Msg), obj> =
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

    let repeatPasswordField =
        Form.meta
            (fun values ->
                Form.passwordField
                    {
                        Parser =
                            fun value ->
                                if value = values.Password then
                                    Ok ()

                                else
                                    Error "The passwords do not match"
                        Value = fun values -> values.RepeatPassword
                        Update =
                            fun newValue values_ ->
                                { values_ with RepeatPassword = newValue }
                        Error =
                            fun _ -> None
                        Attributes =
                            {
                                Label = "Repeat password"
                                Placeholder = "Your password again..."
                            }
                    }
            )

    let makePublicField =
        Form.radioField
            {
                Parser = Ok
                Value = (fun values -> values.MakePublic)
                Update =
                    fun newValue values ->
                        { values with MakePublic = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Make your profile public ?"
                        Options =
                            [
                                "option-yes", "Yes"
                                "option-no", "No"
                            ]
                    }
            }

    let termsAndConditionField =
        Form.checkboxField
            {
                Parser = Ok
                Value = (fun values -> values.AcceptTerms)
                Update =
                    fun newValue values ->
                        { values with AcceptTerms = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Text = "I agree with the terms and conditions"
                    }
            }

    let formOutput =
        (fun email password makePublic acceptTerms ->
            LogIn
                {
                    Email = email
                    Password = password
                    AcceptTerms = acceptTerms
                    MakePublic = makePublic
                }
        )

    Form.succeed formOutput
        |> Form.append emailField
        |> Form.append
            (
                Form.succeed (fun password _ -> password )
                |> Form.append passwordField
                |> Form.append repeatPasswordField
                |> Form.group
            )
        |> Form.append makePublicField
        |> Form.append termsAndConditionField

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
    |> Form.append
        (
            Form.succeed (fun password _ -> password )
            |> Form.append passwordField
            |> Form.append repeatPasswordField
            |> Form.group
        )
    |> Form.append makePublicField
    |> Form.append termsAndConditionField
    """
