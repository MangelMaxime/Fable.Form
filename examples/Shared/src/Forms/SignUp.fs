module Examples.Shared.Forms.SignUp

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Bulma.Fields
open Fable.Form.Simple.Fields.Html
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Sutil.Bulma.Fields
open Fable.Form.Simple.Fields.Html
#endif

// Domain used by the current exemple, in a real application, this should be in a separate file
// But to make the example easier to understand, we keep it here
//
// If you are not familiar with the domain logic, you can ignore this part
// and focus on the form part
//
// Later on, you can come back to this part to understand how the domain logic
// is used in the form.
//
// The "Designing with types" series from F# for fun and profit is a great resource
// to learn more about this topic: https://fsharpforfunandprofit.com/series/designing-with-types/
[<AutoOpen>]
module Domain =

    type EmailAddress = private EmailAddress of string

    [<RequireQualifiedAccess>]
    module EmailAddress =

        let value (EmailAddress email) = email

        let tryParse (text: string) =
            if text.Contains("@") then
                Ok(EmailAddress text)
            else

                Error "The e-mail address must contain a '@' symbol"

    module rec User =

        type ValidEmail = private ValidEmail of EmailAddress
        type Name = private Name of string
        type Password = private Password of string

        let checkEmailAddress (email: EmailAddress) =
            promise {
                // Add some delay to simulate a Server request
                do! Promise.sleep 1000

                if (EmailAddress.value email) = "warded@mail.com" then
                    return Ok(ValidEmail.create email)
                else
                    return Error "The e-mail address is taken. Try this one: warded@mail.com"
            }

        module ValidEmail =

            let value (ValidEmail email) = EmailAddress.value email

            let validateEmailAddress (email: string) =
                match EmailAddress.tryParse email with
                | Ok email -> checkEmailAddress email

                | Error error -> Promise.reject (System.Exception error)

            let create (email: EmailAddress) : ValidEmail = ValidEmail email

        module Name =

            let value (Name name) = name

            let create (text: string) = Name text

            let tryParse (text: string) =
                if text.Length < 2 then
                    Error "The name must have at least 2 characters"

                else
                    Ok(Name text)

        module Password =

            let value (Password password) = password

            let create (text: string) = Password text

            let tryParse (text: string) =
                if text.Length < 4 then
                    Error "The password must have at least 4 characters"

                else
                    Ok(Password text)

        type User =
            {
                Email: ValidEmail
                Name: Name
                Password: Password
                IsProfilePublic: bool
            }

        let signUp (email: EmailAddress) (name: Name) (password: Password) (makePublic: bool) =

            checkEmailAddress email
            |> Promise.mapResult (fun validEmail ->
                {
                    Email = validEmail
                    Name = name
                    Password = password
                    IsProfilePublic = makePublic
                }
            )

/// <summary>
/// Type use to represent a FieldError
///
/// It consists of two fields:
/// - Value: which represent the value of the field when the error is to be shown
/// - Error: The error message to display
/// </summary>
type FieldError =
    {
        Value: string
        Error: string
    }

// Small helper module making it easier to create a FieldError
module FieldError =

    let inline create value error =
        {
            Value = value
            Error = error
        }

/// <summary>
/// Type used to represents all the external error handled by our form
/// </summary>
type FormErrors =
    {
        Email: FieldError option
    }

/// <summary>
/// Type used to represents the result of the form. When the form is submitted this is what we are returned
/// </summary>
type FormResult =
    {
        Email: EmailAddress
        Password: User.Password
        Name: User.Name
        MakePublic: bool
    }

/// <summary>
/// Represents the value of the form, it also have an <c>Errors</c> field to store the external errors
/// </summary>
[<NoComparison>]
type Values =
    {
        Email: string
        Password: string
        RepeatPassword: string
        Name: string
        MakePublic: RadioField.OptionItem option
        Errors: FormErrors
    }

[<NoComparison>]
type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // Used when the form has been submitted with success
    | SignedUp of User.User

[<NoComparison>]
type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<Values>
    // Message sent when the form is submitted
    | SignUp of FormResult
    // Result of a sign up attenpt. It will either be Ok or contains an error message representing the external error for the email field.
    | SignupAttempted of Result<User.User, string>
    // Message sent when the user ask to reset the demo
    | ResetTheDemo

let init =
    {
        Email = ""
        Password = ""
        RepeatPassword = ""
        Name = ""
        MakePublic = None
        Errors =
            {
                Email = None
            } // At first, there is no external error
    }
    |> Form.View.idle // By default, set the form in idle mode

let private convertMakePublicOptionToBool (makePublic: string) =
    match makePublic with
    | "option-yes" -> true
    | "option-no"
    | _ -> false

let form: Form<Values, FormResult> =
    let emailField =
        Form.textField
            {
                Parser = EmailAddress.tryParse
                Value = fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with
                            Email = newValue
                        }
                Error =
                    // Here, we have a bit of custom logic compared to the other field
                    // because we can have external error for this field
                    fun
                        {
                            Email = email
                            Errors = errors
                        } ->
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
                    TextField.create "email"
                    |> TextField.withLabel "Email"
                    |> TextField.withPlaceholder "some@email.com"
            }

    let nameField =
        Form.textField
            {
                Parser = User.Name.tryParse
                Value = fun values -> values.Name
                Update =
                    fun newValue values ->
                        { values with
                            Name = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    TextField.create "name"
                    |> TextField.withLabel "Name"
                    |> TextField.withPlaceholder "Your name"
            }

    let passwordField =
        Form.passwordField
            {
                Parser = User.Password.tryParse
                Value = fun values -> values.Password
                Update =
                    fun newValue values ->
                        { values with
                            Password = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    PasswordField.create "password"
                    |> PasswordField.withLabel "Password"
                    |> PasswordField.withPlaceholder "Your password"
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
                        PasswordField.create "repeat-password"
                        |> PasswordField.withLabel "Repeat password"
                        |> PasswordField.withPlaceholder "Your password again..."
                }
        )

    let makePublicField =
        Form.radioField
            {
                Parser =
                    fun value ->
                        match value with
                        | None -> Ok false
                        | Some value -> convertMakePublicOptionToBool value.Key |> Ok
                Value = fun values -> values.MakePublic
                Update =
                    fun newValue values ->
                        { values with
                            MakePublic = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    RadioField.create "make-public"
                    |> RadioField.withLabel "Make your profile public ?"
                    |> RadioField.withBasicsOptions [
                        "option-yes", "Yes"
                        "option-no", "No"
                    ]
            }

    let onSubmit email name password makePublic =
        {
            Email = email
            Password = password
            Name = name
            MakePublic = makePublic
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

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "Sign up"
        Route = SharedRouter.Route.SignUp
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
