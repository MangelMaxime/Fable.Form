module Page.ValidationStrategies.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Examples.Shared.Forms
open ValidationStrategies.Domain

type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<ValidationStrategies.Values>
    // User when the form has been submitted with success
    | FormFilled of ValidationStrategies.FormResult

type Msg =
    // Used when a change occure in the form
    | FormChanged of Form.View.Model<ValidationStrategies.Values>
    // Used when the user submit the form
    | Submit of ValidationStrategies.FormResult
    // Message sent when the user ask to reset the demo
    | ResetDemo

let init () =
    ValidationStrategies.init |> FillingForm, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | FormFilled _ -> model, Cmd.none

    | Submit formResult ->
        match model with
        | FillingForm _ -> FormFilled formResult, Cmd.none

        | FormFilled _ -> model, Cmd.none

    | ResetDemo -> init ()

// Function used to render the filled view (when the form has been submitted)
let private renderFilledView (email: EmailAddress) (password: Password) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    Html.text "You, "
                    Html.b (EmailAddress.value email)
                    Html.text ", have been signed in using the following password "
                    Html.b (Password.value password)
                ]
            ]

        ]

        Bulma.text.p [
            text.hasTextCentered

            prop.children [
                Bulma.button.button [
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
                OnChange = FormChanged >> dispatch
                OnSubmit = Submit >> dispatch
                Action = Form.View.Action.SubmitOnly "Sign in"
                Validation =
                    if values.Values.ValidationStrategy = "onSubmit" then
                        Form.View.ValidateOnSubmit
                    else
                        Form.View.ValidateOnBlur
            }
            ValidationStrategies.form
            values

    | FormFilled formResult -> renderFilledView formResult.Email formResult.Password dispatch
