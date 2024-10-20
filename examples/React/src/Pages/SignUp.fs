module Page.SignUp.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Examples.Shared.Forms
open SignUp.Domain

[<NoComparison>]
type Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<SignUp.Values>
    // Used when the form has been submitted with success
    | SignedUp of User.User

[<NoComparison>]
type Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<SignUp.Values>
    // Message sent when the form is submitted
    | SignUp of SignUp.FormResult
    // Result of a sign-up attempt. It will either be Ok or contains an error message representing the external error for the email field.
    | SignupAttempted of Result<User.User, string>
    // Message sent when the user ask to reset the demo
    | ResetTheDemo

let init () = SignUp.init |> FillingForm, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | FormChanged formModel ->
        match model with
        | FillingForm _ -> FillingForm formModel, Cmd.none

        | _ -> model, Cmd.none

    | SignUp formResult ->
        match model with
        | FillingForm formModel ->
            let signUp () =
                User.signUp
                    formResult.Email
                    formResult.Name
                    formResult.Password
                    formResult.MakePublic

            formModel |> Form.View.setLoading |> FillingForm,
            Cmd.OfPromise.perform signUp () SignupAttempted

        | _ -> model, Cmd.none

    | SignupAttempted(Ok user) -> SignedUp user, Cmd.none

    // The signup attempt failed, we are going to update the external error received
    | SignupAttempted(Error error) ->
        match model with
        | FillingForm formModel ->
            let values = formModel.Values

            let errors = values.Errors

            { formModel with
                State = Form.View.Idle
                Values =
                    { values with
                        Errors =
                            { errors with
                                Email = Some(SignUp.FieldError.create values.Email error)
                            }
                    }
            }
            |> FillingForm,
            Cmd.none

        | _ -> model, Cmd.none

    | ResetTheDemo -> init ()

let private renderRow (leftValue: string) (rightValue: string) =
    Html.tr [
        Html.td leftValue
        Html.td rightValue
    ]

let private renderSignedUpView (user: User.User) dispatch =
    Bulma.content [

        Bulma.message [
            color.isSuccess

            prop.children [
                Bulma.messageBody [
                    prop.text "User signed up with the following informations"
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
                    renderRow "Email" (User.ValidEmail.value user.Email)
                    renderRow "Name" (User.Name.value user.Name)
                    renderRow "Password" (User.Password.value user.Password)
                    renderRow "Is profil public?" (string user.IsProfilePublic)
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
        Form.View.asHtml
            {
                OnChange = FormChanged >> dispatch
                OnSubmit = SignUp >> dispatch
                Action = Form.View.Action.SubmitOnly "Sign up"
                Validation = Form.View.ValidateOnSubmit
            }
            SignUp.form
            values

    | SignedUp user -> renderSignedUpView user dispatch
