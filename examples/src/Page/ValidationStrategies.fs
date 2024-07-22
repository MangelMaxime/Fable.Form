module Page.ValidationStrategies.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Bulma

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
    | FormFilled of EmailAddress.T * User.Password.T

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<Values>
    // Used when the user submit the form
    | Submit of EmailAddress.T * User.Password.T
    // Message sent when the user ask to reset the demo
    | ResetDemo

let init () =
    {
        ValidationStrategy = "onBlur"
        Email = ""
        Password = ""
    }
    |> Form.View.idle
    |> FillingForm,
    Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | FormFilled _ -> model, Cmd.none

    | Submit(email, password) ->
        match model with
        | FillingForm _ -> FormFilled(email, password), Cmd.none

        | FormFilled _ -> model, Cmd.none

    | ResetDemo -> init ()

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let private form: Form.Form<Values, Msg, _> =
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
                Error = always None
                Attributes =
                    {
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
                Update = fun newValue values -> { values with Email = newValue }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Email"
                        Placeholder = "some@email.com"
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

    let onSmubit _ email password = Submit(email, password)

    Form.succeed onSmubit
    |> Form.append validationStrategiesField
    |> Form.append emailField
    |> Form.append passwordField

// Function used to render the filled view (when the form has been submitted)
let private renderFilledView (email: EmailAddress.T) (password: User.Password.T) dispatch =
    Bulma.content
        [

            Bulma.message
                [
                    color.isSuccess

                    prop.children
                        [
                            Bulma.messageBody
                                [
                                    Html.text "You, "
                                    Html.b (EmailAddress.toString email)
                                    Html.text ", have been signed in using the following password "
                                    Html.b (User.Password.toString password)
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
                                    prop.onClick (fun _ -> dispatch ResetDemo)
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
                Action = Form.View.Action.SubmitOnly "Sign in"
                Validation =
                    if values.Values.ValidationStrategy = "onSubmit" then
                        Form.View.ValidateOnSubmit
                    else
                        Form.View.ValidateOnBlur
            }
            form
            values

    | FormFilled(email, name) -> renderFilledView email name dispatch

let information: DemoInformation.T =
    let remark =
        Bulma.content
            [
                text.hasTextCentered

                prop.children
                    [
                        Html.text
                            "This feature depends on the view implementation, here it is offered by "
                        Html.b "Fable.Form.Simple"
                        Html.text " package"
                    ]
            ]
        |> Some

    {
        Title = "Validation strategies"
        Remark = remark
        Route = Router.Route.ValidationStrategies
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
