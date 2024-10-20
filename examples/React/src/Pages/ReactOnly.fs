module Page.ReactOnly.Component

open Fable.Form.Simple
open Fable.Form.Simple.Fields.Html
open Fable.Form.Simple.Bulma
open Feliz

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

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, _> =
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
                Attributes = PasswordField.create "password" |> PasswordField.withLabel "Password"
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

[<ReactComponent>]
let View () =
    let formState, setFormState =
        {
            Email = ""
            Password = ""
            RememberMe = false
        }
        |> Form.View.idle
        |> React.useState

    let onSubmit =
        fun (_formResult: FormResult) ->
            { formState with
                State = Form.View.Success "You have been logged in successfully"
            }
            |> setFormState

    Form.View.asHtml
        {
            OnChange = setFormState
            OnSubmit = onSubmit
            Action = Form.View.Action.SubmitOnly "Sign in"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        formState

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "React only (no Elmish)"
        Route = Router.Route.FrameworkSpecific Router.ReactRoute.ReactOnly
        Description = "Demonstrates how to use the form component using React only via hooks"
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
