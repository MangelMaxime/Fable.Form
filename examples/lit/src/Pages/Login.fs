module Examples.Lit.Pages.Login

open Lit
open Fable.Form.Simple
open Fable.Form.Simple.Lit.Bulma
open Examples.Shared.Forms
open Login.Domain

let private hmr = HMR.createToken ()

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<Login.Values>
    | Filled of Login.FormResult

[<HookComponent>]
let Page () =
    Hook.useHmr (hmr)

    let state, setState = Login.empty |> State.Filling |> Hook.useState

    match state with
    | State.Filling formValues ->
        Form.View.asHtml
            {
                OnChange = State.Filling >> setState
                OnSubmit = State.Filled >> setState
                Action = Form.View.Action.SubmitOnly "Sign in"
                Validation = Form.View.ValidateOnSubmit
            }
            Login.form
            formValues

    | State.Filled formData ->
        html
            $"""
            <div class="content">
                <div class="message is-success">
                    <div class="message-header">
                        <p>Succesfully logged in with</p>
                    </div>
                    <div class="message-body">
                        <p>Email: {EmailAddress.value formData.Email}</p>
                        <p>Password: {Password.value formData.Password}</p>
                        <p>Remember me: {formData.RememberMe}</p>
                    </div>
                </div>
                <div class="field is-grouped is-grouped-centered">
                    <p class="control">
                        <a
                            @click={fun _ -> Login.empty |> State.Filling |> setState}
                            class="button is-primary">Reset demo</a>
                    </p>
                </div>
            </div>
        """
