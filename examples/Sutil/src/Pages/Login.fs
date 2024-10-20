module Examples.Sutil.Pages.Login

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms
open Login.Domain

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<Login.Values>
    | Filled of Login.FormResult

let Page () =
    let stateStore = Login.init |> State.Filling |> Store.make

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
                        Validation = Form.View.ValidateOnSubmit
                    }
                    Login.form
                    formValues

            | State.Filled formData ->

                bulma.content [
                    bulma.message [
                        bulma.messageHeader [
                            Html.p "Succesfully logged in with"
                        ]
                        bulma.messageBody [
                            Html.p $"Email: {EmailAddress.value formData.Email}"
                            Html.p $"Password: {Password.value formData.Password}"
                            Html.p $"Remember me: {formData.RememberMe}"
                        ]
                    ]
                    bulma.field.div [
                        field.isGrouped
                        field.isGroupedCentered
                        bulma.control.div [
                            bulma.button.a [
                                color.isPrimary
                                prop.text "Reset demo"
                                Ev.onClick (fun _ ->
                                    Login.init |> State.Filling |> Store.set stateStore
                                )
                            ]
                        ]
                    ]
                ]
    )
