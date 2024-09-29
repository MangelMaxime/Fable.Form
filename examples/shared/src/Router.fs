[<RequireQualifiedAccess>]
module SharedRouter

open Elmish.UrlParser

type IRoute =
    abstract member Segments: string

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
type Route<'FrameworkRoute when 'FrameworkRoute :> IRoute> =
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
    | FrameworkSpecific of 'FrameworkRoute
    | Disable
    | NotFound

    member this.Segments =
        match this with
        | Home -> "home"
        | SignUp -> "sign-up"
        | Login -> "login"
        | File -> "file"
        | DynamicForm -> "dynamic-form"
        | FormList -> "form-list"
        | ValidationStrategies -> "validation-strategies"
        | CustomAction -> "custom-actions"
        | CustomField -> "custom-field"
        | Composability subRoute -> "composability/" + ComposabilityRoute.toSegment subRoute
        | FrameworkSpecific route -> route.Segments
        | Disable -> "disable"
        | NotFound -> "not-found"

    member this.HashPart = "#" + this.Segments

let routeParser
    (frameworkRouteParser: Parser<('a -> Route<'a>), Route<'FrameworkRoute>>)
    : Parser<Route<'FrameworkRoute> -> Route<'FrameworkRoute>, Route<'FrameworkRoute>>
    =
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
        map Route.FrameworkSpecific (s "framework-specific" </> frameworkRouteParser)
        map Route.Home top
    ]
