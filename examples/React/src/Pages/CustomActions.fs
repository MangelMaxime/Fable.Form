module Page.CustomAction.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Core
open Examples.Shared.Forms
open CustomActions.Domain

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<CustomActions.Values>
    // Used when the form has been submitted with success
    | LoggedIn of CustomActions.FormResult
    // Used when the form has been cancelled
    | Cancelled of resetDelay: int

type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<CustomActions.Values>
    // Message sent when the form is submitted
    | LogIn of CustomActions.FormResult
    // Message sent when the user ask to reset the demo
    | ResetTheDemo
    // Message sent when the user ask to cancel the form
    | CancelTheForm
    // Message used to reduce the delay before reset by 1 second
    | TickResetDelay

let init () =
    CustomActions.init |> FillingForm, Cmd.none

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

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

let private renderLoggedInUpView (formResult: CustomActions.FormResult) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    prop.text "User logged in with the following informations"
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
                    renderRow "Email" (EmailAddress.value formResult.Email)
                    renderRow "Password" (Password.value formResult.Password)
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

let private renderCancelledView (resetDelay: int) =
    Bulma.content [

        Bulma.message [
            color.isInfo

            let message =
                sprintf "You cancelled the form, it will be reset in %i seconds" resetDelay

            prop.children [
                Bulma.messageBody [
                    prop.text message
                ]
            ]
        ]

    ]

let private formAction (dispatch: Dispatch<Msg>) (state: Form.View.State) =

    Bulma.field.div [
        field.isGrouped
        field.isGroupedCentered

        prop.children [
            // Default submit button
            Bulma.control.div [
                Bulma.button.button [
                    prop.type'.submit
                    color.isPrimary
                    prop.text "Sign up"
                    // If the form is loading animate the button with the loading animation
                    if state = Form.View.Loading then
                        button.isLoading
                ]
            ]

            // Custom button to cancel the form
            Bulma.control.div [
                Bulma.button.button [
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
                OnChange = FormChanged >> dispatch
                OnSubmit = LogIn >> dispatch
                Action = Form.View.Action.Custom(formAction dispatch)
                Validation = Form.View.ValidateOnSubmit
            }
            CustomActions.form
            values

    | LoggedIn formResult -> renderLoggedInUpView formResult dispatch

    | Cancelled resetDelay -> renderCancelledView resetDelay
