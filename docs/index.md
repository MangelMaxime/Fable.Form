---
title: Introduction
---

Fable.Form allows you to build forms that are:

- **Composable:** they can be extended and embedded into other forms
- **Type safe:** makes the most of F# compiler to tie everything together
- **Scalable:** you don't need a `Msg` for each field neither repeat your `view` code
- **Terse:** your field logic is defined in a single place
- **Modular:** you can create your own fields and customize how existing fields are rendered

```fsharp
let form : Form.Form<Values, Msg> =
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
                    {
                        Label = "Email"
                        Placeholder = "some@email.com"
                    }
            }

    let passwordField =
        Form.passwordField
            {
                // ...
            }

    let onSubmit =
        fun email password ->
            LogIn (email, password)

    Form.succeed onSubmit
        |> Form.append emailField
        |> Form.append passwordField
```
