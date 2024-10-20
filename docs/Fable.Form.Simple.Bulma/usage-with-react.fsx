(**
---
layout: standard
title: React only
---
**)

(*** hide ***)

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

open Feliz
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

    let onSubmit =
        fun email password rememberMe ->
            // In a real application, you would prefer to use a record type instead of a tuple
            (email, password, rememberMe)

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append rememberMe

(**

</li>
<li>

Create the component hosting the form.

*)

[<ReactComponent>]
let View () =

    (**
</li>
<li>

Initialize your `Model`, we set the default value of each fields. Then pass the values to the function `Form.View.idle` which will returns a `Form.View.Model`
*)

    let formState, setFormState =
        {
            Email = ""
            Password = ""
            RememberMe = false
        }
        |> Form.View.idle
        |> React.useState

    (**

Create a `onSubmit` function which will be called when the form is submitted.
*)

    let onSubmit =
        fun (formResult: string * string * bool) ->
            { formState with
                State = Form.View.Success "You have been logged in successfully"
            }
            |> setFormState
    (**
</li>
<li>

Call `Form.View.asHtml` to render the form
*)

    Form.View.asHtml
        {
            OnChange = setFormState
            OnSubmit = onSubmit
            Action = Form.View.Action.SubmitOnly "Sign in"
            Validation = Form.View.ValidateOnSubmit
        }
        form
        formState

(**
</li>
</ul>

Congratulations 🎉, you now know how to use `Fable.Form` in your application with React.
*)
