module Examples

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Core
open Fable.Core.JsInterop

importSideEffects "./../style/style.scss"

module SignUp = Page.SignUp.Component
module Login = Page.Login.Component
module DynamicForm = Page.DynamicForm.Component
module FormList = Page.FormList.Component
module ValidationStrategies = Page.ValidationStrategies.Component
module ComposabilitySimple = Page.Composability.Simple.Component
module ComposabilityWithConfiguration = Page.Composability.WithConfiguration.Component

[<RequireQualifiedAccess>]
type Page =
    | Home
    | SignUp of SignUp.Model
    | Login of Login.Model
    | DynamicForm of DynamicForm.Model
    | FormList of FormList.Model
    | ValidationStrategies of ValidationStrategies.Model
    | ComposabilitySimple of ComposabilitySimple.Model
    | ComposabilityWithConfiguration of ComposabilityWithConfiguration.Model
    | NotFound

type Msg =
    | SetRoute of Router.Route option
    | SignUpMsg of SignUp.Msg
    | LoginMsg of Login.Msg
    | DynamicFormMsg of DynamicForm.Msg
    | FormListMsg of FormList.Msg
    | ValidationStrategiesMsg of ValidationStrategies.Msg
    | ComposabilitySimpleMsg of ComposabilitySimple.Msg
    | ComposabilityWithConfigurationMsg of ComposabilityWithConfiguration.Msg


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
            let (subModel, subCmd) = SignUp.init ()
            { model with
                ActivePage = Page.SignUp subModel
            }
            , Cmd.map SignUpMsg subCmd

        | Router.Route.Login ->
            let (subModel, subCmd) = Login.init ()
            { model with
                ActivePage = Page.Login subModel
            }
            , Cmd.map LoginMsg subCmd

        | Router.Route.DynamicForm ->
            let (subModel, subCmd) = DynamicForm.init ()
            { model with
                ActivePage = Page.DynamicForm subModel
            }
            , Cmd.map DynamicFormMsg subCmd

        | Router.Route.FormList ->
            let (subModel, subCmd) = FormList.init ()
            { model with
                ActivePage = Page.FormList subModel
            }
            , Cmd.map FormListMsg subCmd

        | Router.Route.ValidationStrategies ->
            let (subModel, subCmd) = ValidationStrategies.init ()
            { model with
                ActivePage = Page.ValidationStrategies subModel
            }
            , Cmd.map ValidationStrategiesMsg subCmd

        | Router.Route.Composability Router.ComposabilityRoute.Simple ->
            let (subModel, subCmd) = ComposabilitySimple.init ()
            { model with
                ActivePage = Page.ComposabilitySimple subModel
            }
            , Cmd.map ComposabilitySimpleMsg subCmd

        | Router.Route.Composability Router.ComposabilityRoute.WithConfiguration ->
            let (subModel, subCmd) = ComposabilityWithConfiguration.init ()
            { model with
                ActivePage = Page.ComposabilityWithConfiguration subModel
            }
            , Cmd.map ComposabilityWithConfigurationMsg subCmd

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
            SignUp.update subMsg subModel
            |> Tuple.mapFirst Page.SignUp
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map SignUpMsg)

        | _ ->
            model
            , Cmd.none

    | LoginMsg subMsg ->
        match model.ActivePage with
        | Page.Login subModel ->
            Login.update subMsg subModel
            |> Tuple.mapFirst Page.Login
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map LoginMsg)

        | _ ->
            model
            , Cmd.none

    | DynamicFormMsg subMsg ->
        match model.ActivePage with
        | Page.DynamicForm subModel ->
            DynamicForm.update subMsg subModel
            |> Tuple.mapFirst Page.DynamicForm
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map DynamicFormMsg)

        | _ ->
            model
            , Cmd.none

    | FormListMsg subMsg ->
        match model.ActivePage with
        | Page.FormList subModel ->
            FormList.update subMsg subModel
            |> Tuple.mapFirst Page.FormList
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map FormListMsg)

        | _ ->
            model
            , Cmd.none

    | ComposabilitySimpleMsg subMsg ->
        match model.ActivePage with
        | Page.ComposabilitySimple subModel ->
            ComposabilitySimple.update subMsg subModel
            |> Tuple.mapFirst Page.ComposabilitySimple
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map ComposabilitySimpleMsg)

        | _ ->
            model
            , Cmd.none

    | ComposabilityWithConfigurationMsg subMsg ->
        match model.ActivePage with
        | Page.ComposabilityWithConfiguration subModel ->
            ComposabilityWithConfiguration.update subMsg subModel
            |> Tuple.mapFirst Page.ComposabilityWithConfiguration
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map ComposabilityWithConfigurationMsg)

        | _ ->
            model
            , Cmd.none

    | ValidationStrategiesMsg subMsg ->
        match model.ActivePage with
        | Page.ValidationStrategies subModel ->
            ValidationStrategies.update subMsg subModel
            |> Tuple.mapFirst Page.ValidationStrategies
            |> Tuple.mapFirst (fun page -> { model with ActivePage = page })
            |> Tuple.mapSecond (Cmd.map ValidationStrategiesMsg)

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


