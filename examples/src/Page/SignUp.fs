module Page.SignUp.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.View
open Fable.Form.Simple.Bulma

/// <summary>
/// Type use to represent a FieldError
///
/// It consists of two fields:
/// - Value: which represent the value of the field when the error is to be shown
/// - Error: The error message to display
/// </summary>
type FieldError = { Value: string; Error: string }

// Small helper module making it easier to create a FieldError
module FieldError =

    let inline create value error = { Value = value; Error = error }

/// <summary>
/// Type used to represents all the external error handled by our form
/// </summary>
type FormErrors = { Email: FieldError option }

/// <summary>
/// Type used to represents the result of the form. When the form is submitted this is what we are returned
/// </summary>
type FormResult =
    {
        Email: EmailAddress.T
        Password: User.Password.T
        Name: User.Name.T
        MakePublic: bool
    }

/// <summary>
/// Represents the value of the form, it also have an <c>Errors</c> field to store the external errors
/// </summary>
type Values =
    {
        Email: string
        Password: string
        RepeatPassword: string
        Name: string
        MakePublic: string
        Errors: FormErrors
    }

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // Used when the form has been submitted with success
    | SignedUp of User.T

type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<Values>
    // Message sent when the form is submitted
    | SignUp of FormResult
    // Result of a sign up attenpt. It will either be Ok or contains an error message representing the external error for the email field.
    | SignupAttempted of Result<User.T, string>
    // Message sent when the user ask to reset the demo
    | ResetTheDemo

let init () =
    {
        Email = ""
        Password = ""
        RepeatPassword = ""
        Name = ""
        MakePublic = ""
        Errors = { Email = None } // At first, there is no external error
    }
    |> Form.View.idle // By default, set the form in idle mode
    |> FillingForm,
    Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | FormChanged formModel ->
        match model with
        | FillingForm _ -> FillingForm formModel, Cmd.none

        | _ -> model, Cmd.none

    | SignUp formResult ->
        match model with
        | FillingForm formModel ->
            let signUp () =
                User.signUp
                    formResult.Email
                    formResult.Name
                    formResult.Password
                    formResult.MakePublic

            formModel |> Form.View.setLoading |> FillingForm,
            Cmd.OfPromise.perform signUp () SignupAttempted

        | _ -> model, Cmd.none

    | SignupAttempted(Ok user) -> SignedUp user, Cmd.none

    // The signup attempt failed, we are going to update the external error received
    | SignupAttempted(Error error) ->
        match model with
        | FillingForm formModel ->
            let values = formModel.Values

            let errors = values.Errors

            { formModel with
                State = Form.View.Idle
                Values =
                    { values with
                        Errors =
                            { errors with
                                Email = Some(FieldError.create values.Email error)
                            }
                    }
            }
            |> FillingForm,
            Cmd.none

        | _ -> model, Cmd.none

    | ResetTheDemo -> init ()

let private convertMakePublicOptionToBool (makePublic: string) =
    match makePublic with
    | "option-yes" -> true
    | "option-no"
    | _ -> false

let private form: Form.Form<Values, Msg, _> =
    let emailField =
        Form.textField
            {
                Parser = EmailAddress.tryParse
                Value = fun values -> values.Email
                Update = fun newValue values -> { values with Email = newValue }
                Error =
                    // Here, we have a bit of custom logic compared to the other field
                    // because we can have external error for this field
                    fun { Email = email; Errors = errors } ->
                        match errors.Email with
                        // If we have an external error
                        | Some emailError ->
                            // And the value of the field is the same as the one associated with the external error
                            if email = emailError.Value then
                                // Return the external error
                                Some emailError.Error
                            else
                                None

                        | None -> None
                Attributes =
                    {
                        Label = "Email"
                        Placeholder = "some@email.com"
                        HtmlAttributes = []
                    }
            }

    let nameField =
        Form.textField
            {
                Parser = User.Name.tryParse
                Value = fun values -> values.Name
                Update = fun newValue values -> { values with Name = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Name"
                        Placeholder = "Your name"
                        HtmlAttributes = []
                    }
            }

    let passwordField =
        Form.passwordField
            {
                Parser = User.Password.tryParse
                Value = fun values -> values.Password
                Update = fun newValue values -> { values with Password = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Password"
                        Placeholder = "Your password"
                        HtmlAttributes = []
                    }
            }

    let repeatPasswordField =
        Form.meta (fun values ->
            Form.passwordField
                {
                    Parser =
                        fun value ->
                            if value = values.Password then
                                Ok()

                            else
                                Error "The passwords do not match"
                    Value = fun values -> values.RepeatPassword
                    Update =
                        fun newValue values_ ->
                            { values_ with
                                RepeatPassword = newValue
                            }
                    Error = fun _ -> None
                    Attributes =
                        {
                            Label = "Repeat password"
                            Placeholder = "Your password again..."
                            HtmlAttributes = []
                        }
                }
        )

    let makePublicField =
        Form.radioField
            {
                Parser = Ok
                Value = fun values -> values.MakePublic
                Update = fun newValue values -> { values with MakePublic = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Make your profile public ?"
                        Options = [ "option-yes", "Yes"; "option-no", "No" ]
                    }
            }

    let onSubmit email name password makePublic =
        SignUp
            {
                Email = email
                Password = password
                Name = name
                MakePublic = convertMakePublicOptionToBool makePublic
            }

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append nameField
    |> Form.append (
        Form.succeed (fun password _ -> password)
        |> Form.append passwordField
        |> Form.append repeatPasswordField
        |> Form.group
    )
    |> Form.append makePublicField

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [ Html.td leftValue; Html.td rightValue ]

let private renderSignedUpView (user: User.T) dispatch =
    Bulma.content
        [

            Bulma.message
                [
                    color.isSuccess

                    prop.children
                        [
                            Bulma.messageBody
                                [ prop.text "User signed up with the following informations" ]
                        ]
                ]

            Bulma.table
                [
                    table.isStriped

                    prop.children
                        [
                            Html.thead [ Html.tr [ Html.th "Field"; Html.th "Value" ] ]

                            Html.tableBody
                                [
                                    renderRow "Email" (User.ValidEmail.toString user.Email)
                                    renderRow "Name" (User.Name.toString user.Name)
                                    renderRow "Password" (User.Password.toString user.Password)
                                    renderRow "Is profil public?" (string user.IsProfilePublic)
                                ]
                        ]

                ]

            Bulma.text.p
                [
                    text.hasTextCentered

                    prop.children
                        [
                            Bulma.button.button
                                [
                                    prop.onClick (fun _ -> dispatch ResetTheDemo)
                                    color.isPrimary

                                    prop.text "Reset the demo"
                                ]
                        ]
                ]

        ]

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = Form.View.Action.SubmitOnly "Sign up"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | SignedUp user -> renderSignedUpView user dispatch

let information: DemoInformation.T =
    {
        Title = "Sign up"
        Route = Router.Route.SignUp
        Description = "A form demonstrating how to handle external errors"
        Remark = None
        Code =
            """
Form.succeed onSubmit
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
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
