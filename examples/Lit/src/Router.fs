[<RequireQualifiedAccess>]
module Router

open Elmish.UrlParser
open Browser.Dom

[<RequireQualifiedAccess>]
type ComposabilityRoute =
    | Simple
    | WithConfiguration

module ComposabilityRoute =

    let toSegment (route: ComposabilityRoute) =
        match route with
        | ComposabilityRoute.Simple -> "simple"
        | ComposabilityRoute.WithConfiguration -> "with-configuration"

[<RequireQualifiedAccess>]
type Route =
    | Home
    | SignUp
    | Login
    | File
    | DynamicForm
    | FormList
    | Composability of ComposabilityRoute
    | ValidationStrategies
    | CustomAction
    | CustomField
    | ReactOnly
    | Disable
    | NotFound

let toHash page =
    let segmentsPart =
        match page with
        | Route.Home -> "home"
        | Route.SignUp -> "sign-up"
        | Route.Login -> "login"
        | Route.File -> "file"
        | Route.NotFound -> "not-found"
        | Route.DynamicForm -> "dynamic-form"
        | Route.Composability subRoute -> "composability/" + ComposabilityRoute.toSegment subRoute
        | Route.ValidationStrategies -> "validation-strategies"
        | Route.CustomAction -> "custom-actions"
        | Route.CustomField -> "custom-field"
        | Route.Disable -> "disable"
        | Route.FormList -> "form-list"
        | Route.ReactOnly -> "react-only"

    "#" + segmentsPart

let routeParser: Parser<Route -> Route, Route> =
    oneOf [
        map Route.Home (s "home")
        map Route.SignUp (s "sign-up")
        map Route.Login (s "login")
        map Route.File (s "file")
        map Route.DynamicForm (s "dynamic-form")
        map Route.FormList (s "form-list")
        map Route.ValidationStrategies (s "validation-strategies")
        map Route.CustomAction (s "custom-actions")
        map (Route.Composability ComposabilityRoute.Simple) (s "composability" </> s "simple")
        map
            (Route.Composability ComposabilityRoute.WithConfiguration)
            (s "composability" </> s "with-configuration")
        map Route.CustomField (s "custom-field")
        map Route.Disable (s "disable")
        map Route.ReactOnly (s "react-only")
        map Route.Home top
    ]

// let href route = prop.href (toHash route)

// let modifyUrl route = route |> toHash |> Navigation.modifyUrl

// let newUrl route = route |> toHash |> Navigation.newUrl

let modifyLocation route = window.location.href <- toHash route
