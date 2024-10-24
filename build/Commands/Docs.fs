module EasyBuild.Commands.Docs

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel
open EasyBuild.Commands.Example
open EasyBuild.Workspace

type DocsSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Watch for changes and re-build the Docs")>]
    member val IsWatch: bool = false with get, set

    [<CommandOption("-p|--publish")>]
    [<Description("Publish the Docs to the GitHub Pages")>]
    member val Publish: bool = false with get, set

type DocsCommand() =
    inherit Command<DocsSettings>()

    override _.Execute(context, settings) =
        if settings.IsWatch then
            Command.Run("npx", "nacara watch")
        else
            Command.Run("npx", "nacara")

            ReactExampleCommand().Execute(context, ExampleSettings()) |> ignore
            SutilExampleCommand().Execute(context, ExampleSettings()) |> ignore

            if settings.Publish then
                Command.Run("npx", "gh-pages -d docs_deploy")

        0
