module Examples

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Core
open Fable.Core.JsInterop

// Only import the style if we are in DEBUG mode
// otherwise the style will be included by Nacara directly
#if DEBUG
importSideEffects "./../../../docs/style.scss"
#endif

module SignUp = Page.SignUp.Component
module Login = Page.Login.Component
module File = Page.File.Component
module DynamicForm = Page.DynamicForm.Component
module FormList = Page.FormList.Component
module ValidationStrategies = Page.ValidationStrategies.Component
module ComposabilitySimple = Page.Composability.Simple.Component
module ComposabilityWithConfiguration = Page.Composability.WithConfiguration.Component
module CustomAction = Page.CustomAction.Component
module CustomField = Page.CustomField.Component
module Disable = Page.Disable.Component
module ReactOnly = Page.ReactOnly.Component
module CustomView = Page.CustomView.Component

[<RequireQualifiedAccess>]
[<NoComparison>]
type Page =
    | Home
    | SignUp of SignUp.Model
    | Login of Login.Model
    | File of File.Model
    | DynamicForm of DynamicForm.Model
    | FormList of FormList.Model
    | ValidationStrategies of ValidationStrategies.Model
    | ComposabilitySimple of ComposabilitySimple.Model
    | ComposabilityWithConfiguration of ComposabilityWithConfiguration.Model
    | CustomAction of CustomAction.Model
    | CustomField of CustomField.Model
    | Disable of Disable.Model
    | ReactOnly
    | CustomView of CustomView.Model
    | NotFound

[<NoComparison>]
type Msg =
    | SetRoute of Router.Route option
    | SignUpMsg of SignUp.Msg
    | LoginMsg of Login.Msg
    | FileMsg of File.Msg
    | DynamicFormMsg of DynamicForm.Msg
    | FormListMsg of FormList.Msg
    | ValidationStrategiesMsg of ValidationStrategies.Msg
    | ComposabilitySimpleMsg of ComposabilitySimple.Msg
    | ComposabilityWithConfigurationMsg of ComposabilityWithConfiguration.Msg
    | CustomActionMsg of CustomAction.Msg
    | CustomFieldMsg of CustomField.Msg
    | DisableMsg of Disable.Msg
    | CustomViewMsg of CustomView.Msg

[<NoComparison>]
type Model =
    {
        CurrentRoute: Router.Route option
        ActivePage: Page
    }

let private setRoute (optRoute: Router.Route option) (model: Model) =
    let model =
        { model with
            CurrentRoute = optRoute
        }

    match optRoute with
    | None ->
        { model with
            ActivePage = Page.NotFound
        },
        Cmd.none

    | Some route ->
        match route with
        | Router.Route.SignUp ->
            let (subModel, subCmd) = SignUp.init ()

            { model with
                ActivePage = Page.SignUp subModel
            },
            Cmd.map SignUpMsg subCmd

        | Router.Route.Login ->
            let (subModel, subCmd) = Login.init ()

            { model with
                ActivePage = Page.Login subModel
            },
            Cmd.map LoginMsg subCmd

        | Router.Route.File ->
            let (subModel, subCmd) = File.init ()

            { model with
                ActivePage = Page.File subModel
            },
            Cmd.map FileMsg subCmd

        | Router.Route.DynamicForm ->
            let (subModel, subCmd) = DynamicForm.init ()

            { model with
                ActivePage = Page.DynamicForm subModel
            },
            Cmd.map DynamicFormMsg subCmd

        | Router.Route.FormList ->
            let (subModel, subCmd) = FormList.init ()

            { model with
                ActivePage = Page.FormList subModel
            },
            Cmd.map FormListMsg subCmd

        | Router.Route.ValidationStrategies ->
            let (subModel, subCmd) = ValidationStrategies.init ()

            { model with
                ActivePage = Page.ValidationStrategies subModel
            },
            Cmd.map ValidationStrategiesMsg subCmd

        | Router.Route.Composability SharedRouter.ComposabilityRoute.Simple ->
            let (subModel, subCmd) = ComposabilitySimple.init ()

            { model with
                ActivePage = Page.ComposabilitySimple subModel
            },
            Cmd.map ComposabilitySimpleMsg subCmd

        | Router.Route.Composability SharedRouter.ComposabilityRoute.WithConfiguration ->
            let (subModel, subCmd) = ComposabilityWithConfiguration.init ()

            { model with
                ActivePage = Page.ComposabilityWithConfiguration subModel
            },
            Cmd.map ComposabilityWithConfigurationMsg subCmd

        | Router.Route.CustomActions ->
            let (subModel, subCmd) = CustomAction.init ()

            { model with
                ActivePage = Page.CustomAction subModel
            },
            Cmd.map CustomActionMsg subCmd

        | Router.Route.CustomField ->
            let (subModel, subCmd) = CustomField.init ()

            { model with
                ActivePage = Page.CustomField subModel
            },
            Cmd.map CustomFieldMsg subCmd

        | Router.Route.Disable ->
            let (subModel, subCmd) = Disable.init ()

            { model with
                ActivePage = Page.Disable subModel
            },
            Cmd.map DisableMsg subCmd

        | Router.Route.FrameworkSpecific Router.ReactRoute.ReactOnly ->
            { model with
                ActivePage = Page.ReactOnly
            },
            Cmd.none

        | Router.Route.FrameworkSpecific Router.ReactRoute.CustomView ->
            let (subModel, subCmd) = CustomView.init ()

            { model with
                ActivePage = Page.CustomView subModel
            },
            Cmd.map CustomViewMsg subCmd

        | Router.Route.Home ->
            { model with
                ActivePage = Page.Home
            },
            Cmd.none

        | Router.Route.NotFound ->
            { model with
                ActivePage = Page.NotFound
            },
            Cmd.none

