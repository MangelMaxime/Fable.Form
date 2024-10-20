[<RequireQualifiedAccess>]
module Router

open Elmish.UrlParser
open Browser.Dom

[<RequireQualifiedAccess>]
type SutilRoute =
    | CustomView

    interface SharedRouter.IRoute with
        member this.Segments = "custom-view"

type Route = SharedRouter.Route<SutilRoute>

let routeParser: Parser<Route -> Route, Route> =
    oneOf [
        map SutilRoute.CustomView (s "custom-view")
    ]
    |> SharedRouter.routeParser

open Sutil

let href (route: Route) = prop.href route.HashPart

let modifyLocation (route: Route) = window.location.href <- route.HashPart
