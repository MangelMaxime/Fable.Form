module Page.SignUp.Component

open Fable.Form.Simple
open Fable.Form.Simple.Feliz.Bulma
open Elmish
open Feliz
open Feliz.Bulma

type FieldError =
    {
        Value : string
        Error : string
    }

module FieldError =

    let create value error =
        {
            Value = value
            Error = error
        }

type FormErrors =
    {
        Email : FieldError option
    }

type FormResult =
    {
        Email : EmailAddress.T
        Password : User.Password.T
        Name : User.Name.T
        MakePublic : bool
    }

type Values =
    {
        Email : string
        Password : string
        RepeatPassword : string
        Name : string
        MakePublic : string
        Errors : FormErrors
    }

type Model =
    | FillingForm of Form.View.Model<Values>
    | SignedUp of User.T


type Msg =
    | FormChanged of Form.View.Model<Values>
    | SignUp of FormResult
    | SignupAttempted of Result<User.T, string>

let init () =
    {
        Email = ""
        Password = ""
        RepeatPassword = ""
        Name = ""
        MakePublic = ""
        Errors = { Email = None }
    }
    |> Form.View.idle
    |> FillingForm
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    | FormChanged formModel ->
        match model with
        | FillingForm _ ->
            FillingForm formModel
            , Cmd.none

        | _ ->
            model
            , Cmd.none

    | SignUp formResult ->
        match model with
        | FillingForm formModel ->
            let signUp () =
                User.signUp
                    formResult.Email
                    formResult.Name
                    formResult.Password
                    formResult.MakePublic

            formModel
            |> Form.View.setLoading
            |> FillingForm
            , Cmd.OfPromise.perform signUp () SignupAttempted

        | _ ->
            model,
            Cmd.none

    | SignupAttempted (Ok user) ->
        SignedUp user
        , Cmd.none

    | SignupAttempted (Error error) ->
        match model with
        | FillingForm formModel ->
            let values =
                formModel.Values

            let errors =
                values.Errors

            { formModel with
                State = Form.View.Idle
                Values =
                    { values with
                        Errors =
                            { errors with
                                Email = Some (FieldError.create values.Email error)
                            }
                    }
            }
            |> FillingForm
            , Cmd.none

        | _ ->
            model
            , Cmd.none

let convertMakePublicOptionToBool (makePublic : string) =
    match makePublic with
    | "option-yes" -> true
    | "option-no"
    | _ -> false

let form : Form.Form<Values, Msg> =
    let emailField =
        Form.textField
            {
                Parser =
                    EmailAddress.tryParse >> Result.map EmailAddress.toString
                Value =
                    fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with Email = newValue }
                Error =
                    fun { Email = email; Errors = errors } ->
                        match errors.Email with
                        | Some emailError ->
                            if email = emailError.Value then
                                Some emailError.Error
                            else
                                None

                        | None ->
                            None
                Attributes =
                    {
                        Label = "Email"
                        Placeholder = "some@email.com"
                    }
            }

    let nameField =
        Form.textField
            {
                Parser = User.Name.tryParse >> Result.map User.Name.toString
                Value = fun values -> values.Name
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

    let passwordField =
        Form.passwordField
            {
                Parser =
                    User.Password.tryParse >> Result.map User.Password.toString
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
                Value =
                    fun values -> values.MakePublic
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

    let formOutput =
        (fun email name password makePublic ->
            SignUp
                {
                    Email = EmailAddress.create email
                    Password = User.Password.create password
                    Name = User.Name.create name
                    MakePublic = convertMakePublicOptionToBool makePublic
                }
        )

    Form.succeed formOutput
        |> Form.append emailField
        |> Form.append nameField
        |> Form.append
            (
                Form.succeed (fun password _ -> password )
                |> Form.append passwordField
                |> Form.append repeatPasswordField
                |> Form.group
            )
        |> Form.append makePublicField

let view (model : Model) (dispatch : Dispatch<Msg>) =
    match model with
    | FillingForm Values ->

        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = "Submit"
                Loading = "Loading"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            Values

    | SignedUp user ->
        Html.text "User signed up"

let code =
    """
Form.succeed formOutput
    |> Form.append emailField
    |> Form.append nameField
    |> Form.append
        (
            Form.succeed (fun password _ -> password )
            |> Form.append passwordField
            |> Form.append repeatPasswordField
            |> Form.group
        )
    |> Form.append makePublicField
    """

let githubLink =
    Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__

let title =
    "Sign up"

let remark =
    None
