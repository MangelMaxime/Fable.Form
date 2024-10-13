module Examples.Sutil.Pages.Disable

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<Disable.Values>
    | Filled of Disable.FormResult

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

let private renderFilledView
    (email: string)
    (password: string)
    (biography: string)
    (resetDemo: unit -> unit)
    =
    bulma.content [

        bulma.message [
            color.isSuccess

            bulma.messageBody [
                prop.text "You have successfully filled the form"
            ]
        ]

        bulma.table [
            table.isStriped

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
    let stateStore = Disable.init |> State.Filling |> Store.make

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
                    Disable.form
                    formValues

            | State.Filled(email, password, biography) ->
                renderFilledView
                    email
                    password
                    biography
                    (fun _ -> Disable.init |> State.Filling |> Store.set stateStore)
    )
