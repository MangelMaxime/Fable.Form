module Examples.Sutil.App

open Sutil
open Sutil.Bulma
open Sutil.Router
open Sutil.CoreElements
open Fable.Core.JsInterop

// Only import the style if we are in DEBUG mode
// otherwise the style will be included by Nacara directly
#if DEBUG
importSideEffects "./../../../docs/style.scss"
#endif

let private renderDemoPage
    ({
         Title = titleText
         Remark = optRemark
         Code = codeText
         GithubLink = sourceCodeUrl
     }: DemoInformation<_>)
    (content: Core.SutilElement)
    =

    Html.div [
        Html.br []

        Html.div [
            prop.className "content has-text-centered"

            bulma.title.h5 [
                title.is5
                prop.text titleText
            ]
        ]

        match optRemark with
        | Some remark ->
            Html.div [
                Html.parse remark
            ]

        | None -> Html.none

        Html.hr []

        Html.pre [
            prop.className "code-preview"

            Html.code [
                prop.text (codeText.Trim())
            ]
        ]

        Html.div [
            prop.className "has-text-centered"

            Html.a [
                prop.href sourceCodeUrl
                prop.text "Full source code"
            ]
        ]

        Html.hr []

        Html.div [
            content
        ]
    ]

let app () =
    let pageStore: IStore<Router.Route option> = Store.make None

    let routerSubscription =
        Navigable.listenLocation (Parser.parseHash Router.routeParser, Store.set pageStore)

    bulma.columns [
        unsubscribeOnUnmount [
            routerSubscription
        ]

        bulma.column [
            column.is8
            column.isOffset2

            Bind.fragment
                pageStore
                (fun page ->
                    match page with
                    | None -> Html.div "Not found"
                    | Some page ->
                        match page with
                        | Router.Route.NotFound -> Html.div "Not found"
                        | Router.Route.Home ->
                            Html.div [
                                Html.parse (
                                    Examples.Shared.Pages.Home.htmlContent
                                        []
                                        Examples.Sutil.Pages.CustomField.information
                                        Examples.Sutil.Pages.CustomView.information
                                )
                            ]

                        | Router.Route.Login ->
                            renderDemoPage
                                Examples.Shared.Forms.Login.information
                                (Examples.Sutil.Pages.Login.Page())

                        | Router.Route.FormList ->
                            renderDemoPage
                                Examples.Shared.Forms.FormList.information
                                (Examples.Sutil.Pages.FormList.Page())

                        | Router.Route.SignUp ->
                            renderDemoPage
                                Examples.Shared.Forms.SignUp.information
                                (Examples.Sutil.Pages.SignUp.Page())

                        | Router.Route.ValidationStrategies ->
                            renderDemoPage
                                Examples.Shared.Forms.ValidationStrategies.information
                                (Examples.Sutil.Pages.ValidationStrategies.Page())

                        | Router.Route.Disable ->
                            renderDemoPage
                                Examples.Shared.Forms.Disable.information
                                (Examples.Sutil.Pages.Disable.Page())

                        | Router.Route.CustomActions ->
                            renderDemoPage
                                Examples.Shared.Forms.CustomActions.information
                                (Examples.Sutil.Pages.CustomActions.Page())

                        | Router.Route.DynamicForm ->
                            renderDemoPage
                                Examples.Shared.Forms.DynamicForm.information
                                (Examples.Sutil.Pages.DynamicForm.Page())

                        | Router.Route.File ->
                            renderDemoPage
                                Examples.Shared.Forms.File.information
                                (Examples.Sutil.Pages.File.Page())

                        | Router.Route.Composability SharedRouter.ComposabilityRoute.Simple ->
                            renderDemoPage
                                Examples.Shared.Forms.Composability.Simple.information
                                (Examples.Sutil.Pages.Composability.Simple.Page())

                        | Router.Route.Composability SharedRouter.ComposabilityRoute.WithConfiguration ->
                            renderDemoPage
                                Examples.Shared.Forms.Composability.WithConfiguration.information
                                (Examples.Sutil.Pages.Composability.WithConfiguration.Page())

                        | Router.Route.CustomField ->
                            renderDemoPage
                                Examples.Sutil.Pages.CustomField.information
                                (Examples.Sutil.Pages.CustomField.Page())

                        | Router.Route.FrameworkSpecific Router.SutilRoute.CustomView ->
                            renderDemoPage
                                Examples.Sutil.Pages.CustomView.information
                                (Examples.Sutil.Pages.CustomView.Page())
                )
        ]
    ]

Program.mount ("root", app ()) |> ignore
