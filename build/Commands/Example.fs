module EasyBuild.Commands.Example

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel
open System.IO
open EasyBuild.Utils
open EasyBuild.Workspace

type ExampleSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Watch for changes and re-build the examples")>]
    member val IsWatch: bool = false with get, set

type ExampleCommand() =
    inherit Command<ExampleSettings>()

    override _.Execute(context, settings) =
        let destination = DirectoryInfo(VirtualWorkspace.examples.fableBuild)

        destination.ReCreate()

        Npm.install (workingDirectory = Workspace.examples.``.``)

        let fableCommand =
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            |> CmdLine.appendPrefix "--outDir" "fableBuild"
            |> CmdLine.appendRaw "--verbose"
            |> CmdLine.toString

        if settings.IsWatch then
            [
                Command.RunAsync(
                    "dotnet",
                    fableCommand,
                    workingDirectory = Workspace.examples.``.``
                )
                |> Async.AwaitTask

                Command.RunAsync(
                    "npx",
                    "vite serve",
                    workingDirectory = Workspace.examples.``.``
                )
                |> Async.AwaitTask
            ]
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore

        else

            Command.Run("dotnet", fableCommand, workingDirectory = Workspace.examples.``.``)

        Command.Run("npx", "vite build", workingDirectory = Workspace.examples.``.``)

        0
