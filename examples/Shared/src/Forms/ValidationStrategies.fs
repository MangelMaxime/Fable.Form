module Examples.Shared.Forms.ValidationStrategies

open Fable.Form.Simple

// In your application, you should remove the compiler directives
// and use the appropriate module for your UI framework
#if EXAMPLE_REACT
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
    }

/// <summary>
/// Represent the form values
/// </summary>
type Values =
    {
        ValidationStrategy: string
        Email: string
        Password: string
    }

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // User when the form has been submitted with success
    | FormFilled of EmailAddress * Password

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | Submit of FormResult
    // Message sent when the user ask to reset the demo
    | ResetDemo

let init =
    {
        ValidationStrategy = "onBlur"
        Email = ""
        Password = ""
    }
    |> Form.View.idle

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, FormResult> =
    let validationStrategiesField =
        Form.radioField
            {
                Parser = Ok
                Value = fun values -> values.ValidationStrategy
                Update =
                    fun newValue values ->
                        { values with
                            ValidationStrategy = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "validation-strategy"
                        Label = "Validation strategy"
                        Options =
                            [
                                "onSubmit", "Validate on form submit"
                                "onBlur", "Validate on field blur"
                            ]
                    }

            }

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
                    {
                        FieldId = "email"
                        Label = "Email"
                        Placeholder = "some@email.com"
                        AutoComplete = Some "email"
                    }
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
                    {
                        FieldId = "password"
                        Label = "Password"
                        Placeholder = "Your password"
                        AutoComplete = Some "current-password"
                    }
            }

    let onSmubit _ email password =
        {
            Email = email
            Password = password
        }
        : FormResult

    Form.succeed onSmubit
    |> Form.append validationStrategiesField
    |> Form.append emailField
    |> Form.append passwordField

let information<'FrameworkRoute> : DemoInformation<_> =
    let remark =
        $"""
<div class="content has-text-centered">
    This feature depends on the view implementation, here it is offered by <b>Fable.Form.Simple</b> package
</div>
        """
        |> Some

    {
        Title = "Validation strategies"
        Remark = remark
        Route = SharedRouter.Route.ValidationStrategies
        Description = "A form to demonstrate the 2 validation strategies: 'onSubmit' or 'onBlur'."
        Code =
            """
let form =
    Form.succeed onSubmit
        |> Form.append emailField
        |> Form.append passwordField
        |> Form.append rememberMe

Form.View.asHtml
    {
        Dispatch = dispatch
        OnChange = FormChanged
        Action = Form.View.Action.SubmitOnly "Submit"
        Validation =
            if model.Values.ValidationStrategy = "onSubmit" then
                Form.View.ValidateOnSubmit
            else
                Form.View.ValidateOnBlur
    }
    form
    model
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