let private update (msg: Msg) (model: Model) =
    match msg with
    | SetRoute optRoute -> setRoute optRoute model

    | SignUpMsg subMsg ->
        match model.ActivePage with
        | Page.SignUp subModel ->
            SignUp.update subMsg subModel
            |> Tuple.mapFirst Page.SignUp
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map SignUpMsg)

        | _ -> model, Cmd.none

    | LoginMsg subMsg ->
        match model.ActivePage with
        | Page.Login subModel ->
            Login.update subMsg subModel
            |> Tuple.mapFirst Page.Login
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map LoginMsg)

        | _ -> model, Cmd.none

    | FileMsg subMsg ->
        match model.ActivePage with
        | Page.File subModel ->
            File.update subMsg subModel
            |> Tuple.mapFirst Page.File
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map FileMsg)

        | _ -> model, Cmd.none

    | DynamicFormMsg subMsg ->
        match model.ActivePage with
        | Page.DynamicForm subModel ->
            DynamicForm.update subMsg subModel
            |> Tuple.mapFirst Page.DynamicForm
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map DynamicFormMsg)

        | _ -> model, Cmd.none

    | FormListMsg subMsg ->
        match model.ActivePage with
        | Page.FormList subModel ->
            FormList.update subMsg subModel
            |> Tuple.mapFirst Page.FormList
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map FormListMsg)

        | _ -> model, Cmd.none

    | ComposabilitySimpleMsg subMsg ->
        match model.ActivePage with
        | Page.ComposabilitySimple subModel ->
            ComposabilitySimple.update subMsg subModel
            |> Tuple.mapFirst Page.ComposabilitySimple
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map ComposabilitySimpleMsg)

        | _ -> model, Cmd.none

    | ComposabilityWithConfigurationMsg subMsg ->
        match model.ActivePage with
        | Page.ComposabilityWithConfiguration subModel ->
            ComposabilityWithConfiguration.update subMsg subModel
            |> Tuple.mapFirst Page.ComposabilityWithConfiguration
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map ComposabilityWithConfigurationMsg)

        | _ -> model, Cmd.none

    | ValidationStrategiesMsg subMsg ->
        match model.ActivePage with
        | Page.ValidationStrategies subModel ->
            ValidationStrategies.update subMsg subModel
            |> Tuple.mapFirst Page.ValidationStrategies
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map ValidationStrategiesMsg)

        | _ -> model, Cmd.none

    | CustomActionMsg subMsg ->
        match model.ActivePage with
        | Page.CustomAction subModel ->
            CustomAction.update subMsg subModel
            |> Tuple.mapFirst Page.CustomAction
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map CustomActionMsg)

        | _ -> model, Cmd.none

    | CustomFieldMsg subMsg ->
        match model.ActivePage with
        | Page.CustomField subModel ->
            CustomField.update subMsg subModel
            |> Tuple.mapFirst Page.CustomField
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map CustomFieldMsg)

        | _ -> model, Cmd.none

    | DisableMsg subMsg ->
        match model.ActivePage with
        | Page.Disable subModel ->
            Disable.update subMsg subModel
            |> Tuple.mapFirst Page.Disable
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map DisableMsg)

        | _ -> model, Cmd.none

    | CustomViewMsg subMsg ->
        match model.ActivePage with
        | Page.CustomView subModel ->
            CustomView.update subMsg subModel
            |> Tuple.mapFirst Page.CustomView
            |> Tuple.mapFirst (fun page ->
                { model with
                    ActivePage = page
                }
            )
            |> Tuple.mapSecond (Cmd.map CustomViewMsg)

        | _ -> model, Cmd.none

