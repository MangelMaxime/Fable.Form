module Page.Disable.Disable

open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Feliz
open Feliz.Bulma

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
/// Represents the model of your Elmish component
/// </summary>
type Model =
    | FillingForm of Form.View.Model<Values>
    | FilledForm of string * string * string

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

let init () =
    {
        Email = ""
        Password = ""
        Biography = "You can't edit me :)"
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

        | FilledForm _ -> model, Cmd.none

    | Submit(_email, _password, _rememberMe) ->
        match model with
        | FillingForm _ -> FilledForm(_email, _password, _rememberMe), Cmd.none

        | FilledForm _ -> model, Cmd.none

    | ResetTheDemo -> init ()

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, Msg> =
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
                        {
                            FieldId = "password"
                            Label = "Password (disabled when email is empty)"
                            Placeholder = "Your password"
                            HtmlAttributes =
                                [
                                    prop.autoComplete "current-password"
                                ]
                        }
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
                    {
                        FieldId = "biography"
                        Label = "Biography (always disabled)"
                        Placeholder = "Tell us about yourself"
                        HtmlAttributes = []
                    }
            }
        |> Form.disable

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun email password biography -> Submit(email, password, biography)

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append biographyField

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

let private renderFilledView
    (email: string)
    (password: string)
    (biography: string)
    (dispatch: Dispatch<Msg>)
    =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    prop.text "You have successfully filled the form"
                ]
            ]
        ]

        Bulma.table [
            table.isStriped

            prop.children [
                Html.thead [
                    Html.tr [
                        Html.th "Field"
                        Html.th "Value"
                    ]
                ]

                Html.tableBody [
                    renderRow "Email" email
                    renderRow "Password" password
                    renderRow "Biography" biography
                ]
            ]

        ]

        Bulma.text.p [
            text.hasTextCentered

            prop.children [
                Bulma.button.button [
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
        Html.div [
            Bulma.message [
                color.isInfo
                prop.children [
                    Bulma.messageBody [
                        Html.div
                            "This is a silly form that will disable the password field when the email is empty and lock the biography field"
                    ]
                ]
            ]

            Form.View.asHtml
                {
                    Dispatch = dispatch
                    OnChange = FormChanged
                    Action = Form.View.Action.SubmitOnly "Sign in"
                    Validation = Form.View.ValidateOnSubmit
                }
                form
                values
        ]

    | FilledForm(email, password, biography) -> renderFilledView email password biography dispatch

let information: DemoInformation.T =
    {
        Title = "Disable"
        Route = Router.Route.Disable
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
