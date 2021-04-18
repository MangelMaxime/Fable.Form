module Examples

open Feliz
open Feliz.Bulma
open Feliz.ReactSyntaxHighlighter
open Elmish
open Fable.Core
open Fable.Core.JsInterop

importSideEffects "./../style/style.scss"

[<RequireQualifiedAccess>]
type Page =
    | Home
    | SignUp of Page.SignUp.Model
    | Login of Page.Login.Model
    | NotFound

type Msg =
    | SetRoute of Router.Route option
    | SignUpMsg of Page.SignUp.Msg
    | LoginMsg of Page.Login.Msg


type Model =
    {
        CurrentRoute : Router.Route option
        ActivePage : Page
    }

let private setRoute (optRoute : Router.Route option) (model : Model) =
    let model = { model with CurrentRoute = optRoute }

    match optRoute with
    | None ->
        { model with
            ActivePage = Page.NotFound
        }
        , Cmd.none

    | Some route ->
        match route with
        | Router.Route.SignUp ->
            let (subModel, subCmd) = Page.SignUp.init ()
            { model with
                ActivePage = Page.SignUp subModel
            }
            , Cmd.map SignUpMsg subCmd

        | Router.Route.Login ->
            let (subModel, subCmd) = Page.Login.init ()
            { model with
                ActivePage = Page.Login subModel
            }
            , Cmd.map LoginMsg subCmd

        | Router.Route.Home ->
            { model with
                ActivePage = Page.Home
            }
            , Cmd.none

        | Router.Route.NotFound ->
            { model with
                ActivePage = Page.NotFound
            }
            , Cmd.none

let private update (msg : Msg) (model : Model) =
    match msg with
    | SetRoute optRoute ->
        setRoute optRoute model

    | SignUpMsg subMsg ->
        match model.ActivePage with
        | Page.SignUp subModel ->
            Page.SignUp.update subMsg subModel
            |> Tuple.mapFirst Page.SignUp
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map SignUpMsg)

        | _ ->
            model
            , Cmd.none

    | LoginMsg subMsg ->
        match model.ActivePage with
        | Page.Login subModel ->
            Page.Login.update subMsg subModel
            |> Tuple.mapFirst Page.Login
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map LoginMsg)

        | _ ->
            model
            , Cmd.none


let init (location ) =
    setRoute
        location
        {
            ActivePage = Page.Home
            CurrentRoute = None
        }


let renderLink (route : Router.Route) (linkText : string) (description : string) =
    Html.li [
        Html.a [
            Router.href route
            prop.text linkText
        ]

        Bulma.content [
            prop.text description
        ]

    ]

let renderDemoPage (titleText : string) (content : ReactElement) (codeText : string) =
    Html.div [
        Html.br [ ]

        Bulma.content [
            text.hasTextCentered

            prop.children [
                Bulma.title.h5 [
                    title.is5
                    prop.text titleText
                ]
            ]
        ]

        Html.hr [ ]

        Html.pre [
            prop.className "code-preview"

            prop.children [
                Html.code [
                    prop.text (codeText.Trim())
                ]
            ]

        ]

        Html.hr [ ]

        content

    ]

let contentFromPage (page : Page) (dispatch : Dispatch<Msg>) =
    match page with
    | Page.Home ->
        Bulma.content [
            Html.br [ ]

            Bulma.content [
                text.hasTextCentered

                prop.children [
                    Bulma.title.h5 [
                        title.is5
                        prop.text "List of examples"
                    ]
                ]
            ]

            Html.hr [ ]

            Html.ul [
                renderLink
                    Router.Route.Login
                    "Login"
                    "A simple login form with 3 field"

                renderLink
                    Router.Route.SignUp
                    "Sign-up"
                    "A form demonstrating how to handle external errors"
            ]
        ]

    | Page.SignUp subModel ->
        renderDemoPage
            "Sign up"
            (Page.SignUp.view subModel (SignUpMsg >> dispatch))
            Page.SignUp.code

    | Page.Login subModel ->
        renderDemoPage
            "Login"
            (Page.Login.view subModel (LoginMsg >> dispatch))
            Page.Login.code

    | Page.NotFound ->
        Html.text "Page not found"

let view (model : Model) (dispatch : Dispatch<Msg>) =
   Bulma.columns [
       Bulma.column [
           column.is8
           column.isOffset2

           prop.children (contentFromPage model.ActivePage dispatch)
       ]
   ]


open Elmish.React
open Elmish.UrlParser
open Elmish.Navigation


#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withTrace ConsoleTracer.withTrace
|> Program.toNavigable (parseHash Router.routeParser) setRoute
|> Program.withReactSynchronous "root"
|> Program.run
