module Lit.Router

open Fable.Core
open Elmish.UrlParser
open Browser
open Browser.Types

[<Emit("window.navigator.userAgent")>]
let private navigatorUserAgent: string = jsNative

module Router =

    [<Literal>]
    let customNavigationEvent = "LIT_ROUTER_NAVIGATION_EVENT"

    let dispatchCustomNavigationEvent () =
        let ev = document.createEvent ("CustomEvent")
        ev.initEvent (customNavigationEvent, true, true)
        window.dispatchEvent ev |> ignore

type Hook with

    /// Listens for url changes and returns the current path.
    static member inline useRouter(parser: Location -> option<_>) =
        let path, setPath = Hook.useState (parser window.location)

        let onChange (_: Event) = setPath (parser window.location)

        Hook.useEffectOnce (fun () ->
            if navigatorUserAgent.Contains "Trident" || navigatorUserAgent.Contains "MSIE" then
                window.onhashchange <- onChange
            else
                window.onpopstate <- onChange

            window.addEventListener (Router.customNavigationEvent, onChange)

            // Ensure route is applied on page load
            Router.dispatchCustomNavigationEvent ()

            Hook.createDisposable (fun () ->
                window.removeEventListener (Router.customNavigationEvent, onChange)
            )
        )

        path

module Parser =

    // Code copied from Fable.Elmish.Browser

    let parsePath (parser: Parser<_, _>) (location: Location) =
        parse parser location.pathname (parseParams location.search)

    let parseHash (parser: Parser<_, _>) (location: Location) =
        let hash, queryParams =
            let hash =
                if location.hash.Length > 1 then
                    location.hash.Substring 1
                else
                    ""

            let pos = hash.IndexOf "?"

            if pos >= 0 then
                let path = hash.Substring(0, pos)
                let search = hash.Substring(pos + 1)
                path, parseParams search
            else
                hash, Map.empty

        parse parser hash queryParams
