module Page.Login.Component

open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Feliz
open Feliz.Bulma
open Examples.Shared.Forms
open Login.Domain

/// <summary>
/// Represents the model of your Elmish component
///
/// In the case of the Login example, we just need to keep track of the Form model state
/// </summary>
type Model =
    | Filling of Form.View.Model<Login.Values>
    | Filled of Login.FormResult

/// <summary>
/// Represents the different messages that your application can react too
/// </summary>
type Msg =
    // Used when a change occur in the form
    | FormChanged of Form.View.Model<Login.Values>
    // Used when the user submit the form
    | LogIn of Login.FormResult
    | ResetDemo

let init () = Filling Login.init, Cmd.none

let update (msg: Msg) (_: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel -> Filling newModel, Cmd.none

    // Form has been submitted
    // Here, we have access to the value submitted from the from
    | LogIn formResult -> Filled formResult, Cmd.none

    | ResetDemo -> init ()

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | Filling formValues ->
        Form.View.asHtml
            {
                OnChange = FormChanged >> dispatch
                OnSubmit = LogIn >> dispatch
                Action = Form.View.Action.SubmitOnly "Sign in"
                Validation = Form.View.ValidateOnSubmit
            }
            Login.form
            formValues

    | Filled formData ->

        Bulma.content [
            Bulma.message [
                Bulma.messageHeader [
                    Html.p "Succesfully logged in with"
                ]
                Bulma.messageBody [
                    Html.p $"Email: {EmailAddress.value formData.Email}"
                    Html.p $"Password: {Password.value formData.Password}"
                    Html.p $"Remember me: {formData.RememberMe}"
                ]
            ]
            Bulma.field.div [
                field.isGrouped
                field.isGroupedCentered
                prop.children [
                    Bulma.control.div [
                        Bulma.button.a [
                            color.isPrimary
                            prop.text "Reset demo"
                            prop.onClick (fun _ -> ResetDemo |> dispatch)
                        ]
                    ]
                ]
            ]
        ]