let renderDemoPage (titleText : string) (optRemark : ReactElement option) (content : ReactElement) (codeText : string) (sourceCodeUrl : string) =
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

        match optRemark with
        | Some remark ->
            remark

        | None ->
            Html.none

        Html.hr [ ]

        Html.pre [
            prop.className "code-preview"

            prop.children [
                Html.code [
                    prop.text (codeText.Trim())
                ]
            ]

        ]

        Bulma.text.div [
            text.hasTextCentered

            prop.children [
                Html.a [
                    prop.href sourceCodeUrl
                    prop.text "Full source code"
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

            Bulma.content [
                Bulma.subtitle.p [
                    title.is5
                    prop.text "Basic"
                ]

                Html.p [
                    Html.text "The features demonstrated in this section are available for all the library based on "
                    Html.b "Fable.Form"
                ]
            ]

            Html.ul [
                renderLink
                    Router.Route.Login
                    Login.title
                    "A simple login form with 3 fields"

                renderLink
                    Router.Route.SignUp
                    SignUp.title
                    "A form demonstrating how to handle external errors"

                renderLink
                    Router.Route.DynamicForm
                    DynamicForm.title
                    "A form that changes dynamically based on its own values"

                renderLink
                    Router.Route.FormList
                    FormList.title
                    "A form where you can add and remove a list of forms"

                renderLink
                    (Router.Route.Composability Router.ComposabilityRoute.Simple)
                    ComposabilitySimple.title
                    "Demonstrate how you can re-use a form the 'simple way'"

                renderLink
                    (Router.Route.Composability Router.ComposabilityRoute.WithConfiguration)
                    ComposabilityWithConfiguration.title
                    "Demonstrate how you can re-use a form using a 'configuration object'"
            ]


            Bulma.content [
                Bulma.subtitle.p [
                    title.is5
                    prop.text "Advanced"
                ]

                Html.p "The features demonstrated in this section depends on the library which provides the view implementation"

                Html.p "The goal here is to demonstrate advanced usage that you could need when implementing your own view"

            ]

            Html.ul [
                renderLink
                    Router.Route.ValidationStrategies
                    ValidationStrategies.title
                    "A form to demonstrate the 2 validation strategies: 'onSubmit' or 'onBlur'."
            ]
        ]

    | Page.SignUp subModel ->
        renderDemoPage
            SignUp.title
            SignUp.remark
            (SignUp.view subModel (SignUpMsg >> dispatch))
            SignUp.code
            SignUp.githubLink

    | Page.Login subModel ->
        renderDemoPage
            Login.title
            Login.remark
            (Login.view subModel (LoginMsg >> dispatch))
            Login.code
            Login.githubLink

    | Page.DynamicForm subModel ->
        renderDemoPage
            DynamicForm.title
            DynamicForm.remark
            (DynamicForm.view subModel (DynamicFormMsg >> dispatch))
            DynamicForm.code
            DynamicForm.githubLink

    | Page.FormList subModel ->
        renderDemoPage
            FormList.title
            FormList.remark
            (FormList.view subModel (FormListMsg >> dispatch))
            FormList.code
            FormList.githubLink

    | Page.ValidationStrategies subModel ->
        renderDemoPage
            ValidationStrategies.title
            ValidationStrategies.remark
            (ValidationStrategies.view subModel (ValidationStrategiesMsg >> dispatch))
            ValidationStrategies.code
            ValidationStrategies.githubLink

    | Page.ComposabilitySimple subModel ->
        renderDemoPage
            ComposabilitySimple.title
            ComposabilitySimple.remark
            (ComposabilitySimple.view subModel (ComposabilitySimpleMsg >> dispatch))
            ComposabilitySimple.code
            ComposabilitySimple.githubLink

    | Page.ComposabilityWithConfiguration subModel ->
        renderDemoPage
            ComposabilityWithConfiguration.title
            ComposabilityWithConfiguration.remark
            (ComposabilityWithConfiguration.view subModel (ComposabilityWithConfigurationMsg >> dispatch))
            ComposabilityWithConfiguration.code
            ComposabilityWithConfiguration.githubLink

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
