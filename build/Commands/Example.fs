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

type ExampleCommand(destination: string, workingDirectory: string) =
    inherit Command<ExampleSettings>()
    interface ICommandLimiter<ExampleSettings>

    override _.Execute(context, settings) =
        let destination = DirectoryInfo(destination)

        destination.ReCreate()

        Npm.install (workingDirectory = workingDirectory)

        let fableCommand =
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            |> CmdLine.appendPrefix "--outDir" "fableBuild"
            |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.appendRaw "--verbose"
            |> CmdLine.toString

        if settings.IsWatch then
            [
                Command.RunAsync("dotnet", fableCommand, workingDirectory = workingDirectory)
                |> Async.AwaitTask

                Command.RunAsync("npx", "vite serve", workingDirectory = workingDirectory)
                |> Async.AwaitTask
            ]
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore

        else

            Command.Run("dotnet", fableCommand, workingDirectory = workingDirectory)

        Command.Run("npx", "vite build", workingDirectory = workingDirectory)

        0

type ReactExampleCommand() =
    inherit
        ExampleCommand(
            VirtualWorkspace.examples.react.fableBuild.``.``,
            Workspace.examples.React.``.``
        )

type SutilExampleCommand() =
    inherit
        ExampleCommand(
            VirtualWorkspace.examples.sutil.fableBuild.``.``,
            Workspace.examples.Sutil.``.``
        )

type LitExampleCommand() =
    inherit
        ExampleCommand(VirtualWorkspace.examples.lit.fableBuild.``.``, Workspace.examples.Lit.``.``)
