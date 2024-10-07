module Examples.Lit

open global.Lit
open Examples.Lit
open Lit.Router
open Fable.Core.JsInterop

// Only import the style if we are in DEBUG mode
// otherwise the style will be included by Nacara directly
#if DEBUG
importSideEffects "./../../../docs/style.scss"
#endif

// let private renderLink

// Fable.Form used with Fable.Lit
[<LitElement("my-app")>]
let App () =

    let _, _ = LitElement.init (fun init -> init.useShadowDom <- false)

    let route = Hook.useRouter (Parser.parseHash Router.routeParser)

    html
        $"""
            {match route with
             | Some Router.Route.Login -> Pages.Login.Page()
             | Some Router.Route.Home -> html $"<p>Home</p>"
             | None -> html $"<p>Not found</p>"}
        </div>
    """

// let _, props =
//     LitElement.init (fun init ->
//         init.props <-
//             {|
//                 initial =
//                     ({
//                         Email = ""
//                         Password = ""
//                         RememberMe = false
//                     }
//                     : Login.Values)
//                     |> Form.View.idle
//                     |> State.Filling
//                     |> Prop.Of
//             |}

//         // We need to access the global Bulma styles
//         init.useShadowDom <- false
//     )

// let state, setState = Hook.useState props.initial.Value

// match state with
// | State.Filled formData ->
//     html
//         $"""
//         <div>
//             <p>Email: {EmailAddress.toString formData.Email}</p>
//             <p>Password: {formData.Password}</p>
//             <p>Remember me: {formData.RememberMe}</p>
//         </div>
//     """

// | State.Filling formValues ->
//     Form.View.asHtml
//         {
//             OnChange = State.Filling >> setState
//             OnSubmit = State.Filled >> setState
//             Action = Form.View.Action.SubmitOnly "Sign in"
//             Validation = Form.View.ValidateOnSubmit
//         }
//         Login.form
//         formValues
