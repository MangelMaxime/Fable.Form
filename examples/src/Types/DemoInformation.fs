[<RequireQualifiedAccess>]
module DemoInformation

open Feliz

[<NoComparison>]
type T =
    {
        Title : string
        Description : string
        Route : Router.Route
        Remark : ReactElement option
        Code : string
        GithubLink : string
    }
