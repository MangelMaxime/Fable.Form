# Fable.Form

[![Gitpod Ready-to-Code](https://img.shields.io/badge/Gitpod-ready--to--code-blue?logo=gitpod)](https://gitpod.io/#https://github.com/MangelMaxime/Fable.Form)

**[Documentation](https://mangelmaxime.github.io/Fable.Form/)**

Fable.Form allows you to build forms that are:

- **Composable:** they can be extended and embedded in other forms
- **Type safe:** makes the most of F# compiler to tied everything
- **Scalable:** you don't need a `Msg` for each field neither repeat your `view` code
- **Terse:** your field logic is defined in a single place
- **Modular:** you can create your own fields and customize how existing fields are rendered

**Example**

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
                            Error "The e-mail adress must contain a '@' symbol"
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
                Parser = Ok
                Value =
                    fun values -> values.Password
                Update =
                    fun newValue values ->
                        { values with Password = newValue }
                Error =
                    fun _ -> None
                Attributes =
                    {
                        Label = "Password"
                        Placeholder = "Your password"
                    }
            }

    let onSubmit =
        fun email password ->
            LogIn (email, password)

    Form.succeed onSubmit
        |> Form.append emailField
        |> Form.append passwordField
```

## Development

This repository use NPM scripts to control the build system here is a list of the main scripts available:

| Script | Description |
|---|---|
| `npm run clean` | To use when want to clean all the artifacts |
| `npm run watch-examples` | To uses when working on the examples project |
| `npm run watch-tests` | To use when working on the tests suits |
| `npm run watch-docs` | To use when working on the documentation, hosted on [http://localhost:8080](http://localhost:8080) |
| `npm run publish-docs` | Build a new version of the documentation and publish it to Github Pages |
| `npm run release` | Build a new version of the packages if needed and release it |

## The secret pun

Naming library is super annoying.

To make it easy to discover, in general, you need to choose a simple and clear name. Especially in a "small" ecosystem like Fable. The problem is that these names are rarely fun neither exciting.

But, it is possible to spice thing up for this reason `Fable.Form` stands for `Fable.Formidable`.

Special thanks to [Urs Enzler](https://twitter.com/ursenzler) who came up with [this idea](https://twitter.com/ursenzler/status/1385159595526610945)

## History

In 2018, I created [Thoth.Elmish.FormBuilder](https://thoth-org.github.io/Thoth.Elmish.FormBuilder/) as an attempt to prevent having one `Msg` per field and repeat the same `view` code.

At that time, I was not able to find a way to do it in a fully type safe way and use "magic strings" and `boxing/unboxing` to work around the type system a bit.

The problem is that the system is pretty rigid and when you are in form logic you can't access the outside world or pass data in it. For example, it is hard to integrate external errors or pass the user session to send request on the server for auto completion, etc.

Fast forward three years later, I discovered the work done by [Héctor Ramón](https://github.com/hecrj) in elm ecosystem with [hecrj/composable-form](https://github.com/hecrj/composable-form). This library aligned perfectly with my original vision I had in 2018. I finally, decided to accept that [Thoth.Elmish.FormBuilder](https://thoth-org.github.io/Thoth.Elmish.FormBuilder/) should be archived and that I should write a new library which aligned with my original vision and goals.

Fable.Form can be seen has a port of [hecrj/composable-form](https://github.com/hecrj/composable-form) for [Fable](https://fable.io/) but adapted with my vision of how a form should work.
