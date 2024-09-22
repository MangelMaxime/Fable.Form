module Examples

open Sutil
open Sutil.Bulma
open Sutil.CoreElements
open Fable.Core
open Fable.Core.JsInterop
open Fable.Form.Simple
open Fable.Form.Simple.Sutil.Bulma
open Browser
open Browser.Types

// Only import the style if we are in DEBUG mode
// otherwise the style will be included by Nacara directly
#if DEBUG
importSideEffects "./../../../docs/style.scss"
#endif

/// <summary>
/// Type used to represent the form values
/// </summary>
type Values =
    {
        Email: string
        Password: string
        RememberMe: bool
    }

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let form: Form<Values, _> =
    let emailField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Email
                Update =
                    fun newValue values ->
                        { values with
                            Email = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        FieldId = "email"
                        Label = "Email"
                        Placeholder = "some@email.com"
                        HtmlAttributes =
                            [
                                prop.autoComplete "email"
                            ]
                    }
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
                Attributes =
                    {
                        FieldId = "password"
                        Label = "Password"
                        Placeholder = "Your password"
                        HtmlAttributes =
                            [
                                prop.autoComplete "current-password"
                            ]
                    }
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
                    {
                        FieldId = "remember-me"
                        Text = "Remember me"
                    }
            }

    /// <summary>
    /// Function used to map the form values into the message to send back to the update function
    /// </summary>
    /// <returns></returns>
    let onSubmit = fun email password rememberMe -> (email, password, rememberMe)

    Form.succeed onSubmit
    |> Form.append emailField
    |> Form.append passwordField
    |> Form.append rememberMe

[<RequireQualifiedAccess>]
type State =
    | Filling of Form.View.Model<Values>
    | Filled of string * string * bool

let app () =
    let stateStore =
        {
            Email = ""
            Password = ""
            RememberMe = false
        }
        |> Form.View.idle
        |> State.Filling
        |> Store.make

    let stateStore2 =
        {
            Email = ""
            Password = ""
            RememberMe = false
        }
        |> Store.make

    let emailValue = Store.mapDistinct (fun state -> state.Email) stateStore2

    // let inputStore = Store.make ""
    // let mappedInputStore = Store.map (fun x -> x + " mapped") inputStore

    bulma.section [
        Bind.el (
            stateStore,
            fun state ->
                match state with
                | State.Filled(email, password, rememberMe) ->
                    Html.div [
                        Html.h1 [
                            prop.text "Form filled"
                        ]
                        Html.p [
                            prop.text $"Email: %s{email}"
                        ]
                        Html.p [
                            prop.text $"Password: %s{password}"
                        ]
                        Html.p [
                            prop.text $"Remember me: %b{rememberMe}"
                        ]
                    ]

                | State.Filling formValues ->
                    Form.View.asHtml
                        {
                            OnChange =
                                fun values ->
                                    printfn "Form values changed: %A" values
                                    State.Filling values |> Store.set stateStore
                            OnSubmit = fun result -> State.Filled result |> Store.set stateStore
                            Action = Form.View.Action.SubmitOnly "Sign in"
                            Validation = Form.View.ValidateOnSubmit
                        }
                        form
                        formValues
        )
    ]

//     // Html.div [
//     //     // Html.div [
//     //     //     prop.text $"Input value: %s{input}"
//     //     // ]

//     //     Html.input [
//     //         // Ev.onInput (fun ev ->
//     //         //     let value: string = ev.target?value
//     //         //     Store.set inputStore value
//     //         // )
//     //         // prop.value input
//     //         Bind.attr ("value", mappedInputStore)
//     //     ]
//     // ]

//     Bind.el (
//         inputStore,
//         fun input ->
//             Html.div [
//                 Html.div [
//                     prop.text $"Input value: %s{input}"
//                 ]

//                 Html.input [
//                     Ev.onInput (fun ev ->
//                         let value: string = ev.target?value
//                         Store.set inputStore value
//                     )
//                     prop.value input
//                 ]

//                 Html.div [
//                     onUnmount (fun ev ->
//                         let input = (ev.target :?> HTMLInputElement)

//                         printfn "onUnmount"

//                         if document.activeElement = input then
//                             printfn "Focused"
//                     ) [
//                         // CoreElements.EventModifier.Once
//                     ]
//                 ]
//             ]
//     )

// ]
// let inputStore = Store.make ""

// Bind.el (inputStore, fun input ->
//     Html.div [
//         onMount (fun _ ->
//             printfn "Mounted"
//         ) []

//         onUnmount (fun _ ->
//             printfn "Unmounted"
//         ) []

//         unsubscribeOnUnmount ([
//             fun () -> printfn "Unsubscribed"
//         ])

//         Html.div [
//             prop.text $"Input value: %s{input}"
//         ]

//         Html.input [
//             Ev.onInput (fun ev ->
//                 let value: string = ev.target?value
//                 Store.set inputStore value
//             )
//             prop.value input
//         ]
//     ]
// )

let hello () =
    Program.mount ("root", app ()) |> ignore
