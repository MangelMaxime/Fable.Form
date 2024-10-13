module Page.Disable.Component

open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Feliz
open Feliz.Bulma
open Examples.Shared.Forms

/// <summary>
/// Represents the model of your Elmish component
/// </summary>
type Model =
    | FillingForm of Form.View.Model<Disable.Values>
    | FilledForm of string * string * string

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occur in the form
    | FormChanged of Form.View.Model<Disable.Values>
    // Used when the user submit the form
    | Submit of Disable.FormResult
    // Used when the user ask to reset the demo
    | ResetTheDemo

let init () = Disable.init |> FillingForm, Cmd.none

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
                    OnChange = FormChanged >> dispatch
                    OnSubmit = Submit >> dispatch
                    Action = Form.View.Action.SubmitOnly "Sign in"
                    Validation = Form.View.ValidateOnSubmit
                }
                Disable.form
                values
        ]

    | FilledForm(email, password, biography) -> renderFilledView email password biography dispatch