let private init (location) =
    setRoute
        location
        {
            ActivePage = Page.Home
            CurrentRoute = None
        }

let private renderDemoPage
    ({
         Title = titleText
         Remark = optRemark
         Code = codeText
         GithubLink = sourceCodeUrl
     }: DemoInformation<_>)
    (content: ReactElement)
    =

    Html.div [
        Html.br []

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
            Html.div [
                prop.innerHtml remark
            ]

        | None -> Html.none

        Html.hr []

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

        Html.hr []

        content

    ]

let private contentFromPage (page: Page) (dispatch: Dispatch<Msg>) =
    match page with
    | Page.Home ->
        Html.div [
            prop.innerHtml (
                Examples.Shared.Pages.Home.htmlContent
                    [
                        Page.ReactOnly.Component.information
                    ]
                    Page.CustomField.Component.information
                    Page.CustomView.Component.information
            )
        ]

    | Page.SignUp subModel ->
        renderDemoPage
            Examples.Shared.Forms.SignUp.information
            (SignUp.view subModel (SignUpMsg >> dispatch))

    | Page.Login subModel ->
        renderDemoPage
            Examples.Shared.Forms.Login.information
            (Login.view subModel (LoginMsg >> dispatch))

    | Page.File subModel ->
        renderDemoPage
            Examples.Shared.Forms.File.information
            (File.view subModel (FileMsg >> dispatch))

    | Page.DynamicForm subModel ->
        renderDemoPage
            Examples.Shared.Forms.DynamicForm.information
            (DynamicForm.view subModel (DynamicFormMsg >> dispatch))

    | Page.FormList subModel ->
        renderDemoPage
            Examples.Shared.Forms.FormList.information
            (FormList.view subModel (FormListMsg >> dispatch))

    | Page.ValidationStrategies subModel ->
        renderDemoPage
            Examples.Shared.Forms.ValidationStrategies.information
            (ValidationStrategies.view subModel (ValidationStrategiesMsg >> dispatch))

    | Page.ComposabilitySimple subModel ->
        renderDemoPage
            Examples.Shared.Forms.Composability.Simple.information
            (ComposabilitySimple.view subModel (ComposabilitySimpleMsg >> dispatch))

    | Page.ComposabilityWithConfiguration subModel ->
        renderDemoPage
            Examples.Shared.Forms.Composability.WithConfiguration.information
            (ComposabilityWithConfiguration.view
                subModel
                (ComposabilityWithConfigurationMsg >> dispatch))

    | Page.CustomAction subModel ->
        renderDemoPage
            Examples.Shared.Forms.CustomActions.information
            (CustomAction.view subModel (CustomActionMsg >> dispatch))

    | Page.CustomField subModel ->
        renderDemoPage
            CustomField.information
            (CustomField.view subModel (CustomFieldMsg >> dispatch))

    | Page.ReactOnly -> renderDemoPage ReactOnly.information (ReactOnly.View())

    | Page.Disable subModel ->
        renderDemoPage
            Examples.Shared.Forms.Disable.information
            (Disable.view subModel (DisableMsg >> dispatch))

    | Page.CustomView subModel ->
        renderDemoPage CustomView.information (CustomView.view subModel (CustomViewMsg >> dispatch))

    | Page.NotFound -> Html.text "Page not found"

let private view (model: Model) (dispatch: Dispatch<Msg>) =
    Bulma.columns [
        Bulma.column [
            column.is8
            column.isOffset2

            prop.children (contentFromPage model.ActivePage dispatch)
        ]
    ]

open Elmish.UrlParser
open Elmish.Navigation
open Elmish.React

#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.toNavigable (parseHash Router.routeParser) setRoute
|> Program.withReactSynchronous "root"
|> Program.run
