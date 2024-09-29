[<AutoOpen>]
module Global

[<NoComparison>]
type DemoInformation<'FrameworkRoute when 'FrameworkRoute :> SharedRouter.IRoute> =
    {
        Title: string
        Description: string
        Route: SharedRouter.Route<'FrameworkRoute>
        Remark: string option
        Code: string
        GithubLink: string
    }
