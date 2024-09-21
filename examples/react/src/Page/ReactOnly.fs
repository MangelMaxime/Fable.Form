module Page.ReactOnly.Component

open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Feliz

type FormResult =
    {
        Email: EmailAddress.T
        Password: string
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
                    {
                        FieldId = "email"
                        Label = "Email"
                        Placeholder = "some@email.com"
                        HtmlAttributes =
                            [
                                prop.autoComplete "email"
                            ]
                    }
            }

    let passwordField =
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
                    {
                        FieldId = "password"
                        Label = "Password"
                        Placeholder = "Your password"
                        HtmlAttributes =
                            [
                                prop.autoComplete "current-password"
                            ]
                    }
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

let information: DemoInformation.T =
    {
        Title = "React only (no Elmish)"
        Route = Router.Route.ReactOnly
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
