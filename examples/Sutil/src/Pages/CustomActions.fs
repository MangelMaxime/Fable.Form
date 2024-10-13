module Examples.Sutil.Pages.CustomActions

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms
open CustomActions.Domain

[<RequireQualifiedAccess>]
type State =
    // Used when the form is being filled
    | Filling of Form.View.Model<CustomActions.Values>
    // Used when the form has been submitted with success
    | LoggedIn of CustomActions.FormResult
    // Used when the form has been cancelled
    | Cancelled of resetDelay: int

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

let private renderLoggedInUpView (formResult: CustomActions.FormResult) (resetDemo: unit -> unit) =
    bulma.content [

        bulma.message [
            color.isSuccess

            bulma.messageBody [
                prop.text "User logged in with the following informations"
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
                renderRow "Email" (EmailAddress.value formResult.Email)
                renderRow "Password" (Password.value formResult.Password)
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

let private renderCancelledView (resetDelay: int) (stateStore: IStore<State>) =
    bulma.content [
        Ev.onMount (fun _ ->
            if resetDelay > 0 then
                Fable.Core.JS.setTimeout
                    (fun _ -> State.Cancelled(resetDelay - 1) |> Store.set stateStore)
                    1000
                |> ignore
            else
                // Force the store to update after the component is mounted
                // otherwise the page will display both the cancelled message and the form
                Fable.Core.JS.setTimeout
                    (fun _ -> State.Filling CustomActions.init |> Store.set stateStore)
                    0
                |> ignore
        )

        bulma.message [
            color.isInfo

            let message =
                sprintf "You cancelled the form, it will be reset in %i seconds" resetDelay

            bulma.messageBody [
                prop.text message
            ]
        ]

    ]

let private formAction (onCancel: unit -> unit) (state: Form.View.State) =

    bulma.field.div [
        field.isGrouped
        field.isGroupedCentered

        // Default submit button
        bulma.control.div [
            bulma.button.button [
                prop.type' "submit"
                color.isPrimary
                prop.text "Sign up"
                // If the form is loading animate the button with the loading animation
                if state = Form.View.Loading then
                    button.isLoading
            ]
        ]

        // Custom button to cancel the form
        bulma.control.div [
            bulma.button.button [
                prop.text "Cancel"
                // If the form is loading animate the button with the loading animation
                if state = Form.View.Loading then
                    button.isLoading

                Ev.onClick (fun _ -> onCancel ())
            ]
        ]
    ]

let Page () =
    let stateStore = CustomActions.init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling formValues ->
                Form.View.asHtml
                    {
                        OnChange = State.Filling >> (Store.set stateStore)
                        OnSubmit = State.LoggedIn >> Store.set stateStore
                        Action =
                            Form.View.Action.Custom(
                                formAction (fun () -> State.Cancelled 3 |> Store.set stateStore)
                            )
                        Validation = Form.View.ValidateOnSubmit
                    }
                    CustomActions.form
                    formValues

            | State.LoggedIn formData ->
                renderLoggedInUpView
                    formData
                    (fun () -> CustomActions.init |> State.Filling |> Store.set stateStore)

            | State.Cancelled resetDelay -> renderCancelledView resetDelay stateStore
    )
