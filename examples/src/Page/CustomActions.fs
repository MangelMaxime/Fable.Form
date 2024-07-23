module Page.CustomAction.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.View
open Fable.Form.Simple.Bulma
open Fable.Core

/// <summary>
/// Type used to represents the result of the form. When the form is submitted this is what we are returned
/// </summary>
type FormResult =
    {
        Email: EmailAddress.T
        Password: User.Password.T

    }

/// <summary>
/// Represents the value of the form, it also have an <c>Errors</c> field to store the external errors
/// </summary>
type Values = { Email: string; Password: string }

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // Used when the form has been submitted with success
    | LoggedIn of FormResult
    // Used when the form has been cancelled
    | Cancelled of resetDelay: int

type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<Values>
    // Message sent when the form is submitted
    | LogIn of FormResult
    // Message sent when the user ask to reset the demo
    | ResetTheDemo
    // Message sent when the user ask to cancel the form
    | CancelTheForm
    // Message used to reduce the delay before reset by 1 second
    | TickResetDelay

let init () =
    { Email = ""; Password = "" }
    |> Form.View.idle // By default, set the form in idle mode
    |> FillingForm,
    Cmd.none

let private tickDelay dispatch =
    JS.setTimeout (fun _ -> dispatch TickResetDelay) 1000 |> ignore

let update (msg: Msg) (model: Model) =
    match msg with
    | FormChanged formModel ->
        match model with
        | FillingForm _ -> FillingForm formModel, Cmd.none

        | _ -> model, Cmd.none

    | LogIn formResult ->
        match model with
        | FillingForm _ -> LoggedIn formResult, Cmd.none

        | _ -> model, Cmd.none

    | CancelTheForm -> Cancelled 3, Cmd.ofEffect tickDelay

    | TickResetDelay ->
        match model with
        | Cancelled delay ->
            let newDelay = delay - 1

            if newDelay > 0 then
                Cancelled newDelay, Cmd.ofEffect tickDelay
            else
                init ()

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
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Email"
                        Placeholder = "some@email.com"
                        HtmlAttributes = [ prop.autoComplete "email" ]
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
                        HtmlAttributes = [ prop.autoComplete "current-password" ]
                    }
            }

    let onSubmit email password =
        LogIn { Email = email; Password = password }

    Form.succeed onSubmit |> Form.append emailField |> Form.append passwordField

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [ Html.td leftValue; Html.td rightValue ]

let private renderLoggedInUpView (formResult: FormResult) dispatch =
    Bulma.content
        [

            Bulma.message
                [
                    color.isSuccess

                    prop.children
                        [
                            Bulma.messageBody
                                [ prop.text "User logged in with the following informations" ]
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
                                    renderRow "Email" (EmailAddress.toString formResult.Email)
                                    renderRow
                                        "Password"
                                        (User.Password.toString formResult.Password)
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

let private renderCancelledView (resetDelay: int) =
    Bulma.content
        [

            Bulma.message
                [
                    color.isInfo

                    let message =
                        sprintf "You cancelled the form, it will be reset in %i seconds" resetDelay

                    prop.children [ Bulma.messageBody [ prop.text message ] ]
                ]

        ]

let private formAction (state: Form.View.State) (dispatch: Dispatch<Msg>) =

    Bulma.field.div
        [
            field.isGrouped
            field.isGroupedCentered

            prop.children
                [
                    // Default submit button
                    Bulma.control.div
                        [
                            Bulma.button.button
                                [
                                    prop.type'.submit
                                    color.isPrimary
                                    prop.text "Sign up"
                                    // If the form is loading animate the button with the loading animation
                                    if state = Form.View.Loading then
                                        button.isLoading
                                ]
                        ]

                    // Custom button to cancel the form
                    Bulma.control.div
                        [
                            Bulma.button.button
                                [
                                    prop.text "Cancel"
                                    // If the form is loading animate the button with the loading animation
                                    if state = Form.View.Loading then
                                        button.isLoading

                                    prop.onClick (fun _ -> dispatch CancelTheForm)
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
                Action = Form.View.Action.Custom formAction
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | LoggedIn formResult -> renderLoggedInUpView formResult dispatch

    | Cancelled resetDelay -> renderCancelledView resetDelay

let information: DemoInformation.T =
    {
        Title = "Custom actions"
        Route = Router.Route.CustomAction
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
    (state : Form.View.State)
    (dispatch : Dispatch<Msg>) =
    // Definition of the custom action view
    // ...

Form.View.asHtml
    {
        Dispatch = dispatch
        OnChange = FormChanged
        Action = Form.View.Action.Custom formAction
        Validation = Form.View.ValidateOnSubmit
    }
    form
    values
            """
        GithubLink = Env.generateGithubUrl __SOURCE_DIRECTORY__ __SOURCE_FILE__
    }
