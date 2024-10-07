module Examples.Sutil.Pages.SignUp

open Sutil
open Sutil.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Examples.Shared.Forms
open SignUp.Domain

[<RequireQualifiedAccess>]
type State =
    // The user is currently filling the form
    | Filling of Form.View.Model<SignUp.Values>
    // We are waiting for the server to validate the form
    | WaitingServerResponse of Form.View.Model<SignUp.Values>
    // The user has signed up
    | SignedUp of User.User

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

let private renderSignedUpView (user: User.User) (reset: unit -> unit) =
    bulma.content [

        bulma.message [
            color.isSuccess

            bulma.messageBody [
                prop.text "User signed up with the following informations"
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
                renderRow "Email" (User.ValidEmail.value user.Email)
                renderRow "Name" (User.Name.value user.Name)
                renderRow "Password" (User.Password.value user.Password)
                renderRow "Is profil public?" (string user.IsProfilePublic)
            ]
        ]

        bulma.text.p [
            text.hasTextCentered

            bulma.button.button [
                Ev.onClick (fun _ -> reset ())
                color.isPrimary

                prop.text "Reset the demo"
            ]
        ]
    ]

let private onSubmit
    (stateStore: IStore<State>)
    (formValues: Form.View.Model<SignUp.Values>)
    (formResult: SignUp.FormResult)
    =
    // Update in memory state
    formValues
    |> Form.View.setLoading
    |> State.WaitingServerResponse
    |> Store.set stateStore

    // Request validation to the server
    promise {
        let! result =
            User.signUp formResult.Email formResult.Name formResult.Password formResult.MakePublic

        let newState =
            match result with
            | Ok user -> State.SignedUp user
            | Error error ->
                let values = formValues.Values

                let errors = values.Errors

                { formValues with
                    State = Form.View.Idle
                    Values =
                        { values with
                            Errors =
                                { errors with
                                    Email = Some(SignUp.FieldError.create values.Email error)
                                }
                        }
                }
                |> State.Filling

        newState |> Store.set stateStore
    }
    |> Promise.start

let Page () =
    let stateStore = SignUp.init |> State.Filling |> Store.make

    Bind.el (
        stateStore,
        fun state ->
            match state with
            | State.Filling formValues
            | State.WaitingServerResponse formValues ->
                Form.View.asHtml
                    {
                        OnChange = State.Filling >> (Store.set stateStore)
                        OnSubmit = onSubmit stateStore formValues
                        Action = Form.View.Action.SubmitOnly "Sign up"
                        Validation = Form.View.ValidateOnSubmit
                    }
                    SignUp.form
                    formValues

            | State.SignedUp user ->
                renderSignedUpView
                    user
                    (fun () -> SignUp.init |> State.Filling |> Store.set stateStore)
    )
