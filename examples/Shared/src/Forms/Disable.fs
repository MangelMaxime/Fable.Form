module Examples.Shared.Forms.Disable

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

/// <summary>
/// Type used to represent the result of the form
/// </summary>
type FormResult = string * string * string

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Email: string
        Password: string
        Biography: string
    }

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occur in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | Submit of string * string * string
    // Used when the user ask to reset the demo
    | ResetTheDemo

let init =
    {
        Email = ""
        Password = ""
        Biography = "You can't edit me :)"
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
                Parser = Ok
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
        Form.meta (fun values ->
            Form.passwordField
                {
                    Parser = Ok
                    Value = fun values -> values.Password
                    Update =
                        fun newValue values ->
                            { values with
                                Password = newValue
                            }
                    Error = fun _ -> None
                    Attributes =
                        PasswordField.create "password"
                        |> PasswordField.withLabel "Password (disabled when email is empty)"
                        |> PasswordField.withPlaceholder "Your password"
                        |> PasswordField.withAutoComplete "current-password"
                }
            |> Form.disableIf (values.Email.Length = 0)
        )

    let biographyField =
        Form.textareaField
            {
                Parser = Ok
                Value = fun values -> values.Biography
                Update =
                    fun newValue values ->
                        { values with
                            Biography = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    TextareaField.create "biography"
                    |> TextareaField.withLabel "Biography (always disabled)"
                    |> TextareaField.withPlaceholder "Tell us about yourself"
            }
        |> Form.disable

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun email password biography -> (email, password, biography)

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append biographyField

let information<'FrameworkRoute> : DemoInformation<_> =
    {
        Title = "Disable"
        Route = SharedRouter.Route.Disable
        Description = "Demonstrate how to disable a field (same logic can be applied for read-only)"
        Remark = None
        Code =
            """
Form.textField
    {
        // ...
    }
    |> Form.disable

// or

Form.textField
    {
        // ...
    }
    |> Form.disableIf myCondition
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
