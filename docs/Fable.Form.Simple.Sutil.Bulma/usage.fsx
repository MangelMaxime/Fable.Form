(**
---
layout: standard
title: Sutil
---
**)

(*** hide ***)

#r "nuget: Sutil"
#r "./../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.dll"
#r "./../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.dll"
#r "./../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Fields.Html.dll"
#r "./../../packages/Fable.Form.Simple.Sutil.Bulma/bin/Debug/netstandard2.1/Fable.Form.Simple.Sutil.Bulma.dll"

(**

This section will show you how to use `Fable.Form` with Elmish.

<ul class="textual-steps">
<li>

Open the library modules
*)

open Sutil
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Fable.Form.Simple.Fields.Html
(**

</li>
<li>

Define a type `Values` which is used to represents the different fields we have in the form.
*)

type Values =
    {
        Email: string
        Password: string
        RememberMe: bool
    }

(**
</li>
<li>

Create the form logic:

1. First create each field
1. Create an `onSubmit` function which maps the result of the form into a `Msg`
1. Tie the fields and the `onSubmit` function together

*)

let form: Form<Values, _> =
    let emailField =
        Form.textField
            {
                Parser =
                    fun value ->
                        if value.Contains("@") then
                            Ok value
                        else
                            Error "The e-mail address must contain a '@' symbol"
                Value = fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with
                            Email = newValue
                        }
                Error = fun _ -> None
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
                Value = fun values -> values.Password
                Update =
                    fun newValue values ->
                        { values with
                            Password = newValue
                        }
                Error = fun _ -> None
                Attributes = PasswordField.create "password" |> PasswordField.withLabel "Password"
            }

    let rememberMe =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.RememberMe
                Update =
                    fun newValue values ->
                        { values with
                            RememberMe = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    CheckboxField.create "remember-me" |> CheckboxField.withText "Remember me"
            }

    let onSubmit = fun email password rememberMe -> (email, password, rememberMe)

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append rememberMe

(**
</li>
<li>

Define a `State` type which will be used to represent the state of the component.
*)

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<Values>
    | Filled of string * string * bool

(**
</li>
<li>

Create a `Sutil` component and initialize its state as `Filling`.abs

To do so, we set the default value of each fields. Then pass the values to the function `Form.View.idle` which will returns a `Form.View.Model`

*)

let Page () =
    let stateStore =
        {
            Email = ""
            Password = ""
            RememberMe = false
        }
        |> Form.View.idle
        |> State.Filling
        |> Store.make

    (**
</li>
<li>

Use the `stateStore` to either render the form with `Form.View.asHtml` or the filled view.

*)

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
                    form
                    formValues

            | State.Filled(email, password, rememberMe) ->
                Html.ul [
                    Html.li [
                        Html.text $"Email: {email}"
                    ]
                    Html.li [
                        Html.text $"Password: {password}"
                    ]
                    Html.li [
                        Html.text $"Remember me: {rememberMe}"
                    ]
                ]
    )

(**
</li>
</ul>

Congratulations 🎉, you now know how to use `Fable.Form` in your application with Sutil.
*)
