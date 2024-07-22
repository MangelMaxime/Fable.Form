module EasyBuild.Main

open SimpleExec
open EasyBuild.Commands.Example
open EasyBuild.Commands.Release
open EasyBuild.Commands.Test
open EasyBuild.Commands.Docs
open EasyBuild.Commands.Lint
open EasyBuild.Commands.Format
open Spectre.Console.Cli

[<EntryPoint>]
let main args =

    Command.Run("dotnet", "husky install")

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"

        config.AddCommand<TestCommand>("test").WithDescription("Run the tests")
        |> ignore

        config.AddCommand<ExampleCommand>("example") |> ignore

        config.AddCommand<DocsCommand>("docs") |> ignore

        config
            .AddCommand<ReleaseCommand>("release")
            .WithDescription(
                """Release the different packages to NuGet and NPM based on the Git history

The changelogs will be generated based on the commit messages
        """
            )
        |> ignore

        config
            .AddCommand<LintCommand>("lint")
            .WithDescription("Run the linter on the source code")
        |> ignore

        config
            .AddCommand<FormatCommand>("format")
            .WithDescription("Format the source code")
        |> ignore

    )

    app.Run(args)
