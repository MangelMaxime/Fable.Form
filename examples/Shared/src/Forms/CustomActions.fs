module Examples.Shared.Forms.CustomActions

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html
#endif

#if EXAMPLE_LIT
open Fable.Form.Simple.Lit.Bulma
open Fable.Form.Simple.Fields.Html
#endif

#if EXAMPLE_SUTIL
open Fable.Form.Simple.Sutil.Bulma
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
/// Type used to represents the result of the form. When the form is submitted this is what we are returned
/// </summary>
type FormResult =
    {
        Email: EmailAddress
        Password: Password

    }

/// <summary>
/// Represents the value of the form, it also have an <c>Errors</c> field to store the external errors
/// </summary>
type Values =
    {
        Email: string
        Password: string
    }

let init =
    {
        Email = ""
        Password = ""
    }
    |> Form.View.idle // By default, set the form in idle mode

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
            }

    let onSubmit email password =
        {
            Email = email
            Password = password
        }
        : FormResult

    Form.succeed onSubmit |> Form.append emailField |> Form.append passwordField

let information<'R> : DemoInformation<_> =
    {
        Title = "Custom actions"
        Route = SharedRouter.Route.CustomActions
        Description =
            "A form demonstrating how you can customize the actions, can be used to add a cancel button for example."
        Remark = None
        Code =
            """
let form =
    Form.succeed onSubmit
        |> Form.append emailField
        |> Form.append passwordField

let formAction
    (onCancel: unit -> unit) // This is an example of passing an additional parameter
    (dispatch : Dispatch<Msg>) =
    // Definition of the custom action view
    // ...

Form.View.asHtml
    {
        Dispatch = dispatch
        OnChange = FormChanged
        Action = Form.View.Action.Custom (formAction onCancel)
        Validation = Form.View.ValidateOnSubmit
    }
    form
    values
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
