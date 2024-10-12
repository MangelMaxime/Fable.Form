module Examples.Shared.Forms.Login

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma
#endif

#if EXAMPLE_LIT
open Fable.Form.Simple.Lit.Bulma
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
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
    type Password = private Password of string

    [<RequireQualifiedAccess>]
    module EmailAddress =

        let value (EmailAddress email) = email

        let tryParse (text: string) =
            if text.Contains("@") then
                Ok(EmailAddress text)
            else

                Error "The e-mail address must contain a '@' symbol"

    [<RequireQualifiedAccess>]
    module Password =

        let value (Password password) = password

        let tryParse (text: string) =
            if text.Length >= 4 then
                Ok(Password text)
            else
                Error "The password must have at least 4 characters"

/// <summary>
/// Type used to represent the result of the form
/// </summary>
type FormResult =
    {
        Email: EmailAddress
        Password: Password
        RememberMe: bool
    }

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Email: string
        Password: string
        RememberMe: bool
    }

let init =
    {
        Email = ""
        Password = ""
        RememberMe = false
    }
    |> Form.View.idle

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
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
                Error = fun _ -> None
                Attributes =
                    TextField.create "email"
                    |> TextField.withLabel "Email"
                    |> TextField.withPlaceholder "some@email.com"
                    |> TextField.withAutoComplete "email"
            }

    let passwordField =
        Form.passwordField
            {
                Parser = Password.tryParse
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
                    |> PasswordField.withAutoComplete "current-password"
            }

    let rememberMe =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.RememberMe
                Update =
                    fun newValue values ->
                        { values with
                            RememberMe = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "remember-me"
                        Text = "Remember me"
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit =
        fun email password rememberMe ->
            {
                Email = email
                Password = password
                RememberMe = rememberMe
            }
            : FormResult

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append rememberMe

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "Login"
        Route = SharedRouter.Route.Login
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
