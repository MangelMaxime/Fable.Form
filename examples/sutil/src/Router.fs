[<RequireQualifiedAccess>]
module Router

open Elmish.UrlParser
open Browser.Dom

[<RequireQualifiedAccess>]
type SutilRoute =
    | None

    interface SharedRouter.IRoute with
        member this.Segments = ""

type Route = SharedRouter.Route<SutilRoute>

let routeParser: Parser<Route -> Route, Route> =
    oneOf [
        map SutilRoute.None (s "none")
    ]
    |> SharedRouter.routeParser

open Sutil

let href (route: Route) = prop.href route.HashPart

let modifyLocation (route: Route) = window.location.href <- route.HashPart
