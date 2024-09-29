module Examples

open Sutil
open Sutil.Bulma
open Sutil.Router
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

let private renderDemoPage
    ({
         Title = titleText
         Remark = optRemark
         Code = codeText
         GithubLink = sourceCodeUrl
     }: DemoInformation<_>)
    (content: Sutil.Core.SutilElement)
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
        | Some remark -> Html.parse remark

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
                        | Router.Route.Home ->
                            Html.div [
                                Html.parse Examples.Shared.Pages.Home.htmlContent
                            ]
                        | Router.Route.Login ->
                            renderDemoPage
                                Examples.Shared.Forms.Login.information
                                (Examples.Sutil.Pages.Login.Page())

                        | Router.Route.FormList -> Examples.Sutil.Pages.FormList.Page()
                        | Router.Route.SignUp -> Examples.Sutil.Pages.SignUp.Page()
                )
        ]
    ]

Program.mount ("root", app ()) |> ignore
