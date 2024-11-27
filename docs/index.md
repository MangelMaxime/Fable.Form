---
layout: navbar-only
title: Introduction
---

<div class="container content section">
    <div class="columns">
        <div class="column is-8-widescreen is-offset-2-widescreen">

Fable.Form allows you to build forms that are:

- **Composable:** they can be extended and embedded into other forms
- **Type safe:** makes the most of F# compiler to tie everything together
- **Scalable:** you don't need a `Msg` for each field neither repeat your `view` code
- **Terse:** your field logic is defined in a single place
- **Modular:** you can create your own fields and customize how existing fields are rendered
- **Works with any UI library:** you can use it with Elmish + React, React, Sutil, or any other library

```fsharp
let form : Form.Form<Values, Msg, _> =
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
                        HtmlAttributes = [ ]
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

<br />

<div class="is-flex is-justify-content-center">
    <a href="/Fable.Form/Fable.Form/introduction.html" class="button is-primary is-medium">
        Gettting Started
    </a>
</div>

</div>
</div>
</div>
