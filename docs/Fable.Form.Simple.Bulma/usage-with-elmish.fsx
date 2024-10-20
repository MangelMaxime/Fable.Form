(**
---
layout: standard
title: Elmish
---
**)

(*** hide ***)

#r "nuget: Elmish"
#r "nuget: Feliz"
#r "./../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.dll"
#r "./../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.dll"
#r "./../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Fields.Html.dll"
#r "./../../packages/Fable.Form.Simple.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Bulma.dll"

(**

This section will show you how to use `Fable.Form` with Elmish.

<ul class="textual-steps">
<li>

Open the library modules
*)

open Elmish
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html

(**

</li>
<li>

Define a type `Values` which is used to represents the different fields we have in the form.
*)

type Values =
    {
        Email : string
        Password : string
        RememberMe : bool
    }

(**

</li>
<li>

Create your model
*)

type Model =
    Form.View.Model<Values>

(**

*To keep our example simple, we use a type alias but in a real application you will generaly host it inside a discriminated union or a record*

</li>
<li>

Register 2 messages;

*)

type Msg =
    // Used when a change occurs in the form
    | FormChanged of Model
    // Used when the user submit the form
    | LogIn of string * string * bool

(**
</li>
<li>

Initialize your `Model`, we set the default value of each fields. Then pass the values to the function `Form.View.idle` which will returns a `Form.View.Model`

*)

let init : Model * Cmd<Msg> =
    {
        Email = ""
        Password = ""
        RememberMe = false
    }
    |> Form.View.idle
    , Cmd.none

(**
</li>
<li>

Write the logic of the `update` function.

*)

let update (msg : Msg) (model : Model) =
    match msg with
    // We received a new form model, store it
    | FormChanged newModel ->
        newModel
        , Cmd.none

    // The form has been submitted
    // Here, we have access to the value submitted from the from
    | LogIn (_email, _password, _rememberMe) ->
        // For this example, we just set a message in the Form view
        { model with
            State = Form.View.Success "You have been logged in successfully"
        }
        , Cmd.none

(**
</li>
<li>

Create the form logic:

1. First create each field
1. Create an `onSubmit` function which maps the result of the form into a `Msg`
1. Tie the fields and the `onSubmit` function together

*)

let form : Form<Values, Msg> =
    let emailField =
        Form.textField
            {
                Parser =
                    fun value ->
                        if value.Contains("@") then
                            Ok value
                        else
                            Error "The e-mail address must contain a '@' symbol"
                Value =
                    fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with Email = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    TextField.create "email"
                    |> TextField.withLabel "Email"
                    |> TextField.withPlaceholder "some@email.com"
                    |> TextField.withAutoFocus
            }

    let passwordField =
        Form.passwordField
            {
                Parser = Ok
                Value =
                    fun values -> values.Password
                Update =
                    fun newValue values ->
                        { values with Password = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    PasswordField.create "password"
                    |> PasswordField.withLabel "Password"
            }

    let rememberMe =
        Form.checkboxField
            {
                Parser = Ok
                Value =
                    fun values -> values.RememberMe
                Update =
                    fun newValue values ->
                        { values with RememberMe = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    CheckboxField.create "remember-me"
                    |> CheckboxField.withText "Remember me"
            }

    let onSubmit =
        fun email password rememberMe ->
            LogIn (email, password, rememberMe)

    Form.succeed onSubmit
        |> Form.append emailField
        |> Form.append passwordField
        |> Form.append rememberMe

(**

</li>
<li>

Call `Form.View.asHtml` in the view function to render the form

*)

let view (model : Model) (dispatch : Dispatch<Msg>) =
    Form.View.asHtml
        {
            OnChange = FormChanged >> dispatch
            OnSubmit = dispatch
            Action = Form.View.Action.SubmitOnly "Sign in"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        model


(**
</li>
</ul>

Congratulations 🎉, you now know how to use `Fable.Form` in your application with Elmish.
*)
