module Examples.Sutil.Pages.ValidationStrategies

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms
open ValidationStrategies.Domain

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<ValidationStrategies.Values>
    | Filled of ValidationStrategies.FormResult

// Function used to render the filled view (when the form has been submitted)
let private renderFilledView (email: EmailAddress) (password: Password) (resetDemo: unit -> unit) =
    bulma.content [
        bulma.message [
            color.isSuccess

            bulma.messageBody [
                Html.text "You have been signed in as, "
                Html.b (EmailAddress.value email)
                Html.text ", using the following password "
                Html.b (Password.value password)
            ]
        ]

        bulma.text.p [
            text.hasTextCentered

            bulma.button.button [
                Ev.onClick (fun _ -> resetDemo ())
                color.isPrimary

                prop.text "Reset the demo"
            ]
        ]
    ]

let Page () =
    let stateStore = ValidationStrategies.init |> State.Filling |> Store.make

    Html.div [
        bulma.message [
            color.isWarning

            bulma.messageBody [
                prop.text
                    "ValidateOnBlur supports is not perfect when using Sutil, it is recommended to use ValidateOnSubmit"
            ]
        ]

        Bind.el (
            stateStore,
            fun state ->
                match state with
                | State.Filling formValues ->
                    Form.View.asHtml
                        {
                            OnChange = State.Filling >> (Store.set stateStore)
                            OnSubmit = State.Filled >> Store.set stateStore
                            Action = Form.View.Action.SubmitOnly "Sign in"
                            Validation =
                                if formValues.Values.ValidationStrategy = "onSubmit" then
                                    Form.View.ValidateOnSubmit
                                else
                                    Form.View.ValidateOnBlur
                        }
                        ValidationStrategies.form
                        formValues

                | State.Filled formData ->
                    renderFilledView
                        formData.Email
                        formData.Password
                        (fun () ->
                            ValidationStrategies.init |> State.Filling |> Store.set stateStore
                        )
        )

    ]
